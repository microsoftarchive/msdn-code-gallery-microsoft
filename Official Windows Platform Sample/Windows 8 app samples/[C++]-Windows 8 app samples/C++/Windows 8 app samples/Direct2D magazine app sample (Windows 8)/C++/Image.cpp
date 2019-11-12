//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Windows::Data::Xml::Dom;

::Image::Image() :
    Resource(),
    m_bitmap(),
    m_file(nullptr)
{
}

bool ::Image::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Element::Initialize(document, xmlElement))
    {
        return false;
    }

    Platform::String^ value = xmlElement->GetAttribute("file");

    if (value != nullptr)
    {
        // Initiate asynchronous loading of image data from the image file
        m_file = new ImageFile();
        m_file->LoadAsync(document->GetLocation(), value);
    }

    return true;
}

bool ::Image::DrawBitmap(
    _In_ Renderer^ renderer,
    D2D1::Matrix3x2F const& transform,
    D2D1_RECT_F const& destinationBounds,
    D2D1_RECT_F const& bitmapBounds
    )
{
    ComPtr<ID2D1DeviceContext> d2dDeviceContext;
    ComPtr<IWICImagingFactory2> wicFactory;

    // Obtain Direct2D device context and WIC imaging factory
    renderer->GetD2DDeviceContext(&d2dDeviceContext);
    renderer->GetWICImagingFactory(&wicFactory);

    if (m_bitmap == nullptr)
    {
        if (!m_file->Ready())
        {
            // The image file is still be loaded, requesting redraw upon return.
            return true;
        }

        // Use Windows Imaging Component to decode an image file into a
        // WIC bitmap and then make a copy as a Direct2D bitmap in video
        // memory.
        ComPtr<IWICBitmapDecoder> decoder;
        ComPtr<IWICBitmapFrameDecode> frame;
        ComPtr<IWICFormatConverter> wicBitmap;

        DX::ThrowIfFailed(
            wicFactory->CreateDecoderFromStream(
                m_file.Get(),
                nullptr,
                WICDecodeMetadataCacheOnDemand,
                &decoder
                )
            );

        DX::ThrowIfFailed(
            decoder->GetFrame(0, &frame)
            );

        DX::ThrowIfFailed(
            wicFactory->CreateFormatConverter(&wicBitmap)
            );

        DX::ThrowIfFailed(
            wicBitmap->Initialize(
                frame.Get(),
                GUID_WICPixelFormat32bppPBGRA,
                WICBitmapDitherTypeNone,
                nullptr,
                0,
                WICBitmapPaletteTypeCustom
                )
            );

        DX::ThrowIfFailed(
            d2dDeviceContext->CreateBitmapFromWicBitmap(
                wicBitmap.Get(),
                D2D1::BitmapProperties(
                    D2D1::PixelFormat(),
                    96.0,
                    96.0
                    ),
                &m_bitmap
                )
            );
    }

    d2dDeviceContext->SetTransform(transform);

    d2dDeviceContext->DrawBitmap(
        m_bitmap.Get(),
        &destinationBounds,
        1.0f,
        D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC,
        &bitmapBounds
        );

    return false;
}
