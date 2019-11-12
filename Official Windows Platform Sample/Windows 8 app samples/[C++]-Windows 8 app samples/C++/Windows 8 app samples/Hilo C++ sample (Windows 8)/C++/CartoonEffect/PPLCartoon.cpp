// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include <ppltasks.h>
#include "FrameData.h"
#include "FrameProcessing.h"

using concurrency::cancellation_token;

void InitializeFrameData(FrameData& frameData, 
                         FrameProcessing& frameProcessing,
                         byte* sourcePixels,
                         unsigned int width, 
                         unsigned int height, 
                         unsigned int neighborWindow, 
                         unsigned int phases,
                         unsigned int size,
                         cancellation_token token)
{
    frameData.m_BBP = 32;
    frameData.m_colorPlanes = 1;
    frameData.m_endHeight = height;
    frameData.m_endWidth = width;
    frameData.m_neighbourArea = neighborWindow;
    frameData.m_pFrame = sourcePixels;
    frameData.m_pFrameProcessor = nullptr;
    frameData.m_phaseCount = phases;
    frameData.m_pitch = 4 * width;
    frameData.m_size = size;
    frameData.m_startHeight = 0;
    frameData.m_startWidth = 0;

    frameData.m_pFrameProcessor = &frameProcessing;
    frameData.m_pFrameProcessor->SetNeighbourArea(frameData.m_neighbourArea);
    frameData.m_pFrameProcessor->SetCurrentFrame(frameData.m_pFrame, 
        frameData.m_size, 
        frameData.m_endWidth, 
        frameData.m_endHeight, 
        frameData.m_pitch, 
        frameData.m_BBP,
        frameData.m_colorPlanes);
    frameData.m_pFrameProcessor->SetCancellationToken(token);
}

void ApplyCartoonEffectPPL(byte* sourcePixels, 
                           unsigned int width, 
                           unsigned int height, 
                           unsigned int neighborWindow, 
                           unsigned int phases, 
                           unsigned int size, 
                           cancellation_token token)
{
    FrameData frameData;
    FrameProcessing frameProcessing;
    InitializeFrameData(frameData, frameProcessing, sourcePixels, width, height, neighborWindow, phases, size, token);

    frameData.m_pFrameProcessor->ApplyFilters(frameData.m_phaseCount);
}