//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

Texture2D tex2D : register(t0);
SamplerState linearSampler : register(s0);

struct PSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float2 bounds : TEXCOORD1;
};

// This pixel shader simply renders the bound texture using linear sampling.

float4 main(PSInput input) : SV_TARGET
{
    return tex2D.Sample(linearSampler, input.tex);
}
