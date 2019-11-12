//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// This class provides a lightweight throttle feedback mechanism for
// variable frame workloads.  The class is initialized with a target
// frame time and a "history" represented as bits in an unsigned int.
// A 1 bit represents a frame that missed its target frame time.
// If the past N frames, corresponding to the maskDecrease/maskIncrease
// values, have all been missed or made, then the update method will
// return a control recommendation to Decrease or Increase the frame
// workload, respectively.  The initial value for the frame history
// is chosen such that the returned control recommendation is to maintain
// frame workload until the history is primed with accurate values.

#include "pch.h"
#include "AutoThrottle.h"

AutoThrottle::AutoThrottle(float targetFrameTime) :
    m_missedFrameHistory(0xCCCCCCCC),
    m_targetFrameTime(targetFrameTime)
{
}

FrameWorkload AutoThrottle::Update(float frameTime)
{
    static const float epsilon = 0.005f; // Compensation for noise in the frameTime value.
    static const unsigned int maskIncrease = 0x0000001F; // Require 5 made frames to increase workload.
    static const unsigned int maskDecrease = 0x00000003; // Require 2 missed frames to decrease workload.

    // Shift history back by one frame.  The LSB represents the most recent frame.
    m_missedFrameHistory <<= 1;

    if (frameTime > m_targetFrameTime - epsilon)
    {
        m_missedFrameHistory |= 1;
    }

    if ((m_missedFrameHistory & maskIncrease) == 0)
    {
        return FrameWorkload::Increase;
    }
    if ((m_missedFrameHistory & maskDecrease) == maskDecrease)
    {
        return FrameWorkload::Decrease;
    }

    return FrameWorkload::Maintain;
}
