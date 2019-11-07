//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteOpenTypeEnumerationRenderer.h"
#include "DirectXHelper.h"

using namespace DWriteOpenTypeEnumeration;
using namespace DirectX;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace D2D1;

// Initialization.
DWriteOpenTypeEnumerationRenderer::DWriteOpenTypeEnumerationRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_fontFaceIndex(-1),
    m_stylisticSetIndex(-1)
{
    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            72.0f,
            L"en-US", // locale
            &m_dwriteTextFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_dwriteTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_dwriteTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void DWriteOpenTypeEnumerationRenderer::CreateDeviceDependentResources()
{
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::WhiteSmoke),
            &m_d2dSolidColorBrush
            )
        );
}

// Initialization.
void DWriteOpenTypeEnumerationRenderer::CreateWindowSizeDependentResources()
{
    Platform::String^ text = L"DirectWrite";

    auto size = m_deviceResources->GetLogicalSize();

    // Create a DirectWrite Text Layout object.
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextLayout(
            text->Data(),                              // Text to be displayed
            text->Length(),                            // Length of the text
            m_dwriteTextFormat.Get(),                  // DirectWrite Text Format object
            size.Width,                                // Width of the Text Layout
            size.Height,                               // Height of the Text Layout
            &m_dwriteTextLayout
            )
        );

    UpdateFontFace(m_fontFaceIndex);
    UpdateStylisticSet(m_stylisticSetIndex);
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DWriteOpenTypeEnumerationRenderer::ReleaseDeviceDependentResources()
{
    m_d2dSolidColorBrush.Reset();
}

// Renders one frame.
void DWriteOpenTypeEnumerationRenderer::Render()
{
    auto deviceContext = m_deviceResources->GetD2DDeviceContext();
    
    deviceContext->BeginDraw();

    deviceContext->SetTransform(
        m_deviceResources->GetOrientationTransform2D()
        );

    deviceContext->DrawTextLayout(
        D2D1::Point2F(0.0f, 0.0f),
        m_dwriteTextLayout.Get(),
        m_d2dSolidColorBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = deviceContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

void DWriteOpenTypeEnumerationRenderer::UpdateFontFace(int fontFaceIndex)
{
    DWRITE_TEXT_RANGE range = {0, 11};
    m_fontFaceIndex = fontFaceIndex;

    switch (m_fontFaceIndex)
    {
    case 0:
        DX::ThrowIfFailed(
            m_dwriteTextLayout->SetFontFamilyName(L"Arial", range)
            );
        break;
    case 1:
        DX::ThrowIfFailed(
            m_dwriteTextLayout->SetFontFamilyName(L"Times New Roman", range)
            );
        break;
    case 2:
        DX::ThrowIfFailed(
            m_dwriteTextLayout->SetFontFamilyName(L"Meiryo", range)
            );
        break;
    case 3:
        DX::ThrowIfFailed(
            m_dwriteTextLayout->SetFontFamilyName(L"Gabriola", range)
            );
        break;
    }
    Render();
}

void DWriteOpenTypeEnumerationRenderer::UpdateStylisticSet(int stylisticSetIndex)
{
    DWRITE_TEXT_RANGE range = { 0, 11 };

    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTypography(&m_typography)
        );

    DWRITE_FONT_FEATURE_TAG fontFeatureTags[] = {
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_1,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_2,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_3,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_4,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_5,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_6,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_7,
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_20
    };

    if (0 <= stylisticSetIndex && stylisticSetIndex < ARRAYSIZE(fontFeatureTags))
    {
        m_stylisticSetIndex = stylisticSetIndex;

        // Apply the requested stylistic set to the typography.
        m_typography->AddFontFeature({ fontFeatureTags[stylisticSetIndex], 1 });
    }

    // Apply the new typography to our text layout.
    DX::ThrowIfFailed(
        m_dwriteTextLayout->SetTypography(m_typography.Get(), range)
        );

    // Re-render the text layout.
    Render();
}
