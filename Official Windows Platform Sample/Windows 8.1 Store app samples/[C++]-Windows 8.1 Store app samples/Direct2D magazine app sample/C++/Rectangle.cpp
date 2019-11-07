//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Windows::Data::Xml::Dom;

Rectangle::Rectangle() :
    Element(),
    m_color(0),
    m_opacity(1.0f),
    m_brush()
{
}

bool Rectangle::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Element::Initialize(document, xmlElement))
    {
        return false;
    }

    Platform::String^ value;

    value = xmlElement->GetAttribute("color");

    if (value != nullptr)
    {
        m_color = ParseColor(value);
    }

    value = xmlElement->GetAttribute("opacity");

    if (value != nullptr)
    {
        m_opacity = static_cast<float>(_wtof(value->Data()));
    }

    return true;
}

void Rectangle::Measure(
    _In_ Document^ document,
    D2D1_SIZE_F const& parentSize,
    _Out_ D2D1_RECT_F* bounds
    )
{
    *bounds = D2D1::RectF(
        m_offset.x,
        m_offset.y,
        m_offset.x + m_size.width,
        m_offset.y + m_size.height
        );
}

bool Rectangle::Draw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    ComPtr<ID2D1DeviceContext> d2dDeviceContext;
    document->GetRenderer()->GetD2DDeviceContext(&d2dDeviceContext);

    if (m_brush == nullptr)
    {
        ComPtr<ID2D1SolidColorBrush> solidColorBrush;

        DX::ThrowIfFailed(
            d2dDeviceContext->CreateSolidColorBrush(
                D2D1::ColorF(m_color, m_opacity),
                D2D1::BrushProperties(),
                &solidColorBrush
                )
            );

        m_brush = solidColorBrush;
    }

    d2dDeviceContext->SetTransform(transform);

    d2dDeviceContext->FillRectangle(
        D2D1::RectF(
            m_offset.x,
            m_offset.y,
            m_offset.x + m_size.width,
            m_offset.y + m_size.height
            ),
        m_brush.Get()
        );

    return false;
}