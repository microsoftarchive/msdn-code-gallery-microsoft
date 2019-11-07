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

struct GeometryShaderInput
{
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

// This shader generates two triangles that will be used to draw the sprite.
// The vertex properties are calculated based on the per-sprite instance data
// passed in from the vertex shader.

[maxvertexcount(4)]
void main(point GeometryShaderInput input[1], inout TriangleStream<PixelShaderInput> spriteStream)
{
    float sinRotation;
    float cosRotation;
    sincos(input[0].rotation, sinRotation, cosRotation);

    float2 texCoord[4];
    texCoord[0] = float2(0,0);
    texCoord[1] = float2(1,0);
    texCoord[2] = float2(0,1);
    texCoord[3] = float2(1,1);

    float2 posDelta[4];
    posDelta[0] = float2(-input[0].offset.x,  input[0].offset.y);
    posDelta[1] = float2( input[0].offset.x,  input[0].offset.y);
    posDelta[2] = float2(-input[0].offset.x, -input[0].offset.y);
    posDelta[3] = float2( input[0].offset.x, -input[0].offset.y);

    spriteStream.RestartStrip();
    [unroll]
    for (int i = 0; i < 4; i++)
    {
        posDelta[i] = float2(
            posDelta[i].x * cosRotation - posDelta[i].y * sinRotation,
            posDelta[i].x * sinRotation + posDelta[i].y * cosRotation
            );
        posDelta[i] /= renderTargetSize;
        PixelShaderInput streamElement;
        streamElement.pos = float4(input[0].origin + posDelta[i], 0.5f, 1.0f);
        streamElement.tex = texCoord[i];
        streamElement.color = input[0].color;
        spriteStream.Append(streamElement);
    }
    spriteStream.RestartStrip();
}
