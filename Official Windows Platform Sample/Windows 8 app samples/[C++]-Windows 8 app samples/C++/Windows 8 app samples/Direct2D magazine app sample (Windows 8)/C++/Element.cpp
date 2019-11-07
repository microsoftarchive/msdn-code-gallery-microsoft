//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

Element::Element() :
    m_offset(),
    m_size(),
    m_name(nullptr),
    m_parent(nullptr),
    m_nextSibling(nullptr),
    m_firstChild(nullptr)
{
}

//  This method is called to release the reference to the parent element.
//  It is necessary to traverse the tree to release the references explicitly,
//  otherwise the destructor will never be called due to cycle dependency
//  between the child and its parent element.
//
//  This method could be removed once the reference to the parent element
//  can be expressed as a weak reference.
//
void Element::ReleaseParent()
{
    if (m_firstChild != nullptr)
    {
        m_firstChild->ReleaseParent();
    }

    if (m_nextSibling != nullptr)
    {
        m_nextSibling->ReleaseParent();
    }

    m_parent = nullptr;
}

void Element::AddChild(_In_ Element^ element)
{
    if (m_firstChild == nullptr)
    {
        element->m_parent = this;
        m_firstChild = element;
    }
    else
    {
        m_firstChild->AddSibling(element);
    }
}

void Element::AddSibling(_In_ Element^ element)
{
    if (m_nextSibling == nullptr)
    {
        element->m_parent = m_parent;
        m_nextSibling = element;
    }
    else
    {
        m_nextSibling->AddSibling(element);
    }
}

bool Element::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    Platform::String^ value;

    value = xmlElement->GetAttribute("name");

    if (value != nullptr)
    {
        m_name = ref new Platform::String(value->Data());
    }

    value = xmlElement->GetAttribute("x");

    if (value != nullptr)
    {
        m_offset.x = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("y");

    if (value != nullptr)
    {
        m_offset.y = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("width");

    if (value != nullptr)
    {
        m_size.width = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("height");

    if (value != nullptr)
    {
        m_size.height = static_cast<float>(_wtof(value->Data()));
    }

    return true;
}

bool Element::AcceptChildNode(
    _In_ Document^ document,
    _In_ IXmlNode^ childXmlNode
    )
{
    // Not accepting any child element by default
    return false;
}

bool Element::BindResource(_In_ Element^ rootElement)
{
    // Succeed by default without actually binding any resource
    return true;
}

void Element::Measure(
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

        if (childBounds.left < maxBounds.left)
        {
            maxBounds.left = childBounds.left;
        }

        if (childBounds.right > maxBounds.right)
        {
            maxBounds.right = childBounds.right;
        }

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

    // Element size may be adjusted to accommodate the bounds of all its children.
    m_size = D2D1::SizeF(maxBounds.right - maxBounds.left, maxBounds.bottom - maxBounds.top);

    *bounds = D2D1::RectF(
        maxBounds.left + m_offset.x,
        maxBounds.top + m_offset.y,
        maxBounds.right + m_offset.x,
        maxBounds.bottom + m_offset.y
        );
}

bool Element::PrepareToDraw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    return PrepareToDrawChildren(
        document,
        D2D1::Matrix3x2F::Translation(m_offset.x, m_offset.y) * transform
        );
}

bool Element::PrepareToDrawChildren(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    Element^ child = GetFirstChild();

    while (child)
    {
        if (child->PrepareToDraw(document, transform))
            return true;

        child = child->GetNextSibling();
    }

    return false;
}

bool Element::Draw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    return DrawChildren(
        document,
        D2D1::Matrix3x2F::Translation(m_offset.x, m_offset.y) * transform
        );
}

bool Element::DrawChildren(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    Element^ child = GetFirstChild();

    while (child)
    {
        if (child->Draw(document, transform))
            return true;

        child = child->GetNextSibling();
    }

    return false;
}

uint32 Element::ParseColor(_In_ Platform::String^ value)
{
    uint32 color = 0;

    wchar_t const* string = value->Data();

    if (!value->IsEmpty() && string[0] == '#')
    {
        color = wcstoul(string + 1, nullptr, 16);
    }

    return color;
}

Platform::String^ Element::ParseNameIdentifier(_In_ Platform::String^ value)
{
    wchar_t const* string = value->Data();

    if (!value->IsEmpty() && string[0] == '#')
    {
        Platform::String^ name = ref new Platform::String(const_cast<wchar_t*>(string + 1));
        return name;
    }

    return nullptr;
}
