// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

cbuffer RenderTargetInfoCB
{
    float2 renderTargetSize;
};

struct VertexShaderInput
{
    float2 pos : POSITION;
    float2 tex : TEXCOORD0;
    float2 origin : TRANSFORM0;
    float2 offset : TRANSFORM1;
    float rotation : TRANSFORM2;
    float4 color : COLOR0;
};

struct PixelShaderInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float4 color : COLOR0;
};

// This shader takes a fixed-position vertex buffer, and using per-sprite
// instance data, transforms the vertices so that the sprite is rendered
// at the desired position, rotation, and scale.

PixelShaderInput main(VertexShaderInput input)
{
    PixelShaderInput ret;
    float sinRotation;
    float cosRotation;
    sincos(input.rotation, sinRotation, cosRotation);
    float2 posDelta = input.pos * input.offset;
    posDelta = float2(
        posDelta.x * cosRotation - posDelta.y * sinRotation,
        posDelta.x * sinRotation + posDelta.y * cosRotation
        );
    posDelta /= renderTargetSize;
    ret.pos = float4(input.origin + posDelta, 0.5f, 1.0f);
    ret.tex = input.tex;
    ret.color = input.color;
    return ret;
}
