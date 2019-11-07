//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

static const int targetIterations = 256;

struct FractalBufferElement
{
    int iteration;
};

cbuffer BufferInfoCB : register(b0)
{
    int bufferRowWidth;
};

cbuffer ComputeCB : register(b1)
{
    float2 originDtid00;
    float2 deltaPerDtidX;
    float2 deltaPerDtidY;
};

RWStructuredBuffer<FractalBufferElement> next;

// This shader computes the escape speed of different points in the complex plane to
// generate the Mandelbrot set.  It runs the target number of iterations, and stores
// the escape iteration to a structured buffer.

[numthreads(16,16,1)]
void main(uint3 dtid : SV_DispatchThreadID)
{
    int iteration;
    float2 coord = float2(0,0);
    float2 number = originDtid00 + dtid.x * deltaPerDtidX + dtid.y * deltaPerDtidY;
    for (iteration = 0; iteration < targetIterations; iteration++)
    {
        // Determine how quickly coord escapes to infinity, if at all,
        // using the equation coord(i+1) = coord(i)^2 + number.  If coord
        // does not escape, number is part of the Mandelbrot set.
        coord = float2(
            coord.x * coord.x - coord.y * coord.y + number.x, // real component
            2.0f * coord.x * coord.y + number.y // imaginary component
            );
        // If abs(coord(i)) is ever greater than 2, then coord will continue
        // to escape to infinity, and number is not part of the Mandelbrot set.
        if (coord.x * coord.x + coord.y * coord.y > 2.0f * 2.0f)
        {
            break;
        }
    }
    FractalBufferElement ret;
    ret.iteration = iteration;
    next[dtid.y * bufferRowWidth + dtid.x] = ret;
}
