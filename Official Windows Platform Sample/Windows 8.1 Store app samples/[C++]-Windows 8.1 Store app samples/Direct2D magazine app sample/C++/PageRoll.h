//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

//  <page-roll> contains all the pages in the book. It draws each page next to one another
//  but shows only the pages visible in the display view. There may be multiple pages visible
//  in the view while visual transition is happening. PageRoll responds to user's horizontal
//  flick gesture by initiating an animation transition sequence. Note that <page-roll> element
//  can only parent a <page> element. This rule is explicitly enforced in the class definition
//  by overriding Element::AcceptChildNode virtual method.
//
ref class PageRoll : public Element
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
};
