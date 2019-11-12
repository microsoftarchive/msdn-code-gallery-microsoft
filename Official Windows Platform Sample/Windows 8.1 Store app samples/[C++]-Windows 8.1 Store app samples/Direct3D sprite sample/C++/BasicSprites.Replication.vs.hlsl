// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

struct VertexShaderInput
{
    float2 pos : POSITIONT;
    float2 tex : TEXCOORD0;
    float4 color : COLOR0;
};

struct PixelShaderInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float4 color : COLOR0;
};

// This shader simply passes pre-transformed vertex data to the pixel shader.

PixelShaderInput main(VertexShaderInput input)
{
    PixelShaderInput ret;
    ret.pos = float4(input.pos, 0.5f, 1.0f);
    ret.tex = input.tex;
    ret.color = input.color;
    return ret;
}
