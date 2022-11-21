#version 460 core

out vec4 FragColor;

in vec3 fragColor;
in vec2 fragTexture;
in float visibility;

uniform sampler2D fragTextureColor;

void main(){
    FragColor = texture(fragTextureColor, fragTexture) * vec4(1.0f, 1.0f, 1.0f, 1.0f);
    FragColor = mix(vec4(0.5f, 0.5f, 0.5f, 1.0f), FragColor, visibility);
};