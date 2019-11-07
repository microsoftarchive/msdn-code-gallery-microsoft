//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

class InlineImage
    :   public IDWriteInlineObject
{
public:
    InlineImage(
        Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext,
        Microsoft::WRL::ComPtr<IWICImagingFactory> IWICFactory,
        PCWSTR uri
        );

    STDMETHOD(Draw)(
        _In_opt_ void* clientDrawingContext,
        IDWriteTextRenderer* renderer,
        FLOAT originX,
        FLOAT originY,
        BOOL isSideways,
        BOOL isRightToLeft,
        IUnknown* clientDrawingEffect
        );

    STDMETHOD(GetMetrics)(
        _Out_ DWRITE_INLINE_OBJECT_METRICS* metrics
        );

    STDMETHOD(GetOverhangMetrics)(
        _Out_ DWRITE_OVERHANG_METRICS* overhangs
        );

    STDMETHOD(GetBreakConditions)(
        _Out_ DWRITE_BREAK_CONDITION* breakConditionBefore,
        _Out_ DWRITE_BREAK_CONDITION* breakConditionAfter
        );

public:

    unsigned long STDMETHODCALLTYPE AddRef();
    unsigned long STDMETHODCALLTYPE Release();
    HRESULT STDMETHODCALLTYPE QueryInterface(
        IID const& riid,
        void** ppvObject
        );

private:

    HRESULT LoadBitmapFromFile(
        Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext,
        Microsoft::WRL::ComPtr<IWICImagingFactory> IWICFactory,
        PCWSTR uri,
        _Outptr_ ID2D1Bitmap **ppBitmap
        );

private:
    Microsoft::WRL::ComPtr<ID2D1Bitmap> bitmap;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext;

    D2D1_RECT_F rect; // coordinates in image, similar to index of HIMAGE_LIST
    float baseline;

    unsigned long refCount;
};
