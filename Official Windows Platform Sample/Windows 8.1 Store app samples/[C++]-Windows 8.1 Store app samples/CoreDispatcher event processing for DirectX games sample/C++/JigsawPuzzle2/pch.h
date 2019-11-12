//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <wrl.h>
#include <wrl/client.h>
#include <d3d11_2.h>
#include <d2d1_2.h>
#include <d2d1effects_1.h>
#include <dwrite_2.h>
#include <wincodec.h>
#include <DirectXColors.h>
#include <DirectXMath.h>
#include <memory>
#include <agile.h>
#include <concrt.h>

namespace Constants
{
    const UINT InvalidPointerId = 0xFFFFFFFF;

    const UINT PuzzlePieceCount = 4;
    const float PuzzlePieceMargin = 70.0f;
    const float PuzzlePieceSize = 150.0f;
    const float PuzzlePieceConnectorInnerRadius = 8.0f;
    const float PuzzlePieceConnectorOuterRadius = 15.0f;

    // 2. Animation support.
    const UINT PuzzlePieceAnimationFrames = 15;

    const float PuzzleBoardMargin = PuzzlePieceMargin * 2 + PuzzlePieceSize;

    const float FrameCounterLeft = PuzzleBoardMargin;
    const float FrameCounterTop = PuzzleBoardMargin + PuzzlePieceMargin + 2 * PuzzlePieceSize;
    const float FrameCounterRight = FrameCounterLeft + 2 * PuzzlePieceSize;
    const float FrameCounterBottom = FrameCounterTop + PuzzlePieceMargin;
}
