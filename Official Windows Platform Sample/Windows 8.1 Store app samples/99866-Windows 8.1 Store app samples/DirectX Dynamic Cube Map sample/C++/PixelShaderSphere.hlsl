TextureCube environmentMapCube : register(t0) ;
SamplerState SimpleSampler : register(s0) ;

struct PixelShaderInput
{
    float4 pos      : SV_POSITION;
    float3 normal   : NORMAL;
    float2 tex      : TEXCOORD0;
};

float4 main(PixelShaderInput input) : SV_TARGET
{
    // Invert the input normal x axis to mirror texture when sampling the texturecube.
    float3 color = environmentMapCube.Sample(SimpleSampler, float3(-input.normal.x, input.normal.y, input.normal.z)).rgb;
    return float4(color.x, color.y, color.z, 1.0f);
}
