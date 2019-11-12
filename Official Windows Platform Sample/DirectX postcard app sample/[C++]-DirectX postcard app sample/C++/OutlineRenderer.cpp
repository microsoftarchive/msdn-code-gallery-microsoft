//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "OutlineRenderer.h"

using namespace Microsoft::WRL;

// This class represents a custom "text renderer" that extracts the
// glyph runs of a text layout object and turns them into a geometry.
void OutlineRenderer::CreateOutlineRenderer(
    _In_ ID2D1Factory* pFactory,
    _Outptr_ OutlineRenderer** ppOutlineRenderer
    )
{
    *ppOutlineRenderer = new OutlineRenderer(pFactory);
    (*ppOutlineRenderer)->AddRef();
}

OutlineRenderer::OutlineRenderer(_In_ ID2D1Factory* pFactory) :
    m_pFactory(pFactory),
    m_ref(0)
{
}

HRESULT OutlineRenderer::DrawGlyphRun(
    void* /*clientDrawingContext*/,
    FLOAT baselineOriginX,
    FLOAT baselineOriginY,
    DWRITE_MEASURING_MODE /*measuringMode*/,
    DWRITE_GLYPH_RUN const* glyphRun,
    DWRITE_GLYPH_RUN_DESCRIPTION const* /*glyphRunDescription*/,
    IUnknown* /*clientDrawingEffect*/
    )
{
    HRESULT hr;

    ComPtr<ID2D1PathGeometry> pPathGeometry;

    hr = m_pFactory->CreatePathGeometry(&pPathGeometry);

    if (SUCCEEDED(hr))
    {
        ComPtr<ID2D1GeometrySink> pSink;

        hr = pPathGeometry->Open(&pSink);

        if (SUCCEEDED(hr))
        {
            hr = glyphRun->fontFace->GetGlyphRunOutline(
                glyphRun->fontEmSize,
                glyphRun->glyphIndices,
                glyphRun->glyphAdvances,
                glyphRun->glyphOffsets,
                glyphRun->glyphCount,
                glyphRun->isSideways,
                (glyphRun->bidiLevel % 2) == 1,
                pSink.Get()
                );

            if (SUCCEEDED(hr))
            {
                hr = pSink->Close();

                if (SUCCEEDED(hr))
                {
                    ComPtr<ID2D1TransformedGeometry> pTransformedGeometry;

                    hr = m_pFactory->CreateTransformedGeometry(
                        pPathGeometry.Get(),
                        D2D1::Matrix3x2F::Translation(baselineOriginX, baselineOriginY),
                        &pTransformedGeometry
                        );

                    if (SUCCEEDED(hr))
                    {
                        AddGeometry(pTransformedGeometry.Get());
                    }
                }
            }
        }
    }

    return hr;
}

HRESULT OutlineRenderer::DrawUnderline(
    void* /*clientDrawingContext*/,
    FLOAT /*baselineOriginX*/,
    FLOAT /*baselineOriginY*/,
    DWRITE_UNDERLINE const* /*underline*/,
    IUnknown* /*clientDrawingEffect*/
    )
{
    // Implement this to add support for underlines.
    return E_NOTIMPL;
}

HRESULT OutlineRenderer::DrawStrikethrough(
    void* /*clientDrawingContext*/,
    FLOAT /*baselineOriginX*/,
    FLOAT /*baselineOriginY*/,
    DWRITE_STRIKETHROUGH const* /*strikethrough*/,
    IUnknown* /*clientDrawingEffect*/
    )
{
    // Implement this to add support for strikethroughs.
    return E_NOTIMPL;
}

HRESULT OutlineRenderer::DrawInlineObject(
    void* /*clientDrawingContext*/,
    FLOAT /*originX*/,
    FLOAT /*originY*/,
    IDWriteInlineObject* /*inlineObject*/,
    BOOL /*isSideways*/,
    BOOL /*isRightToLeft*/,
    IUnknown* /*clientDrawingEffect*/
    )
{
    // Implement this to add support for inline objects.
    return E_NOTIMPL;
}

HRESULT OutlineRenderer::IsPixelSnappingDisabled(
    void* /*clientDrawingContext*/,
    BOOL* isDisabled
    )
{
    *isDisabled = TRUE;
    return S_OK;
}

HRESULT OutlineRenderer::GetCurrentTransform(
    void* /*clientDrawingContext*/,
    DWRITE_MATRIX* transform
    )
{
    DWRITE_MATRIX matrix = {1, 0, 0, 1, 0, 0};
    *transform = matrix;
    return S_OK;
}

HRESULT OutlineRenderer::GetPixelsPerDip(
    void* /*clientDrawingContext*/,
    FLOAT* pixelsPerDip
    )
{
    *pixelsPerDip = 1.0f;
    return S_OK;
}

void OutlineRenderer::GetGeometry(ID2D1Geometry** ppGeometry)
{
    if (m_pGeometry)
    {
        *ppGeometry = m_pGeometry.Get();
        (*ppGeometry)->AddRef();
    }
    else
    {
        ComPtr<ID2D1PathGeometry> pGeometry;
        DX::ThrowIfFailed(
            m_pFactory->CreatePathGeometry(&pGeometry)
            );

        ComPtr<ID2D1GeometrySink> pSink;
        DX::ThrowIfFailed(
            pGeometry->Open(&pSink)
            );

        DX::ThrowIfFailed(
            pSink->Close()
            );

        *ppGeometry = pGeometry.Get();
        (*ppGeometry)->AddRef();
    }
}

void OutlineRenderer::AddGeometry(ID2D1Geometry* pGeometry)
{
    if (m_pGeometry == nullptr)
    {
        // Just save the geometry.
        m_pGeometry = ComPtr<ID2D1Geometry>(pGeometry);
    }
    else
    {
        // Combine the provided geometry with the saved geometry.

        ComPtr<ID2D1Factory> pFactory;
        m_pGeometry->GetFactory(&pFactory);

        ComPtr<ID2D1PathGeometry> pPathGeometry;
        DX::ThrowIfFailed(
            pFactory->CreatePathGeometry(&pPathGeometry)
            );

        ComPtr<ID2D1GeometrySink> pSink;
        DX::ThrowIfFailed(
            pPathGeometry->Open(&pSink)
            );

        DX::ThrowIfFailed(
            m_pGeometry->CombineWithGeometry(
                pGeometry,
                D2D1_COMBINE_MODE_UNION,
                nullptr, // world transform
                pSink.Get()
                )
            );

        DX::ThrowIfFailed(
            pSink->Close()
            );

        m_pGeometry = pPathGeometry;
    }
}

//
// IUnknown methods.
//
// These use a basic, non-thread-safe implementation of the
// standard reference-counting logic.
//
HRESULT OutlineRenderer::QueryInterface(REFIID riid, _Outptr_ void** object)
{
    *object = nullptr;
    return E_NOTIMPL;
}

ULONG OutlineRenderer::AddRef()
{
    m_ref++;
    return m_ref;
}

ULONG OutlineRenderer::Release()
{
    m_ref--;

    if (m_ref == 0)
    {
        delete this;
        return 0;
    }

    return m_ref;
}
