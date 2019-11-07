//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteVerticalHelloWorldRenderer.h"

#include "DirectXHelper.h"

using namespace DWriteVerticalHelloWorld;

using namespace DirectX;
using namespace Windows::Foundation;

// Creates a DWriteTextFormat with our text properties. This includes setting our flow and reading direction.
DWriteVerticalHelloWorldRenderer::DWriteVerticalHelloWorldRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources)
{
    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Meiryo UI",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            32.0f,
            L"en-US", // locale
            &m_dwriteTextFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_dwriteTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_dwriteTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    // Set the reading direction to top-to-bottom.
    DX::ThrowIfFailed(
        m_dwriteTextFormat->SetReadingDirection(DWRITE_READING_DIRECTION_TOP_TO_BOTTOM)
        );

    // Set the flow direction to right-to-left.
    DX::ThrowIfFailed(
        m_dwriteTextFormat->SetFlowDirection(DWRITE_FLOW_DIRECTION_RIGHT_TO_LEFT)
        );

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void DWriteVerticalHelloWorldRenderer::CreateDeviceDependentResources()
{
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::WhiteSmoke),
            &m_d2dSolidColorBrush
            )
        );
}

// Initialization.
void DWriteVerticalHelloWorldRenderer::CreateWindowSizeDependentResources()
{
    Platform::String^ text = L"現在のアプリケーションでは、高品質のテキスト レンダリングと解像度に依存しないアウトライン フォントをサポートし、Unicode テキストとレイアウトを完全にサポートする必要があります。新しい DirectX API である DirectWrite で提供している機能には、次のようなものがあります。";

    auto size = m_deviceResources->GetLogicalSize();

    // Create a DirectWrite Text Layout object
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextLayout(
            text->Data(),                              // Text to be displayed
            text->Length(),                            // Length of the text
            m_dwriteTextFormat.Get(),                  // DirectWrite Text Format object
            size.Width,                                // Width of the Text Layout
            (size.Height - 200),                       // Height of the Text Layout
            &m_dwriteTextLayout
            )
        );
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DWriteVerticalHelloWorldRenderer::ReleaseDeviceDependentResources()
{
    m_d2dSolidColorBrush.Reset();
}

// Renders one frame.
void DWriteVerticalHelloWorldRenderer::Render()
{
    m_deviceResources->GetD2DDeviceContext()->BeginDraw();

    m_deviceResources->GetD2DDeviceContext()->SetTransform(
        m_deviceResources->GetOrientationTransform2D()
        );

    m_deviceResources->GetD2DDeviceContext()->DrawTextLayout(
        D2D1::Point2F(0.0f, 100.0f),
        m_dwriteTextLayout.Get(),
        m_d2dSolidColorBrush.Get()
        );

    m_deviceResources->GetD2DDeviceContext()->EndDraw();
}
