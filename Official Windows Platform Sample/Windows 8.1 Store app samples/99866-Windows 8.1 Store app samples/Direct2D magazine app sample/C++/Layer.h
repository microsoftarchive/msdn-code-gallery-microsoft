//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

//  <layer> represents an intermediate bitmap surface to which the child content draws.
//  It handles the user's scrolling gesture by initiating an animation transition sequence.
//  Layer can fade to become fully transparent upon a user's tap gesture. This allows the
//  content underneath it to become fully visible to the user.
//
//  Layer accepts no markup attribute in addition to the base attributes defined in Element.
//
ref class Layer : public Element
{
internal:
    Layer();

    virtual bool AcceptChildNode(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::IXmlNode^ childXmlNode
        ) override;

    virtual float GetOpacity() override
    {
        return m_currentOpacity;
    }

private:
    // Initial layer opacity
    float m_initialOpacity;

    // Current layer opacity. This would be 0 when a layer is faded out.
    float m_currentOpacity;
};
