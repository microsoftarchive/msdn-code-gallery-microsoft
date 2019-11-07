// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Contents:    Implementation of text analyzer source and sink.
//
//----------------------------------------------------------------------------
#include "pch.h"
#include "TextAnalysis.h"

TextAnalysis::TextAnalysis(
    const wchar_t* text,
    UINT32 textLength,
    const wchar_t* localeName,
    IDWriteNumberSubstitution* numberSubstitution,
    ReadingDirection readingDirection,
    GlyphOrientationMode glyphOrientationMode
    )
:   m_text(text),
    m_textLength(textLength),
    m_localeName(localeName),
    m_readingDirection(readingDirection),
    m_numberSubstitution(numberSubstitution),
    m_glyphOrientationMode(glyphOrientationMode),
    m_currentPosition(0),
    m_currentRunIndex(0)
{
}

STDMETHODIMP TextAnalysis::GenerateResults(
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer,
    _Out_ std::vector<TextAnalysis::Run>& runs,
    _Out_ std::vector<DWRITE_LINE_BREAKPOINT>& breakpoints
    )
{
    // Analyzes the text using each of the analyzers and returns
    // their results as a series of runs.

    HRESULT hr = S_OK;

    // Initially start out with one result that covers the entire range.
    // This result will be subdivided by the analysis processes.
    m_runs.resize(1);
    LinkedRun& initialRun   = m_runs[0];
    initialRun.nextRunIndex = 0;
    initialRun.textStart    = 0;
    initialRun.textLength   = m_textLength;
    initialRun.bidiLevel    = (m_readingDirection & ReadingDirectionPrimaryProgression) != 0;

    // Allocate enough room to have one breakpoint per code unit.
    m_breakpoints.resize(m_textLength);

    // Call each of the analyzers in sequence, recording their results.
    if (SUCCEEDED(hr = textAnalyzer->AnalyzeLineBreakpoints(this, 0, m_textLength, this))
        &&  SUCCEEDED(hr = AnalyzeBidi(textAnalyzer.Get(), this, 0, m_textLength, this))
        &&  SUCCEEDED(hr = textAnalyzer->AnalyzeScript(this, 0, m_textLength, this))
        &&  SUCCEEDED(hr = textAnalyzer->AnalyzeNumberSubstitution(this, 0, m_textLength, this))
        &&  SUCCEEDED(hr = AnalyzeGlyphOrientation(textAnalyzer, this, 0, m_textLength, this))
        )
    {
        // Exchange our results with the caller's.
        breakpoints.swap(m_breakpoints);

        // Resequence the resulting runs in order before returning to caller.
        size_t totalRuns = m_runs.size();
        runs.resize(totalRuns);

        UINT32 nextRunIndex = 0;
        for (size_t i = 0; i < totalRuns; ++i)
        {
            runs[i]         = m_runs[nextRunIndex];
            nextRunIndex    = m_runs[nextRunIndex].nextRunIndex;
        }
    }

    return hr;
}

////////////////////////////////////////////////////////////////////////////////
// IDWriteTextAnalysisSource source implementation

IFACEMETHODIMP TextAnalysis::GetTextAtPosition(
    UINT32 textPosition,
    _Out_ WCHAR const** textString,
    _Out_ UINT32* textLength
    )
{
    if (textPosition >= m_textLength)
    {
        // Return no text if a query comes for a position at the end of
        // the range. Note that is not an error and we should not return
        // a failing HRESULT. Although the application is not expected
        // to return any text that is outside of the given range, the
        // analyzers may probe the ends to see if text exists.
        *textString = nullptr;
        *textLength = 0;
    }
    else
    {
        *textString = &m_text[textPosition];
        *textLength = m_textLength - textPosition;
    }
    return S_OK;
}

IFACEMETHODIMP TextAnalysis::GetTextBeforePosition(
    UINT32 textPosition,
    _Out_ WCHAR const** textString,
    _Out_ UINT32* textLength
    )
{
    if (textPosition == 0 || textPosition > m_textLength)
    {
        // Return no text if a query comes for a position at the end of
        // the range. Note that is not an error and we should not return
        // a failing HRESULT. Although the application is not expected
        // to return any text that is outside of the given range, the
        // analyzers may probe the ends to see if text exists.
        *textString = nullptr;
        *textLength = 0;
    }
    else
    {
        *textString = &m_text[0];
        *textLength = textPosition - 0; // text length is valid from current position backward
    }
    return S_OK;
}


DWRITE_READING_DIRECTION STDMETHODCALLTYPE TextAnalysis::GetParagraphReadingDirection()
{
    return (m_readingDirection & ReadingDirectionPrimaryProgression) != 0
        ? DWRITE_READING_DIRECTION_RIGHT_TO_LEFT
        : DWRITE_READING_DIRECTION_LEFT_TO_RIGHT;
}

