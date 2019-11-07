//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

Design::Design() :
    Element(),
    m_pageSize()
{
}

bool Design::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Element::Initialize(document, xmlElement))
    {
        return false;
    }

    Platform::String^ value = xmlElement->GetAttribute("page-width");

    if (value != nullptr)
    {
        m_pageSize.width = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("page-height");

    if (value != nullptr)
    {
        m_pageSize.height = static_cast<float>(_wtof(value->Data()));
    }

    return true;
}

bool Design::AcceptChildNode(
    _In_ Document^ document,
    _In_ IXmlNode^ childXmlNode
    )
{
    if (childXmlNode->NodeType == NodeType::ElementNode)
    {
        XmlElement^ xmlElement = dynamic_cast<XmlElement^>(childXmlNode);

        Element^ child = nullptr;
        Platform::String^ elementName = xmlElement->TagName;

        if (elementName != nullptr)
        {
            if (elementName == "page-roll")
            {
                child = Element::Create<PageRoll>(document, childXmlNode);
            }
        }

        if (child == nullptr)
        {
            return false;
        }

        AddChild(child);
    }

    return true;
}
