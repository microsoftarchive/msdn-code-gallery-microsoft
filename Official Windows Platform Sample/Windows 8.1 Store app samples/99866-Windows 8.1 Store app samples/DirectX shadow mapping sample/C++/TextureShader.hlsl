// Per-pixel color data passed to the pixel shader.
struct PixelShaderInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float3 color : COLOR0;
};

Texture2D shadowMap : register(t0) ;
SamplerState textureSampler : register(s0) ;

// A basic texture shader with no lighting.
float4 main(PixelShaderInput input) : SV_TARGET
{
    float4 output = float4(input.color, 1.0f);

    float4 texVal = shadowMap.Sample(textureSampler, input.tex);

    output = texVal.xxxw;

    return output;
}
