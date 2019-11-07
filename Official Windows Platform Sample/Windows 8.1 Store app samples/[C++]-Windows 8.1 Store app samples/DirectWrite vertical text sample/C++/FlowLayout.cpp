// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "TextAnalysis.h"
#include "FlowSource.h"
#include "FlowSink.h"
#include "FlowLayout.h"

namespace
{
    // Estimates the maximum number of glyph indices needed to hold a string of
    // a given length.  This is the formula given in the Uniscribe SDK and should
    // cover most cases. Degenerate cases will require a reallocation.
    UINT32 EstimateGlyphCount(UINT32 textLength)
    {
        return 3 * textLength / 2 + 16;
    }

    HRESULT GetGlyphMetrics(
        IDWriteFontFace* fontFace,
        DWRITE_MEASURING_MODE measuringMode,
        float fontEmSize,
        bool isSideways,
        UINT32 glyphCount,
        _In_reads_(glyphCount) UINT16* glyphIds,
        _Out_writes_(glyphCount) DWRITE_GLYPH_METRICS* glyphRunMetrics
        )
    {
        // Call the right function depending on the measuring mode.
        switch (measuringMode)
        {
        case DWRITE_MEASURING_MODE_GDI_CLASSIC:
        case DWRITE_MEASURING_MODE_GDI_NATURAL:
            return fontFace->GetGdiCompatibleGlyphMetrics(
                fontEmSize,
                1.0,
                nullptr,
                measuringMode == DWRITE_MEASURING_MODE_GDI_NATURAL,
                glyphIds,
                glyphCount,
                glyphRunMetrics,
                isSideways
                );

        case DWRITE_MEASURING_MODE_NATURAL:
        default:
            return fontFace->GetDesignGlyphMetrics(glyphIds, glyphCount, glyphRunMetrics, isSideways);
        }
    }
}

void FlowLayout::SetTextFormat(Microsoft::WRL::ComPtr<IDWriteTextFormat> textFormat)
{
    // Initializes properties using a text format, like font family, font size,
    // and reading direction. For simplicity, this custom layout supports
    // minimal formatting. No mixed formatting or alignment modes are supported.
    Microsoft::WRL::ComPtr<IDWriteFontCollection>  fontCollection;
    Microsoft::WRL::ComPtr<IDWriteFontFamily>      fontFamily;
    Microsoft::WRL::ComPtr<IDWriteFont>            font;

    wchar_t fontFamilyName[100];

    // note the reading direction from the format is ignored
    m_fontEmSize = textFormat->GetFontSize();

    DX::ThrowIfFailed(
        textFormat->GetLocaleName(m_localeName, ARRAYSIZE(m_localeName))
        );

    // Map font and style to fontFace.
    // Need the font collection to map from font name to actual font.
    DX::ThrowIfFailed(
        textFormat->GetFontCollection(&fontCollection)
        );

    if (fontCollection == nullptr)
    {
        // No font collection was set in the format, so use the system default.
        m_dwriteFactory->GetSystemFontCollection(&fontCollection);
    }

    // Find matching family name in collection.
    textFormat->GetFontFamilyName(fontFamilyName, ARRAYSIZE(fontFamilyName));

    UINT32 fontIndex = 0;

    BOOL fontExists = false;

    DX::ThrowIfFailed(
        fontCollection->FindFamilyName(fontFamilyName, &fontIndex, &fontExists)
        );

    if (!fontExists)
    {
        // If the given font does not exist, take what we can get
        // (displaying something instead of nothing), choosing the foremost
        // font in the collection.
        fontIndex = 0;
    }

    DX::ThrowIfFailed(
        fontCollection->GetFontFamily(fontIndex, &fontFamily)
        );

    DX::ThrowIfFailed(
        fontFamily->GetFirstMatchingFont(
            textFormat->GetFontWeight(),
            textFormat->GetFontStretch(),
            textFormat->GetFontStyle(),
            &font
            )
        );

    DX::ThrowIfFailed(
        font->CreateFontFace(&m_fontFace)
    );
}

