//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

//  <design> represents a design template of the element tree. Different
//  display size potentially requires different design template in order to
//  preserve visual quality of the content. The design element exposes
//  the design width and height of the template. It is the responsibility
//  of the parent element to decide what design to use based on the aspect
//  ratio of the template.
//
//  Design element accepts the following additional markup attributes.
//      page-width = "<float>"
//          Width of a page in design unit
//      page-height = "<float>"
//          Height of a page in design unit
//
ref class Design : public Element
{
internal:
    Design();

    virtual bool Initialize(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::XmlElement^ xmlElement
        ) override;

    virtual bool AcceptChildNode(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::IXmlNode^ childXmlNode
        ) override;

    inline D2D1_SIZE_F GetPageSize()
    {
        return m_pageSize;
    }

private:
    D2D1_SIZE_F m_pageSize;
};
