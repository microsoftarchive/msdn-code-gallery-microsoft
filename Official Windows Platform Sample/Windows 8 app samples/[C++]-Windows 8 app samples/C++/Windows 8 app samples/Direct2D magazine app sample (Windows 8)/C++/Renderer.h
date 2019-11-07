//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <initguid.h>
#include <wrl.h>
#include <d3d11_1.h>
#include <dxgi1_2.h>
#include <d2d1_1.h>
#include <d2d1effects.h>
#include <dwrite_1.h>
#include <wincodec.h>

delegate void DeviceLostEventHandler();

// Helper class that initializes and maintains the DirectX API objects in one place.
ref class Renderer
{
internal:
    Renderer(
        Windows::Foundation::Rect windowBounds,
        float dpi
        );

    event DeviceLostEventHandler^ DeviceLost;

    void HandleDeviceLost();

    void UpdateWindowSize();

    void SetDpi(float dpi);

    void GetD2DDeviceContext(_Outptr_ ID2D1DeviceContext** d2dDeviceContext);

    void GetWICImagingFactory(_Outptr_ IWICImagingFactory2** wicFactory);

    void GetDXGIDevice(_Outptr_ IDXGIDevice** dxgiDevice);

    inline float GetDisplayWidth()
    {
        return m_windowBounds.Width;
    }

    inline float GetDisplayHeight()
    {
        return m_windowBounds.Height;
    }

    inline D2D1_SIZE_F GetDisplaySize()
    {
        return D2D1::SizeF(GetDisplayWidth(), GetDisplayHeight());
    }

    inline bool IsOrientationLandscape()
    {
        return GetDisplayWidth() >= GetDisplayHeight();
    }

protected private:
    void CreateDeviceIndependentResources();
    void CreateDeviceResources();

    // Declare Direct2D objects
    Microsoft::WRL::ComPtr<ID2D1Factory1> m_d2dFactory;
    Microsoft::WRL::ComPtr<ID2D1Device> m_d2dDevice;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> m_d2dDeviceContext;

    // Declare Windows Imaging Component objects
    Microsoft::WRL::ComPtr<IWICImagingFactory2> m_wicFactory;

    // Direct3D objects
    Microsoft::WRL::ComPtr<IDXGIDevice> m_dxgiDevice;

    // Direct3D feature level
    D3D_FEATURE_LEVEL m_featureLevel;

    // Bounds of the current window
    Windows::Foundation::Rect m_windowBounds;

    float m_dpi;
};

// Helper utilities to make DX APIs work with exceptions in the samples apps.
namespace DX
{
    inline void ThrowIfFailed(HRESULT hr)
    {
        if (FAILED(hr))
        {
            // Set a breakpoint on this line to catch DX API errors.
            throw Platform::Exception::CreateException(hr);
        }
    }
}