void FlowLayout::SetNumberSubstitution(Microsoft::WRL::ComPtr<IDWriteNumberSubstitution> numberSubstitution)
{
    m_numberSubstitution = numberSubstitution;
}

void FlowLayout::SetReadingDirection(ReadingDirection readingDirection)
{
    m_readingDirection = readingDirection;
}

ReadingDirection FlowLayout::GetReadingDirection()
{
    return m_readingDirection;
}

void FlowLayout::SetGlyphOrientationMode(GlyphOrientationMode glyphOrientationMode)
{
    m_glyphOrientationMode = glyphOrientationMode;
}

GlyphOrientationMode FlowLayout::GetGlyphOrientationMode()
{
    return m_glyphOrientationMode;
}

void FlowLayout::SetJustificationMode(JustificationMode justificationMode)
{
    m_justificationMode = justificationMode;
}

void FlowLayout::SetText(
    const wchar_t* text,
    UINT32 textLength
    )
{
    // Analyzes the given text and keeps the results for later reflow.
    m_isTextAnalysisComplete = false;

    m_text.assign(text, textLength);
}

void FlowLayout::GetText(
    _Out_ const wchar_t** text,
    _Out_ UINT32* textLength
    )
{
    *text = &m_text[0];
    *textLength = static_cast<UINT32>(m_text.size());
}

void FlowLayout::AnalyzeText()
{
    // Analyzes the given text and keeps the results for later reflow.

    m_isTextAnalysisComplete = false;

    if (m_fontFace == nullptr)
    {
        throw ref new Platform::FailureException;
    }

    // Create a text analyzer.
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer0;
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer;

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextAnalyzer(&textAnalyzer0)
        );

    DX::ThrowIfFailed(
        textAnalyzer0.As(&textAnalyzer)
        );

    // Record the analyzer's results.
    TextAnalysis textAnalysis(
        m_text.c_str(),
        static_cast<UINT32>(m_text.size()),
        m_localeName,
        m_numberSubstitution.Get(),
        m_readingDirection,
        m_glyphOrientationMode
        );

    DX::ThrowIfFailed(
        textAnalysis.GenerateResults(textAnalyzer, m_runs, m_breakpoints)
        );

    // Convert the entire text to glyphs.
    ShapeGlyphRuns(textAnalyzer);

    m_isTextAnalysisComplete = true;
}

void FlowLayout::ShapeGlyphRuns(Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer)
{
    UINT32 textLength = static_cast<UINT32>(m_text.size());

    // Estimate the maximum number of glyph indices needed to hold a string.
    UINT32 estimatedGlyphCount = EstimateGlyphCount(textLength);

    m_glyphIndices.resize(estimatedGlyphCount);
    m_glyphOffsets.resize(estimatedGlyphCount);
    m_glyphAdvances.resize(estimatedGlyphCount);
    m_glyphClusters.resize(textLength);

    UINT32 glyphStart = 0;

    // Shape each run separately. This is needed whenever script, locale,
    // or reading direction changes.
    for (UINT32 runIndex = 0; runIndex < m_runs.size(); ++runIndex)
    {
        ShapeGlyphRun(textAnalyzer, runIndex, glyphStart);
    }

    m_glyphIndices.resize(glyphStart);
    m_glyphOffsets.resize(glyphStart);
    m_glyphAdvances.resize(glyphStart);
}

