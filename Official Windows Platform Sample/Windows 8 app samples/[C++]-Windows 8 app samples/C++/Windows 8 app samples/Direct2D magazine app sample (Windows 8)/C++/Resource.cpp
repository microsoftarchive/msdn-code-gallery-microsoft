//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

bool Resource::AcceptChildNode(
    _In_ Document^ document,
    _In_ IXmlNode^ childXmlNode
    )
{
    if (childXmlNode->NodeType == NodeType::ElementNode)
    {
        XmlElement^ xmlElement = dynamic_cast<XmlElement^>(childXmlNode);

        Element^ child = nullptr;
        Platform::String^ elementName = xmlElement->TagName;

        if (elementName != nullptr)
        {
            if (elementName == "image")
            {
                child = Element::Create<::Image>(document, childXmlNode);
            }
            else if (elementName == "text")
            {
                child = Element::Create<Text>(document, childXmlNode);
            }
        }

        if (child == nullptr)
        {
            return false;
        }

        AddChild(child);
    }

    return true;
}

void Resource::Measure(
    _In_ Document^ document,
    D2D1_SIZE_F const& parentSize,
    _Out_ D2D1_RECT_F* bounds
    )
{
    // Resource has no bounds of its own.
    *bounds = D2D1::RectF();
}

bool Resource::PrepareToDraw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    // Resource doesn't draw.
    return false;
}

bool Resource::Draw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    // Resource doesn't draw.
    return false;
}
