//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

ref class Design;

//  <story> represents the root element of the entire content. It hosts a set of
//  resources from the child <resource> element and a set of design templates defined
//  in the child <design> elements that utilize the defined resources. The story element
//  will choose the right design template based on the document's current aspect ratio.
//
//  Story accepts no additional markup attributes.
//
ref class Story : public Element
{
internal:
    virtual bool AcceptChildNode(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::IXmlNode^ childXmlNode
        ) override;

    Design^ GetDesign(D2D1_SIZE_F displaySize);
};
