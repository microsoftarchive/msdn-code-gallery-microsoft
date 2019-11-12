//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Common\DeviceResources.h"
#include "Content\PuzzlePiece.h"

namespace JigsawPuzzle
{
    class PuzzlePieceRenderer
    {
    public:
        PuzzlePieceRenderer();

        void Initialize(
            const std::shared_ptr<PuzzlePiece>& piece,
            const std::shared_ptr<DX::DeviceResources>& deviceResources
            );

        void SetResources(ID2D1Geometry* puzzleShape, ID2D1Brush* brush);
        void ReleaseResources();

        void Render();

    private:
        std::shared_ptr<PuzzlePiece> m_piece;
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        Microsoft::WRL::ComPtr<ID2D1TransformedGeometry> m_geometry;
        Microsoft::WRL::ComPtr<ID2D1Brush> m_geometryBrush;
        D2D1_MATRIX_3X2_F m_geometryBrushTransform;
        D2D1_RECT_F m_selectionBounds;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_selectionBrush;
    };
}
