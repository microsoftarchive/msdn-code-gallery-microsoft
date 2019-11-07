//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

static const int targetIterations = 256;

Texture2D gradient;
SamplerState linearSampler;

struct PSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float2 testComplexNumber : TEXCOORD1;
};

// This shader computes the escape speed of different points in the complex plane to
// generate the Mandelbrot set.  It runs the target number of iterations, and returns
// a color based on a predefined iteration-color gradient.

float4 main(PSInput input) : SV_TARGET
{
    int iteration;
    float2 coord = float2(0,0);
    float2 number = input.testComplexNumber;
    for (iteration = 0; iteration < targetIterations; iteration++)
    {
        // Determine how quickly coord escapes to infinity, if at all,
        // using the equation coord(i+1) = coord(i)^2 + number.  If coord
        // does not escape, number is part of the Mandelbrot set.
        coord = float2(
            coord.x * coord.x - coord.y * coord.y + number.x, // real component
            2 * coord.x * coord.y + number.y // imaginary component
            );
        // If abs(coord(i)) is ever greater than 2, then coord will continue
        // to escape to infinity, and number is not part of the Mandelbrot set.
        if (coord.x * coord.x + coord.y * coord.y > 2.0f * 2.0f)
        {
            break;
        }
    }
    return gradient.Sample(linearSampler, float2(((float)iteration) / (float)(targetIterations), 0.5f));
}
