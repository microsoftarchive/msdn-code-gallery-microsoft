//--------------------------------------------------------------------------------------
// File: DownScaleBrightPass9.hlsl
//
// A PS used in the D3D Post Processing sample
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
static const float  brightThreshold = 0.35f;

SamplerState linearSampler : register (s0);

struct QuadVertexShaderOutput
{
    float4 pos : SV_POSITION;              
    float2 tex : TEXCOORD0;
};

Texture2D s0 : register(t0);

cbuffer cb0
{
    float pixelWidth;
    float pixelHeight;
};

float4 DownScaleBrightPass9(QuadVertexShaderOutput input) : SV_TARGET
{
    float3 brightColor = 0.0f;

    // Gather 16 adjacent pixels (each bilinear sample reads a 2x2 region)
    brightColor  = s0.Sample(linearSampler, input.tex + float2(-pixelWidth,-pixelHeight)).rgb;
    brightColor += s0.Sample(linearSampler, input.tex + float2( pixelWidth,-pixelHeight)).rgb;
    brightColor += s0.Sample(linearSampler, input.tex + float2(-pixelWidth, pixelHeight)).rgb;
    brightColor += s0.Sample(linearSampler, input.tex + float2( pixelWidth, pixelHeight)).rgb;
    brightColor /= 4.0f;

    // Brightness thresholding
    brightColor = max(0.0f, brightColor - brightThreshold);

    return float4(brightColor, 1.0f);

}
