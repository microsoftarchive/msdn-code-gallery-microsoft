// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

class FrameProcessing;

struct FrameData
{
    int m_size;
    unsigned int m_startWidth;
    unsigned int m_startHeight;
    unsigned int m_endWidth;
    unsigned int m_endHeight;
    int m_pitch;
    int m_BBP;
    int m_colorPlanes;
    byte* m_pFrame;
    int m_phaseCount;
    int m_neighbourArea;
    FrameProcessing* m_pFrameProcessor;

    FrameData()
    {
        m_neighbourArea = 3;
        m_phaseCount = 0;
        m_size = 0;
        m_startWidth = 0;
        m_endWidth = 0;
        m_startHeight = 0;
        m_endHeight = 0;
        m_pitch = 0;
        m_BBP = 0;
        m_pFrame = nullptr;
        m_pFrameProcessor = nullptr;
        m_colorPlanes = 0;
    }
};