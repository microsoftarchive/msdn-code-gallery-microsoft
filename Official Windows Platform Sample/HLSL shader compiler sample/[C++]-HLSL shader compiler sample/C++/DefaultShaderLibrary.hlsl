// This file defines the defaults for the editor.

// This is the default code in the fixed header section.
// @@@ Begin Header
Texture2D<float3> Texture : register(t0);
SamplerState Anisotropic : register(s0);
cbuffer CameraData : register(b0)
{
    float4x4 Model;
    float4x4 View;
    float4x4 Projection;
};
cbuffer TimeVariantSignals : register(b1)
{
    float SineWave;
    float SquareWave;
    float TriangleWave;
    float SawtoothWave;
};
// @@@ End Header

// This is the default code in the source section.
// @@@ Begin Source
export void VertexFunction(
    inout float4 position, inout float2 texcoord, inout float4 normal)
{
    position = mul(position, Model);
    position = mul(position, View);
    position = mul(position, Projection);
    normal = mul(normal, Model);
}

export float3 ColorFunction(float2 texcoord, float3 normal)
{
    return Texture.Sample(Anisotropic, texcoord);
}
// @@@ End Source

// This code is not displayed, but is used as part of the linking process.
// @@@ Begin Hidden
cbuffer HiddenBuffer : register(b2)
{
    float3 LightDirection;
};

export float3 AddLighting(float3 color, float3 normal)
{
    static const float ambient = 0.2f;
    float brightness = ambient + (1.0f - ambient) * saturate(dot(normal, LightDirection));
    return color * brightness;
}

export float3 AddDepthFog(float3 color, float depth)
{
    float3 fogColor = float3(0.4f, 0.9f, 0.5f); // Greenish.
    return lerp(color, fogColor, exp2(-depth));
}

export float3 AddGrayscale(float3 color)
{
    float luminance = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
    return float3(luminance, luminance, luminance);
}

export float4 Float3ToFloat4SetW1(float3 value)
{
    // Convert a float3 value to a float4 value with the w component set to 1.0f.
    // Used for initializing homogeneous 3D coordinates and generating fully opaque color values.
    return float4(value, 1.0f);
}
// @@@ End Hidden
