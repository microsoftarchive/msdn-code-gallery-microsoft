// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Contents:    Sink interface for where text is placed.
//
//----------------------------------------------------------------------------
#include "pch.h"
#include "DirectXBase.h"
#include "FlowSink.h"
#include "TextAnalysis.h"

void FlowLayoutSink::Reset()
{
    m_glyphRuns.clear();
    m_glyphIndices.clear();
    m_glyphAdvances.clear();
    m_glyphOffsets.clear();
}

void FlowLayoutSink::Prepare(UINT32 glyphCount)
{
    // Reserve a known glyph count up front.
    m_glyphIndices.reserve (glyphCount);
    m_glyphAdvances.reserve(glyphCount);
    m_glyphOffsets.reserve (glyphCount);
}

void FlowLayoutSink::SetGlyphRun(
    float x,
    float y,
    UINT32 glyphCount,
    const UINT16* glyphIndices,                 // [glyphCount]
    const float* glyphAdvances,                 // [glyphCount]
    const DWRITE_GLYPH_OFFSET* glyphOffsets,    // [glyphCount]
    Microsoft::WRL::ComPtr<IDWriteFontFace> fontFace,
    float fontEmSize,
    UINT8 glyphOrientation,
    bool isReversed,
    bool isSideways
    )
{
    // Append this glyph run to the list.
    m_glyphRuns.resize(m_glyphRuns.size() + 1);
    CustomGlyphRun& glyphRun = m_glyphRuns.back();
    UINT32 glyphStart = static_cast<UINT32>(m_glyphAdvances.size());

    m_glyphIndices.insert (m_glyphIndices.end(),  glyphIndices,  glyphIndices  + glyphCount);
    m_glyphAdvances.insert(m_glyphAdvances.end(), glyphAdvances, glyphAdvances + glyphCount);
    m_glyphOffsets.insert (m_glyphOffsets.end(),  glyphOffsets,  glyphOffsets  + glyphCount);

    glyphRun.x                  = x;
    glyphRun.y                  = y;
    glyphRun.glyphOrientation   = glyphOrientation;
    glyphRun.glyphStart         = glyphStart;
    glyphRun.isSideways         = isSideways;
    glyphRun.isReversed         = isReversed;
    glyphRun.glyphCount         = glyphCount;
    glyphRun.fontEmSize         = fontEmSize;
    glyphRun.fontFace           = fontFace.Get();
}

void FlowLayoutSink::DrawGlyphRuns(
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext,
    Microsoft::WRL::ComPtr<IDWriteRenderingParams> renderingParams,
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> textColor
    ) const
{
    // Just iterate through all the saved glyph runs
    // and have DWrite to draw each one.
    if (m_dwriteFactory == nullptr)
    {
        return;
    }

    if (m_textAnalyzer == nullptr)
    {
        Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer0;

        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTextAnalyzer(&textAnalyzer0)
            );

        DX::ThrowIfFailed(
            textAnalyzer0.As(&m_textAnalyzer)
            );
    }

    // Mapping from small orientation back to DWrite compatible angle.
    const static DWRITE_GLYPH_ORIENTATION_ANGLE glyphOrientationAngle[] = {
        DWRITE_GLYPH_ORIENTATION_ANGLE_0_DEGREES,   // LTR TTB
        DWRITE_GLYPH_ORIENTATION_ANGLE_0_DEGREES,   // RTL TTB * no mapping, so just zero
        DWRITE_GLYPH_ORIENTATION_ANGLE_0_DEGREES,   // LTR BTT * no mapping, so just zero
        DWRITE_GLYPH_ORIENTATION_ANGLE_180_DEGREES, // RTL BTT
        DWRITE_GLYPH_ORIENTATION_ANGLE_0_DEGREES,   // TTB LTR * no mapping, so just zero
        DWRITE_GLYPH_ORIENTATION_ANGLE_270_DEGREES, // BTT LTR
        DWRITE_GLYPH_ORIENTATION_ANGLE_90_DEGREES,  // TTB RTL
        DWRITE_GLYPH_ORIENTATION_ANGLE_0_DEGREES,   // BTT RTL * no mapping, so just zero
    };

    DWRITE_MATRIX transform = { };

    for (size_t i = 0; i < m_glyphRuns.size(); ++i)
    {
        DWRITE_GLYPH_RUN glyphRun;
        const CustomGlyphRun& customGlyphRun = m_glyphRuns[i];

        if (customGlyphRun.glyphCount == 0)
        {
            continue;
        }

        // Massage the custom glyph run to something directly
        // digestable by DrawGlyphRun.
        customGlyphRun.Convert(
            &m_glyphIndices [0],
            &m_glyphAdvances[0],
            &m_glyphOffsets [0],
            &glyphRun
            );

        // This call is valid for both horizontal and vertical
        // (though it's a nop for horizontal, returning identity).
        m_textAnalyzer->GetGlyphOrientationTransform(
            glyphOrientationAngle[customGlyphRun.glyphOrientation],
            customGlyphRun.isSideways,
            OUT &transform
            );

        transform.dx = customGlyphRun.x;
        transform.dy = customGlyphRun.y;
        deviceContext->SetTransform((D2D_MATRIX_3X2_F*) &transform);

        deviceContext->DrawGlyphRun(
            D2D1::Point2F(0, 0),
            &glyphRun,
            nullptr,
            textColor.Get()
            );
    }
}

void FlowLayoutSink::CustomGlyphRun::Convert(
    const UINT16* glyphIndices,
    const float* glyphAdvances,
    const DWRITE_GLYPH_OFFSET* glyphOffsets,
    OUT DWRITE_GLYPH_RUN* glyphRun
    ) const
{
    // Populate the DWrite glyph run.
    glyphRun->glyphIndices  = &glyphIndices [glyphStart];
    glyphRun->glyphAdvances = &glyphAdvances[glyphStart];
    glyphRun->glyphOffsets  = &glyphOffsets [glyphStart];
    glyphRun->glyphCount    = glyphCount;
    glyphRun->fontEmSize    = fontEmSize;
    glyphRun->fontFace      = fontFace;
    glyphRun->bidiLevel     = isReversed;
    glyphRun->isSideways    = isSideways;
}