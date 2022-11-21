#include <stdio.h>
#include <stdlib.h>
#include<iostream>
#include <string>
#include <sstream>
#include <vector>

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

#include "Shader.hpp"
#include "Mesh.hpp" 


using namespace glm;
using namespace std;

const int width = 800;
const int height = 800;

int main()
{
	GLFWwindow* window;
	if (!glfwInit())
	{
		return -1;
	}
	window = glfwCreateWindow(width, height, "Projekt Opengl", NULL, NULL);
	if (window == NULL) {
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);

	if (glewInit() != GLEW_OK) {
		glfwTerminate();
		return -1;
	}
	glViewport(0, 0, width, height);
	
	ShaderClass shader(readFile("vertex.vert"), readFile("fragment.frag"));

	vector<Mesh> terrain = vector<Mesh>();
	vector<Mesh> tops = vector<Mesh>();
	vector<Mesh> bottoms = vector<Mesh>();

	Texture terrainTexutre = Texture("Textures/grass.jpg", 0, GL_RGBA, GL_UNSIGNED_BYTE);
	for (int i = -25; i < 25; i++) {
		for (int j = -25; j < 25; j++) {
			Mesh plane("Obj/plane2.obj", &shader, terrainTexutre, 0.5f);
			plane.SetPosition(vec3(i * 2.0f, 0.0f, j * 2.0f));
			terrain.push_back(plane);
		}
	}
	
	Texture treeTopTexture = Texture("Textures/tree.jpg", 0, GL_RGBA, GL_UNSIGNED_BYTE);
	Texture treeBotTexture = Texture("Textures/leafs.jpg", 0, GL_RGBA, GL_UNSIGNED_BYTE);
	for (int i = -5; i < 5; i++) {
		for (int j = -5; j < 5; j++) {
			Mesh top("Obj/pien.obj", &shader, treeTopTexture, 10.0f);
			top.SetPosition(vec3(i * 10, 1.3f, j * 10));
			tops.push_back(top);
			Mesh bot("Obj/korona.obj", &shader, treeBotTexture, 5.0f);
			bot.SetPosition(vec3(i * 10, 1.3f, j * 10));
			bottoms.push_back(bot);
		}
	}

	Camera camera(width, height, vec3(-5.0f, 3.0f, 5.0f));
	glEnable(GL_DEPTH_TEST);
	while (!glfwWindowShouldClose(window)) {
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glClearColor(0.5f, 0.5f, 0.5f, 1.0f);

		camera.Input(window);
		camera.UpdateMatrix(45.0f, 0.1f, 100.0f);

		for (int i = 0; i < terrain.size(); i++) {
			terrain[i].Uniform();
			terrain[i].Draw(camera);
		}
		for (int i = 0; i < tops.size(); i++) {
			tops[i].Uniform();
			tops[i].Draw(camera);
			bottoms[i].Uniform();
			bottoms[i].Draw(camera);
		}
		glfwSwapBuffers(window);
		glfwPollEvents();
	}
	shader.Delete();
	glfwDestroyWindow(window);
	glfwTerminate();

	return 0;
}