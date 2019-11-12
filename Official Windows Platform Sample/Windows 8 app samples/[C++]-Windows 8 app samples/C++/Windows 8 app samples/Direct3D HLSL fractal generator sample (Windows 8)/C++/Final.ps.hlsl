//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

Texture2D temp : register(t0);
Texture2D gradient : register(t1);
SamplerState pointSampler : register(s0);
SamplerState linearSampler : register(s1);

static const int finalIterations = 4;
static const int targetIterations = 256;

struct PSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float2 testComplexNumber : TEXCOORD1;
};

// This shader computes the escape speed of different points in the complex plane to
// generate the Mandelbrot set.  It loads temporary values from an intermediate texture,
// runs several iterations, and returns a color based on a predefined iteration-color gradient.

float4 main(PSInput input) : SV_TARGET
{
    float4 tempValue = temp.Sample(pointSampler,input.tex);

    int iteration = 0;
    float2 coord = tempValue.xy;
    float2 number = input.testComplexNumber;
    [unroll]
    for (iteration = 0; iteration < finalIterations; iteration++)
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
            break;
        }
    }
    float sampleCoord = iteration == finalIterations ? 1.0f :
        (tempValue.z + (float)iteration) / (float)targetIterations;
    return gradient.Sample(linearSampler, float2(sampleCoord, 0.5f));
}
