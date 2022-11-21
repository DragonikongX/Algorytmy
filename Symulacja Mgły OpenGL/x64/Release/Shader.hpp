#include <fstream>

using namespace std;

static string readFile(const string& filePath) {
	ifstream fs(filePath, ios::in);
	stringstream buffer;
	string line;
	while (getline(fs, line)) {
		buffer << line << "\n";
	}
	fs.close();
	return buffer.str();
}

static GLuint compileShader(GLuint type, const string& source)
{
	GLuint hShader = glCreateShader(type);
	GLint result;
	const char* src = source.c_str();

	glShaderSource(hShader, 1, &src, nullptr);
	glCompileShader(hShader);
	glGetShaderiv(hShader, GL_COMPILE_STATUS, &result);

	return hShader;
}

class ShaderClass
{
public:
	GLuint ID;
	ShaderClass(const string& vertexShader, const string& fragmentShader) {
		Create(vertexShader, fragmentShader);
	}

	void Create(const string& vertexShader, const string& fragmentShader) {
		GLint result;
		GLuint fs = compileShader(GL_FRAGMENT_SHADER, fragmentShader);
		GLuint vs = compileShader(GL_VERTEX_SHADER, vertexShader);
		if (vs == 0 || fs == 0) {
			return;
		}
		ID = glCreateProgram();
		glAttachShader(ID, vs);
		glAttachShader(ID, fs);
		glLinkProgram(ID);
		glGetShaderiv(ID, GL_COMPILE_STATUS, &result);
		glValidateProgram(ID);
		glDetachShader(ID, vs);
		glDetachShader(ID, fs);
	}

	void Activate() {
		glUseProgram(ID);
	}

	void Delete() {
		glDeleteProgram(ID);
	}
};