//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Common\DeviceResources.h"
#include "Content\DropTargetRenderer.h"
#include "Content\GameState.h"
#include "Content\PuzzlePieceRenderer.h"

namespace JigsawPuzzle
{
    // Render the puzzle board and the current location of each of the pieces.
    class JigsawPuzzleRenderer
    {
    public:
        JigsawPuzzleRenderer(
            const std::shared_ptr<DX::DeviceResources>& deviceResources,
            const std::shared_ptr<GameState>& gameState
            );

        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void Render();

    private:
        void CreatePuzzlePieceGeometry();

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Variables used with the rendering loop.
        std::shared_ptr<GameState> m_state;
        PuzzlePieceRenderer m_pieceRenderers[Constants::PuzzlePieceCount];
        DropTargetRenderer m_targetRenderers[Constants::PuzzlePieceCount];
        Microsoft::WRL::ComPtr<ID2D1Geometry> m_geometry;
        Microsoft::WRL::ComPtr<IDWriteTextFormat> m_textFormat;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_textBrush;
        UINT m_framesRendered;
    };
}
