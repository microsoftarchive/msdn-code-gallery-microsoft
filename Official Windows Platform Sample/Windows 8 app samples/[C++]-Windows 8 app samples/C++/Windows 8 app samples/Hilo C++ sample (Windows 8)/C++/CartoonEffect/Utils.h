// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

class Utils
{
public:
    static const float64 Wr;
    static const float64 Wb;
    static const float64 Wg;

    static inline void RGBToYUV(COLORREF color, float64& y, float64& u, float64& v)
    {
        float64 r = GetRValue(color) / 255.0;
        float64 g = GetGValue(color) / 255.0;
        float64 b = GetBValue(color) / 255.0;

        y = Wr * r + Wb * b + Wg * g;
        u = 0.436 * (b - y) / (1 - Wb);
        v = 0.615 * (r - y) / (1 - Wr);
    }

    static inline float64 GetDistance(COLORREF color1, COLORREF color2)
    {
        float64 y1, u1, v1, y2, u2, v2;
        RGBToYUV(color1, y1, u1, v1);
        RGBToYUV(color2, y2, u2, v2);

        return sqrt(pow(u1 - u2, 2) + pow(v1 - v2, 2));
    }

    static inline float SmoothStep(float a, float b, float x)
    {
        if (x < a) return 0.0f;
        else if (x >= b) return 1.0f;

        x = (x - a) / (b - a);
        return (x * x * (3.0f - 2.0f * x));
    }
};