// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

cbuffer SimpleConstantBuffer : register(b0)
{
    matrix model;
    matrix view;
    matrix projection;
};

struct VertexShaderInput
{
    float3 pos : POSITION;
    float3 norm : NORMAL;
    float2 tex : TEXCOORD0;
};

struct PixelShaderInput
{
    float4 Pos : SV_POSITION;
    float3 WorldNorm : TEXCOORD0;
    float3 CameraPos : TEXCOORD1;
    float3 WorldPos : TEXCOORD2;
};

PixelShaderInput SimpleVertexShader(VertexShaderInput input)
{
    PixelShaderInput output = (PixelShaderInput)0;
    float4 worldPos = mul(float4(input.pos, 1), model);
    float4 cameraPos = mul(worldPos, view);

    output.WorldPos = worldPos.xyz;
    output.WorldNorm = normalize(mul(input.norm, (float3x3)model));
    output.CameraPos = cameraPos.xyz;
    output.Pos = mul(cameraPos, projection);

    return output;
}
