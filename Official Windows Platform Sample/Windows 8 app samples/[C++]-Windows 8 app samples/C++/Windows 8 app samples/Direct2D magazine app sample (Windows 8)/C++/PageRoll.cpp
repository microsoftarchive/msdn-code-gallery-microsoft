//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

bool PageRoll::AcceptChildNode(
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
            if (elementName == "page")
            {
                child = Element::Create<::Page>(document, childXmlNode);
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

//  Page roll places its children side-by-side horizontally.
//
void PageRoll::Measure(
    _In_ Document^ document,
    D2D1_SIZE_F const& parentSize,
    _Out_ D2D1_RECT_F* bounds
    )
{
    D2D1_SIZE_F size = m_size;

    if (size.width <= 0 && size.height <= 0)
    {
        // When no size is specified, assume its parent size.
        size = parentSize;
    }

    D2D1_RECT_F maxBounds = {0};

    Element^ child = GetFirstChild();

    while (child)
    {
        D2D1_RECT_F childBounds = {0};

        child->Measure(document, size, &childBounds);

        if (maxBounds.right + childBounds.left < maxBounds.left)
        {
            // Accommodate child placed left to all other previous children.
            maxBounds.left = childBounds.left;
        }

        // The current child is laid next to the right of the previous child.
        maxBounds.right += childBounds.right;

        if (childBounds.top < maxBounds.top)
        {
            maxBounds.top = childBounds.top;
        }

        if (childBounds.bottom > maxBounds.bottom)
        {
            maxBounds.bottom = childBounds.bottom;
        }

        child = child->GetNextSibling();
    }

    m_size = D2D1::SizeF(maxBounds.right - maxBounds.left, maxBounds.bottom - maxBounds.top);

    *bounds = D2D1::RectF(
        maxBounds.left + m_offset.x,
        maxBounds.top + m_offset.y,
        maxBounds.right + m_offset.x,
        maxBounds.bottom + m_offset.y
        );
}
