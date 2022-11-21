#include "Texture.hpp"
#include "Camera.hpp"
#include "VertexArray.hpp"

using namespace std;

class Mesh
{
public:
	ShaderClass* shader;
	vector<Vertex> vertices;
	Texture texture;
	VertexArray vao;

	vec3 pos = vec3(0.0f, 0.0f, 0.0f);
	vec3 rotation = vec3(0.0f, 0.0f, 0.0f);
	vec3 scale = vec3(1.0f, 1.0f, 1.0f);
	vec4 color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
	mat4 matrix = mat4(1.0f);

	float textureScale;

	Mesh(){}
	Mesh(const char* filename, ShaderClass* shader ,Texture newtexture, float textureScale) {
		this->shader = shader;
		vao.Bind();
		this->texture = newtexture;
		this->textureScale = textureScale;
		vertices = loadOBJ(filename);
		
		VertexBuffer vertexMeshBuffer(vertices);
		vao.Link(vertexMeshBuffer, 0, 3, GL_FLOAT, sizeof(Vertex), (void*)0);
		vao.Link(vertexMeshBuffer, 1, 3, GL_FLOAT, sizeof(Vertex), (void*)(3 * sizeof(float)));
		vao.Link(vertexMeshBuffer, 2, 3, GL_FLOAT, sizeof(Vertex), (void*)(6 * sizeof(float)));
		vao.Link(vertexMeshBuffer, 3, 2, GL_FLOAT, sizeof(Vertex), (void*)(9 * sizeof(float)));
		vao.Unbind();
		vertexMeshBuffer.Unbind();
	}

	vector<Vertex> loadOBJ(const char* file_name)
	{
		std::vector<glm::fvec3> vertex_positions;
		std::vector<glm::fvec2> vertex_texcoords;
		std::vector<glm::fvec3> vertex_normals;

		std::vector<GLint> vertex_position_indicies;
		std::vector<GLint> vertex_texcoord_indicies;
		std::vector<GLint> vertex_normal_indicies;

		std::vector<Vertex> vertices;

		std::stringstream ss;
		std::ifstream in_file(file_name);
		std::string line = "";
		std::string prefix = "";
		glm::vec3 temp_vec3;
		glm::vec2 temp_vec2;
		GLint temp_glint = 0;

		if (!in_file.is_open())
		{
			throw "ERROR::OBJLOADER::Could not open file.";
		}

		while (std::getline(in_file, line))
		{
			ss.clear();
			ss.str(line);
			ss >> prefix;

			if (prefix == "v")
			{
				ss >> temp_vec3.x >> temp_vec3.y >> temp_vec3.z;
				vertex_positions.push_back(temp_vec3);
			}
			else if (prefix == "vt")
			{
				ss >> temp_vec2.x >> temp_vec2.y;
				vertex_texcoords.push_back(temp_vec2);
			}
			else if (prefix == "vn")
			{
				ss >> temp_vec3.x >> temp_vec3.y >> temp_vec3.z;
				vertex_normals.push_back(temp_vec3);
			}
			else if (prefix == "f")
			{
				int counter = 0;
				while (ss >> temp_glint)
				{
					if (counter == 0)
						vertex_position_indicies.push_back(temp_glint);
					else if (counter == 1)
						vertex_texcoord_indicies.push_back(temp_glint);
					else if (counter == 2)
						vertex_normal_indicies.push_back(temp_glint);

					if (ss.peek() == '/')
					{
						++counter;
						ss.ignore(1, '/');
					}
					else if (ss.peek() == ' ')
					{
						++counter;
						ss.ignore(1, ' ');
					}

					if (counter > 2)
						counter = 0;
				}
			}
		}
		vertices.resize(vertex_position_indicies.size(), Vertex());
		for (size_t i = 0; i < vertices.size(); ++i)
		{
			vertices[i].pos = vertex_positions[vertex_position_indicies[i] - 1];
			vertices[i].texUv = vertex_texcoords[vertex_texcoord_indicies[i] - 1] * textureScale;
		}
		return vertices;
	}

	void Draw(Camera& camera) {
		shader->Activate();
		vao.Bind();
		GLuint textureDiffuseNumber = 0;
		string textureName;
		textureName = to_string(textureDiffuseNumber++);
		texture.TextureUnit(*shader, "fragTextureColor", 0);
		texture.Bind();
		camera.Matrix(*shader, "vertCamMatrix");
		glDrawArrays(GL_TRIANGLES, 0, vertices.size());
	}

	void SetPosition(vec3 position) {
		this->pos = position;
	}

	void Uniform() {
		matrix = mat4(1.0f);
		matrix = translate(matrix, pos * (1.0f / scale));
		shader->Activate();
		glUniformMatrix4fv(glGetUniformLocation(shader->ID, "vertModel"), 1, GL_FALSE, value_ptr(matrix));
	}
};
