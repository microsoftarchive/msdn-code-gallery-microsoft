//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

enum class FrameWorkload
{
    Increase,
    Decrease,
    Maintain
};

ref class AutoThrottle
{
internal:
    AutoThrottle(float targetFrameTime);
    FrameWorkload Update(float frameTime);

private:
    unsigned int m_missedFrameHistory;
    float m_targetFrameTime;
};
