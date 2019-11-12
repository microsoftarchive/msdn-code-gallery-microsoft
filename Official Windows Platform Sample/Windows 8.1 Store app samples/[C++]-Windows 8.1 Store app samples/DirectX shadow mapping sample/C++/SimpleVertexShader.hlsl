// Constant buffer that stores two of the basic column-major matrices for composing geometry.
cbuffer ViewProjectionConstantBuffer : register(b0)
{
    matrix view;
    matrix projection;
};

// Constant buffer that stores the per-model transform matrix for composing geometry.
cbuffer ModelConstantBuffer : register(b1)
{
    matrix model;
};

// Per-vertex data used as input to the vertex shader.
struct VertexShaderInput
{
    float3 pos : POSITION;
    float2 tex : TEXCOORD0;
    float3 norm : NORMAL0;
    float3 color : COLOR0;
};

// Per-pixel color data passed to the pixel shader.
struct PixelShaderInput
{
    float4 pos : SV_POSITION;
};

// Shader to do vertex processing for camera view position and light view position.
PixelShaderInput main(VertexShaderInput input)
{
    PixelShaderInput output;
    float4 pos = float4(input.pos, 1.0f);

    // Transform the vertex position into projected space.
    pos = mul(pos, model);
    pos = mul(pos, view);
    pos = mul(pos, projection);
    output.pos = pos;

    return output;
}