void FlowLayout::ShapeGlyphRun(
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer,
    UINT32 runIndex,
    _Inout_ UINT32& glyphStart
    )
{
    // Shapes a single run of text into glyphs.
    // Alternately, you could iteratively interleave shaping and line
    // breaking to reduce the number glyphs held onto at once. It's simpler
    // for this demostration to just do shaping and line breaking as two
    // separate processes, but realize that this does have the consequence that
    // certain advanced fonts containing line specific features (like Gabriola)
    // will shape as if the line is not broken.

    TextAnalysis::Run& run  = m_runs[runIndex];
    UINT32 textStart        = run.textStart;
    UINT32 textLength       = run.textLength;
    UINT32 maxGlyphCount    = static_cast<UINT32>(m_glyphIndices.size() - glyphStart);
    UINT32 actualGlyphCount = 0;

    run.glyphStart          = glyphStart;
    run.glyphCount          = 0;

    if (textLength == 0)
    {
        return;
    }

    // Allocate space for shaping to fill with glyphs and other information,
    // with about as many glyphs as there are text characters. We'll actually
    // need more glyphs than codepoints if they are decomposed into separate
    // glyphs, or fewer glyphs than codepoints if multiple are substituted
    // into a single glyph. In any case, the shaping process will need some
    // room to apply those rules to even make that determintation.
    if (textLength > maxGlyphCount)
    {
        maxGlyphCount = EstimateGlyphCount(textLength);
        UINT32 totalGlyphsArrayCount = glyphStart + maxGlyphCount;
        m_glyphIndices.resize(totalGlyphsArrayCount);
    }

    std::vector<DWRITE_SHAPING_TEXT_PROPERTIES>  textProps(textLength);
    std::vector<DWRITE_SHAPING_GLYPH_PROPERTIES> glyphProps(maxGlyphCount);

    // Get the glyphs from the text, retrying if needed.
    DWRITE_FONT_FEATURE verticalFontFeatures[] = {DWRITE_FONT_FEATURE_TAG_VERTICAL_WRITING, 1};
    DWRITE_TYPOGRAPHIC_FEATURES verticalFontFeatureList = {&verticalFontFeatures[0], 1};
    DWRITE_TYPOGRAPHIC_FEATURES emptyFontFeatureList = {nullptr, 0};
    DWRITE_TYPOGRAPHIC_FEATURES const* fontFeatureListPointer = nullptr;
    const UINT32 fontFeatureLengths[1] = {textLength};

    bool useFontFeatures = false;
    if (run.isSideways)
    {
        fontFeatureListPointer = &verticalFontFeatureList;
        useFontFeatures = true;
    }

    DX::ThrowIfFailed(
        textAnalyzer->GetGlyphs(
            &m_text[textStart],
            textLength,
            m_fontFace.Get(),
            run.isSideways,
            run.isReversed,
            &run.script,
            m_localeName,
            (run.isNumberSubstituted) ? m_numberSubstitution.Get() : nullptr,
            nullptr,
            nullptr,
            0,
            maxGlyphCount,
            &m_glyphClusters[textStart],
            &textProps[0],
            &m_glyphIndices[glyphStart],
            &glyphProps[0],
            &actualGlyphCount
            )
        );

    // Get the placement of the all the glyphs.
    m_glyphAdvances.resize(max(static_cast<size_t>(glyphStart + actualGlyphCount), m_glyphAdvances.size()));
    m_glyphOffsets.resize(max(static_cast<size_t>(glyphStart + actualGlyphCount), m_glyphOffsets.size()));

    DX::ThrowIfFailed(
        textAnalyzer->GetGlyphPlacements(
            &m_text[textStart],
            &m_glyphClusters[textStart],
            &textProps[0],
            textLength,
            &m_glyphIndices[glyphStart],
            &glyphProps[0],
            actualGlyphCount,
            m_fontFace.Get(),
            m_fontEmSize,
            run.isSideways,
            run.isReversed,
            &run.script,
            m_localeName,
            nullptr,
            nullptr,
            0,
            &m_glyphAdvances[glyphStart],
            &m_glyphOffsets[glyphStart]
            )
        );

    // Certain fonts, like Batang, contain glyphs for hidden control
    // and formatting characters. So we'll want to explicitly force their
    // advance to zero.
    if (run.script.shapes & DWRITE_SCRIPT_SHAPES_NO_VISUAL)
    {
        std::fill(
            m_glyphAdvances.begin() + glyphStart,
            m_glyphAdvances.begin() + glyphStart + actualGlyphCount,
            0.0f
            );
    }

    // Set the final glyph count of this run and advance the starting glyph.
    run.glyphCount = actualGlyphCount;
    glyphStart    += actualGlyphCount;
}

