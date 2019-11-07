//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

ImageFrame::ImageFrame() :
    Element(),
    m_sourceName(nullptr),
    m_sourceImage(nullptr),
    m_bitmapPixelBounds()
{
}

bool ImageFrame::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Element::Initialize(document, xmlElement))
    {
        return false;
    }

    Platform::String^ value = xmlElement->GetAttribute("source");

    if (value != nullptr)
    {
        m_sourceName = ParseNameIdentifier(value);
    }

    value = xmlElement->GetAttribute("image-pixel-left");

    if (value != nullptr)
    {
        m_bitmapPixelBounds.left = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("image-pixel-top");

    if (value != nullptr)
    {
        m_bitmapPixelBounds.top = static_cast<float>(_wtof(value->Data()));
    }

    m_bitmapPixelBounds.right = m_bitmapPixelBounds.left;

    value = xmlElement->GetAttribute("image-pixel-width");

    if (value != nullptr)
    {
        m_bitmapPixelBounds.right += static_cast<float>(_wtof(value->Data()));
    }

    m_bitmapPixelBounds.bottom = m_bitmapPixelBounds.top;

    value = xmlElement->GetAttribute("image-pixel-height");

    if (value != nullptr)
    {
        m_bitmapPixelBounds.bottom += static_cast<float>(_wtof(value->Data()));
    }

    return true;
}

bool ImageFrame::BindResource(_In_ Element^ rootElement)
{
    if (rootElement != nullptr)
    {
        TreeIterator<Element> it(rootElement);

        // Traverse the tree looking for the source image
        do
        {
            ::Image^ image(dynamic_cast<::Image^>(it.GetCurrentNode()));

            if (    image != nullptr
                &&  image->GetName() == m_sourceName
                )
            {
                m_sourceImage = image;
                return true;
            }

        } while (++it);
    }

    return false;
}

void ImageFrame::Measure(
    _In_ Document^ document,
    D2D1_SIZE_F const& parentSize,
    _Out_ D2D1_RECT_F* bounds
    )
{
    if (m_size.width <= 0 && m_size.height <= 0)
    {
        m_size = D2D1::SizeF(parentSize.width - m_offset.x, parentSize.height - m_offset.y);
    }

    *bounds = D2D1::RectF(
        m_offset.x,
        m_offset.y,
        m_offset.x + m_size.width,
        m_offset.y + m_size.height
        );
}

bool ImageFrame::Draw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    if (m_sourceImage != nullptr)
    {
        auto renderer = document->GetRenderer();

        return m_sourceImage->DrawBitmap(
            renderer,
            transform,
            D2D1::RectF(
                m_offset.x,
                m_offset.y,
                m_offset.x + m_size.width,
                m_offset.y + m_size.height
                ),
            m_bitmapPixelBounds
            );
    }

    return false;
}
