//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "FPSCounter.h"
#include <math.h>
#include <strsafe.h>

using namespace Microsoft::WRL;

FPSCounter::FPSCounter(
    _In_ ID2D1DeviceContext* d2dContext,
    _In_ float dpi,
    _In_ Platform::String^ staticText,
    _In_ IDWriteTextFormat* textFormat,
    _In_ D2D1_POINT_2F position
    ) : m_d2dContext(d2dContext),
        m_dpi(dpi),
        m_staticText(staticText),
        m_textFormat(textFormat),
        m_position(position)
{
}

void FPSCounter::Initialize()
{
    DX::ThrowIfFailed(
        DWriteCreateFactory(
            DWRITE_FACTORY_TYPE_SHARED,
            __uuidof(IDWriteFactory),
            &m_dwriteFactory
            )
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_textBrush
            )
        );

    // The static text will be placed at the top left corner of the atlas

    ComPtr<IDWriteTextLayout> staticTextLayout;

    PositionAtlasText(
        m_staticText,
        D2D1::Point2U(0, 0), // offsetInAtlas
        &staticTextLayout,
        &m_cachedStaticText
        );

    // Determine the offset for the FPS counter. We place
    // the FPS counter on screen just to the right
    // of the last character in the text layout.

    float pointX;
    float pointY;
    DWRITE_HIT_TEST_METRICS hitTestMetrics;
    DX::ThrowIfFailed(
        staticTextLayout->HitTestTextPosition(
            m_staticText->Length() - 1, // textPosition
            TRUE, // isTrailingHit
            &pointX,
            &pointY,
            &hitTestMetrics
            )
        );

    m_firstDigitOffset.x = static_cast<uint32>(pointX);
    m_firstDigitOffset.y = static_cast<uint32>(pointY);

    // Position each digit in the atlas just to the right of everything else
    // we've already positioned. At the same time, grow the atlas size to ensure
    // that it is big enough.
    //
    // Note that more general atlasing approaches will position various elements
    // in an atlas in two dimensions. This class only grows the atlas in one
    // dimension.

    uint32 bitmapWidth = m_cachedStaticText.atlasRect.right - m_cachedStaticText.atlasRect.left;
    uint32 bitmapHeight = m_cachedStaticText.atlasRect.bottom - m_cachedStaticText.atlasRect.top;

    ComPtr<IDWriteTextLayout> digitLayouts[10];
    for (uint32 i = 0; i < 10; i++)
    {
        char16 digit = static_cast<char16>(L'0' + i);

        PositionAtlasText(
            digit.ToString(),
            D2D1::Point2U(bitmapWidth, 0), // offsetInAtlas
            &digitLayouts[i],
            &m_cachedDigits[i]
            );

        bitmapWidth += m_cachedDigits[i].atlasRect.right - m_cachedDigits[i].atlasRect.left;
        bitmapHeight = max(bitmapHeight, m_cachedDigits[i].atlasRect.bottom - m_cachedDigits[i].atlasRect.top);
    }

    // This class does not handle the case where the text is so big that it
    // doesn't fit in the atlas. More generally, an atlasing approach would need
    // to create several atlas bitmaps when one bitmap is not large enough to
    // hold all the content required.

    if (bitmapWidth > m_d2dContext->GetMaximumBitmapSize() ||
        bitmapHeight > m_d2dContext->GetMaximumBitmapSize()
        )
    {
        DX::ThrowIfFailed(E_UNEXPECTED);
    }

    // Now that we know how big the atlas should be, create the atlas bitmap.
    // We use an A8 format since we are only storing text in this atlas and
    // don't need to store the color of the text to do this.

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmap(
            D2D1::SizeU(bitmapWidth, bitmapHeight),
            nullptr,
            0,
            D2D1::BitmapProperties1(
                D2D1_BITMAP_OPTIONS_TARGET,
                D2D1::PixelFormat(DXGI_FORMAT_A8_UNORM, D2D1_ALPHA_MODE_STRAIGHT),
                m_dpi,
                m_dpi
                ),
            &m_textAtlasBitmap
            )
        );

    // Get the Dxgi device from the bitmap

    ComPtr<IDXGISurface> dxgiSurface;
    DX::ThrowIfFailed(
        m_textAtlasBitmap->GetSurface(&dxgiSurface)
        );

    DX::ThrowIfFailed(
        dxgiSurface->GetDevice(IID_PPV_ARGS(&m_dxgiDevice))
        );

    // Draw all the text into the atlas

    ComPtr<ID2D1Image> oldTarget;
    m_d2dContext->GetTarget(&oldTarget);

    m_d2dContext->SetTarget(m_textAtlasBitmap.Get());
    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black, 0.0f));

    m_d2dContext->DrawTextLayout(
        D2D1::Point2F(
            PixelsToDips(m_cachedStaticText.atlasRect.left - m_cachedStaticText.originOffset.x),
            PixelsToDips(m_cachedStaticText.atlasRect.top - m_cachedStaticText.originOffset.y)
            ),
        staticTextLayout.Get(),
        m_textBrush.Get()
        );

    for (uint32 i = 0; i < 10; i++)
    {
        m_d2dContext->DrawTextLayout(
            D2D1::Point2F(
                PixelsToDips(m_cachedDigits[i].atlasRect.left - m_cachedDigits[i].originOffset.x),
                PixelsToDips(m_cachedDigits[i].atlasRect.top - m_cachedDigits[i].originOffset.y)
                ),
            digitLayouts[i].Get(),
            m_textBrush.Get()
            );
    }

    m_d2dContext->SetTarget(oldTarget.Get());
}

