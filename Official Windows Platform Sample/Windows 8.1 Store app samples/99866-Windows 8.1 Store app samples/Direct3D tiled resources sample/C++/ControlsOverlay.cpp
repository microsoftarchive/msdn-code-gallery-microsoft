//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ControlsOverlay.h"

using namespace TiledResources;

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Windows::UI::ViewManagement;
using namespace Windows::Graphics::Display;
using namespace D2D1;

ControlsOverlay::ControlsOverlay(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_paddingX(8.0f),
    m_paddingY(24.0f),
    m_overlaySize(256.0f) // Known a priori.
{
    CreateDeviceDependentResources();
}

void ControlsOverlay::CreateDeviceDependentResources()
{
    ComPtr<IWICBitmapDecoder> wicBitmapDecoder;
    DX::ThrowIfFailed(
        m_deviceResources->GetWicImagingFactory()->CreateDecoderFromFilename(
            L"controls.png",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &wicBitmapDecoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> wicBitmapFrame;
    DX::ThrowIfFailed(
        wicBitmapDecoder->GetFrame(0, &wicBitmapFrame)
        );

    ComPtr<IWICFormatConverter> wicFormatConverter;
    DX::ThrowIfFailed(
        m_deviceResources->GetWicImagingFactory()->CreateFormatConverter(&wicFormatConverter)
        );

    DX::ThrowIfFailed(
        wicFormatConverter->Initialize(
            wicBitmapFrame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0,
            WICBitmapPaletteTypeCustom
            )
        );

    double dpiX = 96.0;
    double dpiY = 96.0;
    DX::ThrowIfFailed(
        wicFormatConverter->GetResolution(&dpiX, &dpiY)
        );

    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateBitmapFromWicBitmap(
            wicFormatConverter.Get(),
            BitmapProperties(
            PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
            static_cast<float>(dpiX),
            static_cast<float>(dpiY)
            ),
            &m_controlsBitmap
            )
        );
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void ControlsOverlay::ReleaseDeviceDependentResources()
{
    m_controlsBitmap.Reset();
}

void ControlsOverlay::Render()
{
    m_deviceResources->GetD2DDeviceContext()->BeginDraw();
    m_deviceResources->GetD2DDeviceContext()->SetTransform(m_deviceResources->GetOrientationTransform2D());
    m_deviceResources->GetD2DDeviceContext()->DrawBitmap(
        m_controlsBitmap.Get(),
        D2D1::RectF(
            m_deviceResources->GetLogicalSize().Width - m_overlaySize,
            m_paddingY,
            m_deviceResources->GetLogicalSize().Width,
            m_overlaySize + m_paddingY
            )
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_deviceResources->GetD2DDeviceContext()->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

void ControlsOverlay::EvaluateTouchControl(float touchX, float touchY, float& x, float& y, float& z, float& rx, float& ry, float& rz)
{
    // Based on the content of the controls overlay image and its position in the window,
    // convert a touch coordinate into the corresponding sample input control value.

    // Initialize outputs to zero.
    x = y = z = rx = ry = rz = 0.0f;

    // Normalize positions.
    float nX = (touchX - (m_deviceResources->GetLogicalSize().Width - (m_overlaySize / 2.0f))) / (m_overlaySize / 2.0f);
    float nY = (touchY - (m_overlaySize / 2.0f + m_paddingY)) / (m_overlaySize / 2.0f);

    float distance = sqrtf(nX * nX + nY * nY);

    if (distance > 0.05f && distance <= 1.0f)
    {
        float angle = atan2f(-nY, nX) * 180.0f / DirectX::XM_PI;

        if (distance > 0.8f)
        {
            // For distances over 0.8, interpret as rotation control.
            if (angle < -135.0f)
            {
                rx = -1.0f;
            }
            else if (angle < -90.0f)
            {
                ry = -1.0f;
            }
            else if (angle < -45.0f)
            {
                ry = 1.0f;
            }
            else if (angle < 45.0f)
            {
                // No control in this region.
            }
            else if (angle < 90.0f)
            {
                rz = -1.0f;
            }
            else if (angle < 135.0f)
            {
                rz = 1.0f;
            }
            else
            {
                rx = 1.0f;
            }
        }
        else
        {
            // For distances within 0.8, interpret as translation control.
            if (angle < -157.5f)
            {
                x = -1.0f;
            }
            else if (angle < -112.5f)
            {
                z = 1.0f;
            }
            else if (angle < -45.0f)
            {
                y = -1.0f;
            }
            else if (angle < 22.5f)
            {
                x = 1.0f;
            }
            else if (angle < 67.5f)
            {
                z = -1.0f;
            }
            else if (angle < 135.0f)
            {
                y = 1.0f;
            }
            else
            {
                x = -1.0f;
            }
        }
    }
}
