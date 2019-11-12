//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"

namespace Scenario1Component
{
    public ref class Scenario1ImageSource sealed : Windows::UI::Xaml::Media::Imaging::SurfaceImageSource
    {
    public:
        Scenario1ImageSource(int pixelWidth, int pixelHeight, bool isOpaque);

        void BeginDraw(Windows::Foundation::Rect updateRect);
        void BeginDraw()    { BeginDraw(Windows::Foundation::Rect(0, 0, (float)m_width, (float)m_height)); }
        void EndDraw();

        void SetDpi(float dpi);

        void Clear(Windows::UI::Color color);
        void FillSolidRect(Windows::UI::Color color, Windows::Foundation::Rect rect);

    private protected:
        void CreateDeviceIndependentResources();
        void CreateDeviceResources();

        Microsoft::WRL::ComPtr<ISurfaceImageSourceNative>   m_sisNative;

        // Direct3D device
        Microsoft::WRL::ComPtr<ID3D11Device>                m_d3dDevice;

        // Direct2D objects
        Microsoft::WRL::ComPtr<ID2D1Device>                 m_d2dDevice;
        Microsoft::WRL::ComPtr<ID2D1DeviceContext>          m_d2dContext;

        int                                                 m_width;
        int                                                 m_height;
    };
}