IFACEMETHODIMP TextAnalysis::GetLocaleName(
    UINT32 textPosition,
    _Out_ UINT32* textLength,
    _Out_ WCHAR const** localeName
    )
{
    // The pointer returned should remain valid until the next call,
    // or until analysis ends. Since only one locale name is supported,
    // the text length is valid from the current position forward to
    // the end of the string.

    *localeName = m_localeName;
    *textLength = m_textLength - textPosition;

    return S_OK;
}

IFACEMETHODIMP TextAnalysis::GetNumberSubstitution(
    UINT32 textPosition,
    _Out_ UINT32* textLength,
    _Out_ IDWriteNumberSubstitution** numberSubstitution
    )
{
    if (m_numberSubstitution != nullptr)
    {
        m_numberSubstitution->AddRef();
    }

    *numberSubstitution = m_numberSubstitution;
    *textLength = m_textLength - textPosition;

    return S_OK;
}

STDMETHODIMP TextAnalysis::GetVerticalGlyphOrientation(
    UINT32 textPosition,
    _Out_ UINT32* textLength,
    _Out_ DWRITE_VERTICAL_GLYPH_ORIENTATION* glyphOrientation,
    _Out_ UINT8* bidiLevel
    )
{
    SetCurrentRun(textPosition);
    const LinkedRun& run    = m_runs[m_currentRunIndex];
    *bidiLevel              = run.bidiLevel;
    *glyphOrientation       = DWRITE_VERTICAL_GLYPH_ORIENTATION_DEFAULT;

    // Find the next range where a bidi level changes.

    const size_t totalRuns = m_runs.size();
    UINT32 lastRunPosition = textPosition;
    UINT32 nextRunIndex = m_currentRunIndex;

    do
    {
        const LinkedRun& nextRun = m_runs[nextRunIndex];
        if (nextRun.bidiLevel != run.bidiLevel)
        {
            break;
        }
        lastRunPosition = nextRun.textStart + nextRun.textLength;
        nextRunIndex = nextRun.nextRunIndex;
    }
    while (nextRunIndex < totalRuns && nextRunIndex != 0);

    *textLength = lastRunPosition - textPosition;

    return S_OK;
}

////////////////////////////////////////////////////////////////////////////////
// IDWriteTextAnalysisSink implementation

IFACEMETHODIMP TextAnalysis::SetLineBreakpoints(
    UINT32 textPosition,
    UINT32 textLength,
    DWRITE_LINE_BREAKPOINT const* lineBreakpoints   // [textLength]
    )
{
    if (textLength > 0)
    {
        memcpy(&m_breakpoints[textPosition], lineBreakpoints, textLength * sizeof(lineBreakpoints[0]));
    }
    return S_OK;
}

IFACEMETHODIMP TextAnalysis::SetScriptAnalysis(
    UINT32 textPosition,
    UINT32 textLength,
    DWRITE_SCRIPT_ANALYSIS const* scriptAnalysis
    )
{
    try
    {
        SetCurrentRun(textPosition);
        SplitCurrentRun(textPosition);
        while (textLength > 0)
        {
            LinkedRun& run  = FetchNextRun(&textLength);
            run.script      = *scriptAnalysis;
        }
    }
    catch (...)
    {
        return E_FAIL; // Unknown error, probably out of memory.
    }

    return S_OK;
}

IFACEMETHODIMP TextAnalysis::SetBidiLevel(
    UINT32 textPosition,
    UINT32 textLength,
    UINT8 explicitLevel,
    UINT8 resolvedLevel
    )
{
    try
    {
        SetCurrentRun(textPosition);
        SplitCurrentRun(textPosition);
        while (textLength > 0)
        {
            LinkedRun& run  = FetchNextRun(&textLength);
            run.bidiLevel   = resolvedLevel;
            run.isReversed  = !!(resolvedLevel & 1);
        }
    }
    catch (...)
    {
        return E_FAIL; // Unknown error, probably out of memory.
    }

    return S_OK;
}

IFACEMETHODIMP TextAnalysis::SetNumberSubstitution(
    UINT32 textPosition,
    UINT32 textLength,
    IDWriteNumberSubstitution* numberSubstitution
    )
{
    try
    {
        SetCurrentRun(textPosition);
        SplitCurrentRun(textPosition);
        while (textLength > 0)
        {
            LinkedRun& run          = FetchNextRun(&textLength);
            run.isNumberSubstituted = (numberSubstitution != nullptr);
        }
    }
    catch (...)
    {
        return E_FAIL; // Unknown error, probably out of memory.
    }

    return S_OK;
}

