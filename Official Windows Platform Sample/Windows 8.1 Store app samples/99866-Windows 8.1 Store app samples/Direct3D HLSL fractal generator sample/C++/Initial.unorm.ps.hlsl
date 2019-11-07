//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

static const int initialIterations = 4;
static const int targetIterations = 256;

struct PSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float2 testComplexNumber : TEXCOORD1;
};

// This shader computes the escape speed of different points in the complex plane to
// generate the Mandelbrot set.  It runs several iterations, and saves temporary values
// to an intermediate texture.  The intermediate texture is in a UNORM format, so values
// must be compacted to fit within 8 bits representing the range 0..1.

float4 main(PSInput input) : SV_TARGET
{
    int iteration;
    float2 coord = float2(0,0);
    float2 number = input.testComplexNumber;
    float4 ret = float4(0,0,0,0);
    [unroll]
    for (iteration = 0; iteration < initialIterations; iteration++)
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
    // Compact values into UNORM (8-bit fixed-point values in 0..1 range).
    ret.xy = (coord + float2(2.5f, 2.5f)) / 5.0f;
    ret.z = (float)iteration / (float)targetIterations;
    return ret;
}
