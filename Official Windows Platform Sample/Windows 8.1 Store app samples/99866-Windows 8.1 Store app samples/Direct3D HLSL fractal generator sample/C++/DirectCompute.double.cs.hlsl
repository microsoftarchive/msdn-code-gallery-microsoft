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
    // Double-precision value representations cannot be used in constant buffers.
    // Instead, the data must be interpreted as a 32-bit format, then converted
    // using the asdouble intrinsic function.
    uint4 originDtid00AsUints;
    uint4 deltaPerDtidXAsUints;
    uint4 deltaPerDtidYAsUints;
};

RWStructuredBuffer<FractalBufferElement> next;

double2 UnpackDouble2(uint4 valueAsUint4)
{
    // If represented as a struct of two doubles on the CPU,
    // the lower 32 bits of each double will be saved in the
    // x and z components, and the upper 32 bits in the y and
    // w components, respectively.
    return asdouble(valueAsUint4.xz, valueAsUint4.yw);
}

// This shader computes the escape speed of different points in the complex plane to
// generate the Mandelbrot set.  It runs the target number of iterations, and stores
// the escape iteration to a structured buffer.  It uses double-precision values to
// enable deeper zoom.

[numthreads(16,16,1)]
void main(uint3 dtid : SV_DispatchThreadID)
{
    double2 originDtid00 = UnpackDouble2(originDtid00AsUints);
    double2 deltaPerDtidX = UnpackDouble2(deltaPerDtidXAsUints);
    double2 deltaPerDtidY = UnpackDouble2(deltaPerDtidYAsUints);
    int iteration;
    double2 coord = double2(0,0);
    double2 number = originDtid00 + dtid.x * deltaPerDtidX + dtid.y * deltaPerDtidY;
    for (iteration = 0; iteration < targetIterations; iteration++)
    {
        // Determine how quickly coord escapes to infinity, if at all,
        // using the equation coord(i+1) = coord(i)^2 + number.  If coord
        // does not escape, number is part of the Mandelbrot set.
        coord = double2(
            coord.x * coord.x - coord.y * coord.y + number.x, // real component
            2.0l * coord.x * coord.y + number.y // imaginary component
            );
        // If abs(coord(i)) is ever greater than 2, then coord will continue
        // to escape to infinity, and number is not part of the Mandelbrot set.
        if (coord.x * coord.x + coord.y * coord.y > 2.0l * 2.0l)
        {
            break;
        }
    }
    FractalBufferElement ret;
    ret.iteration = iteration;
    next[dtid.y * bufferRowWidth + dtid.x] = ret;
}