void FlowLayout::FlowText(
    Microsoft::WRL::ComPtr<FlowLayoutSource> flowSource,
    Microsoft::WRL::ComPtr<FlowLayoutSink> flowSink
    )
{
    // Reflow all the text, from source to sink.
    if (!m_isTextAnalysisComplete)
    {
        throw ref new Platform::FailureException();
    }

    // Determine the font line height, needed by the flow source.
    DWRITE_FONT_METRICS fontMetrics = {};
    m_fontFace->GetMetrics(&fontMetrics);

    float fontHeight = (fontMetrics.ascent + fontMetrics.descent + fontMetrics.lineGap)
        * m_fontEmSize / fontMetrics.designUnitsPerEm;

    // Get ready for series of glyph runs.
    flowSink->Prepare(static_cast<UINT32>(m_glyphIndices.size()));

    // Set initial cluster position to beginning of text.
    ClusterPosition cluster, nextCluster;
    SetClusterPosition(cluster, 0);

    FlowLayoutSource::RectF rect;
    UINT32 textLength = static_cast<UINT32>(m_text.size());

    // Iteratively pull rect's from the source,
    // and push as much text will fit to the sink.
    while (cluster.textPosition < textLength)
    {
        // Pull the next rect from the source.
        if (!flowSource->GetNextRect(fontHeight, m_readingDirection, &rect))
        {
            break;
        }

        if (rect.right <= rect.left && rect.bottom <= rect.top)
        {
            break; // Stop upon reaching zero sized rects.
        }

        // Determine length of line.
        float uSize = (m_readingDirection & ReadingDirectionPrimaryAxis)
                    ? rect.bottom - rect.top
                    : rect.right  - rect.left;

        if (uSize >= 0)
        {
            // Fit as many clusters between breakpoints that will go in.
            if (!FitText(cluster, textLength, uSize, &nextCluster))
                break;

            // Push the glyph runs to the sink.
            if (!ProduceGlyphRuns(flowSink, rect, cluster, nextCluster))
                break;

            cluster = nextCluster;
        }
    }
}

bool FlowLayout::FitText(
    const ClusterPosition& clusterStart,
    UINT32 textEnd,
    float maxWidth,
    _Out_ ClusterPosition* clusterEnd
    )
{
    // Fits as much text as possible into the given width,
    // using the clusters and advances returned by DWrite.

    // Set the starting cluster to the starting text position,
    // and continue until we exceed the maximum width or hit
    // a hard break.
    ClusterPosition cluster(clusterStart);
    ClusterPosition nextCluster(clusterStart);
    UINT32 validBreakPosition   = cluster.textPosition;
    UINT32 bestBreakPosition    = cluster.textPosition;
    float textWidth             = 0;

    while (cluster.textPosition < textEnd)
    {
        // Use breakpoint information to find where we can safely break words.
        AdvanceClusterPosition(nextCluster);
        const DWRITE_LINE_BREAKPOINT breakpoint = m_breakpoints[nextCluster.textPosition - 1];

        // Check whether we exceeded the amount of text that can fit,
        // unless it's whitespace, which we allow to flow beyond the end.
        textWidth += GetClusterRangeWidth(cluster, nextCluster);
        if (textWidth > maxWidth && !breakpoint.isWhitespace)
        {
            // Want a minimum of one cluster.
            if (validBreakPosition > clusterStart.textPosition)
            {
                break;
            }
        }

        validBreakPosition = nextCluster.textPosition;

        // See if we can break after this character cluster, and if so,
        // mark it as the new potential break point.
        if (breakpoint.breakConditionAfter != DWRITE_BREAK_CONDITION_MAY_NOT_BREAK)
        {
            bestBreakPosition = validBreakPosition;
            if (breakpoint.breakConditionAfter == DWRITE_BREAK_CONDITION_MUST_BREAK)
            {
                break; // we have a hard return, so we've fit all we can.
            }
        }
        cluster = nextCluster;
    }

    // Want last best position that didn't break a word, but if that's not available,
    // fit at least one cluster (emergency line breaking).
    if (bestBreakPosition == clusterStart.textPosition)
    {
        bestBreakPosition =  validBreakPosition;
    }

    SetClusterPosition(cluster, bestBreakPosition);

    *clusterEnd = cluster;

    return 1;
}

