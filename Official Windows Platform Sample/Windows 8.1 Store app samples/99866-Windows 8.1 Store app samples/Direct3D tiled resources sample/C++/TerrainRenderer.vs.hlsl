Texture2D<float> HeightTexture : register(t0);
SamplerState Anisotropic : register(s0);

cbuffer VertexShaderConstants : register(b0)
{
    row_major float4x4 ViewMatrix;
    row_major float4x4 ProjectionMatrix; // Also may include device orientation rotation transform.
};

struct VS_IN
{
    float3 pos : POSITION;
};

struct VS_OUT
{
    float3 tex : TEXCOORD0;
    float3 utan : TANGENT0;
    float3 vtan : TANGENT1;
    float4 pos : SV_POSITION;
};

VS_OUT main(VS_IN input)
{
    VS_OUT output;
    output.tex = normalize(input.pos);
    output.utan = normalize(float3(-input.pos.y, input.pos.x, 0.0f));
    output.vtan = normalize(cross(input.pos, output.utan));
    float4 pos = float4(input.pos, 1.0f);
    pos = mul(pos, ViewMatrix);
    pos = mul(pos, ProjectionMatrix);
    output.pos = pos;
    return output;
}
