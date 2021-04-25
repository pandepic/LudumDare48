#version 450

layout(set = 0, binding = 0) uniform ProjectionViewBuffer {
    mat4x4 uProjection;
    mat4x4 uView;
};

layout(set = 1, binding = 0) uniform MyUniforms {
    mat4x4 uUVTransform;
};

layout (set = 2, binding = 0) uniform texture2D fTexture;
layout (set = 2, binding = 1) uniform sampler fTextureSampler;

layout (set = 3, binding = 0) uniform texture2D fBg;
layout (set = 3, binding = 1) uniform sampler fBgSampler;

layout (location = 0) in vec2 fTexCoords;
layout (location = 1) in vec4 fColor;

layout (location = 0) out vec4 fFragColor;

void main()
{
    vec4 testMask = texture(sampler2D(fTexture, fTextureSampler), fTexCoords);

    if (testMask.x < 0.5) { discard; }

    vec4 diffuse = texture(sampler2D(fBg, fBgSampler), (uUVTransform * vec4(fTexCoords.x, fTexCoords.y, 0.0, 1.0)).xy);

    fFragColor = vec4(diffuse.r, diffuse.g, diffuse.b, fColor.a);
}
