//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Windows::Data::Xml::Dom;

bool Story::AcceptChildNode(
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
            if (elementName == "design")
            {
                child = Element::Create<Design>(document, childXmlNode);
            }
            else if (elementName == "resource")
            {
                child = Element::Create<Resource>(document, childXmlNode);
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

// Choose the design with the least aspect ratio deviation from the specified display size.
Design^ Story::GetDesign(D2D1_SIZE_F displaySize)
{
    float displayAspectRatio = displaySize.width / displaySize.height;
    float leastDeviation = FLT_MAX;

    Design^ chosenDesign = nullptr;

    Element^ child = GetFirstChild();

    while (child != nullptr)
    {
        Design^ design = dynamic_cast<Design^>(child);

        if (design != nullptr)
        {
            D2D1_SIZE_F pageSize = design->GetPageSize();

            float deviation = abs(pageSize.width / pageSize.height - displayAspectRatio);

            if (deviation < leastDeviation)
            {
                leastDeviation = deviation;
                chosenDesign = design;
            }
        }

        child = child->GetNextSibling();
    }

    return chosenDesign;
}
