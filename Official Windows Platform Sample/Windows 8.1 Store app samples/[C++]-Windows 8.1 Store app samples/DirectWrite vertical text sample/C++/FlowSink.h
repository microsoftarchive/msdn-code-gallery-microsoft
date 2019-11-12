// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Contents:    Source interface for where text is allowed to flow.
//
//----------------------------------------------------------------------------
#pragma once

class DECLSPEC_UUID("7712E74A-C7E0-4664-B58E-854394F2ACB4") FlowLayoutSink
    :   public ComBase<QiListSelf<FlowLayoutSink, QiList<IUnknown>>>
{
public:
    FlowLayoutSink(Microsoft::WRL::ComPtr<IDWriteFactory> dwriteFactory):
        m_dwriteFactory(dwriteFactory),
        m_textAnalyzer()
    {
    }

    void Reset();

    void Prepare(UINT32 glyphCount);

    void SetGlyphRun(
        float x,
        float y,
        UINT32 glyphCount,
        const UINT16* glyphIndices,
        const float* glyphAdvances,
        const DWRITE_GLYPH_OFFSET* glyphOffsets,
        Microsoft::WRL::ComPtr<IDWriteFontFace> fontFace,
        float fontEmSize,
        UINT8 glyphOrientation,
        bool isReversed,
        bool isSideways
        );

    void DrawGlyphRuns(
        Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext,
        Microsoft::WRL::ComPtr<IDWriteRenderingParams> renderingParams,
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> textColor
        ) const;

protected:
    // This glyph run is based off DWRITE_GLYPH_RUN
    // and is trivially convertable to it, but stores
    // pointers as relative indices instead instead
    // of raw pointers, which makes it more useful for
    // storing in a vector. Additionally, it stores
    // the x, y coordinate.

    struct CustomGlyphRun
    {
        CustomGlyphRun()
        :   fontFace(),
            fontEmSize(),
            glyphStart(),
            glyphCount(),
            glyphOrientation(),
            isSideways(),
            isReversed(),
            x(),
            y()
        { }

        CustomGlyphRun(const CustomGlyphRun& b)
        {
            memcpy(this, &b, sizeof(*this));
            fontFace = SafeAcquire(b.fontFace);
        }

        CustomGlyphRun& operator =(const CustomGlyphRun& b)
        {
            if (this != &b)
            {
                // Define assignment operator in terms of destructor and
                // placement new constructor, paying heed to self assignment.
                this->~CustomGlyphRun();
                new(this) CustomGlyphRun(b);
            }
            return *this;
        }

        IDWriteFontFace* fontFace;
        float fontEmSize;
        float x;
        float y;
        UINT32 glyphStart;
        UINT32 glyphCount;
        UINT8 glyphOrientation;
        bool isSideways;
        bool isReversed;

        void Convert(
            const UINT16* glyphIndices,                 // [glyphCount]
            const float* glyphAdvances,                 // [glyphCount]
            const DWRITE_GLYPH_OFFSET* glyphOffsets,    // [glyphCount]
            OUT DWRITE_GLYPH_RUN* glyphRun
            ) const;
    };

    std::vector<CustomGlyphRun>         m_glyphRuns;
    std::vector<UINT16>                 m_glyphIndices;
    std::vector<float>                  m_glyphAdvances;
    std::vector<DWRITE_GLYPH_OFFSET>    m_glyphOffsets;

    Microsoft::WRL::ComPtr<IDWriteFactory> m_dwriteFactory;
    mutable Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> m_textAnalyzer;
};
