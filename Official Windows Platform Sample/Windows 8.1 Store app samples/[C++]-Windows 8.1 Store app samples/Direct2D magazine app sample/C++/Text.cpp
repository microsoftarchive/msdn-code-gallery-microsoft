//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Windows::Data::Xml::Dom;

Text::Text() :
    Resource(),
    m_textLayout(nullptr),
    m_textPosition(),
    m_textLength()
{
}

bool Text::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Resource::Initialize(document, xmlElement))
    {
        return false;
    }

    float fontSize = 16.0f;
    DWRITE_FONT_WEIGHT fontWeight = DWRITE_FONT_WEIGHT_NORMAL;
    DWRITE_FONT_STYLE fontStyle = DWRITE_FONT_STYLE_NORMAL;
    DWRITE_FONT_STRETCH fontStretch = DWRITE_FONT_STRETCH_NORMAL;

    Platform::String^ value = xmlElement->GetAttribute("font-size");

    if (value != nullptr)
    {
        fontSize = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("font-weight");

    if (value != nullptr)
    {
        fontWeight = static_cast<DWRITE_FONT_WEIGHT>(_wtol(value->Data()));
    }

    value = xmlElement->GetAttribute("font-style");

    if (value != nullptr)
    {
        fontStyle = static_cast<DWRITE_FONT_STYLE>(_wtol(value->Data()));
    }

    value = xmlElement->GetAttribute("font-stretch");

    if (value != nullptr)
    {
        fontStretch = static_cast<DWRITE_FONT_STRETCH>(_wtol(value->Data()));
    }

    ComPtr<IDWriteFactory> dwriteFactory;

    // Obtain DirectWrite factory from the current document
    document->GetDWriteFactory(&dwriteFactory);

    value = xmlElement->GetAttribute("font-family");

    if (value != nullptr)
    {
        // Creating and setting up text format and layout. This steps are
        // merely property setting. The actual work of laying out text is
        // deferred until the application asks for result or until the layout
        // is being drawn.
        ComPtr<IDWriteTextFormat> textFormat;

        DX::ThrowIfFailed(
            dwriteFactory->CreateTextFormat(
                value->Data(),
                nullptr,    // Proper font collection will be set upon drawing
                fontWeight,
                fontStyle,
                fontStretch,
                fontSize,
                L"en-US",
                &textFormat
                )
            );

        // This call concatenates all text within this DOM element including
        // text inside each child element.
        Platform::String^ text = GetText(xmlElement);
        m_textLength = text->Length();

        if (m_textLength != 0)
        {
            ComPtr<IDWriteTextLayout> textLayout;

            DX::ThrowIfFailed(
                dwriteFactory->CreateTextLayout(
                    text->Data(),
                    static_cast<uint32>(m_textLength),
                    textFormat.Get(),
                    FLT_MAX,
                    FLT_MAX,
                    &textLayout
                    )
                );

            textLayout.As(&m_textLayout);

            ComPtr<IDWriteFontCollection> fontCollection;
            document->GetFontCollection(&fontCollection);

            DWRITE_TEXT_RANGE range = {0};
            range.length = static_cast<uint32>(m_textLength);

            DX::ThrowIfFailed(
                m_textLayout->SetFontCollection(fontCollection.Get(), range)
                );
        }
    }

    return true;
}

Platform::String^ Text::GetText(_In_ IXmlNode^ xmlNode)
{
    if (xmlNode->NodeType == NodeType::TextNode)
    {
        return dynamic_cast<Platform::String^>(xmlNode->NodeValue);
    }

    Platform::String^ text = nullptr;

    if (xmlNode->HasChildNodes())
    {
        IXmlNode^ childXmlNode = xmlNode->FirstChild;

        while (childXmlNode != nullptr)
        {
            text = Platform::String::Concat(text, GetText(childXmlNode));

            childXmlNode = childXmlNode->NextSibling;
        }
    }

    return text;
}

bool Text::AcceptChildNode(
    _In_ Document^ document,
    _In_ IXmlNode^ childXmlNode
    )
{
    if (childXmlNode->NodeType == NodeType::TextNode)
    {
        Platform::String^ text = GetText(childXmlNode);

        if (text != nullptr)
        {
            // Since all text within child node is previously collected
            // when the text element is initialized, the text of the child
            // text node here isn't important. We only need to track proper
            // location of each span within the text element for formatting
            // purpose.
            m_textPosition += text->Length();
        }
    }
    else if (childXmlNode->NodeType == NodeType::ElementNode)
    {
        XmlElement^ xmlElement = dynamic_cast<XmlElement^>(childXmlNode);

        if (xmlElement->TagName == "bold")
        {
            ComPtr<IDWriteFactory> dwriteFactory;

            // Obtain DirectWrite factory from the current active document
            document->GetDWriteFactory(&dwriteFactory);

            Platform::String^ spanText = GetText(childXmlNode);

            DWRITE_TEXT_RANGE range = {0};

            range.startPosition = static_cast<uint32>(m_textPosition);
            range.length = static_cast<uint32>(spanText->Length());

            DX::ThrowIfFailed(
                m_textLayout->SetFontWeight(
                    DWRITE_FONT_WEIGHT_BOLD,
                    range
                    )
                );

            m_textPosition += spanText->Length();
        }
    }

    return true;
}

void Text::GetTextLayout(_Outptr_ IDWriteTextLayout** textLayout)
{
    *textLayout = ComPtr<IDWriteTextLayout>(m_textLayout).Detach();
}
