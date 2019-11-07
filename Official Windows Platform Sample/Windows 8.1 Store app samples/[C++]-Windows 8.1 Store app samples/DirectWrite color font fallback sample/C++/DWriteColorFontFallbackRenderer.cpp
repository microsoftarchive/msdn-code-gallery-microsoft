//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteColorFontFallbackRenderer.h"
#include "DirectXHelper.h"

using namespace DWriteColorFontFallback;

using namespace DirectX;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace D2D1;

DWriteColorFontFallbackRenderer::DWriteColorFontFallbackRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_scaleFactor(1.0f),
    m_maxVisibleTextBlocks(SampleConstants::MaxTextBlocks),
    m_fontFallbackId(0)
{
    auto dwriteFactory = m_deviceResources->GetDWriteFactory();

    // Create a series of Font fallbacks
    // Emoji
    // Emoji -> System Default
    // Emoji -> Symbol
    // Emoji -> Symbol -> System Default
    // Symbol
    // Symbol -> System Default

    ComPtr<IDWriteFontFallbackBuilder> fallbackBuilder;
    ComPtr<IDWriteFontFallback> systemFallback;

    DX::ThrowIfFailed(dwriteFactory->GetSystemFontFallback(&systemFallback));
    m_fallbackList[SampleConstants::FontFallbackSystem] = systemFallback;

    DWRITE_UNICODE_RANGE range[] = {
        {0x00000, 0xffffffff},
    };
    WCHAR const* fallbackEmoji[1] = {
        SampleConstants::EmojiFontFamilyName,
    };

    DX::ThrowIfFailed(dwriteFactory->CreateFontFallbackBuilder(&fallbackBuilder));
    DX::ThrowIfFailed(fallbackBuilder->AddMapping(range, 1, fallbackEmoji, 1));
    DX::ThrowIfFailed(fallbackBuilder->CreateFontFallback(&m_fallbackList[SampleConstants::FontFallbackEmoji]));

    DX::ThrowIfFailed(dwriteFactory->CreateFontFallbackBuilder(&fallbackBuilder));
    DX::ThrowIfFailed(fallbackBuilder->AddMapping(range, 1, fallbackEmoji, 1));
    DX::ThrowIfFailed(fallbackBuilder->AddMappings(systemFallback.Get()));
    DX::ThrowIfFailed(fallbackBuilder->CreateFontFallback(&m_fallbackList[SampleConstants::FontFallbackEmojiSystem]));

    WCHAR const* fallbackEmojiSymbol[2] = {
        SampleConstants::EmojiFontFamilyName,
        SampleConstants::SymbolFontFamilyName,
    };

    DX::ThrowIfFailed(dwriteFactory->CreateFontFallbackBuilder(&fallbackBuilder));
    DX::ThrowIfFailed(fallbackBuilder->AddMapping(range, 1, fallbackEmojiSymbol, 2));
    DX::ThrowIfFailed(fallbackBuilder->CreateFontFallback(&m_fallbackList[SampleConstants::FontFallbackEmojiSymbol]));

    DX::ThrowIfFailed(dwriteFactory->CreateFontFallbackBuilder(&fallbackBuilder));
    DX::ThrowIfFailed(fallbackBuilder->AddMapping(range, 1, fallbackEmojiSymbol, 2));
    DX::ThrowIfFailed(fallbackBuilder->AddMappings(systemFallback.Get()));
    DX::ThrowIfFailed(fallbackBuilder->CreateFontFallback(&m_fallbackList[SampleConstants::FontFallbackEmojiSymbolSystem]));

    WCHAR const* fallbackSymbol[1] = {
        SampleConstants::SymbolFontFamilyName
    };

    DX::ThrowIfFailed(dwriteFactory->CreateFontFallbackBuilder(&fallbackBuilder));
    DX::ThrowIfFailed(fallbackBuilder->AddMapping(range, 1, fallbackSymbol, 1));
    DX::ThrowIfFailed(fallbackBuilder->CreateFontFallback(&m_fallbackList[SampleConstants::FontFallbackSymbol]));

    DX::ThrowIfFailed(dwriteFactory->CreateFontFallbackBuilder(&fallbackBuilder));
    DX::ThrowIfFailed(fallbackBuilder->AddMapping(range, 1, fallbackSymbol, 1));
    DX::ThrowIfFailed(fallbackBuilder->AddMappings(systemFallback.Get()));
    DX::ThrowIfFailed(fallbackBuilder->CreateFontFallback(&m_fallbackList[SampleConstants::FontFallbackSymbolSystem]));

    ComPtr<IDWriteTextFormat> textFormat;

    for (unsigned int i = 0; i < SampleConstants::MaxFormats; i++)
    {
        DX::ThrowIfFailed(
            dwriteFactory->CreateTextFormat(
                SampleConstants::Formats[i].FontFamilyName,
                nullptr,
                DWRITE_FONT_WEIGHT_LIGHT,
                DWRITE_FONT_STYLE_NORMAL,
                DWRITE_FONT_STRETCH_NORMAL,
                SampleConstants::Formats[i].FontSize,
                SampleConstants::LocaleName,
                &textFormat
                )
            );
        DX::ThrowIfFailed(textFormat.As(&m_textFormat[i]));
        DX::ThrowIfFailed(m_textFormat[i]->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING));
        DX::ThrowIfFailed(m_textFormat[i]->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR));
    }

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

