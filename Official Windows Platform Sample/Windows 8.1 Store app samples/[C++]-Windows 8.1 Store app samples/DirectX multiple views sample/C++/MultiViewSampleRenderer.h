//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"

namespace MultiViewSample
{
    // This sample renderer instantiates a basic rendering pipeline.
    class MultiViewSampleRenderer
    {
    public:
        MultiViewSampleRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources, Platform::String^ textToDraw);
        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void Render();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample-specific resources.
        Platform::String^ m_textToDraw;
        Microsoft::WRL::ComPtr<IDWriteTextFormat> m_textFormat;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_brush;
    };
}
