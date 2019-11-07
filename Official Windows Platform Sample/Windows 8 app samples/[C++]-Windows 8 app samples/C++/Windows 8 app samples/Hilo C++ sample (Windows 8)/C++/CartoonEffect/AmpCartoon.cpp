// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "AmpPixel.h"
#include <amp.h>
#include <amp_math.h>

using namespace std;
using concurrency::cancellation_token;
using concurrency::array;
using concurrency::array_view;
using concurrency::index;
using concurrency::extent;

void SimplifyIndexRows(const array_view<const AmpPixel, 2>& src_frame, array_view<AmpPixel, 2> dst_frame, index<2> idx, int shift) restrict(amp)
{
    // Simplify a portion of the image by removing features that fall below a given color contrast.
    // We do this by calculating the difference in color between neighboring pixels and applying an
    // exponential decay function then summing the results.
    // We then set the RGB values to the nomalized sums
    float sum = 0;
    float partial_sum_r = 0, partial_sum_g = 0, partial_sum_b = 0;

    // k is the exponential decay constant and is calculated from a standard deviation of 0.025
    const float standard_deviation = 0.025f;
    const float k = -0.5f / (standard_deviation * standard_deviation);

    const int idx_y = idx[1];
    const int y = idx_y;
    const int idx_x = idx[0] + shift;
    const int x_start = idx_x - shift;
    const int x_end = idx_x + shift;

    AmpPixel org_clr = src_frame(idx_x, idx_y);

    for (int x = x_start; x <= x_end; ++x)
    {
        // Don't apply filter to the requested index, only to the neighbors
        if (x != idx_x ) 
        {
            AmpPixel clr = src_frame(x, y);
            float distance = concurrency::fast_math::sqrt(
                concurrency::fast_math::pow(org_clr.u - clr.u, 2) + 
                concurrency::fast_math::pow(org_clr.v - clr.v, 2));
            float value = concurrency::fast_math::pow(float(M_E), k * distance * distance);

            sum += value;
            partial_sum_r += clr.r * value;
            partial_sum_g += clr.g * value;
            partial_sum_b += clr.b * value;
        }
    }

    partial_sum_r = min(max(partial_sum_r / sum, 0.0F), 1.0F);
    partial_sum_g = min(max(partial_sum_g / sum, 0.0F), 1.0F);
    partial_sum_b = min(max(partial_sum_b / sum, 0.0F), 1.0F);

    AmpPixel p;
    p.SetFromRGB(partial_sum_r, partial_sum_g, partial_sum_b);
    dst_frame(idx_x, idx_y) = p;
}

void SimplifyIndexColumns(const array_view<const AmpPixel, 2>& src_frame, array_view<AmpPixel, 2> dst_frame, index<2> idx, int shift) restrict(amp)
{
    // Simplify a portion of the image by removing features that fall below a given color contrast.
    // We do this by calculating the difference in color between neighboring pixels and applying an
    // exponential decay function then summing the results.
    // We then set the RGB values to the nomalized sums
    float sum = 0;
    float partial_sum_r = 0, partial_sum_g = 0, partial_sum_b = 0;

    // k is the exponential decay constant and is calculated from a standard deviation of 0.025
    const float standard_deviation = 0.025f;
    const float k = -0.2f / (standard_deviation * standard_deviation);

    const int idx_y = idx[1] + shift;
    const int idx_x = idx[0];
    const int x = idx_x;
    const int y_start = idx_y - shift;
    const int y_end = idx_y + shift;

    AmpPixel org_clr = src_frame(idx_x, idx_y);

    for (int y = y_start; y <= y_end; ++y)
    {
        // Don't apply filter to the requested index, only to the neighbors
        if (y != idx_y) 
        {
            AmpPixel clr = src_frame(x, y);
            float distance = concurrency::fast_math::sqrt(
                concurrency::fast_math::pow(org_clr.u - clr.u, 2) + 
                concurrency::fast_math::pow(org_clr.v - clr.v, 2));
            float value = concurrency::fast_math::pow(float(M_E), k * distance * distance);

            sum += value;
            partial_sum_r += clr.r * value;
            partial_sum_g += clr.g * value;
            partial_sum_b += clr.b * value;
        }
    }

    partial_sum_r = min(max(partial_sum_r / sum, 0.0F), 1.0F);
    partial_sum_g = min(max(partial_sum_g / sum, 0.0F), 1.0F);
    partial_sum_b = min(max(partial_sum_b / sum, 0.0F), 1.0F);

    AmpPixel p;
    p.SetFromRGB(partial_sum_r, partial_sum_g, partial_sum_b);
    dst_frame(idx_x, idx_y) = p;
}

void CalculateSobel(const array_view<const AmpPixel, 2> &src_frame, index<2> idx, float &dy) restrict(amp)
{
    // Apply the Sobel operator to the image.  The Sobel operation is analogous
    // to a first derivative of the grayscale part of the image. Portions of
    // the image that change rapidly (edges) have higher sobel values.
    const int gx[3][3] = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
    const int gy[3][3] = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

    const int idx_x = idx[0];
    const int idx_y = idx[1];
    float gradient_x = 0, gradient_y = 0;
    for (int y = -1; y < 2; y++)
    {
        for (int x = -1; x < 2; x++)
        {
            index<2> idx_new(idx_x + x, idx_y + y);
            float src_y = src_frame[idx_new].y;

            gradient_x += gx[x + 1][y + 1] * src_y;
            gradient_y += gy[x + 1][y + 1] * src_y;
        }
    }

    // Calculate the magnitude of the gradient from the horizontal and vertical gradients
    dy = concurrency::fast_math::sqrt((gradient_x * gradient_x) + (gradient_y * gradient_y));
}

