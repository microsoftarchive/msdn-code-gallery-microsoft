//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

static const int targetIterations = 256;

struct FractalBufferElement
{
    int iteration;
};

cbuffer BufferInfoCB
{
    int bufferRowWidth;
};

StructuredBuffer<FractalBufferElement> buffer : register(t0);
Texture2D gradient : register(t1);
SamplerState linearSampler;

struct PSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float2 unused : TEXCOORD1;
};

float4 main(PSInput input) : SV_TARGET
{
    int i = buffer[floor(input.tex.y) * bufferRowWidth + floor(input.tex.x)].iteration;
    return gradient.Sample(linearSampler, float2((float)i / (float)targetIterations, 0.5f));
}
