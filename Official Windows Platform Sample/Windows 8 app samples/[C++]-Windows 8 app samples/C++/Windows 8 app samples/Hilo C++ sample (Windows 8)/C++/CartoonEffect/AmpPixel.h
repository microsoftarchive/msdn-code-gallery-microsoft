// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#define C_WR 0.229F
#define C_WB 0.114F
#define C_WG (1.0F - C_WR - C_WB)
#define C_UMAX 0.436F
#define C_VMAX 0.615F

// Data structure used to store pixel information that can be used on the GPU
struct AmpPixel
{
    float y; // Luma
    float u; // Blue difference + 0.5
    float v; // Red difference + 0.5

    float r; // Red
    float g; // Green
    float b; // Blue

    AmpPixel() restrict(cpu, amp) : y(0), u(0), v(0), r(0), g(0), b(0) {}

    // Set YUV and RGB values from given RGB values
    inline void SetFromRGB(float r, float g, float b) restrict(cpu, amp)
    {
        y = C_WR * r + C_WB * b + C_WG * g;
        u = C_UMAX * (b - y) / (1 - C_WB) + 0.5F;
        v = C_VMAX * (r - y) / (1 - C_WR) + 0.5F;
        this->r = r;
        this->g = g;
        this->b = b;
    }

    // Update the RGB values based on the current YUV values
    inline void UpdateRGB() restrict(cpu, amp)
    {
        float u = this->u - 0.5F;
        float v = this->v - 0.5F;
        r = y   + v * (1 - C_WR) / C_VMAX;
        g = y   - u * C_WB * (1 - C_WB) / C_WG * C_UMAX
            - v * C_WR * (1 - C_WR) / C_WG * C_VMAX;
        b = y   + u * (1 - C_WB) / C_UMAX;
    }
};
