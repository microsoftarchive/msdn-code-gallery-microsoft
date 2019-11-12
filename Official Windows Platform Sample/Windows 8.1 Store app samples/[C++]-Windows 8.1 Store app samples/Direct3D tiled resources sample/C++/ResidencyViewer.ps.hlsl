TextureCube<float> tex;
SamplerState sam;

float4 main(float4 coord : TEXCOORD0) : SV_TARGET
{
    float val = 1.0f - tex.Sample(sam, normalize(coord.xyz));
    float3 color = float3(0.0f, 0.0f, 0.0f);
    if (val < 0.25f)
    {
        color.r = 1.0f;
        color.g = val / 0.25f;
    }
    else if (val < 0.5f)
    {
        color.g = 1.0f;
        color.r = 1.0f - (val - 0.25f) / 0.25f;
    }
    else if (val < 0.75f)
    {
        color.g = 1.0f;
        color.b = (val - 0.5f) / 0.25f;
    }
    else
    {
        color.b = 1.0f;
        color.g = 1.0f - (val - 0.75f) / 0.25f;
    }
    return float4(color, 1.0f);
}
