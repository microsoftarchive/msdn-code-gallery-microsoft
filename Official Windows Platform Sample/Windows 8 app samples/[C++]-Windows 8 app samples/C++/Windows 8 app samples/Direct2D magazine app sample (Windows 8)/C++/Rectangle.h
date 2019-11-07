//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

//  <rect> represents a rectangle drawing on the surface.
//
//  Rectangle accepts the following markup attributes:
//      color = "#<rgb>"
//          Color in red-green-blue format i.e. the value of "#FF0000" is red
//      opacity = "<0..1>"
//          Opacity value from 0.0 to 1.0 where 0.0 is fully transparent and 1.0 is fully opaque
//
ref class Rectangle : public Element
{
internal:
    Rectangle();

    virtual bool Initialize(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::XmlElement^ xmlElement
        ) override;

    virtual void Measure(
        _In_ Document^ document,
        D2D1_SIZE_F const& parentSize,
        _Out_ D2D1_RECT_F* bounds
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
    // Fill color
    uint32 m_color;

    // Fill opacity
    float m_opacity;

    // Brush used to fill the rectangle area.
    Microsoft::WRL::ComPtr<ID2D1Brush> m_brush;
};