// Positions text within the atlas. The resulting position will have its top-left
// corner at offsetInAtlas.

void FPSCounter::PositionAtlasText(
    _In_ Platform::String^ text,
    _In_ D2D1_POINT_2U offsetInAtlas,
    _Outptr_ IDWriteTextLayout** layout,
    _Out_ AtlasText* atlasText
    )
{
    // Create the text layout

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            text->Data(),
            text->Length(),
            m_textFormat.Get(),
            0.0f,
            0.0f,
            layout
            )
        );

    DX::ThrowIfFailed(
        (*layout)->SetWordWrapping(DWRITE_WORD_WRAPPING_NO_WRAP)
        );

    // Get the text metrics and compute the black box.

    DWRITE_TEXT_METRICS textMetrics;
    DX::ThrowIfFailed(
        (*layout)->GetMetrics(&textMetrics)
        );

    DWRITE_OVERHANG_METRICS overhangMetrics;
    DX::ThrowIfFailed(
        (*layout)->GetOverhangMetrics(&overhangMetrics)
        );

    // We use the overhang metrics here to determine the black box for the text.
    // The extra pixel of padding is added to make sure that we have enough
    // space reserved for an extra pixel of antialiasing falloff.
    D2D1_RECT_L blackBox = {
        DipsToPixelsRoundDown(-overhangMetrics.left) - 1,
        DipsToPixelsRoundDown(-overhangMetrics.top) - 1,
        DipsToPixelsRoundUp(overhangMetrics.right + textMetrics.layoutWidth) + 1,
        DipsToPixelsRoundUp(overhangMetrics.bottom + textMetrics.layoutHeight) + 1
    };

    // Fill in the atlas text position.

    atlasText->advanceWidth = DipsToPixelsRoundUp(textMetrics.widthIncludingTrailingWhitespace);

    atlasText->atlasRect = D2D1::RectU(
        offsetInAtlas.x,
        offsetInAtlas.y,
        offsetInAtlas.x + blackBox.right - blackBox.left,
        offsetInAtlas.y + blackBox.bottom - blackBox.top
        );

    atlasText->originOffset = D2D1::Point2L(
        blackBox.left,
        blackBox.top
        );
}

// Draws the cached text to the device context using FillOpacityMask.

void FPSCounter::DrawAtlasText(
    _In_ const AtlasText& atlasText,
    _In_ D2D1_POINT_2F offset
    )
{
    D2D1_ANTIALIAS_MODE oldAAMode = m_d2dContext->GetAntialiasMode();
    m_d2dContext->SetAntialiasMode(D2D1_ANTIALIAS_MODE_ALIASED);

    D2D1_RECT_F sourceRect;
    sourceRect.left = PixelsToDips(atlasText.atlasRect.left);
    sourceRect.top = PixelsToDips(atlasText.atlasRect.top);
    sourceRect.right = PixelsToDips(atlasText.atlasRect.right);
    sourceRect.bottom = PixelsToDips(atlasText.atlasRect.bottom);

    D2D1_RECT_F destRect;
    destRect.left = offset.x + PixelsToDips(atlasText.originOffset.x);
    destRect.top = offset.y + PixelsToDips(atlasText.originOffset.y);
    destRect.right = destRect.left + sourceRect.right - sourceRect.left;
    destRect.bottom = destRect.top + sourceRect.bottom - sourceRect.top;

    m_d2dContext->FillOpacityMask(
        m_textAtlasBitmap.Get(),
        m_textBrush.Get(),
        destRect,
        sourceRect
        );

    m_d2dContext->SetAntialiasMode(oldAAMode);
}


void FPSCounter::Render()
{
    // Calculate the FPS
    LARGE_INTEGER frequency;
    QueryPerformanceFrequency(&frequency);

    LARGE_INTEGER time;
    QueryPerformanceCounter(&time);

    if (m_times.size() == 50)
    {
        m_times.pop_front();
    }
    m_times.push_back(static_cast<int>(time.QuadPart));

    uint32 fps = 0;
    if (m_times.size() >= 2)
    {
        fps = static_cast<uint32>(
            0.5f +
            (static_cast<float>(m_times.size()-1) * static_cast<float>(frequency.QuadPart)) /
            static_cast<float>(m_times.back() - m_times.front())
            );
    }

    // Draw the static text

    D2D1_POINT_2F currentPosition = m_position;

    DrawAtlasText(m_cachedStaticText, currentPosition);
    currentPosition.x += m_firstDigitOffset.x;
    currentPosition.y += m_firstDigitOffset.y;

    // Draw the FPS number

    Platform::String^ fpsString = fps.ToString();

    for (uint32 i = 0; i < fpsString->Length(); i++)
    {
        uint32 digitIndex = fpsString->Data()[i] - '0';

        DrawAtlasText(m_cachedDigits[digitIndex], currentPosition);
        currentPosition.x += PixelsToDips(m_cachedDigits[digitIndex].advanceWidth);
    }
}

// Convert from device independent pixels to the nearest pixel, rounding down.
int FPSCounter::DipsToPixelsRoundDown(float value)
{
    return static_cast<int>(floor(value * m_dpi / 96.0f));
}

// Convert from device independent pixels to the nearest pixel, rounding up.
int FPSCounter::DipsToPixelsRoundUp(float value)
{
    return static_cast<int>(ceil(value * m_dpi / 96.0f));
}

// Convert from pixels to device independent pixels.
float FPSCounter::PixelsToDips(int value)
{
    return static_cast<float>(value) * 96.0f / m_dpi;
}