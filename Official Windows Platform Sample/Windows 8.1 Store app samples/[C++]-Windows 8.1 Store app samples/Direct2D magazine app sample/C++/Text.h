//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Resource.h"

ref class Document;

//  <text> represents a block of formatted text, which can be used in multiple text frames.
//  Text is a resource and as such it cannot draw. It allows child element to represent run
//  of text such as the <bold> element.
//
//  Text accepts the following additional markup attributes:
//      font-size = "<float>"
//          Size of the font em height in design unit
//      font-weight = "<100..999>"
//          Weight of the font from 1 to 999. DirectWrite defines known weights for convenience in the
//          DWRITE_FONT_WEIGHT enum.
//      font-style = "<0|1|2>"
//          Style of the font. DirectWrite defines known styles in DWRITE_FONT_STYLE enum.
//      font-stretch = "<0..9>"
//          Width of the font. DirectWrite defines known width in DWRITE_FONT_STRETCH enum.
//      font-family = "<font family name>"
//          Font family name as defined in the font's name table by the font author i.e. "Cambria" etc.
//
ref class Text : public Resource
{
internal:
    Text();

    virtual bool Initialize(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::XmlElement^ xmlElement
        ) override;

    virtual bool AcceptChildNode(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::IXmlNode^ childXmlNode
        ) override;

    void GetTextLayout(_Outptr_ IDWriteTextLayout** textLayout);

    inline size_t GetLength()
    {
        return m_textLength;
    }

private:
    Platform::String^ GetText(_In_ Windows::Data::Xml::Dom::IXmlNode^ xmlNode);

    // DirectWrite text layout
    Microsoft::WRL::ComPtr<IDWriteTextLayout1> m_textLayout;

    // Character offset to the current text run. Used during span addition
    size_t m_textPosition;

    // Total character length of the text including all spans
    size_t m_textLength;
};
