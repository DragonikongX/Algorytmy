#version 460 core

out vec3 fragColor;
out vec2 fragTexture;
out float visibility;

layout (location = 0) in vec3 vertPosition;
layout (location = 1) in vec3 vertNormals;
layout (location = 2) in vec3 vertColor;
layout (location = 3) in vec2 vertTexturePosition;

uniform mat4 vertCamMatrix;
uniform mat4 vertModel;

const float density = 0.04f;
const float gradient = 3.5f;

void main(){
	vec3 fragCurrentPosition = vec3(vertModel * vec4(vertPosition, 1.0f));
	vec4 fragRelativePosition =  vertCamMatrix * vec4(fragCurrentPosition,1.0f);
	gl_Position = fragRelativePosition;
	
	fragColor = vertColor;
	fragTexture = vertTexturePosition;

	float cameraDistance = length(fragRelativePosition.xyz);
	visibility = exp(-pow((cameraDistance * density), gradient));
	visibility = clamp(visibility, 0.0f, 1.0f);
};