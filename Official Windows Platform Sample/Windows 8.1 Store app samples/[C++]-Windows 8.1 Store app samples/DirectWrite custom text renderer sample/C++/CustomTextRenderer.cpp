//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "CustomTextRenderer.h"

////  CustomTextRenderer::CustomTextRenderer
////
////  The constructor stores the Direct2D factory, the render
////  target, and the outline and fill brushes used for drawing the
////  glyphs, underlines, and strikethroughs.

CustomTextRenderer::CustomTextRenderer(
    Microsoft::WRL::ComPtr<ID2D1Factory> D2DFactory,
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> D2DDeviceContext,
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> outlineBrush,
    Microsoft::WRL::ComPtr<ID2D1BitmapBrush> fillBrush
    ) :
    refCount(0),
    D2DFactory(D2DFactory),
    D2DDeviceContext(D2DDeviceContext),
    outlineBrush(outlineBrush),
    fillBrush(fillBrush)
{
}

////  CustomTextRenderer::DrawGlyphRun
////
////  Gets GlyphRun outlines via IDWriteFontFace::GetGlyphRunOutline
////  and then draws and fills them using Direct2D path geometries

IFACEMETHODIMP CustomTextRenderer::DrawGlyphRun(
    _In_opt_ void* clientDrawingContext,
    FLOAT baselineOriginX,
    FLOAT baselineOriginY,
    DWRITE_MEASURING_MODE measuringMode,
    _In_ DWRITE_GLYPH_RUN const* glyphRun,
    _In_ DWRITE_GLYPH_RUN_DESCRIPTION const* glyphRunDescription,
    IUnknown* clientDrawingEffect
    )
{
    HRESULT hr = S_OK;

    // Create the path geometry.
    Microsoft::WRL::ComPtr<ID2D1PathGeometry> pathGeometry;
    hr = D2DFactory->CreatePathGeometry(&pathGeometry);

    // Write to the path geometry using the geometry sink.
    Microsoft::WRL::ComPtr<ID2D1GeometrySink> sink;
    if (SUCCEEDED(hr))
    {
        hr = pathGeometry->Open(&sink);
    }

    // Get the glyph run outline geometries back from DirectWrite and place them within the
    // geometry sink.
    if (SUCCEEDED(hr))
    {
        hr = glyphRun->fontFace->GetGlyphRunOutline(
            glyphRun->fontEmSize,
            glyphRun->glyphIndices,
            glyphRun->glyphAdvances,
            glyphRun->glyphOffsets,
            glyphRun->glyphCount,
            glyphRun->isSideways,
            glyphRun->bidiLevel%2,
            sink.Get()
            );
    }

    // Close the geometry sink
    if (SUCCEEDED(hr))
    {
        hr = sink.Get()->Close();
    }

    // Initialize a matrix to translate the origin of the glyph run.
    D2D1::Matrix3x2F const matrix = D2D1::Matrix3x2F(
        1.0f, 0.0f,
        0.0f, 1.0f,
        baselineOriginX, baselineOriginY
        );

    // Create the transformed geometry
    Microsoft::WRL::ComPtr<ID2D1TransformedGeometry> transformedGeometry;
    if (SUCCEEDED(hr))
    {
        hr = D2DFactory.Get()->CreateTransformedGeometry(
            pathGeometry.Get(),
            &matrix,
            &transformedGeometry
            );
    }

    // Draw the outline of the glyph run
    D2DDeviceContext->DrawGeometry(
        transformedGeometry.Get(),
        outlineBrush.Get()
        );

    // Fill in the glyph run
    D2DDeviceContext->FillGeometry(
        transformedGeometry.Get(),
        fillBrush.Get()
        );

    return hr;
}

////  CustomTextRenderer::DrawUnderline
////
////  Draws underlines below the text using a Direct2D recatangle
////  geometry

IFACEMETHODIMP CustomTextRenderer::DrawUnderline(
    _In_opt_ void* clientDrawingContext,
    FLOAT baselineOriginX,
    FLOAT baselineOriginY,
    _In_ DWRITE_UNDERLINE const* underline,
    IUnknown* clientDrawingEffect
    )
{
    HRESULT hr;

    D2D1_RECT_F rect = D2D1::RectF(
        0,
        underline->offset,
        underline->width,
        underline->offset + underline->thickness
        );

    Microsoft::WRL::ComPtr<ID2D1RectangleGeometry> rectangleGeometry;
    hr = D2DFactory.Get()->CreateRectangleGeometry(
        &rect,
        &rectangleGeometry
        );

    // Initialize a matrix to translate the origin of the underline
    D2D1::Matrix3x2F const matrix = D2D1::Matrix3x2F(
        1.0f, 0.0f,
        0.0f, 1.0f,
        baselineOriginX, baselineOriginY
        );

    Microsoft::WRL::ComPtr<ID2D1TransformedGeometry> transformedGeometry;
    if (SUCCEEDED(hr))
    {
        hr = D2DFactory.Get()->CreateTransformedGeometry(
            rectangleGeometry.Get(),
            &matrix,
            &transformedGeometry
            );
    }

    // Draw the outline of the rectangle
    D2DDeviceContext.Get()->DrawGeometry(
        transformedGeometry.Get(),
        outlineBrush.Get()
        );

    // Fill in the rectangle
    D2DDeviceContext.Get()->FillGeometry(
        transformedGeometry.Get(),
        fillBrush.Get()
        );

    return S_OK;
}

