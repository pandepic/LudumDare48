#version 450

layout (set = 1, binding = 0) uniform texture2D fTexture;
layout (set = 1, binding = 1) uniform sampler fTextureSampler;

layout (set = 2, binding = 0) uniform texture2D fMask;
layout (set = 2, binding = 1) uniform sampler fMaskSampler;

layout (location = 0) in vec2 fTexCoords;
layout (location = 1) in vec4 fColor;

layout (location = 0) out vec4 fFragColor;

void main()
{
    vec4 testMask = texture(sampler2D(fMask, fMaskSampler), fTexCoords);
    
    if (testMask.x == 1.0 && testMask.y == 1.0 && testMask.z == 1.0 && testMask.w == 1.0)
    {
        discard;
    }
    
    fFragColor = texture(sampler2D(fTexture, fTextureSampler), fTexCoords) * fColor;
}