void EdgeDetection(const array<AmpPixel, 2> &src_frame, array<AmpPixel, 2> &dst_frame, index<2> idx, const array_view<const AmpPixel, 2> &org_frame) restrict(amp)
{
    const float alpha = 0.3f; // Weighting of original frame for edge detection
    const float beta = 0.8f;  // Weighting of source (color simplied) frame for edge detection

    const float s0 = 0.054f;  // Minimum threshold of source frame Sobel value to detect an edge
    const float s1 = 0.064f;  // Maximum threshold of source frame Sobel value to effect the darkness of the edge
    const float a0 = 0.3f;    // Minimum threshold of original frame Sobel value to detect an edge
    const float a1 = 0.7f;    // Maximum threshold of original frame Sobel value to effect the darkness of the edge

    index<2> idxc(idx[0] + 1, idx[1] + 1);  // Corrected index for border offset

    float sy = 0.0f;
    float ay = 0.0f;

    CalculateSobel(src_frame, idxc, sy);
    CalculateSobel(org_frame, idxc, ay);

    float edge_s = (1 - alpha) * sy;
    float edge_a = (1 - alpha) * ay;
    float i = (1 - beta) * concurrency::direct3d::smoothstep(s0, s1, edge_s) + beta * concurrency::direct3d::smoothstep(a0, a1, edge_a);

    AmpPixel clr = src_frame[idxc];
    clr.UpdateRGB();

    clr.r = clr.r * (1 - i);
    clr.g = clr.g * (1 - i);
    clr.b = clr.b * (1 - i);
    dst_frame[idxc] = clr;
}

void PopulateInitialRGBValues(concurrency::array<AmpPixel, 2>* frame, concurrency::array<AmpPixel, 2> sourceFrame)
{
    concurrency::array_view<AmpPixel, 2> currentFrameView((*frame));
    parallel_for_each(sourceFrame.extent, [=, &sourceFrame](index<2> idx) restrict(amp)
    {
        currentFrameView[idx] = sourceFrame[idx];
    });
}

void ApplyColorSimplifier(concurrency::array<AmpPixel, 2> frames[], 
                          const unsigned int width, 
                          const unsigned int height, 
                          const unsigned int neighborWindow, 
                          const unsigned int phases, 
                          unsigned int current, 
                          unsigned int next, 
                          cancellation_token token)
{
    concurrency::extent<2> computeDomain(height - neighborWindow, width - neighborWindow);
    for (unsigned int i = 0; i < phases; i++)
    {
        const concurrency::array_view<const AmpPixel, 2> source(frames[current]);
        concurrency::array_view<AmpPixel, 2> destination(frames[next]);
        const unsigned int shift = neighborWindow;
            
        parallel_for_each(computeDomain, [=](index<2> idx) restrict(amp)
        {
            SimplifyIndexColumns(source, destination, idx, shift);
            SimplifyIndexRows(source, destination, idx, shift);
        });

        swap(current, next);
        if (token.is_canceled())
        {
            break;
        }
    }
}

void ApplyEdgeDetection(concurrency::array<AmpPixel, 2>* frame, 
                        concurrency::array<AmpPixel, 2> sourceFrame, 
                        concurrency::array<AmpPixel, 2> destFrame, 
                        unsigned int width, 
                        unsigned int height, 
                        const unsigned int phases)
{
    if (phases > 0)
    {
        const concurrency::array<AmpPixel, 2> &source((*frame));
        parallel_for_each(destFrame.section(1, 1, height - 1, width - 1).extent, [=, &source, &sourceFrame, &destFrame](index<2> idx) restrict(amp)
        {
            EdgeDetection(source, destFrame, idx, sourceFrame);
        });
        (*frame) = destFrame;
    }
}

void ApplyCartoonEffectAmp(AmpPixel* sourcePixels, 
                           AmpPixel* destinationPixels,
                           unsigned int width, 
                           unsigned int height, 
                           unsigned int neighborWindow, 
                           unsigned int phases, 
                           cancellation_token token)
{
    concurrency::extent<2> sourceExtent(height, width);
    concurrency::array<AmpPixel, 2> sourceFrame(sourceExtent, sourcePixels);
    concurrency::array<AmpPixel, 2> destFrame(sourceExtent);
    concurrency::array<AmpPixel, 2> frames[2] = 
    {
        concurrency::array<AmpPixel, 2>(sourceFrame.extent),
        concurrency::array<AmpPixel, 2>(sourceFrame.extent)
    };
    unsigned int current = 0;
    unsigned int next = 1;

    PopulateInitialRGBValues(&frames[current], sourceFrame);
    if (token.is_canceled())
    {
        return;
    }
    
    ApplyColorSimplifier(frames, width, height, neighborWindow, phases, current, next, token);
    ApplyEdgeDetection(&frames[current], sourceFrame, destFrame, width, height, phases);
    if (token.is_canceled())
    {
        return;
    }

    concurrency::copy(frames[current], stdext::checked_array_iterator<AmpPixel*>(destinationPixels, width * height));
}