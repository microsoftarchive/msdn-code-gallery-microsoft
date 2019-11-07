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
#pragma once


/// <summary>
/// Direction for how reading progresses.
/// </summary>
enum ReadingDirection
{
    ReadingDirectionLeftToRightTopToBottom = 0,
    ReadingDirectionRightToLeftTopToBottom = 1,
    ReadingDirectionLeftToRightBottomToTop = 2,
    ReadingDirectionRightToLeftBottomToTop = 3,
    ReadingDirectionTopToBottomLeftToRight = 4,
    ReadingDirectionBottomToTopLeftToRight = 5,
    ReadingDirectionTopToBottomRightToLeft = 6,
    ReadingDirectionBottomToTopRightToLeft = 7,

    // Contributing bits
    ReadingDirectionPrimaryProgression     = 1, // False = Ltr/Ttb,    True = Rtl/Btt
    ReadingDirectionSecondaryProgression   = 2, // False = Ttb/Ltr,    True = Btt/Rtl
    ReadingDirectionPrimaryAxis            = 4, // False = Horizontal, True = Vertical

    // Shorter Aliases
    ReadingDirectionEs = ReadingDirectionLeftToRightTopToBottom,
    ReadingDirectionSw = ReadingDirectionTopToBottomRightToLeft,
    ReadingDirectionWn = ReadingDirectionRightToLeftBottomToTop,
    ReadingDirectionNe = ReadingDirectionBottomToTopLeftToRight,

    // A single direction
    ReadingDirectionE = ReadingDirectionLeftToRightTopToBottom,
    ReadingDirectionS = ReadingDirectionTopToBottomLeftToRight,
    ReadingDirectionW = ReadingDirectionRightToLeftTopToBottom,
    ReadingDirectionN = ReadingDirectionBottomToTopLeftToRight,
};


enum GlyphOrientationMode
{
    GlyphOrientationModeDefault,    // rotated or upright according to script default
    GlyphOrientationModeStacked,    // stacked if script allows it
    GlyphOrientationModeTotal
};

// Direction for how reading progresses.
enum GlyphOrientation
{
    GlyphOrientationLeftToRightTopToBottom = 0,
    GlyphOrientationRightToLeftTopToBottom = 1,
    GlyphOrientationLeftToRightBottomToTop = 2,
    GlyphOrientationRightToLeftBottomToTop = 3,
    GlyphOrientationTopToBottomLeftToRight = 4,
    GlyphOrientationBottomToTopLeftToRight = 5,
    GlyphOrientationTopToBottomRightToLeft = 6,
    GlyphOrientationBottomToTopRightToLeft = 7,

    // Rotational aliases
    GlyphOrientationCW0                    = GlyphOrientationLeftToRightTopToBottom,
    GlyphOrientationCW90                   = GlyphOrientationTopToBottomRightToLeft,
    GlyphOrientationCW180                  = GlyphOrientationRightToLeftBottomToTop,
    GlyphOrientationCW270                  = GlyphOrientationBottomToTopLeftToRight,

    // Internal shorter aliases
    GlyphOrientationES                     = GlyphOrientationLeftToRightTopToBottom,
    GlyphOrientationSW                     = GlyphOrientationTopToBottomRightToLeft,
    GlyphOrientationWN                     = GlyphOrientationRightToLeftBottomToTop,
    GlyphOrientationNE                     = GlyphOrientationBottomToTopLeftToRight,
    GlyphOrientationFH                     = GlyphOrientationRightToLeftTopToBottom,
    GlyphOrientationFV                     = GlyphOrientationLeftToRightBottomToTop,

    // Individual bits
    GlyphOrientationFlipHorizontal         = 1,  // Glyph Mirrored Horizontally  (Left <-> Right)
    GlyphOrientationFlipVertical           = 2,  // Glyph Mirrored Vertically (Top <-> Bottom)
    GlyphOrientationFlipDiagonal           = 4,  // Glyph Transposed Around Diagonal
};