IFACEMETHODIMP TextAnalysis::SetGlyphOrientation(
    UINT32 textPosition,
    UINT32 textLength,
    DWRITE_GLYPH_ORIENTATION_ANGLE glyphOrientationAngle,
    UINT8 adjustedBidiLevel,
    BOOL isSideways,
    BOOL isRightToLeft
    )
{
    try
    {
        // Mapping from angle down to small orientation.
        const static UINT8 glyphOrientations[] = {
            GlyphOrientationCW0,
            GlyphOrientationCW90,
            GlyphOrientationCW180,
            GlyphOrientationCW270,
        };

        SetCurrentRun(textPosition);
        SplitCurrentRun(textPosition);
        while (textLength > 0)
        {
            LinkedRun& run          = FetchNextRun(&textLength);
            run.glyphOrientation    = glyphOrientations[glyphOrientationAngle & 3];
            run.isSideways          = !!isSideways;
            run.isReversed          = !!isRightToLeft;
            run.bidiLevel           = adjustedBidiLevel;
        }
    }
    catch (...)
    {
        return E_FAIL; // Unknown error, probably out of memory.
    }

    return S_OK;
}

STDMETHODIMP TextAnalysis::AnalyzeGlyphOrientation(
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer,
    IDWriteTextAnalysisSource1* analysisSource,
    UINT32 firstPosition,
    UINT32 textLength,
    IDWriteTextAnalysisSink1* analysisSink
    )
{
    if (firstPosition + textLength < firstPosition)
    {
        return E_INVALIDARG;
    }

    if (textLength == 0)
    {
        return S_OK;
    }

    if ((m_readingDirection & ReadingDirectionPrimaryAxis) != 0) // if vertical direction
    {
        switch (m_glyphOrientationMode)
        {
        case GlyphOrientationModeDefault:
        case GlyphOrientationModeStacked:
            textAnalyzer->AnalyzeVerticalGlyphOrientation(analysisSource, firstPosition, textLength, analysisSink);
            break;
        }
    }

    return S_OK;
}

STDMETHODIMP TextAnalysis::AnalyzeBidi(
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer,
    IDWriteTextAnalysisSource1* analysisSource,
    UINT32 firstPosition,
    UINT32 textLength,
    IDWriteTextAnalysisSink1* analysisSink
    )
{
    if (firstPosition + textLength < firstPosition)
    {
        return E_INVALIDARG;
    }

    if (textLength == 0)
    {
        return S_OK;
    }

    textAnalyzer->AnalyzeBidi(analysisSource, firstPosition, textLength, analysisSink);

    return S_OK;
}

// Run modification.
TextAnalysis::LinkedRun& TextAnalysis::FetchNextRun(
    _Inout_ UINT32* textLength
    )
{
    // Used by the sink setters, this returns a reference to the next run.
    // Position and length are adjusted to now point after the current run
    // being returned.
    UINT32 runIndex      = m_currentRunIndex;
    UINT32 runTextLength = m_runs[m_currentRunIndex].textLength;

    // Split the tail if needed (the length remaining is less than the
    // current run's size).
    if (*textLength < runTextLength)
    {
        runTextLength       = *textLength; // Limit to what's actually left.
        UINT32 runTextStart = m_runs[m_currentRunIndex].textStart;

        SplitCurrentRun(runTextStart + runTextLength);
    }
    else
    {
        // Just advance the current run.
        m_currentRunIndex = m_runs[m_currentRunIndex].nextRunIndex;
    }
    *textLength -= runTextLength;


    // Return a reference to the run that was just current.
    return m_runs[runIndex];
}

void TextAnalysis::SetCurrentRun(UINT32 textPosition)
{
    // Move the current run to the given position.
    // Since the analyzers generally return results in a forward manner,
    // this will usually just return early. If not, find the
    // corresponding run for the text position.
    if (m_currentRunIndex < m_runs.size()
        &&  m_runs[m_currentRunIndex].ContainsTextPosition(textPosition))
    {
        return;
    }

    m_currentRunIndex = static_cast<UINT32>(
        std::find(m_runs.begin(), m_runs.end(), textPosition)
        - m_runs.begin()
        );
}

void TextAnalysis::SplitCurrentRun(UINT32 splitPosition)
{
    // Splits the current run and adjusts the run values accordingly.

    UINT32 runTextStart = m_runs[m_currentRunIndex].textStart;

    if (splitPosition <= runTextStart)
    {
        return; // no change
    }

    // Grow runs by one.
    size_t totalRuns = m_runs.size();
    try
    {
        m_runs.resize(totalRuns + 1);
    }
    catch (...)
    {
        return; // Can't increase size. Return same run.
    }

    // Copy the old run to the end.
    LinkedRun& frontHalf = m_runs[m_currentRunIndex];
    LinkedRun& backHalf  = m_runs.back();
    backHalf             = frontHalf;

    // Adjust runs' text positions and lengths.
    UINT32 splitPoint       = splitPosition - runTextStart;
    backHalf.textStart     += splitPoint;
    backHalf.textLength    -= splitPoint;
    frontHalf.textLength    = splitPoint;
    frontHalf.nextRunIndex  = static_cast<UINT32>(totalRuns);
    m_currentRunIndex       = static_cast<UINT32>(totalRuns);
}