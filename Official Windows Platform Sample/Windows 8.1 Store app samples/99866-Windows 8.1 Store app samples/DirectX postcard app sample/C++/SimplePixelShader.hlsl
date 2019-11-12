// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

struct PixelShaderInput
{
    float4 Pos : SV_POSITION;
    float3 WorldNorm : TEXCOORD0;
    float3 CameraPos : TEXCOORD1;
    float3 WorldPos : TEXCOORD2;
};

struct ColorsOutput
{
    float4 Diffuse;
    float4 Specular;
};

ColorsOutput CalcLighting( float3 worldNormal, float3 worldPos, float3 cameraPos)
{
    ColorsOutput output = (ColorsOutput)0.0;
    
    float specularBrightness = 0.7;
    float shininess = 75;
    float4 specular = float4(specularBrightness * float3(1, 1, 1), 1);

    float ambient = 0.4f;

    float4 lightColor = float4(0.9f, 0.0f, 0.0f, 1.0f);

    float3 lightPosition = float3(0, -250, -1000.0f);
    float3 lightDir = normalize(lightPosition - worldPos);

    output.Diffuse += min(max(0, dot(lightDir, worldNormal)) + ambient, 1) * lightColor;

    float3 halfAngle = normalize(normalize(cameraPos) + lightDir);
    output.Specular += max(0, pow(abs(max(0, dot(halfAngle, worldNormal))), shininess) * specular);
    
    return output;
}

float4 SimplePixelShader(PixelShaderInput input) : SV_TARGET
{
    float4 finalColor = 0;
    
    ColorsOutput cOut = CalcLighting(input.WorldNorm, input.WorldPos, input.CameraPos);

    finalColor += cOut.Diffuse + cOut.Specular;

    finalColor.a = 1;
    return finalColor;

}