////  CustomTextRenderer::DrawStrikethrough
////
////  Draws strikethroughs below the text using a Direct2D
////  recatangle geometry
////

IFACEMETHODIMP CustomTextRenderer::DrawStrikethrough(
    _In_opt_ void* clientDrawingContext,
    FLOAT baselineOriginX,
    FLOAT baselineOriginY,
    _In_ DWRITE_STRIKETHROUGH const* strikethrough,
    IUnknown* clientDrawingEffect
    )
{
    HRESULT hr;

    D2D1_RECT_F rect = D2D1::RectF(
        0,
        strikethrough->offset,
        strikethrough->width,
        strikethrough->offset + strikethrough->thickness
        );

    Microsoft::WRL::ComPtr<ID2D1RectangleGeometry> rectangleGeometry;
    hr = D2DFactory.Get()->CreateRectangleGeometry(
        &rect,
        &rectangleGeometry
        );

    // Initialize a matrix to translate the origin of the strikethrough
    D2D1::Matrix3x2F const matrix = D2D1::Matrix3x2F(
        1.0f, 0.0f,
        0.0f, 1.0f,
        baselineOriginX, baselineOriginY
        );

    Microsoft::WRL::ComPtr<ID2D1TransformedGeometry> transformedGeometry;
    if (SUCCEEDED(hr))
    {
        hr = D2DFactory.Get()->CreateTransformedGeometry(
            rectangleGeometry.Get(),
            &matrix,
            &transformedGeometry
            );
    }

    // Draw the outline of the rectangle
    D2DDeviceContext.Get()->DrawGeometry(
        transformedGeometry.Get(),
        outlineBrush.Get()
        );

    // Fill in the rectangle
    D2DDeviceContext.Get()->FillGeometry(
        transformedGeometry.Get(),
        fillBrush.Get()
        );

    return S_OK;
}

////  CustomTextRenderer::DrawInlineObject
////
////  This function is not implemented for the purposes of this
////  sample.

IFACEMETHODIMP CustomTextRenderer::DrawInlineObject(
    _In_opt_ void* clientDrawingContext,
    FLOAT originX,
    FLOAT originY,
    IDWriteInlineObject* inlineObject,
    BOOL isSideways,
    BOOL isRightToLeft,
    IUnknown* clientDrawingEffect
    )
{
    // Not implemented
    return E_NOTIMPL;
}

////  CustomTextRenderer::AddRef
////
////  Increments the ref count

IFACEMETHODIMP_(unsigned long) CustomTextRenderer::AddRef()
{
    return InterlockedIncrement(&refCount);
}

////  CustomTextRenderer::Release
////
////  Decrements the ref count and deletes the instance if the ref
////  count becomes 0

IFACEMETHODIMP_(unsigned long) CustomTextRenderer::Release()
{
    unsigned long newCount = InterlockedDecrement(&refCount);
    if (newCount == 0)
    {
        delete this;
        return 0;
    }

    return newCount;
}

////  CustomTextRenderer::IsPixelSnappingDisabled
////
////  Determines whether pixel snapping is disabled. The recommended
////  default is FALSE, unless doing animation that requires
////  subpixel vertical placement.

IFACEMETHODIMP CustomTextRenderer::IsPixelSnappingDisabled(
    _In_opt_ void* clientDrawingContext,
    _Out_ BOOL* isDisabled
    )
{
    *isDisabled = FALSE;
    return S_OK;
}

////  CustomTextRenderer::GetCurrentTransform
////
////  Returns the current transform applied to the render target..

IFACEMETHODIMP CustomTextRenderer::GetCurrentTransform(
    _In_opt_ void* clientDrawingContext,
    _Out_ DWRITE_MATRIX* transform
    )
{
    // forward the render target's transform
    D2DDeviceContext->GetTransform(reinterpret_cast<D2D1_MATRIX_3X2_F*>(transform));
    return S_OK;
}

////  CustomTextRenderer::GetPixelsPerDip
////
////  This returns the number of pixels per DIP.
////

IFACEMETHODIMP CustomTextRenderer::GetPixelsPerDip(
    _In_opt_ void* clientDrawingContext,
    _Out_ FLOAT* pixelsPerDip
    )
{
    float x, yUnused;

    D2DDeviceContext.Get()->GetDpi(&x, &yUnused);
    *pixelsPerDip = x / 96;

    return S_OK;
}

////  CustomTextRenderer::QueryInterface
////
////  Query interface implementation

IFACEMETHODIMP CustomTextRenderer::QueryInterface(
    IID const& riid,
    void** ppvObject
    )
{
    if (__uuidof(IDWriteTextRenderer) == riid)
    {
        *ppvObject = this;
    }
    else if (__uuidof(IDWritePixelSnapping) == riid)
    {
        *ppvObject = this;
    }
    else if (__uuidof(IUnknown) == riid)
    {
        *ppvObject = this;
    }
    else
    {
        *ppvObject = nullptr;
        return E_FAIL;
    }

    this->AddRef();

    return S_OK;
}
