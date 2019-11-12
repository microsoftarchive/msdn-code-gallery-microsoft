// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

cbuffer SimpleConstantBuffer : register( b0 )
{
    matrix model;
    matrix view;
    matrix projection;
	float3 toLight;
};

struct sVSInput
{
    float3 pos : POSITION;
    float2 tex : TEXCOORD0;
    float3 utan : TANGENT0;
	float3 vtan : TANGENT1;
};

struct sPSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
	float3 utan : TANGENT0;
    float3 vtan : TANGENT1;
};

sPSInput SimpleVertexShader( sVSInput input )
{
    sPSInput output;
    float4 temp = float4( input.pos, 1.0f );
    temp = mul( temp, model );
    temp = mul( temp, view );
    temp = mul( temp, projection );
    output.pos = temp;

    output.tex = input.tex;

    output.utan = mul( float4( input.utan, 1.0f ), model ).xyz;		// convert u-tangent to world coords
    output.vtan = mul( float4( input.vtan, 1.0f ), model ).xyz;		// convert v-tangent to world coords

    return output;
}
