//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXSample.h"
#include <list>
#include <vector>

// This class displays how fast an application is rendering in frames per second,
// or FPS. So as not to take too much time rendering the text and interfere with
// the performance of the application, this class caches all the digits 0-9 in
// an atlas bitmap when it is first initialized, and then it renders the cached
// digits from that atlas each frame.
ref class FPSCounter
{
internal:
    FPSCounter(
        _In_ ID2D1DeviceContext* d2dContext,
        _In_ float dpi,
        _In_ Platform::String^ text,
        _In_ IDWriteTextFormat* textFormat,
        _In_ D2D1_POINT_2F position
        );

    void Initialize();

    void Render();

private:

    // This struct represents the position of a piece of text within the atlas.
    // All units are in pixels.
    struct AtlasText
    {
        D2D1_RECT_U atlasRect;      // The black box of text in the atlas
        D2D1_POINT_2L originOffset; // The offset to draw the text (relative to the origin)
        uint32 advanceWidth;        // The distance to the next character
    };

    void PositionAtlasText(
        _In_ Platform::String^ text,
        _In_ D2D1_POINT_2U offsetInAtlas,
        _Outptr_ IDWriteTextLayout** layout,
        _Out_ AtlasText* atlasText
        );

    void DrawAtlasText(
        _In_ const AtlasText& atlasText,
        _In_ D2D1_POINT_2F offset
        );

    int DipsToPixelsRoundDown(float value);
    int DipsToPixelsRoundUp(float value);
    float PixelsToDips(int value);

    Microsoft::WRL::ComPtr<ID2D1DeviceContext> m_d2dContext;
    float m_dpi;
    Microsoft::WRL::ComPtr<IDXGIDevice2> m_dxgiDevice;
    Microsoft::WRL::ComPtr<IDWriteFactory> m_dwriteFactory;
    Platform::String^ m_staticText;
    Microsoft::WRL::ComPtr<IDWriteTextFormat> m_textFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_textBrush;
    Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_textAtlasBitmap;

    D2D1_POINT_2F m_position;
    AtlasText m_cachedStaticText;
    D2D1_POINT_2U m_firstDigitOffset;
    AtlasText m_cachedDigits[10];
    std::list<int> m_times;
};
