// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

class FrameProcessing
{
public:
    FrameProcessing();
    ~FrameProcessing();

    void SetCancellationToken(concurrency::cancellation_token token);
    void SetNeighbourArea(unsigned int area);
    void SetCurrentFrame(byte* pFrame, int size, int width, int height, int pitch, int bpp, int clrPlanes);
    void ApplyFilters(int nPhases);

private:
    byte* m_pCurrentImage;
    byte* m_pBufferImage;
    unsigned int m_neighborWindow;
    unsigned int m_width;
    unsigned int m_height;
    unsigned int m_size;
    int m_pitch;
    unsigned int m_BPP;
    unsigned int m_colorPlanes;
    concurrency::cancellation_token m_token;

    unsigned int GetNeighbourArea();
    void StopFilters();
    void ApplyColorSimplifier();
    void ApplyColorSimplifier(int nPhases);
    void ApplyColorSimplifier(unsigned int startHeight, unsigned int endHeight, unsigned int startWidth, unsigned int endWidth);
    void SimplifyIndexOptimized(byte* pFrame, int x, int y);
    void ApplyEdgeDetection();
    void ApplyEdgeDetectionParallel(unsigned int startHeight, unsigned int endHeight, unsigned int startWidth, unsigned int endWidth);
    void CalculateSobel(byte* pSource, int row, int column, float& dy, float& du, float& dv);

    static inline COLORREF GetPixel(byte* pFrame, int x, int y, int pitch, int bpp)
    {
        int width = abs((int)pitch);
        int bytesPerPixel = bpp / 8;
        int byteIndex = y * width + x * bytesPerPixel;
        return RGB(pFrame[byteIndex + 2], 
            pFrame[byteIndex + 1],
            pFrame[byteIndex]);
    }

    static inline void SetPixel(byte* pFrame, int x, int y, int pitch, int bpp, COLORREF color)
    {
        int width = abs((int)pitch);
        int bytesPerPixel = bpp / 8;
        int byteIndex = y * width + x * bytesPerPixel;
        pFrame[byteIndex + 2] = GetRValue(color);
        pFrame[byteIndex + 1] = GetGValue(color);
        pFrame[byteIndex] = GetBValue(color);
    }
};