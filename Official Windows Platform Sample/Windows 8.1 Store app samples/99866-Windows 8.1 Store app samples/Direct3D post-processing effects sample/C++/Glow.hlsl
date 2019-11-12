//--------------------------------------------------------------------------------------
// File: Glow.hlsl
//
// A Pixel Shader used to apply separable blur kernel to the brightness threshold image
// to produce the glow image for the D3D Post Processing sample
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------


SamplerState pointSampler : register (s0);

struct QuadVertexShaderOutput
{
    float4 pos : SV_POSITION;              
    float2 tex : TEXCOORD0;
};

Texture2D s0 : register(t0);

cbuffer cb0
{
    float2 sampleOffsets[15];
    float4 sampleWeights[15];
}

float4 Glow(QuadVertexShaderOutput input) : SV_TARGET
{    
    float4 glow = 0.0f;
    float4 color = 0.0f;
    float2 samplePosition;

    for(int sampleIndex = 0; sampleIndex < 15; sampleIndex++)
    {
        // Sample from adjacent points
        samplePosition = input.tex + sampleOffsets[sampleIndex];
        color = s0.Sample(pointSampler, samplePosition);

        glow += sampleWeights[sampleIndex] * color;
    }

    return glow;
}
