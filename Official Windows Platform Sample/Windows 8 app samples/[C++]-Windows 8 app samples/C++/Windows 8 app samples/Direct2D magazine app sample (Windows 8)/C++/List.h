//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Rectangle.h"

//  <list> arranges its child content by stacking them vertically from top to bottom.
//  If the child content can't fit within the original size of the list, the list will
//  expand to accommodate them. List is direct subclass of Rectangle. It has all the
//  characteristic of a rectangle drawing.
//
//  List accepts the following additional markup attributes:
//      margin = "<float>"
//          Amount of spacing around the four edges of the list to where the child
//          element is drawn
//
ref class List : public Rectangle
{
internal:
    List();

    virtual bool AcceptChildNode(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::IXmlNode^ childXmlNode
        ) override;

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

private:
    // Amount of margin around the drawing area of a list.
    float m_margin;
};
