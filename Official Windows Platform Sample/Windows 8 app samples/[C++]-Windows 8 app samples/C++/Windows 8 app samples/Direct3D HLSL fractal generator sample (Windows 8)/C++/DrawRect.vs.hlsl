//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

cbuffer DrawRectCB
{
    float4x4 boundsTransform;
    float2 texScale;
};

struct VSInput
{
    float2 pos : POSITION;
    float2 tex : TEXCOORD0;
    float2 bounds : TEXCOORD1;
};

struct PSInput
{
    float4 pos : SV_POSITION;              
    float2 tex : TEXCOORD0;
    float2 bounds : TEXCOORD1;
};

// This is a general-purpose vertex shader for all Fractal generation techniques.

PSInput main(VSInput input)
{
    PSInput output;
    output.pos = float4(input.pos, 0.5f, 1.0f);
    float4 bounds = float4(input.bounds, 0.0f, 1.0f);
    bounds = mul(bounds, boundsTransform);
    output.bounds = bounds.xy;
    output.tex = input.tex * texScale;
    return output;
}
