//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"

class OutlineRenderer : public IDWriteTextRenderer
{
public:
    static void CreateOutlineRenderer(
        _In_ ID2D1Factory* pFactory,
        _Outptr_ OutlineRenderer** ppOutlineRenderer
        );

    STDMETHOD(DrawGlyphRun)(
        void* /*clientDrawingContext*/,
        FLOAT baselineOriginX,
        FLOAT baselineOriginY,
        DWRITE_MEASURING_MODE /*measuringMode*/,
        DWRITE_GLYPH_RUN const* glyphRun,
        DWRITE_GLYPH_RUN_DESCRIPTION const* /*glyphRunDescription*/,
        IUnknown* /*clientDrawingEffect*/
        );

    STDMETHOD(DrawUnderline)(
        void* /*clientDrawingContext*/,
        FLOAT /*baselineOriginX*/,
        FLOAT /*baselineOriginY*/,
        DWRITE_UNDERLINE const* /*underline*/,
        IUnknown* /*clientDrawingEffect*/
        );

    STDMETHOD(DrawStrikethrough)(
        void* /*clientDrawingContext*/,
        FLOAT /*baselineOriginX*/,
        FLOAT /*baselineOriginY*/,
        DWRITE_STRIKETHROUGH const* /*strikethrough*/,
        IUnknown* /*clientDrawingEffect*/
        );

    STDMETHOD(DrawInlineObject)(
        void* /*clientDrawingContext*/,
        FLOAT /*originX*/,
        FLOAT /*originY*/,
        IDWriteInlineObject* /*inlineObject*/,
        BOOL /*isSideways*/,
        BOOL /*isRightToLeft*/,
        IUnknown* /*clientDrawingEffect*/
        );

    STDMETHOD(IsPixelSnappingDisabled)(
        void* /*clientDrawingContext*/,
        BOOL* isDisabled
        );

    STDMETHOD(GetCurrentTransform)(
        void* /*clientDrawingContext*/,
        DWRITE_MATRIX* transform
        );

    STDMETHOD(GetPixelsPerDip)(
        void* /*clientDrawingContext*/,
        FLOAT* pixelsPerDip
        );

    // IUknown methods.
    STDMETHOD(QueryInterface)(REFIID riid, _Outptr_ void** object) override;
    STDMETHOD_(ULONG, AddRef)() override;
    STDMETHOD_(ULONG, Release)() override;

    void GetGeometry(ID2D1Geometry** ppGeometry);

private:
    // Private constructor used by CreateOutlineRenderer method.
    OutlineRenderer(_In_ ID2D1Factory* pFactory);

    void AddGeometry(ID2D1Geometry* pGeometry);

    Microsoft::WRL::ComPtr<ID2D1Factory> m_pFactory;
    Microsoft::WRL::ComPtr<ID2D1Geometry> m_pGeometry;

    ULONG m_ref;    // Reference count for AddRef and Release.
};