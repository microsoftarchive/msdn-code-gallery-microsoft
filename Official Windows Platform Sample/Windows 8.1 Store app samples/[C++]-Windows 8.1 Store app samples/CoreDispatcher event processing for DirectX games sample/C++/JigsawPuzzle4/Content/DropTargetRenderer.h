//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Common\DeviceResources.h"
#include "Content\DropTarget.h"

namespace JigsawPuzzle
{
    class DropTargetRenderer
    {
    public:
        DropTargetRenderer();

        void Initialize(
            const std::shared_ptr<DropTarget>& target,
            const std::shared_ptr<DX::DeviceResources>& deviceResources
            );

        void SetResources(ID2D1Geometry* puzzleShape, ID2D1SolidColorBrush* brush);
        void ReleaseResources();

        void Render();

    private:
        std::shared_ptr<DropTarget> m_target;
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        Microsoft::WRL::ComPtr<ID2D1TransformedGeometry> m_geometry;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_brush;
    };
}
