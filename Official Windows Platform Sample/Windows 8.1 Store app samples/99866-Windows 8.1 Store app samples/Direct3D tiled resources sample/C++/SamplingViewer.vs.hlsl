cbuffer CB0
{
    float4x4 texTransform;
    float4x4 posTransform;
};

struct VS_IN
{
    float2 pos : POSITION;
    float2 tex : TEXCOORD0;
};

struct VS_OUT
{
    float2 tex : TEXCOORD0;
    float4 pos : SV_POSITION;
};

VS_OUT main(VS_IN input)
{
    VS_OUT output;
    output.tex = mul(texTransform, float4(input.tex, 0.0f, 1.0f)).xy;
    output.pos = mul(float4(input.pos, 0.5f, 1.0f), posTransform);
    return output;
}