bool FlowLayout::ProduceGlyphRuns(
    Microsoft::WRL::ComPtr<FlowLayoutSink> flowSink,
    const FlowLayoutSource::RectF& rect,
    const ClusterPosition& clusterStart,
    const ClusterPosition& clusterEnd
    ) const
{
    // Produce a series of glyph runs from the given range
    // and send them to the sink. If the entire text fit
    // into the rect, then we'll only pass on a single glyph
    // run.

    const bool isVertical = (m_readingDirection & ReadingDirectionPrimaryAxis) != 0;
    const bool isReversedPrimaryDirection = (m_readingDirection & ReadingDirectionPrimaryProgression) != 0;

    // Determine length of line.
    const float uSize = (m_readingDirection & ReadingDirectionPrimaryAxis)
        ? rect.bottom - rect.top
        : rect.right  - rect.left;

    // Figure out how many runs we cross, because this is the number
    // of distinct glyph runs we'll need to reorder visually.

    UINT32 runIndexEnd = clusterEnd.runIndex;
    if (clusterEnd.textPosition > m_runs[runIndexEnd].textStart)
    {
        ++runIndexEnd; // Only partially cover the run, so round up.
    }

    // Fill in mapping from visual run to logical sequential run.
    UINT32 bidiOrdering[100];
    UINT32 totalRuns = runIndexEnd - clusterStart.runIndex;
    totalRuns = min(totalRuns, static_cast<UINT32>(ARRAYSIZE(bidiOrdering)));

    ProduceBidiOrdering(
        clusterStart.runIndex,
        totalRuns,
        &bidiOrdering[0]
        );

    // Ignore any trailing whitespace

    // Look backward from end until we find non-space.
    UINT32 trailingWsPosition = clusterEnd.textPosition;
    for (; trailingWsPosition > clusterStart.textPosition; --trailingWsPosition)
    {
        if (!m_breakpoints[trailingWsPosition-1].isWhitespace)
        {
            break; // Encountered last significant character.
        }
    }

    // Set the glyph run's ending cluster to the last whitespace.
    ClusterPosition clusterWsEnd(clusterStart);
    SetClusterPosition(clusterWsEnd, trailingWsPosition);

    // Produce justified advances to reduce the jagged edge.
    std::vector<float> justifiedAdvances;
    ProduceJustifiedAdvances(uSize, clusterStart, clusterWsEnd, justifiedAdvances);
    UINT32 justificationGlyphStart = GetClusterGlyphStart(clusterStart);

    // Determine starting point for alignment.
    float u         = (isVertical) ? rect.top  : rect.left;
    float v         = (isVertical) ? rect.left : rect.bottom;
    float ascent    = 0;
    float descent   = 0;
    float central   = 0;

    DWRITE_FONT_METRICS fontMetrics;
    m_fontFace->GetMetrics(&fontMetrics);

    ascent  = (fontMetrics.ascent  * m_fontEmSize / fontMetrics.designUnitsPerEm);
    descent = (fontMetrics.descent * m_fontEmSize / fontMetrics.designUnitsPerEm);
    central = (((fontMetrics.descent + fontMetrics.ascent) / 2) * m_fontEmSize / fontMetrics.designUnitsPerEm);

    if (isReversedPrimaryDirection)
    {
        // For RTL, we neeed the run width to adjust the origin
        // so it starts on the right side.
        UINT32 glyphStart = GetClusterGlyphStart(clusterStart);
        UINT32 glyphEnd   = GetClusterGlyphStart(clusterWsEnd);

        if (glyphStart < glyphEnd)
        {
            float lineWidth = GetClusterRangeWidth(
                glyphStart - justificationGlyphStart,
                glyphEnd   - justificationGlyphStart,
                &justifiedAdvances.front()
                );
            u += uSize - lineWidth;
        }
    }

    for (size_t i = 0; i < totalRuns; ++i)
    {
        const TextAnalysis::Run& run    = m_runs[bidiOrdering[i]];
        UINT32 glyphStart               = run.glyphStart;
        UINT32 glyphEnd                 = glyphStart + run.glyphCount;

        // If the run is only partially covered, we'll need to find
        // the subsection of glyphs that were fit.
        if (clusterStart.textPosition > run.textStart)
        {
            glyphStart = GetClusterGlyphStart(clusterStart);
        }
        if (clusterWsEnd.textPosition < run.textStart + run.textLength)
        {
            glyphEnd = GetClusterGlyphStart(clusterWsEnd);
        }
        if ((glyphStart >= glyphEnd)
            || (run.script.shapes & DWRITE_SCRIPT_SHAPES_NO_VISUAL))
        {
            // The itemizer told us not to draw this character,
            // either because it was a formatting, control, or other hidden character.
            continue;
        }

        // The run width is needed to know how far to move forward,
        // and to flip the origin for right-to-left text.
        float runWidth = GetClusterRangeWidth(
            glyphStart - justificationGlyphStart,
            glyphEnd   - justificationGlyphStart,
            &justifiedAdvances.front()
            );

        // Adjust glyph run to the appropriate baseline.
        float vAdjustment;
        if (run.isSideways)
        {
            vAdjustment = central;
        }
        else
        {
            if (!!(run.bidiLevel & 1) != run.isReversed)
            {
                vAdjustment = ascent; // occurs for Arabic in stacked vertical and Mongolian in inline horizontal
            }
            else
            {
                vAdjustment = descent; // the common case
            }
        }
        float adjustedV = v + (isVertical ? vAdjustment : -vAdjustment);
        float adjustedU = u + ((run.bidiLevel & 1) ? runWidth : 0); // origin starts from right if RTL

        // Flush this glyph run.
        flowSink->SetGlyphRun(
            isVertical ? adjustedV : adjustedU,
            isVertical ? adjustedU : adjustedV,
            glyphEnd - glyphStart,
            &m_glyphIndices[glyphStart],
            &justifiedAdvances[glyphStart - justificationGlyphStart],
            &m_glyphOffsets[glyphStart],
            m_fontFace,
            m_fontEmSize,
            run.glyphOrientation,
            run.isReversed,
            run.isSideways
            );

        u += runWidth;
    }
    return 1;
}

