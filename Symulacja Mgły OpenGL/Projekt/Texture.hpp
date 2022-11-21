#include <stb_image.h>

using namespace std;

class Texture
{
public:
	GLuint ID;
	GLuint unit;
	Texture() {};
	Texture(const char* imgPath, GLuint slot, GLenum format, GLenum pixelType) {

		int imgWidth, imgHeight, imgChannels;

		unsigned char* imgBytes = stbi_load(imgPath, &imgWidth, &imgHeight, &imgChannels, 4);
		if (!imgBytes) {
			cout << "Texture Loading Error!" << endl;
			return;
		}
		glGenTextures(1, &ID);
		glActiveTexture(GL_TEXTURE0 + slot);
		unit = slot;
		glBindTexture(GL_TEXTURE_2D, ID);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imgWidth, imgHeight, 0, format, pixelType, imgBytes);
		glGenerateMipmap(GL_TEXTURE_2D);
		stbi_image_free(imgBytes);
		glBindTexture(GL_TEXTURE_2D, 0);
	}

	void TextureUnit(ShaderClass shader, const char *uniform, GLuint unit) {
		GLuint texID = glGetUniformLocation(shader.ID, uniform);
		shader.Activate();
		glUniform1i(texID, unit);
	}

	void Bind() {
		glActiveTexture(GL_TEXTURE0 + unit);
		glBindTexture(GL_TEXTURE_2D, ID);
	}

	void Unbind() {
		glBindTexture(GL_TEXTURE_2D, 0);
	}

	void Delete() {
		glDeleteTextures(1, &ID);
	}

};