// Helper source/sink class for text analysis.
class TextAnalysis
    :   public ComBase<
        QiList<IDWriteTextAnalysisSource1,
        QiList<IDWriteTextAnalysisSink1,
        QiList<IUnknown
    >>>>
{
public:
    // A single contiguous run of characters containing the same analysis results.
    struct Run
    {
        Run()
        :   textStart(),
            textLength(),
            glyphStart(),
            glyphCount(),
            bidiLevel(),
            script(),
            glyphOrientation(),
            isNumberSubstituted(),
            isSideways(),
            isReversed()
        { }

        UINT32 textStart;   // starting text position of this run
        UINT32 textLength;  // number of contiguous code units covered
        UINT32 glyphStart;  // starting glyph in the glyphs array
        UINT32 glyphCount;  // number of glyphs associated with this run of text
        DWRITE_SCRIPT_ANALYSIS script;
        UINT8 bidiLevel;
        UINT8 glyphOrientation;
        bool isNumberSubstituted;
        bool isSideways;
        bool isReversed;    // reversed along primary axis (LTR->RTL, TTB->BTT)

        inline bool ContainsTextPosition(UINT32 desiredTextPosition) const
        {
            return desiredTextPosition >= textStart
                && desiredTextPosition <  textStart + textLength;
        }

        inline bool operator ==(UINT32 desiredTextPosition) const
        {
            // Allows search by text position using std::find
            return ContainsTextPosition(desiredTextPosition);
        }
    };

    // Single text analysis run, which points to the next run.
    struct LinkedRun : Run
    {
        LinkedRun()
        :   nextRunIndex(0)
        { }

        UINT32 nextRunIndex;  // index of next run
    };

public:
    TextAnalysis(
        const wchar_t* text,
        UINT32 textLength,
        const wchar_t* localeName,
        IDWriteNumberSubstitution* numberSubstitution,
        ReadingDirection readingDirection,
        GlyphOrientationMode glyphOrientationMode
        );

    STDMETHODIMP GenerateResults(
        Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer,
        _Out_ std::vector<Run>& runs,
        _Out_ std::vector<DWRITE_LINE_BREAKPOINT>& breakpoints_
        );

    STDMETHODIMP AnalyzeGlyphOrientation(
        Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer,
        IDWriteTextAnalysisSource1* analysisSource,
        UINT32 textPosition,
        UINT32 textLength,
        IDWriteTextAnalysisSink1* analysisSink
        );

    STDMETHODIMP AnalyzeBidi(
        Microsoft::WRL::ComPtr<IDWriteTextAnalyzer1> textAnalyzer,
        IDWriteTextAnalysisSource1* analysisSource,
        UINT32 firstPosition,
        UINT32 textLength,
        IDWriteTextAnalysisSink1* analysisSink
        );

    // IDWriteTextAnalysisSource implementation
    IFACEMETHODIMP GetTextAtPosition(
        UINT32 textPosition,
        _Out_ WCHAR const** textString,
        _Out_ UINT32* textLength
        );

    IFACEMETHODIMP GetTextBeforePosition(
        UINT32 textPosition,
        _Out_ WCHAR const** textString,
        _Out_ UINT32* textLength
        );

    IFACEMETHODIMP_(DWRITE_READING_DIRECTION) GetParagraphReadingDirection();

    IFACEMETHODIMP GetLocaleName(
        UINT32 textPosition,
        _Out_ UINT32* textLength,
        _Out_ WCHAR const** localeName
        );

    IFACEMETHODIMP GetNumberSubstitution(
        UINT32 textPosition,
        _Out_ UINT32* textLength,
        _Out_ IDWriteNumberSubstitution** numberSubstitution
        );

    IFACEMETHODIMP TextAnalysis::GetVerticalGlyphOrientation(
        UINT32 textPosition,
        _Out_ UINT32* textLength,
        _Out_ DWRITE_VERTICAL_GLYPH_ORIENTATION* glyphOrientation,
        _Out_ UINT8* bidiLevel
        );

    // IDWriteTextAnalysisSink implementation
    IFACEMETHODIMP SetScriptAnalysis(
        UINT32 textPosition,
        UINT32 textLength,
        DWRITE_SCRIPT_ANALYSIS const* scriptAnalysis
        );

    IFACEMETHODIMP SetLineBreakpoints(
        UINT32 textPosition,
        UINT32 textLength,
        const DWRITE_LINE_BREAKPOINT* lineBreakpoints
        );

    IFACEMETHODIMP SetBidiLevel(
        UINT32 textPosition,
        UINT32 textLength,
        UINT8 explicitLevel,
        UINT8 resolvedLevel
        );

    IFACEMETHODIMP SetNumberSubstitution(
        UINT32 textPosition,
        UINT32 textLength,
        IDWriteNumberSubstitution* numberSubstitution
        );

    IFACEMETHODIMP SetGlyphOrientation(
        UINT32 textPosition,
        UINT32 textLength,
        DWRITE_GLYPH_ORIENTATION_ANGLE glyphOrientationAngle,
        UINT8 adjustedBidiLevel,
        BOOL isSideways,
        BOOL isRightToLeft
        );

    // Helpers
protected:
    LinkedRun& FetchNextRun(_Inout_ UINT32* textLength);

    void SetCurrentRun(UINT32 textPosition);

    void SplitCurrentRun(UINT32 splitPosition);

protected:
    // Input
    // (weak references are fine here, since this class is a transient
    //  stack-based helper that doesn't need to copy data)
    UINT32 m_textLength;
    const wchar_t* m_text;
    const wchar_t* m_localeName;
    IDWriteNumberSubstitution* m_numberSubstitution;
    ReadingDirection m_readingDirection;
    GlyphOrientationMode m_glyphOrientationMode;

    // Current processing state
    UINT32 m_currentPosition;
    UINT32 m_currentRunIndex;

    // Output
    std::vector<LinkedRun> m_runs;
    std::vector<DWRITE_LINE_BREAKPOINT> m_breakpoints;
};
