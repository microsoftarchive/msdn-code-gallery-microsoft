//--------------------------------------------------------------------------------------
// File: DownScale3x3BrightPass.hlsl
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

float4 DownScale3x3BrightPass(QuadVertexShaderOutput input) : SV_TARGET
{   
    float3 brightColor = 0.0f;

    // Gather 16 adjacent pixels (each bilinear sample reads a 2x2 region)
    brightColor  = s0.Sample(linearSampler, input.tex, int2(-1,-1)).rgb;
    brightColor += s0.Sample(linearSampler, input.tex, int2( 1,-1)).rgb;
    brightColor += s0.Sample(linearSampler, input.tex, int2(-1, 1)).rgb;
    brightColor += s0.Sample(linearSampler, input.tex, int2( 1, 1)).rgb;
    brightColor /= 4.0f;

    // Brightness thresholding
    brightColor = max(0.0f, brightColor - brightThreshold);

    return float4(brightColor, 1.0f);
}
