// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include <ppltasks.h>
#include "FrameProcessing.h"
#include "FrameData.h"
#include "Utils.h"

using namespace concurrency;

const float64 Utils::Wr = 0.299;
const float64 Utils::Wb = 0.114;
const float64 Utils::Wg = 1 - Utils::Wr - Utils::Wb;

FrameProcessing::FrameProcessing() 
    : m_size(0)
    , m_BPP(0)
    , m_pitch(0)
    , m_neighborWindow(3)
    , m_pCurrentImage(nullptr)
    , m_pBufferImage(nullptr)
    , m_token(cancellation_token::none())
{
}

FrameProcessing::~FrameProcessing()
{
    if (m_pCurrentImage != nullptr)
    {
        concurrency::Free(m_pCurrentImage);
        m_pCurrentImage = nullptr;
    }
}

void FrameProcessing::SetCancellationToken(cancellation_token token)
{
    m_token = token;
}

void FrameProcessing::SetNeighbourArea(unsigned int area)
{
    m_neighborWindow = area;
}

unsigned int FrameProcessing::GetNeighbourArea()
{
    return m_neighborWindow;
}

void FrameProcessing::ApplyFilters(int nPhases)
{
    ApplyColorSimplifier(nPhases);
    ApplyEdgeDetection();
}

void FrameProcessing::SetCurrentFrame(byte* pFrame, int size, int width, int height, int pitch, int bpp, int clrPlanes)
{
    m_width = width;
    m_height = height;
    m_pitch = pitch;
    m_BPP = bpp;
    m_colorPlanes = clrPlanes;

    if (m_pCurrentImage != nullptr)
    {
        concurrency::Free(m_pCurrentImage);
        m_pCurrentImage = nullptr;
    }
    if (m_pCurrentImage == nullptr)
    {
        m_pCurrentImage = (byte*)concurrency::Alloc(size);
    }
    
    memcpy(m_pCurrentImage, pFrame, size);
    m_pBufferImage = pFrame;
    m_size = size;
}

void FrameProcessing::ApplyColorSimplifier()
{
    unsigned int shift = m_neighborWindow / 2;
    ApplyColorSimplifier(shift, (m_height - shift), shift, (m_width - shift));
}

void FrameProcessing::ApplyColorSimplifier(int nPhases)
{
    for (int i = 0; i < nPhases; ++i)
    {
        ApplyColorSimplifier();
    }
}

void FrameProcessing::ApplyColorSimplifier(unsigned int startHeight, unsigned int endHeight, unsigned int startWidth, unsigned int endWidth)
{
    if (m_pBufferImage != nullptr)
    {
        run_with_cancellation_token([this, startHeight, endHeight, startWidth, endWidth]
        {
            parallel_for(startHeight, endHeight, [&](unsigned int j)
            {
                if (m_token.is_canceled()) return;
                for (unsigned int i = startWidth; i < endWidth; ++i)
                {
                    SimplifyIndexOptimized(m_pBufferImage, i, j);
                }
            });
        }, m_token);
    }
}

void FrameProcessing::SimplifyIndexOptimized(byte* pFrame, int x, int y)
{
    COLORREF orgClr =  GetPixel(pFrame, x, y, m_pitch, m_BPP);

    int shift = m_neighborWindow / 2;
    float64 sSum = 0;
    float64 partialSumR = 0; 
    float64 partialSumG = 0; 
    float64 partialSumB = 0;
    float64 standardDeviation = 0.025;

    for (int j = y - shift; j <= (y + shift); ++j)
    {
        for (int i = x - shift; i <= (x + shift); ++i)
        {
            // Don't apply filter to the requested index, only to the neighbors
            if (i != x ||  j != y) 
            {
                COLORREF clr = GetPixel(pFrame, i, j, m_pitch, m_BPP);
                float64 distance = Utils::GetDistance(orgClr, clr);
                float64 sValue = pow(M_E, -0.5 * pow(distance / standardDeviation, 2));
                sSum += sValue;
                partialSumR += GetRValue(clr) * sValue;
                partialSumG += GetGValue(clr) * sValue;
                partialSumB += GetBValue(clr) * sValue;
            }
        }
    }

    COLORREF simplifiedClr;
    int simpleRed, simpleGreen, simpleBlue;

    simpleRed   = (int)min(max(partialSumR / sSum, 0), 255);
    simpleGreen = (int)min(max(partialSumG / sSum, 0), 255);
    simpleBlue  = (int)min(max(partialSumB / sSum, 0), 255);
    simplifiedClr = RGB(simpleRed, simpleGreen, simpleBlue);
    SetPixel(m_pBufferImage, x, y, m_pitch, m_BPP, simplifiedClr);
}