void FlowLayout::ProduceBidiOrdering(
    UINT32 spanStart,
    UINT32 spanCount,
    _Out_writes_(spanCount) UINT32* spanIndices
    ) const
{
    // Produces an index mapping from sequential order to visual bidi order.
    // The function progresses forward, checking the bidi level of each
    // pair of spans, reversing when needed.
    //
    // See the Unicode technical report 9 for an explanation.
    // http://www.unicode.org/reports/tr9/tr9-17.html

    // Fill all entries with initial indices
    for (UINT32 i = 0; i < spanCount; ++i)
    {
        spanIndices[i] = spanStart + i;
    }

    if (spanCount <= 1)
    {
        return;
    }

    size_t runStart = 0;
    UINT32 currentLevel = m_runs[spanStart].bidiLevel;

    // Rearrange each run to produced reordered spans.
    for (size_t i = 0; i < spanCount; ++i)
    {
        size_t runEnd       = i + 1;
        UINT32 nextLevel    = (runEnd < spanCount)
                            ? m_runs[spanIndices[runEnd]].bidiLevel
                            : 0; // past last element

        // We only care about transitions, particularly high to low,
        // because that means we have a run behind us where we can
        // do something.

        if (currentLevel <= nextLevel) // This is now the beginning of the next run.
        {
            if (currentLevel < nextLevel)
            {
                currentLevel = nextLevel;
                runStart     = i + 1;
            }
            continue; // Skip past equal levels, or increasing stairsteps.
        }

        do // currentLevel > nextLevel
        {
            // Recede to find start of the run and previous level.
            UINT32 previousLevel;
            for (;;)
            {
                if (runStart <= 0) // reached front of index list
                {
                    previousLevel = 0; // position before string has bidi level of 0
                    break;
                }
                if (m_runs[spanIndices[--runStart]].bidiLevel < currentLevel)
                {
                    previousLevel = m_runs[spanIndices[runStart]].bidiLevel;
                    ++runStart; // compensate for going one element past
                    break;
                }
            }

            // Reverse the indices, if the difference between the current and
            // next/previous levels is odd. Otherwise, it would be a no-op, so
            // don't bother.
            if ((min(currentLevel - nextLevel, currentLevel - previousLevel) & 1) != 0)
            {
                std::reverse(spanIndices + runStart, spanIndices + runEnd);
            }

            // Descend to the next lower level, the greater of previous and next
            currentLevel = max(previousLevel, nextLevel);
        }
        while (currentLevel > nextLevel); // Continue until completely flattened.
    }
}

