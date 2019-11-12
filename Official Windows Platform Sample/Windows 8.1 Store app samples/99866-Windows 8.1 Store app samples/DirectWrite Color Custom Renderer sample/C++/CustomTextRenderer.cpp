//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "CustomTextRenderer.h"
#include "DirectXHelper.h"

using namespace DWriteColorTextRenderer;

////  CustomTextRenderer::CustomTextRenderer
////
////  The constructor stores the Direct2D factory, the render
////  target, and the outline and fill brushes used for drawing the
////  glyphs, underlines, and strikethroughs.

CustomTextRenderer::CustomTextRenderer(
    Microsoft::WRL::ComPtr<ID2D1Factory> d2dFactory,
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dDeviceContext
    ) :
    m_refCount(0),
    m_d2dFactory(d2dFactory),
    m_d2dDeviceContext(d2dDeviceContext)
{
    DX::ThrowIfFailed(
        DWriteCreateFactory(
        DWRITE_FACTORY_TYPE_SHARED,
        __uuidof(IDWriteFactory),
        &m_dwriteFactory2
        )
    );
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
    HRESULT hrColor = DWRITE_E_NOCOLOR;
    DWRITE_MATRIX* worldToDeviceTransform = nullptr;

    Microsoft::WRL::ComPtr<IDWriteColorGlyphRunEnumerator> colorLayers;
    hrColor = m_dwriteFactory2->TranslateColorGlyphRun(
        baselineOriginX,
        baselineOriginY,
        glyphRun,
        nullptr,
        measuringMode,
        worldToDeviceTransform,
        0,
        &colorLayers
        );

    if (hrColor != DWRITE_E_NOCOLOR)
    {
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> solidBrush;

        for (;;){
        BOOL haveRun;
        DX::ThrowIfFailed(colorLayers->MoveNext(&haveRun));
        if (!haveRun)
            break;

            DWRITE_COLOR_GLYPH_RUN const* colorRun;
            DX::ThrowIfFailed(colorLayers->GetCurrentRun(&colorRun));

            ID2D1Brush* layerBrush;
            if (colorRun->paletteIndex == 0xFFFF)
            {
                layerBrush = m_outlineBrush.Get();
            }
            else
            {
                if (solidBrush == nullptr)
                {
                    DX::ThrowIfFailed(m_d2dDeviceContext->CreateSolidColorBrush(&colorRun->runColor, nullptr, &solidBrush));
                }
                else
                {
                    solidBrush->SetColor(colorRun->runColor);
                }
                layerBrush = solidBrush.Get();
            }

            m_d2dDeviceContext->DrawGlyphRun(
                D2D1::Point2(colorRun->baselineOriginX,colorRun->baselineOriginY),
                &colorRun->glyphRun,
                colorRun->glyphRunDescription,
                layerBrush,
                measuringMode
            );
        }
    }

    if (hrColor == DWRITE_E_NOCOLOR)
    {
        // Usual case: the run does not have any colored glyphs.
        m_d2dDeviceContext->DrawGlyphRun(
            D2D1::Point2(baselineOriginX,baselineOriginY), 
            glyphRun, 
            m_outlineBrush.Get(),
            measuringMode
            );
    }

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
    // Not implemented
    return E_NOTIMPL;
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
    // Not implemented
    return E_NOTIMPL;
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
    return InterlockedIncrement(&m_refCount);
}

////  CustomTextRenderer::Release
////
////  Decrements the ref count and deletes the instance if the ref
////  count becomes 0

IFACEMETHODIMP_(unsigned long) CustomTextRenderer::Release()
{
    unsigned long newCount = InterlockedDecrement(&m_refCount);
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
    m_d2dDeviceContext->GetTransform(reinterpret_cast<D2D1_MATRIX_3X2_F*>(transform));
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

    m_d2dDeviceContext.Get()->GetDpi(&x, &yUnused);
    *pixelsPerDip = x / 96.0f;

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
