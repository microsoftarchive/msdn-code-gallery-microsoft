Texture2D SimpleTexture : register(t0) ;
SamplerState SimpleSampler : register(s0) ;

struct PixelShaderInput
{
    float4 pos      : SV_POSITION;
    float3 normal   : NORMAL;
    float2 tex      : TEXCOORD0;
};

float4 main(PixelShaderInput input) : SV_TARGET
{
    // Sample the texture to detemine color.
    float3 color = SimpleTexture.Sample(SimpleSampler, input.tex).rgb;
    return float4(color, 1.0f);
}
