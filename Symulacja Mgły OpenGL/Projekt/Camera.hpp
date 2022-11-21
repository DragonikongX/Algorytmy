#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>
#include <gtx/rotate_vector.hpp>
#include <gtx/vector_angle.hpp>

using namespace glm;

class Camera {
public:
	vec3 pos, ori = vec3(0.0f, 0.0f, -1.0f), up = vec3(0.0f, 1.0f, 0.0f);
	int width, height;
	float speed = 0.1, sens = 100;
	mat4 camMatrix = mat4(1.0f);

	Camera(int width, int height, vec3 pos) {
		this->pos = pos;
		this->width = width;
		this->height = height;
	}

	void Matrix(ShaderClass& shader, const char* uniform) {
		glUniformMatrix4fv(glGetUniformLocation(shader.ID, uniform), 1, GL_FALSE, value_ptr(camMatrix));
	}

	void UpdateMatrix(float fovDegrees, float near, float far) {
		mat4 view = mat4(1.0f);
		mat4 projection = mat4(1.0f);

		view = lookAt(pos, pos + ori, up);
		projection = perspective(radians(fovDegrees), (float)(width / height), near, far);

		camMatrix = projection * view;
	}

	void Input(GLFWwindow* window)
	{
		double mouseX;
		double mouseY;
		if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
		{
			exit(0);
		}
		if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
		{
			pos += speed * ori;
		}
		if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
		{
			pos += speed * -glm::normalize(glm::cross(ori, up));
		}
		if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
		{
			pos += speed * -ori;
		}
		if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
		{
			pos += speed * glm::normalize(glm::cross(ori, up));
		}
		glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_HIDDEN);
		glfwGetCursorPos(window, &mouseX, &mouseY);

		float rotationX = sens * (float)(mouseY - (height / 2)) / height;
		float rotationY = sens * (float)(mouseX - (width / 2)) / width;

		glm::vec3 newori = glm::rotate(ori, glm::radians(-rotationX), glm::normalize(glm::cross(ori, up)));

		if (abs(glm::angle(newori, up) - glm::radians(90.0f)) <= glm::radians(85.0f))
		{
			ori = newori;
		}
		ori = glm::rotate(ori, glm::radians(-rotationY), up);
		glfwSetCursorPos(window, (width / 2), (height / 2));
	}
};