void FlowLayout::ProduceJustifiedAdvances(
    float maxWidth,
    const ClusterPosition& clusterStart,
    const ClusterPosition& clusterEnd,
    _Out_ std::vector<float>& justifiedAdvances
    ) const
{
    // Performs simple inter-word justification
    // using the breakpoint analysis whitespace property.

    // Copy out default, unjustified advances.
    UINT32 glyphStart   = GetClusterGlyphStart(clusterStart);
    UINT32 glyphEnd     = GetClusterGlyphStart(clusterEnd);

    justifiedAdvances.assign(m_glyphAdvances.begin() + glyphStart, m_glyphAdvances.begin() + glyphEnd);


    if (m_justificationMode == JustificationModeNone)
    {
        return; // Nothing to justify.
    }

    if (glyphEnd - glyphStart == 0)
    {
        return; // No glyphs to modify.
    }

    if (maxWidth <= 0)
    {
        return; // Text can't fit anyway.
    }

    // First, count how many spaces there are in the text range.
    ClusterPosition cluster(clusterStart);
    UINT32 whitespaceCount = 0;

    while (cluster.textPosition < clusterEnd.textPosition)
    {
        if (m_breakpoints[cluster.textPosition].isWhitespace)
        {
            ++whitespaceCount;
        }
        AdvanceClusterPosition(cluster);
    }
    if (whitespaceCount <= 0)
    {
        return; // Can't justify using spaces, since none exist.
    }

    // Second, determine the needed contribution to each space.
    float lineWidth             = GetClusterRangeWidth(glyphStart, glyphEnd, &m_glyphAdvances.front());
    float justificationPerSpace = (maxWidth - lineWidth) / whitespaceCount;

    if (justificationPerSpace  <= 0)
    {
        return; // Either already justified or would be negative justification.
    }

    if (justificationPerSpace > m_maxSpaceWidth)
    {
        return; // Avoid justification if it would space the line out awkwardly far.
    }

    // Lastly, adjust the advance widths, adding the difference to each space character.
    cluster = clusterStart;
    while (cluster.textPosition < clusterEnd.textPosition)
    {
        if (m_breakpoints[cluster.textPosition].isWhitespace)
        {
            justifiedAdvances[GetClusterGlyphStart(cluster) - glyphStart] += justificationPerSpace;
        }

        AdvanceClusterPosition(cluster);
    }
}

