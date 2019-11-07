//--------------------------------------------------------------------------------------
// File: FinalPass.hlsl
//
// A PS used in D3D Post Processing sample
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
static const float  glowScale = 4.0f; // This controls glow intensity

SamplerState pointSampler : register (s0);
SamplerState linearSampler : register (s1);

struct QuadVertexShaderOutput
{
    float4 pos : SV_POSITION;              
    float2 tex : TEXCOORD0;
};

Texture2D s0 : register(t0);
Texture2D s1 : register(t1);

float4 FinalPass(QuadVertexShaderOutput input) : SV_TARGET
{   

    float4 sceneColor = s0.Sample(pointSampler, input.tex);         // sample scene image from intermediate texture

    float3 glowColor = s1.Sample(linearSampler, input.tex).rgb;     // sample from final glow image    

    sceneColor.rgb += glowScale * glowColor;                        // scale up the glow and add to the scene image
    sceneColor.a = 1.0f;

    return sceneColor;
}