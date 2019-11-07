//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace DWriteColorFontFallback
{
    struct FormatInfo
    {
        WCHAR*  FontFamilyName;
        float   FontSize;
        bool    CustomFallback;
    };

    struct TextInfo
    {
        unsigned int    FormatId;
        unsigned int    BrushId;
        bool            IncludeFontFallbackDescription;
        char16*         TextString;
    };

    // Constants used in the Sample.
    namespace SampleConstants
    {
        static const float UIMaxZoom                = 9.0f;
        static const float UIMinZoom                = 1.0f;

        static const float TextMargin               = 10.0f;
        static const float TopMargin                = 150.0f;
        static const float BottomMargin             = 20.0f;
        static const float LeftMargin               = 50.0f;
        static const float RightMargin              = 50.0f;

        static const WCHAR* LocaleName              = L"en-us";
        static const WCHAR* EmojiFontFamilyName     = L"Segoe UI Emoji";
        static const WCHAR* SymbolFontFamilyName    = L"Segoe UI Symbol";

        static const D2D1_COLOR_F BrushColors[] =
        {
            D2D1::ColorF(D2D1::ColorF::Black),
            D2D1::ColorF(D2D1::ColorF::AntiqueWhite),
            D2D1::ColorF(0xdb7100, 1.0f),
            D2D1::ColorF(D2D1::ColorF::Navy),
        };
        static const unsigned int MaxBrushes        = ARRAYSIZE(BrushColors);
        static const unsigned int BrushBlack        = 0;
        static const unsigned int BrushBackground   = 1;
        static const unsigned int BrushOrange       = 2;
        static const unsigned int BrushNavy         = 3;

        static const FormatInfo Formats[] =
        {
            { L"Segoe UI Light", 28.0F, false },
            { L"Segoe UI Light", 22.0F, false },
            { L"Segoe UI",       20.0F, false },
            { L"Segoe UI",       20.0F, true },
            { L"Segoi UI Light", 16.0F, false }
        };
        static const unsigned int MaxFormats        = ARRAYSIZE(Formats);
        static const unsigned int FormatTitle       = 0;
        static const unsigned int FormatSubTitle    = 1;
        static const unsigned int FormatBody        = 2;
        static const unsigned int FormatBodyCustom  = 3;
        static const unsigned int FormatDirections  = 4;

        static const TextInfo TextStrings[] =
        {
            { FormatDirections, BrushNavy,      false, L"This sample demonstrates multicolor font rendering and custom font fallback.  All text below is set to the font 'Segoe UI'.  When codepoints are not found in the base font, font fallback is invoked.  If no appropriate glyph is found in the fallback list, then a square box is displayed.\nTo change the fallback invoke the AppBar and select 'Settings'.\nPan and Zoom controls are enabled for both touch and mouse/mouse wheel.\n" },
            { FormatTitle,      BrushOrange,    false, L"Segoe UI with standard System Font Fallback" },
            { FormatBody,       BrushBlack,     false, L"This is a sentence of Latin text." },
            { FormatBody,       BrushBlack,     false, L"This is a set of symbols in the Emoji range  '🌱🌲🌳🌴🌷🌹🌻🌽🌾🍅🍆🍇🍟🍠🍡🍢🍣🍤🍥🍦🍧🍨🍩🍪🍫🎂🎃🎄🎅🎆🎇🎉🎊🎋🎌🎍🎎'." },
            { FormatBody,       BrushBlack,     false, L"This is a set of symbols outside the Emoji range  '➊➊➋➌➍➎➏➐➑➒'." },
            { FormatBody,       BrushBlack,     false, L"This is a set of Katakana letters  'カガキギクグケゲコゴサザ'." },
            { FormatSubTitle,   BrushOrange,    false, L"" },
            { FormatTitle,      BrushOrange,    false, L"Segoe UI with custom Font Fallback" },
            { FormatSubTitle,   BrushOrange,    true,  L"Font fallback list : " },
            { FormatBody,       BrushBlack,     false, L"This is a sentence of Latin text." },
            { FormatDirections, BrushOrange,    false, L"\t(Codepoints are in Segoe UI.)" },
            { FormatBodyCustom, BrushBlack,     false, L"This is a set of symbols in the Emoji range  '🌱🌲🌳🌴🌷🌹🌻🌽🌾🍅🍆🍇🍟🍠🍡🍢🍣🍤🍥🍦🍧🍨🍩🍪🍫🎂🎃🎄🎅🎆🎇🎉🎊🎋🎌🎍🎎'." },
            { FormatDirections, BrushOrange,    false, L"\t(Codepoints are not in Segoe UI, but are in Segoe UI Emoji and Segoe UI Symbol.)" },
            { FormatBodyCustom, BrushBlack,     false, L"This is a set of symbols outside the Emoji range  '➊➊➋➌➍➎➏➐➑➒'." },
            { FormatDirections, BrushOrange,    false, L"\t(Codepoints are not in Segoe UI Emoji, but are in Segoe UI Symbol.)" },
            { FormatBodyCustom, BrushBlack,     false, L"This is a set of Katakana letters  'カガキギクグケゲコゴサザ'." },
            { FormatDirections, BrushOrange,    false, L"\t(Codepoints are not in Segoe UI, Segoe UI Emoji or Segoe UI Symbol.)" },
        };
        static const unsigned int MaxTextBlocks = ARRAYSIZE(TextStrings);

        static char16* FontFallbackDescriptions[] =
        {
            L"System",
            L"Segoe UI Emoji",
            L"Segoe UI Emoji -> System",
            L"Segoe UI Emoji -> Segoe UI Symbol",
            L"Segoe UI Emoji -> Segoe UI Symbol -> System",
            L"Segoe UI Symbol",
            L"Segoe UI Symbol -> System",
        };
        static const unsigned int MaxFontFallbackScenarios          = ARRAYSIZE(FontFallbackDescriptions);
        static const unsigned int FontFallbackSystem                = 0;
        static const unsigned int FontFallbackEmoji                 = 1;
        static const unsigned int FontFallbackEmojiSystem           = 2;
        static const unsigned int FontFallbackEmojiSymbol           = 3;
        static const unsigned int FontFallbackEmojiSymbolSystem     = 4;
        static const unsigned int FontFallbackSymbol                = 5;
        static const unsigned int FontFallbackSymbolSystem          = 6;
    };
};
