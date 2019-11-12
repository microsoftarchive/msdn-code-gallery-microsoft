//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

List::List() :
    Rectangle(),
    m_margin(0)
{
}

bool List::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Rectangle::Initialize(document, xmlElement))
    {
        return false;
    }

    Platform::String^ value;

    value = xmlElement->GetAttribute("margin");

    if (value != nullptr)
    {
        m_margin = static_cast<float>(_wtof(value->Data()));
    }

    return true;
}

bool List::AcceptChildNode(
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
            if (elementName == "text-frame")
            {
                child = Element::Create<TextFrame>(document, childXmlNode);
            }
            else if (elementName == "image-frame")
            {
                child = Element::Create<ImageFrame>(document, childXmlNode);
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

void List::Measure(
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

    if (m_margin * 2 < size.width && m_margin * 2 < size.height)
    {
        // Compute the content area for child elements excluding list's designated margins
        size = D2D1::SizeF(size.width - 2 * m_margin, size.height - 2 * m_margin);
    }
    else
    {
        m_margin = 0;
    }

    D2D1_RECT_F maxBounds = D2D1::RectF(0, 0, size.width, 0);

    Element^ child = GetFirstChild();

    while (child)
    {
        D2D1_RECT_F childBounds = {0};

        child->Measure(document, size, &childBounds);

        if (maxBounds.bottom - maxBounds.top + childBounds.top < 0)
        {
            maxBounds.top = childBounds.top + maxBounds.bottom;
        }

        maxBounds.bottom += childBounds.bottom;

        if (childBounds.left < maxBounds.left)
        {
            maxBounds.left = childBounds.left;
        }

        if (childBounds.right > maxBounds.right)
        {
            maxBounds.right = childBounds.right;
        }

        // Update the list size for the next child
        size = D2D1::SizeF(size.width, size.height - childBounds.bottom + childBounds.top);

        child = child->GetNextSibling();
    }

    // List final size needs to accommodate all its children.
    m_size = D2D1::SizeF(maxBounds.right - maxBounds.left + 2 * m_margin, maxBounds.bottom - maxBounds.top + 2 * m_margin);

    *bounds = D2D1::RectF(
        m_offset.x,
        m_offset.y,
        m_offset.x + m_size.width,
        m_offset.y + m_size.height
        );
}

bool List::Draw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    Rectangle::Draw(document, transform);

    float offsetY = 0;

    Element^ child = GetFirstChild();

    while (child)
    {
        if (child->Draw(
            document,
            D2D1::Matrix3x2F::Translation(m_offset.x + m_margin, m_offset.y + m_margin + offsetY) * transform
            ))
        {
            return true;
        }

        offsetY += child->GetSize().height;

        child = child->GetNextSibling();
    }

    return false;
}