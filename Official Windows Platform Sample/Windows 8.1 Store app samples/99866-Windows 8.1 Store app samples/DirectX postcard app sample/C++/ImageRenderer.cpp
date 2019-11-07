//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ImageRenderer.h"

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;

static const float FullSaturation = 1.0f;

void ImageRenderer::CreateDeviceIndependentResources(
    ComPtr<IWICImagingFactory2> wicFactory
    )
{
    // Save the shared WIC factory.
    m_wicFactory = wicFactory;
}

void ImageRenderer::CreateDeviceResources(
    ComPtr<ID2D1DeviceContext> d2dContext
    )
{
    // Save the shared device context.
    m_d2dContext = d2dContext;

    // Create a bitmap source for rendering the image.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect)
        );

    // The mipmap linear interpolation mode on the BitmapSource effect instructs it to construct
    // a software (CPU memory) mipmap. If Direct2D requests the image at a small scale factor
    // (<= 0.5), BitmapSource only needs to perform the scale operation from the nearest
    // mip level, which is a performance optimization. In addition, for very large images,
    // performing scaling on the CPU reduces GPU bus bandwidth consumption and GPU memory
    // consumption.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
            D2D1_BITMAPSOURCE_INTERPOLATION_MODE_MIPMAP_LINEAR
            )
        );

    // Create saturation effect.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1Saturation, &m_saturationEffect)
        );

    // Set the initial saturation level of the effect.
    DX::ThrowIfFailed(
        m_saturationEffect->SetValue(D2D1_SATURATION_PROP_SATURATION, FullSaturation)
        );

    // Connect the effects graph.
    m_saturationEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void ImageRenderer::CreateWindowSizeDependentResources(float dpi)
{
    // Save the shared DPI.
    m_dpi = dpi;

    UpdateImageZoom();
}

void ImageRenderer::DrawImageAndEffects()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Set the context transform to center the image.
    m_d2dContext->SetTransform(
        D2D1::Matrix3x2F::Translation(
            (size.width - (m_imageSizeDips.width * m_imageZoomFactor)) / 2,
            (size.height - (m_imageSizeDips.height * m_imageZoomFactor)) / 2
            )
        );

    m_d2dContext->DrawImage(m_saturationEffect.Get());
}

void ImageRenderer::LoadImage(IRandomAccessStream^ randomAccessStream)
{
    ComPtr<IStream> stream;
    CreateStreamOverRandomAccessStream(
        reinterpret_cast<IUnknown*>(randomAccessStream),
        IID_PPV_ARGS(&stream)
        );

    // Load the image using the Windows Imaging Component decoder.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromStream(
            stream.Get(),
            nullptr,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> frame;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    ComPtr<IWICFormatConverter> wicFormatConverter;
    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&wicFormatConverter)
        );

    // We format convert to a pixel format that is compatible with Direct2D.
    // To optimize for performance when using WIC and Direct2D together, we need to
    // select the target pixel format based on the image's native precision:
    // - <= 8 bits per channel precision: use BGRA channel order
    //   (example: all JPEGs, including the image in this sample, are 8bpc)
    // -  > 8 bits per channel precision: use RGBA channel order
    //   (example: TIFF and JPEG-XR images support 32bpc float
    // Note that a fully robust system will arbitrate between various WIC pixel formats and
    // hardware feature level support using the IWICPixelFormatInfo2 interface.
    DX::ThrowIfFailed(
        wicFormatConverter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            wicFormatConverter.Get()
            )
        );

    uint32 bitmapWidth;
    uint32 bitmapHeight;
    DX::ThrowIfFailed(
        frame->GetSize(&bitmapWidth, &bitmapHeight)
        );

    m_imagePixelSize = D2D1::SizeU(bitmapWidth, bitmapHeight);

    UpdateImageZoom();
}

void ImageRenderer::UpdateImageZoom()
{
    // Compute the minimum zoom to ensure the image fits on the screen.
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // m_imageSize is in DIPs and changes depending on the app DPI.
    // This app ignores the source bitmap's DPI and assumes it is 96, in order to
    // display it at full resolution.
    m_imageSizeDips = D2D1::SizeF(
        m_imagePixelSize.width * 96.0f / m_dpi,
        m_imagePixelSize.height * 96.0f / m_dpi
        );

    m_imageZoomFactor = std::min<float>(
        size.width / m_imageSizeDips.width,
        size.height / m_imageSizeDips.height
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_SCALE,
            D2D1::Vector2F(m_imageZoomFactor, m_imageZoomFactor)
            )
        );
}

void ImageRenderer::SetEffectIntensity(float value)
{
    DX::ThrowIfFailed(
        m_saturationEffect->SetValue(D2D1_SATURATION_PROP_SATURATION, value)
        );
}

void ImageRenderer::Reset()
{
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, NULL)
        );

    DX::ThrowIfFailed(
        m_saturationEffect->SetValue(D2D1_SATURATION_PROP_SATURATION, FullSaturation)
        );
}