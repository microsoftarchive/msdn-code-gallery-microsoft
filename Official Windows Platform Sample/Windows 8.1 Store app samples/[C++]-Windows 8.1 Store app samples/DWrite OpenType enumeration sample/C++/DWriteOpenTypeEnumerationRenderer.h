//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"

namespace DWriteOpenTypeEnumeration
{
    // This sample renderer creates and draws a text layout with different typographies.
    class DWriteOpenTypeEnumerationRenderer
    {
    public:
        DWriteOpenTypeEnumerationRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void UpdateFontFace(int fontFaceIndex);
        void UpdateStylisticSet(int stylisticSetIndex);
        void Render();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample-specific resources.
        Microsoft::WRL::ComPtr<IDWriteTextLayout>                                m_dwriteTextLayout;
        Microsoft::WRL::ComPtr<IDWriteTextFormat>                                m_dwriteTextFormat;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                             m_d2dSolidColorBrush;
        Microsoft::WRL::ComPtr<IDWriteTypography>                                m_typography;

        int m_fontFaceIndex;
        int m_stylisticSetIndex;
    };
}
