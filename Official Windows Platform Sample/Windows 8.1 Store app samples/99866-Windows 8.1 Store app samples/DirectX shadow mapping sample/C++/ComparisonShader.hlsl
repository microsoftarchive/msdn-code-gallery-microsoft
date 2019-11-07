// Per-pixel color data passed to the pixel shader.
struct PixelShaderInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float3 color : COLOR0;
};

Texture2D shadowMap : register(t0) ;
SamplerComparisonState shadowSampler : register(s0) ;

// A comparison shader to show the shadow stencil.
float4 main(PixelShaderInput input) : SV_TARGET
{
    float4 output = float4(input.color, 1.0f);

    output.xyz = 1.f - shadowMap.SampleCmpLevelZero(
        shadowSampler,
        input.tex,
        1.f
        ).xxx;

    return output;
}
