//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Element.h"

ref class Image;

//  <image-frame> represents a document of an image loaded from an image file.
//  Image frame is a primitive drawing element. It doesn't have children.
//
//  Image frame accepts the following markup attributes:
//      source = "url(#<string>)"
//          A unique resource identifier of the source image of the document
//      image-pixel-left = "<float>"
//          Offset to the left of the area of the image to render in pixels
//      image-pixel-top = "<float>"
//          Offset to the top of the area of the image to render in pixels
//      image-pixel-width = "<float>"
//          Width of the area of the image to render in pixels
//      image-pixel-height = "<float>"
//          Height of the area of the image to render in pixels
//
ref class ImageFrame : public Element
{
internal:
    ImageFrame();

    virtual bool Initialize(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::XmlElement^ xmlElement
        ) override;

    virtual bool BindResource(_In_ Element^ rootElement) override;

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
    // Name string of the source image
    Platform::String^ m_sourceName;

    // Source image of the image frame
    ::Image^ m_sourceImage;

    // Bounding rectangle of portion of the bitmap to render in pixels
    D2D1_RECT_F m_bitmapPixelBounds;
};

