#include "VertexBuffer.hpp"

using namespace std;

class VertexArray
{
	
public:
	GLuint vao;
	VertexArray() {
		glGenVertexArrays(1, &vao);
	}

	void Link(VertexBuffer& vbo, GLuint layout, GLuint numComponents, GLenum type, GLsizeiptr stride, void* offset) {
		vbo.Bind();
		glVertexAttribPointer(layout, numComponents, type, GL_TRUE, stride, offset);
		glEnableVertexAttribArray(layout);
		vbo.Unbind();
	}
	void Bind() {
		glBindVertexArray(vao);
	}
	void Unbind() {
		glBindVertexArray(0);
	}

	void Delete() {
		glDeleteVertexArrays(1, &vao);
	}
};