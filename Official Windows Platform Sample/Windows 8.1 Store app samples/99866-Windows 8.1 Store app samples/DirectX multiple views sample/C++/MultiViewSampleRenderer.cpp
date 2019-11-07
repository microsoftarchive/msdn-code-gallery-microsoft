//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MultiViewSampleRenderer.h"

#include "DirectXHelper.h"

using namespace MultiViewSample;

using namespace DirectX;
using namespace Windows::Foundation;

// Initialization.
MultiViewSampleRenderer::MultiViewSampleRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources, Platform::String^ textToDraw) : 
    m_deviceResources(deviceResources)
{
    // Drawing instructional text that is specific to the view.
    m_textToDraw = textToDraw;

    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            32.0f,
            L"en-us",
            &m_textFormat
            )
        );

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void MultiViewSampleRenderer::CreateDeviceDependentResources()
{
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_brush
            )
        );
}

// Initialization.
void MultiViewSampleRenderer::CreateWindowSizeDependentResources()
{
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void MultiViewSampleRenderer::ReleaseDeviceDependentResources()
{
    m_brush.Reset();
}

// Renders one frame.
void MultiViewSampleRenderer::Render()
{
    auto deviceContext = m_deviceResources->GetD2DDeviceContext();

    deviceContext->BeginDraw();
    deviceContext->SetTransform(m_deviceResources->GetOrientationTransform2D());

    // Writes the text within a rectangle.
    deviceContext->DrawText(
        m_textToDraw->Data(),
        m_textToDraw->Length(),
        m_textFormat.Get(),
        D2D1::RectF(100, 200, 600, 800),
        m_brush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = deviceContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}