void DWriteColorFontFallbackRenderer::CreateDeviceDependentResources()
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    for (unsigned int i = 0; i < SampleConstants::MaxBrushes; i++)
    {
        DX::ThrowIfFailed(
            d2dContext->CreateSolidColorBrush(
                SampleConstants::BrushColors[i],
                &m_brush[i]
                )
            );
    }
}

// Initialization.
void DWriteColorFontFallbackRenderer::CreateWindowSizeDependentResources()
{
    auto windowSize = m_deviceResources->GetLogicalSize();

    m_pageRect.top = SampleConstants::TopMargin;
    m_pageRect.left = SampleConstants::LeftMargin;
    m_pageRect.right = windowSize.Width - SampleConstants::RightMargin;
    m_pageRect.bottom = windowSize.Height - SampleConstants::BottomMargin;

    m_textRect.top = m_pageRect.top + SampleConstants::TextMargin;
    m_textRect.left = m_pageRect.left + SampleConstants::TextMargin;
    m_textRect.right = m_pageRect.right - SampleConstants::TextMargin;
    m_textRect.bottom = m_pageRect.bottom - SampleConstants::TextMargin;

    auto dwriteFactory = m_deviceResources->GetDWriteFactory();

    DWRITE_TEXT_METRICS metrics = {0};
    FLOAT availableLayoutHeight = m_textRect.bottom - m_textRect.top;

    m_maxVisibleTextBlocks = SampleConstants::MaxTextBlocks;

    for (unsigned int i = 0; i < SampleConstants::MaxTextBlocks; i++)
    {
        if (availableLayoutHeight <= 0.0F)
        {
            // We've run out of room in the layout area for the remaining text strings so
            // record the value and stop layout.
            m_maxVisibleTextBlocks = i;
            break;
        }

        Platform::String^ string;

        if (SampleConstants::TextStrings[i].IncludeFontFallbackDescription)
        {
            string = Platform::String::Concat(
                ref new Platform::String(SampleConstants::TextStrings[i].TextString),
                ref new Platform::String(SampleConstants::FontFallbackDescriptions[m_fontFallbackId])
                );
        }
        else
        {
            string = ref new Platform::String(SampleConstants::TextStrings[i].TextString);
        }

        DX::ThrowIfFailed(
            dwriteFactory->CreateTextLayout(
                string->Data(),
                string->Length(),
                m_textFormat[SampleConstants::TextStrings[i].FormatId].Get(),
                m_textRect.right - m_textRect.left,
                availableLayoutHeight,
                &m_layout[i]
                )
            );

        DX::ThrowIfFailed(m_layout[i]->GetMetrics(&metrics));
        m_metrics[i] = metrics;
        availableLayoutHeight -= metrics.height;
    }
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DWriteColorFontFallbackRenderer::ReleaseDeviceDependentResources()
{
    for (unsigned int i = 0; i < SampleConstants::MaxBrushes; i++)
    {
        m_brush[i].Reset();
    }
}

// Renders one frame.
void DWriteColorFontFallbackRenderer::Render(unsigned int fallback, bool colorGlyphs, float zoom, D2D1_POINT_2F point)
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    if (m_fontFallbackId != fallback)
    {
        for (unsigned int i = 0; i < SampleConstants::MaxFormats; i++)
        {
            if (SampleConstants::Formats[i].CustomFallback)
            {
                if (fallback < SampleConstants::MaxFontFallbackScenarios)
                {
                    m_fontFallbackId = fallback;
                    DX::ThrowIfFailed(m_textFormat[i]->SetFontFallback(m_fallbackList[m_fontFallbackId].Get()));
                }
                else
                {
                    m_fontFallbackId = SampleConstants::FontFallbackSystem;
                    DX::ThrowIfFailed(m_textFormat[i]->SetFontFallback(nullptr));
                }
            }
        }
        CreateWindowSizeDependentResources();
    }

    d2dContext->BeginDraw();
    d2dContext->Clear(D2D1::ColorF(0.1f, 0.1f, 0.4f));
    d2dContext->SetTransform(
        Matrix3x2F::Translation(point.x, point.y) *
        Matrix3x2F::Scale(zoom, zoom) *
        m_deviceResources->GetOrientationTransform2D()
        );
    d2dContext->FillRectangle(&m_pageRect, m_brush[SampleConstants::BrushBackground].Get());

    D2D1_POINT_2F layoutOrigin;
    layoutOrigin.x = m_textRect.left;
    layoutOrigin.y = m_textRect.top;

    D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS_CLIP;
    if (colorGlyphs)
    {
        options |= D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT;
    }

    for (unsigned int i = 0; i < m_maxVisibleTextBlocks; i++)
    {
        d2dContext->DrawTextLayout(
            layoutOrigin,
            m_layout[i].Get(),
            m_brush[SampleConstants::TextStrings[i].BrushId].Get(),
            options
            );
        layoutOrigin.y += m_metrics[i].height;
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}
