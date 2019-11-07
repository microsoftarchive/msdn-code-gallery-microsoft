//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

static const int stepIterations = 4;

Texture2D temp;
SamplerState pointSampler;

struct PSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float2 testComplexNumber : TEXCOORD1;
};

// This shader computes the escape speed of different points in the complex plane to
// generate the Mandelbrot set.  It loads temporary values from an intermediate texture,
// runs several iterations, and saves temporary values back to an intermediate texture.

float4 main(PSInput input) : SV_TARGET
{
    float4 tempValue = temp.Sample(pointSampler, input.tex);
    if (tempValue.w > 0.0f)
    {
        return tempValue;
    }
    int iteration = 0;
    float2 coord = tempValue.xy;
    float2 number = input.testComplexNumber;
    float4 ret = float4(0,0,0,0);
    [unroll]
    for (iteration = 0; iteration < stepIterations; iteration++)
    {
        // Determine how quickly coord escapes to infinity, if at all,
        // using the equation coord(i+1) = coord(i)^2 + base.  If coord
        // does not escape, base is part of the Mandelbrot set.
        coord = float2(
            coord.x * coord.x - coord.y * coord.y + number.x, // real component
            2.0f * coord.x * coord.y + number.y // imaginary component
            );
        // If abs(coord(i)) is ever greater than 2, then coord will continue
        // to escape to infinity, and base is not part of the Mandelbrot set.
        if (coord.x * coord.x + coord.y * coord.y > 2.0f * 2.0f)
        {
            ret.w = 1.0f;
            break;
        }
    }
    ret.xy = coord;
    ret.z = tempValue.z + (float)iteration;
    return ret;
}
