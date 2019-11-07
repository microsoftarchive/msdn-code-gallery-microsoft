//--------------------------------------------------------------------------------------
// File: QuadVertexShader.hlsl
//
// A simple pass-through Vertex Shader used for rendering the full window Quad in the 
// D3D Post Processing sample
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------

struct QuadVertexShaderInput
{
    float4 pos : POSITION;
    float2 tex : TEXCOORD0;
};

struct QuadVertexShaderOutput
{
    float4 pos : SV_POSITION;              
    float2 tex : TEXCOORD0;
};

QuadVertexShaderOutput QuadVertexShader(QuadVertexShaderInput input)
{
    QuadVertexShaderOutput output;
    output.pos = input.pos;
    output.tex = input.tex;
    return output;
}