//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteInlineObject.h"

InlineImage::InlineImage(
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext,
    Microsoft::WRL::ComPtr<IWICImagingFactory> IWICFactory,
    PCWSTR uri
    ) :
    refCount(0)
{
    // Save the render target for later.
    this->deviceContext = deviceContext;

    // Load the bitmap from a file.
    LoadBitmapFromFile(
        deviceContext,
        IWICFactory,
        uri,
        &bitmap
        );
}

HRESULT STDMETHODCALLTYPE InlineImage::Draw(
    _In_opt_ void* clientDrawingContext,
    IDWriteTextRenderer* renderer,
    FLOAT originX,
    FLOAT originY,
    BOOL isSideways,
    BOOL isRightToLeft,
    IUnknown* clientDrawingEffect
    )
{
    float height    = rect.bottom - rect.top;
    float width     = rect.right  - rect.left;
    D2D1_RECT_F destRect  = {originX, originY, originX + width, originY + height};

    deviceContext.Get()->DrawBitmap(bitmap.Get(), destRect);

    return S_OK;
}

HRESULT STDMETHODCALLTYPE InlineImage::GetMetrics(
    _Out_ DWRITE_INLINE_OBJECT_METRICS* metrics
    )
{
    DWRITE_INLINE_OBJECT_METRICS inlineMetrics = {};
    inlineMetrics.width     = rect.right  - rect.left;
    inlineMetrics.height    = rect.bottom - rect.top;
    inlineMetrics.baseline  = baseline;
    *metrics = inlineMetrics;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE InlineImage::GetOverhangMetrics(
    _Out_ DWRITE_OVERHANG_METRICS* overhangs
    )
{
    overhangs->left      = 0;
    overhangs->top       = 0;
    overhangs->right     = 0;
    overhangs->bottom    = 0;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE InlineImage::GetBreakConditions(
    _Out_ DWRITE_BREAK_CONDITION* breakConditionBefore,
    _Out_ DWRITE_BREAK_CONDITION* breakConditionAfter
    )
{
    *breakConditionBefore = DWRITE_BREAK_CONDITION_NEUTRAL;
    *breakConditionAfter  = DWRITE_BREAK_CONDITION_NEUTRAL;
    return S_OK;
}

////  InlineImage::LoadBitmapFromFile
////
////  Loads a bitmap from a file

HRESULT InlineImage::LoadBitmapFromFile(
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext,
    Microsoft::WRL::ComPtr<IWICImagingFactory> IWICFactory,
    PCWSTR uri,
    _Outptr_ ID2D1Bitmap **ppBitmap
    )
{
    HRESULT hr = S_OK;

    Microsoft::WRL::ComPtr<IWICBitmapDecoder> decoder;
    Microsoft::WRL::ComPtr<IWICBitmapFrameDecode> source;
    Microsoft::WRL::ComPtr<IWICFormatConverter> converter;

    hr = IWICFactory->CreateDecoderFromFilename(
        uri, nullptr, GENERIC_READ, WICDecodeMetadataCacheOnLoad, &decoder
        );

    // Create the initial frame.
    if (SUCCEEDED(hr))
    {
        hr = decoder->GetFrame(0, &source);
    }

    // Store the image size for later use.
    UINT imageWidth = 0, imageHeight = 0;

    source->GetSize(&imageWidth, &imageHeight);

    rect.left      = 0;
    rect.top       = 0;
    rect.right     = static_cast<FLOAT>(imageWidth);
    rect.bottom    = static_cast<FLOAT>(imageHeight);

    baseline       = static_cast<FLOAT>(imageHeight);

    // Convert the image format to 32bppPBGRA -- which Direct2D expects.
    if (SUCCEEDED(hr))
    {
        hr = IWICFactory->CreateFormatConverter(&converter);
    }

    if (SUCCEEDED(hr))
    {
        hr = converter->Initialize(
            source.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.f,
            WICBitmapPaletteTypeMedianCut
            );
    }

    // Create a Direct2D bitmap from the WIC bitmap.
    if (SUCCEEDED(hr))
    {
        hr = deviceContext.Get()->CreateBitmapFromWicBitmap(
            converter.Get(),
            nullptr,
            ppBitmap
            );
    }

    return hr;
}

//// InlineImage::QueryInterface
////
//// Query interface implementation

STDMETHODIMP InlineImage::QueryInterface(
    IID const& riid,
    void** ppvObject
    )
{
    if (__uuidof(IDWriteInlineObject) == riid)
    {
        *ppvObject = dynamic_cast<IDWriteInlineObject*>(this);
    }
    else if (__uuidof(IUnknown) == riid)
    {
        *ppvObject = dynamic_cast<IUnknown*>(this);
    }
    else
    {
        *ppvObject = nullptr;
        return E_FAIL;
    }

    return S_OK;
}

////  InlineImage::AddRef
////
////  Increments the ref count

STDMETHODIMP_(unsigned long) InlineImage::AddRef()
{
    return InterlockedIncrement(&refCount);
}

////  InlineImage::Release
////
////  Decrements the ref count and deletes the instance if the ref
////  count becomes 0

STDMETHODIMP_(unsigned long) InlineImage::Release()
{
    unsigned long newCount = InterlockedDecrement(&refCount);

    if (newCount == 0)
    {
        delete this;
        return 0;
    }

    return newCount;
}
