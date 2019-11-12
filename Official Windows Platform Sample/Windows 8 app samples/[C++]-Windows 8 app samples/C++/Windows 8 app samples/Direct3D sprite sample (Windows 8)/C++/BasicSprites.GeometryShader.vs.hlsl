// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

struct VertexShaderInput
{
    float2 origin : TRANSFORM0;
    float2 offset : TRANSFORM1;
    float rotation : TRANSFORM2;
    float4 color : COLOR0;
};

struct GeometryShaderInput
{
    float2 origin : TRANSFORM0;
    float2 offset : TRANSFORM1;
    float rotation : TRANSFORM2;
    float4 color : COLOR0;
};

// This shader simply passes per-sprite instance data to the geometry shader.

GeometryShaderInput main(VertexShaderInput input)
{
    GeometryShaderInput ret;
    ret.origin = input.origin;
    ret.offset = input.offset;
    ret.rotation = input.rotation;
    ret.color = input.color;
    return ret;
}
