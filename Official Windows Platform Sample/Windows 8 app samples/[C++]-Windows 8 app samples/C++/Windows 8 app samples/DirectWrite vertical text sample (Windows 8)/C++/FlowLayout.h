// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Contents:    Custom layout, demonstrating usage of shaping and glyph
//              results.
//
//----------------------------------------------------------------------------
#pragma once

#include "FlowSource.h"
#include "FlowSink.h"
#include "TextAnalysis.h"
#include "DirectXBase.h"


class DECLSPEC_UUID("E304E995-6157-48ec-8D44-ACB308A210D0") FlowLayout:
    public ComBase<QiListSelf<FlowLayout, QiList<IUnknown>>>
{
    // This custom layout processes layout in two stages.
    //
    // 1. Analyze the text, given the current font and size
    //      a. Bidirectional analysis
    //      b. Script analysis
    //      c. Number substitution analysis
    //      d. Shape glyphs
    //      e. Intersect run results
    //
    // 2. Fill the text to the given shape
    //      a. Pull next rect from flow source
    //      b. Fit as much text as will go in
    //      c. Push text to flow sink

public:
    struct ClusterPosition
    {
        ClusterPosition():
            textPosition(),
            runIndex(),
            runEndPosition()
        {
        }
        UINT32 textPosition;    // Current text position
        UINT32 runIndex;        // Associated analysis run covering this position
        UINT32 runEndPosition;  // Text position where this run ends
    };

    enum JustificationMode
    {
        JustificationModeNone,
        JustificationModeInterword,
    };

public:
    FlowLayout(Microsoft::WRL::ComPtr<IDWriteFactory> dwriteFactory)
    :   m_dwriteFactory(SafeAcquire(dwriteFactory.Get())),
        m_fontFace(),
        m_numberSubstitution(),
        m_readingDirection(ReadingDirectionLeftToRightTopToBottom),
        m_glyphOrientationMode(GlyphOrientationModeDefault),
        m_justificationMode(JustificationModeInterword),
        m_fontEmSize(12),
        m_maxSpaceWidth(8),
        m_isTextAnalysisComplete(false)
    {
    }

    void SetTextFormat(Microsoft::WRL::ComPtr<IDWriteTextFormat> textFormat);
    void SetNumberSubstitution(Microsoft::WRL::ComPtr<IDWriteNumberSubstitution> numberSubstitution);
    void SetReadingDirection(ReadingDirection readingDirection);
    void SetGlyphOrientationMode(GlyphOrientationMode glyphOrientationMode);
    void SetJustificationMode(JustificationMode justificationMode);
    GlyphOrientationMode GetGlyphOrientationMode();
    ReadingDirection GetReadingDirection();

    void SetText(
        const wchar_t* text,
        UINT32 textLength
        );

    void GetText(
        _Out_ const wchar_t** text,
        _Out_ UINT32* textLength
        );

    // Perform analysis on the current text, converting text to glyphs.
    void AnalyzeText();

    // Reflow the text analysis into
    void FlowText(
        Microsoft::WRL::ComPtr<FlowLayoutSource> flowSource,
        Microsoft::WRL::ComPtr<FlowLayoutSink> flowSink
        );

protected:
    void ShapeGlyphRuns(Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer);

    void ShapeGlyphRun(
        Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer,
        UINT32 runIndex,
        _Inout_ UINT32& glyphStart
        );

    void ShapeSimpleGlyphRun(
        UINT32 runIndex,
        _Inout_ UINT32& glyphStart
        );

    bool FitText(
        const ClusterPosition& clusterStart,
        UINT32 textEnd,
        float maxWidth,
        _Out_ ClusterPosition* clusterEnd
        );

    bool ProduceGlyphRuns(
        Microsoft::WRL::ComPtr<FlowLayoutSink> flowSink,
        const FlowLayoutSource::RectF& rect,
        const ClusterPosition& clusterStart,
        const ClusterPosition& clusterEnd
        ) const;

    void ProduceJustifiedAdvances(
        float maxWidth,
        const ClusterPosition& clusterStart,
        const ClusterPosition& clusterEnd,
        _Out_ std::vector<float>& justifiedAdvances
        ) const;

    void ProduceBidiOrdering(
        UINT32 spanStart,
        UINT32 spanCount,
        _Out_writes_(spanCount) UINT32* spanIndices
        ) const;

    void SetClusterPosition(
        _Inout_ ClusterPosition& cluster,
        UINT32 textPosition
        ) const;

    void AdvanceClusterPosition(
        _Inout_ ClusterPosition& cluster
        ) const;

    UINT32 GetClusterGlyphStart(
        const ClusterPosition& cluster
        ) const;

    float GetClusterRangeWidth(
        const ClusterPosition& clusterStart,
        const ClusterPosition& clusterEnd
        ) const;

    float GetClusterRangeWidth(
        UINT32 glyphStart,
        UINT32 glyphEnd,
        const float* glyphAdvances
        ) const;

protected:
    Microsoft::WRL::ComPtr<IDWriteFactory> m_dwriteFactory;

    // Input information.
    std::wstring m_text;
    wchar_t m_localeName[LOCALE_NAME_MAX_LENGTH];
    ReadingDirection m_readingDirection;
    GlyphOrientationMode m_glyphOrientationMode;
    JustificationMode m_justificationMode;
    Microsoft::WRL::ComPtr<IDWriteFontFace> m_fontFace;
    Microsoft::WRL::ComPtr<IDWriteNumberSubstitution> m_numberSubstitution;
    float m_fontEmSize;

    // Output text analysis results
    std::vector<TextAnalysis::Run> m_runs;
    std::vector<DWRITE_LINE_BREAKPOINT> m_breakpoints;
    std::vector<DWRITE_GLYPH_OFFSET> m_glyphOffsets;
    std::vector<UINT16> m_glyphClusters;
    std::vector<UINT16> m_glyphIndices;
    std::vector<float>  m_glyphAdvances;

    float m_maxSpaceWidth;           // maximum stretch of space allowed for justification
    bool m_isTextAnalysisComplete;   // text analysis was done.
};
