//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

ref class ::Text;

//  <text-frame> represents the drawing of a text block which may contain multiple paragraphs.
//  It supports margin in order to control alignment of text content inside a container such as
//  a list. It supports leading to better control spacing to the first line and between the lines.
//  Text inside this element is typeset by document-specific font. Document-specific font resides
//  as embedded resource within the document and is only available to that document. DirectWrite
//  fully supports document-specific font through custom font collection loader callback.
//
//  Text frame accepts the following additional markup attributes:
//      font-size = "<float>"
//          Font size. If not specified, the text size follows the 'font-size' attribute of <text> element.
//      color = "#<rgb>"
//          Color in red-green-blue format i.e. the value of "#FF0000" is red
//      opacity = "<0..1>"
//          Opacity value from 0.0 to 1.0 where 0.0 is fully transparent and 1.0 is fully opaque
//      top-margin = "<float>
//          Amount of spacing from the top edge of the element to the top of the text in design unit
//      bottom-margin = "<float>"
//          Amount of spacing from the bottom edge of the element to the bottom of the text in design unit
//      leading = "<float>"
//          Amount of spacing between two adjacent lines
//      baseline = "<float>"
//          Amount of spacing from the top of the text to the top of the first line drawn in design unit
//      text-align = "leading|trailing|center"
//          How text is aligned within the element
//      glow = "<float>"
//          Standard deviation value of the glow effect of the text.
//      glow-color = "#<rgb>"
//          Glow color of the glow effect of the text in color format.
//
ref class TextFrame : public Element
{
internal:
    TextFrame();

    virtual bool Initialize(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::XmlElement^ xmlElement
        ) override;

    virtual bool BindResource(_In_ Element^ rootElement) override;

    virtual void Measure(
        _In_ Document^ document,
        D2D1_SIZE_F const& parentSize,
        _Out_ D2D1_RECT_F* bounds
        ) override;

    virtual bool PrepareToDraw(
        _In_ Document^ document,
        D2D1::Matrix3x2F const& transform
        ) override;

    virtual bool Draw(
        _In_ Document^ document,
        D2D1::Matrix3x2F const& transform
        ) override;

    virtual float GetOpacity() override
    {
        return m_opacity;
    }

private:
    // Name string of the source text
    Platform::String^ m_sourceName;

    // Source text of the text frame
    ::Text^ m_sourceText;

    // Brush to fill the text
    Microsoft::WRL::ComPtr<ID2D1Brush> m_brush;

    // Direct2D shadow effect for glow
    Microsoft::WRL::ComPtr<ID2D1Effect> m_glowEffect;

    // Direct2D composite effect to composite text over the glow
    Microsoft::WRL::ComPtr<ID2D1Effect> m_compositeEffect;

    // Intermediate surface containing the drawing of text content.
    // This is only needed when effect is used for the element.
    Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_contentBitmap;

    // The display size the content caching is for.
    D2D1_SIZE_F m_displaySize;

    // Top margin of the text
    float m_topMargin;

    // Bottom margin of the text
    float m_bottomMargin;

    // Baseline-to-baseline leading between two lines
    float m_leading;

    // Offset to the baseline of the first line when leading is in use
    float m_baseline;

    // Text horizontal alignment
    DWRITE_TEXT_ALIGNMENT m_textAlign;

    // Text color
    uint32 m_color;

    // Text opacity
    float m_opacity;

    // Font size
    float m_fontSize;
};