// Since layout should never split text clusters, we want to move ahead whole
// clusters at a time.
void FlowLayout::SetClusterPosition(
    _Inout_ ClusterPosition& cluster,
    UINT32 textPosition
    ) const
{
    // Updates the current position and seeks its matching text analysis run.
    cluster.textPosition = textPosition;

    // If the new text position is outside the previous analysis run,
    // find the right one.
    if (textPosition >= cluster.runEndPosition
        || !m_runs[cluster.runIndex].ContainsTextPosition(textPosition))
    {
        // If we can resume the search from the previous run index,
        // (meaning the new text position comes after the previously
        // seeked one), we can save some time. Otherwise restart from
        // the beginning.

        UINT32 newRunIndex = 0;
        if (textPosition >= m_runs[cluster.runIndex].textStart)
        {
            newRunIndex = cluster.runIndex;
        }

        // Find new run that contains the text position.
        newRunIndex = static_cast<UINT32>(
            std::find(m_runs.begin() + newRunIndex, m_runs.end(), textPosition)
            - m_runs.begin()
            );

        // Keep run index within the list, rather than pointing off the end.
        if (newRunIndex >= m_runs.size())
        {
            newRunIndex  = static_cast<UINT32>(m_runs.size() - 1);
        }

        // Cache the position of the next analysis run to efficiently
        // move forward in the clustermap.
        const TextAnalysis::Run& matchingRun    = m_runs[newRunIndex];
        cluster.runIndex                        = newRunIndex;
        cluster.runEndPosition                  = matchingRun.textStart + matchingRun.textLength;
    }
}

void FlowLayout::AdvanceClusterPosition(
    _Inout_ ClusterPosition& cluster
    ) const
{
    // Looks forward in the cluster map until finding a new cluster,
    // or until we reach the end of a cluster run returned by shaping.
    //
    // Glyph shaping can produce a clustermap where a:
    //  - A single codepoint maps to a single glyph (simple Latin and precomposed CJK)
    //  - A single codepoint to several glyphs (diacritics decomposed into distinct glyphs)
    //  - Multiple codepoints are coalesced into a single glyph.
    //
    UINT32 textPosition = cluster.textPosition;
    UINT32 clusterId    = m_glyphClusters[textPosition];

    for (++textPosition; textPosition < cluster.runEndPosition; ++textPosition)
    {
        if (m_glyphClusters[textPosition] != clusterId)
        {
            // Now pointing to the next cluster.
            cluster.textPosition = textPosition;
            return;
        }
    }
    if (textPosition == cluster.runEndPosition)
    {
        // We crossed a text analysis run.
        SetClusterPosition(cluster, textPosition);
    }
}

UINT32 FlowLayout::GetClusterGlyphStart(const ClusterPosition& cluster) const
{
    // Maps from text position to corresponding starting index in the glyph array.
    // This is needed because there isn't a 1:1 correspondence between text and
    // glyphs produced.

    UINT32 glyphStart = m_runs[cluster.runIndex].glyphStart;

    return (cluster.textPosition < m_glyphClusters.size())
        ? glyphStart + m_glyphClusters[cluster.textPosition]
        : glyphStart + m_runs[cluster.runIndex].glyphCount;
}

float FlowLayout::GetClusterRangeWidth(
    const ClusterPosition& clusterStart,
    const ClusterPosition& clusterEnd
    ) const
{
    // Sums the glyph advances between two cluster positions,
    // useful for determining how long a line or word is.
    return GetClusterRangeWidth(
        GetClusterGlyphStart(clusterStart),
        GetClusterGlyphStart(clusterEnd),
        &m_glyphAdvances.front()
        );
}

float FlowLayout::GetClusterRangeWidth(
    UINT32 glyphStart,
    UINT32 glyphEnd,
    const float* glyphAdvances
    ) const
{
    // Sums the glyph advances between two glyph offsets, given an explicit
    // advances array - useful for determining how long a line or word is.
    return std::accumulate(glyphAdvances + glyphStart, glyphAdvances + glyphEnd, 0.0f);
}