void FrameProcessing::ApplyEdgeDetection()
{
    ApplyEdgeDetectionParallel(1, (m_height - 1), 1, (m_width - 1));
}

void FrameProcessing::ApplyEdgeDetectionParallel(unsigned int startHeight, unsigned int endHeight, unsigned int startWidth, unsigned int endWidth)
{
    const float alpha = 0.3f;
    const float beta = 0.8f;
    const float s0 = 0.054f;
    const float s1 = 0.064f;
    const float a0 = 0.3f;
    const float a1 = 0.7f;

    byte* pFrame = new byte[m_size];
    memcpy_s(pFrame, m_size, m_pBufferImage, m_size);

    run_with_cancellation_token([=]()
    {
        parallel_for(startHeight , endHeight, [&](int y)
        {
            if (m_token.is_canceled()) return;
            for(unsigned int x = startWidth; x < endWidth; ++x)
            {
                float Sy, Su, Sv;
                float Ay, Au, Av;

                CalculateSobel(m_pBufferImage, x, y, Sy, Su, Sv);
                CalculateSobel(m_pCurrentImage, x, y, Ay, Au, Av);

                float edgeS = (1 - alpha) * Sy + alpha * (Su + Sv) / 2;
                float edgeA = (1 - alpha) * Ay + alpha * (Au + Av) / 2;
                float i = (1 - beta) * Utils::SmoothStep(s0, s1, edgeS) + beta * Utils::SmoothStep(a0, a1, edgeA);

                float oneMinusi = 1 - i;
                COLORREF clr = this->GetPixel(m_pBufferImage, x, y, m_pitch, m_BPP);
                COLORREF newClr = RGB(GetRValue(clr) * oneMinusi, GetGValue(clr) * oneMinusi, GetBValue(clr) * oneMinusi);
                this->SetPixel(pFrame, x, y, m_pitch, m_BPP, newClr);
            }
        });
    }, m_token);

    memcpy_s(m_pBufferImage, m_size, pFrame, m_size);
    delete[] pFrame;
}

void FrameProcessing::CalculateSobel(byte* pSource, int row, int column, float& dy, float& du, float& dv)
{
    int gx[3][3] = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } }; 
    int gy[3][3] = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

    float64 new_yX = 0;
    float64 new_yY = 0;
    float64 new_uX = 0;
    float64 new_uY = 0;
    float64 new_vX = 0;
    float64 new_vY = 0;
    for (int i = -1; i < 2; i++)
    {
        for (int j = -1; j < 2; j++)
        {
            const int gX = gx[i + 1][j + 1];
            const int gY = gy[i + 1][j + 1];
            float64 y;
            float64 u;
            float64 v;

            Utils::RGBToYUV(GetPixel(pSource, row + i, column + j, m_pitch, m_BPP), y, u, v);

            new_yX += gX * y;
            new_yY += gY * y;
            new_uX += gX * u;
            new_uY += gY * u;
            new_vX += gX * v;
            new_vY += gY * v;
        }
    }

    dy = (float)sqrt((new_yX * new_yX) + (new_yY * new_yY));
    du = (float)sqrt((new_uX * new_uX) + (new_uY * new_uY));
    dv = (float)sqrt((new_vX * new_vX) + (new_vY * new_vY));
}
