//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Resource.h"
#include "ImageFile.h"

ref class Document;
ref class Renderer;

//  <image> represents an image loaded from an image memory stream. 
//  Image is a resource and as such it cannot draw. It is also a primitive 
//  content and therefore cannot have children.
//
//  Image accepts the following additional markup attributes:
//      file = "<file name>"
//          Relative path file name of the image file to load
//
ref class Image : public Resource
{
internal:
    Image();

    virtual bool Initialize(
        _In_ Document^ document,
        _In_ Windows::Data::Xml::Dom::XmlElement^ xmlElement
        ) override;

    bool DrawBitmap(
        _In_ Renderer^ renderer,
        D2D1::Matrix3x2F const& transform,
        D2D1_RECT_F const& destinationBounds,
        D2D1_RECT_F const& bitmapBounds
        );

private:
    // Decoded image wrapped inside a Direct2D bitmap
    Microsoft::WRL::ComPtr<ID2D1Bitmap> m_bitmap;

    // Image data file being asynchronously loaded
    Microsoft::WRL::ComPtr<ImageFile> m_file;
};

