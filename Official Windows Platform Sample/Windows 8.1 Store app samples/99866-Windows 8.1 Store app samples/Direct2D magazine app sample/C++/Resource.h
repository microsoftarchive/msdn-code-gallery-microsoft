//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

//  <resource> is the base definition of content element which may be used in multiple
//  drawings. Resource neither measures nor draws as it is only the drawing state not
//  the drawing itself. Resource may be media content such as image or text, or it may
//  be types of brush.
//
ref class Resource : public Element
{
internal:
    virtual bool AcceptChildNode(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::IXmlNode^ childXmlNode
        ) override;

    virtual void Measure(
        _In_ Document^ document,
        D2D1_SIZE_F const& parentSize,
        _Out_ D2D1_RECT_F* bounds
        ) override;

    virtual bool PrepareToDraw(
        _In_ Document^ document,
        D2D1::Matrix3x2F const& transform
        ) override;

    virtual bool Draw(
        _In_ Document^ document,
        D2D1::Matrix3x2F const& transform
        ) override;
};

