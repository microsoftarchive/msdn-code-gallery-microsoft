//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

struct PS_INPUT
{
	float4 Position : SV_POSITION;  // interpolated vertex position (system value)
	float4 Color    : COLOR0;       // interpolated diffuse color
};

struct PS_OUTPUT
{
	float4 RGBColor : SV_TARGET;  // pixel color (your PS computes this system value)
};

PS_OUTPUT main(PS_INPUT In)
{
	PS_OUTPUT Output;

	Output.RGBColor = In.Color;

	return Output;
}


// You can return a float4 (instead of a struct) by applying the semantic like so.
/*
float4 main(PS_INPUT In) : SV_TARGET
{
return In.Color;
}
*/