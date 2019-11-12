// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// The following is a relatively simple implementation of realizations, built
// on top of Direct2D's mesh and opacity mask primitives (meshes for aliased and
// multi-sampled anti-aliased rendering and opacity masks for per-primitive
// anti-aliased rendering).
//
// One IGeometryRealization object can hold up to 4 "sub-realizations",
// corresponding to the matrix {Aliased, PPAA} x {Filled, Stroked}. The user
// can specify which sub-realizations to generate by passing in the appropriate
// REALIZATION_CREATION_OPTIONS.
//
// The implementation of the PPAA realizations is somewhat primitive. It will
// attempt to reuse existing bitmaps when possible, but it will not, for
// instance, attempt to use a single bitmap to store multiple realizations
// ("atlasing"). An atlased implementation would be somewhat more performant,
// as the number of state switches that Direct2D would have to make when
// interleaving realizations of different geometries would be greatly reduced.
//
// Another limitation in the PPAA implementation below is that it can use very
// large amounts of video-memory even for very simple primitives. Consider, for
// instance, a thin diagonal line stretching from the top-left corner of the
// render-target to the bottom-right. To store this as a single opacity mask, a
// bitmap the size of the entire render target must be created, even though the
// area of the stroke is very small. This can also have a severe impact on
// performance, as the video card has to waste numerous cycles rendering fully-
// transparent pixels.
//
// A more sophisticated implementation of PPAA realizations would divide the
// geometry up into a grid of bitmaps. Grid cells that contained no
// content or were fully covered by the geometry could be optimized away. The
// partially covered grid cells could be atlased for a further boost in
// performance.
//


#include "pch.h"
#ifndef _NO_PRECOMPILED_HEADER_
#include "GeometryRealizationPackage.h"
#endif


//+-----------------------------------------------------------------------------
//
//  Class:
//      GeometryRealization
//
//------------------------------------------------------------------------------
class GeometryRealization : public IGeometryRealization
{
public:
    STDMETHOD(Fill)(
        ID2D1DeviceContext *deviceContext,
        ID2D1Brush *brush,
        REALIZATION_RENDER_MODE mode
        );

    STDMETHOD(Draw)(
        ID2D1DeviceContext *deviceContext,
        ID2D1Brush *brush,
        REALIZATION_RENDER_MODE mode
        );

    STDMETHOD(Update)(
        ID2D1Geometry *geometry,
        REALIZATION_CREATION_OPTIONS options,
        CONST D2D1_MATRIX_3X2_F *worldTransform,
        float strokeWidth,
        ID2D1StrokeStyle *strokeStyle
        );

    STDMETHOD_(ULONG, AddRef)();
    STDMETHOD_(ULONG, Release)();

    STDMETHOD(QueryInterface)(
        REFIID iid,
        void ** ppvObject
        );

    static HRESULT Create(
        ID2D1DeviceContext *deviceContext,
        UINT maxRealizationDimension,
        ID2D1Geometry *geometry,
        REALIZATION_CREATION_OPTIONS options,
        CONST D2D1_MATRIX_3X2_F *worldTransform,
        float strokeWidth,
        ID2D1StrokeStyle *strokeStyle,
        IGeometryRealization **realization
        );

// These methods are declared protected for white-box unit-testing.
protected:
    // Non-interface methods
    GeometryRealization();
    ~GeometryRealization();

    HRESULT Initialize(
        ID2D1DeviceContext *deviceContext,
        UINT maxRealizationDimension,
        ID2D1Geometry *geometry,
        REALIZATION_CREATION_OPTIONS options,
        CONST D2D1_MATRIX_3X2_F *worldTransform,
        float strokeWidth,
        ID2D1StrokeStyle *strokeStyle
        );

    static HRESULT GenerateOpacityMask(
        bool fill,
        ID2D1DeviceContext *baseRT,
        UINT maxRealizationDimension,
        ID2D1BitmapRenderTarget **bitmapRT,
        ID2D1Geometry *geometry,
        const D2D1_MATRIX_3X2_F *worldTransform,
        float strokeWidth,
        ID2D1StrokeStyle *strokeStyle,
        D2D1_RECT_F *maskDestBounds,
        D2D1_RECT_F *maskSourceBounds
        );

    HRESULT RenderToTarget(
        bool fill,
        ID2D1DeviceContext *deviceContext,
        ID2D1Brush *brush,
        REALIZATION_RENDER_MODE mode
        );

    ID2D1Mesh *m_fillMesh;
    ID2D1Mesh *m_strokeMesh;

    ID2D1BitmapRenderTarget *m_fillRT;
    ID2D1BitmapRenderTarget *m_strokeRT;

    ID2D1Geometry *m_geometry;
    ID2D1StrokeStyle *m_strokeStyle;
    float m_strokeWidth;

    D2D1_RECT_F m_fillMaskDestBounds;
    D2D1_RECT_F m_fillMaskSourceBounds;

    D2D1_RECT_F m_strokeMaskDestBounds;
    D2D1_RECT_F m_strokeMaskSourceBounds;

    ID2D1DeviceContext *m_deviceContext;

    bool m_realizationTransformIsIdentity;
    D2D1::Matrix3x2F m_realizationTransform;
    D2D1::Matrix3x2F m_realizationTransformInv;

    BOOL m_swRT;

    UINT m_maxRealizationDimension;

    ULONG volatile m_ref;
};

//+-----------------------------------------------------------------------------
//
//  Class:
//      GeometryRealizationFactory
//
//------------------------------------------------------------------------------
class GeometryRealizationFactory : public IGeometryRealizationFactory
{
public:
    STDMETHOD(CreateGeometryRealization)(
        IGeometryRealization **realization
        );

    STDMETHOD(CreateGeometryRealization)(
        ID2D1Geometry *geometry,
        REALIZATION_CREATION_OPTIONS options,
        CONST D2D1_MATRIX_3X2_F *worldTransform,
        float strokeWidth,
        ID2D1StrokeStyle *strokeStyle,
        IGeometryRealization **realization
        );

    STDMETHOD_(ULONG, AddRef)();
    STDMETHOD_(ULONG, Release)();

    STDMETHOD(QueryInterface)(
        REFIID iid,
        void ** object
        );

    // Non-interface methods
    static HRESULT Create(
        ID2D1DeviceContext *deviceContext,
        UINT maxRealizationDimension,
        IGeometryRealizationFactory **factory
        );

// These methods are declared protected for white-box unit-testing.
protected:

    GeometryRealizationFactory();
    ~GeometryRealizationFactory();

    HRESULT Initialize(
        ID2D1DeviceContext *deviceContext,
        UINT maxRealizationDimension
        );

    ID2D1DeviceContext *m_deviceContext;

    ULONG volatile m_ref;

    UINT m_maxRealizationDimension;
};

// The maximum granularity of bitmap sizes we allow for AA realizations.
static const UINT sc_bitmapChunkSize = 64;

//+-----------------------------------------------------------------------------
//
//  Function:
//      CreateGeometryRealizationFactory
//
//------------------------------------------------------------------------------
HRESULT CreateGeometryRealizationFactory(
    ID2D1DeviceContext *deviceContext,
    UINT maxRealizationDimension,
    IGeometryRealizationFactory **factory
    )
{
    return GeometryRealizationFactory::Create(
        deviceContext,
        maxRealizationDimension,
        factory
        );
}

//+-----------------------------------------------------------------------------
//
//  Function:
//      CreateGeometryRealizationFactory
//
//------------------------------------------------------------------------------
HRESULT CreateGeometryRealizationFactory(
    ID2D1DeviceContext *deviceContext,
    IGeometryRealizationFactory **factory
    )
{
    return CreateGeometryRealizationFactory(
        deviceContext,
        0xffffffff, // maxRealizationDimension
        factory
        );
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::Create
//
//------------------------------------------------------------------------------
/* static */
HRESULT GeometryRealizationFactory::Create(
    ID2D1DeviceContext *deviceContext,
    UINT maxRealizationDimension,
    IGeometryRealizationFactory **factory
)
{
    HRESULT hr = S_OK;

    GeometryRealizationFactory *realizationFactory = nullptr;
    realizationFactory = new (std::nothrow) GeometryRealizationFactory();
    hr = realizationFactory ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        hr = realizationFactory->Initialize(deviceContext, maxRealizationDimension);

        if (SUCCEEDED(hr))
        {
            *factory = realizationFactory;
            (*factory)->AddRef();
        }

        realizationFactory->Release();
    }

    return hr;
}


//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::GeometryRealizationFactory
//
//------------------------------------------------------------------------------
GeometryRealizationFactory::GeometryRealizationFactory() :
    m_ref(1),
    m_deviceContext(nullptr)
{
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      ~GeometryRealizationFactory::GeometryRealizationFactory
//
//------------------------------------------------------------------------------
GeometryRealizationFactory::~GeometryRealizationFactory()
{
    SafeRelease(&m_deviceContext);
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::Initialize
//
//------------------------------------------------------------------------------
HRESULT GeometryRealizationFactory::Initialize(
    ID2D1DeviceContext *deviceContext,
    UINT maxRealizationDimension
    )
{
    HRESULT hr = S_OK;

    if (maxRealizationDimension == 0)
    {
        //
        // 0-sized bitmaps aren't very useful for realizations, and
        // DXGI surface render targets don't support them, anyway.
        //
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        m_deviceContext = deviceContext;
        m_deviceContext->AddRef();

        m_maxRealizationDimension = min(
            deviceContext->GetMaximumBitmapSize(),
            maxRealizationDimension
            );
    }

    return hr;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::CreateGeometryRealization
//
//------------------------------------------------------------------------------
STDMETHODIMP GeometryRealizationFactory::CreateGeometryRealization(
    ID2D1Geometry *geometry,
    REALIZATION_CREATION_OPTIONS options,
    CONST D2D1_MATRIX_3X2_F *worldTransform,
    float strokeWidth,
    ID2D1StrokeStyle *strokeStyle,
    IGeometryRealization **realization
    )
{
    return GeometryRealization::Create(
        m_deviceContext,
        m_maxRealizationDimension,
        geometry,
        options,
        worldTransform,
        strokeWidth,
        strokeStyle,
        realization
        );
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::CreateGeometryRealization
//
//------------------------------------------------------------------------------
STDMETHODIMP GeometryRealizationFactory::CreateGeometryRealization(
    IGeometryRealization **realization
    )
{
    return GeometryRealization::Create(
        m_deviceContext,
        m_maxRealizationDimension,
        nullptr, // pGeometry
        REALIZATION_CREATION_OPTIONS_ALIASED, // ignored.
        nullptr,
        0.0f, // strokeWidth
        nullptr, // pIStrokeStyle
        realization
        );
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::Create
//
//------------------------------------------------------------------------------
/* static */
HRESULT GeometryRealization::Create(
    ID2D1DeviceContext *deviceContext,
    UINT maxRealizationDimension,
    ID2D1Geometry *geometry,
    REALIZATION_CREATION_OPTIONS options,
    CONST D2D1_MATRIX_3X2_F *worldTransform,
    float strokeWidth,
    ID2D1StrokeStyle *strokeStyle,
    IGeometryRealization **realization
    )
{
    HRESULT hr = S_OK;

    GeometryRealization *geometryRealization = nullptr;

    geometryRealization = new (std::nothrow) GeometryRealization();
    hr = geometryRealization ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        hr = geometryRealization->Initialize(
            deviceContext,
            maxRealizationDimension,
            geometry,
            options,
            worldTransform,
            strokeWidth,
            strokeStyle
            );
        if (SUCCEEDED(hr))
        {
            *realization = geometryRealization;
            (*realization)->AddRef();
        }

        geometryRealization->Release();
    }

    return hr;
}

//+----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::GeometryRealization
//
//-----------------------------------------------------------------------------

GeometryRealization::GeometryRealization() :
    m_ref(1),
    m_fillMesh(nullptr),
    m_strokeMesh(nullptr),
    m_fillRT(nullptr),
    m_strokeRT(nullptr),
    m_geometry(nullptr),
    m_strokeStyle(nullptr),
    m_deviceContext(nullptr)
{
}

//+----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::GeometryRealization
//
//-----------------------------------------------------------------------------
GeometryRealization::~GeometryRealization()
{
    SafeRelease(&m_fillMesh);
    SafeRelease(&m_strokeMesh);
    SafeRelease(&m_fillRT);
    SafeRelease(&m_strokeRT);
    SafeRelease(&m_geometry);
    SafeRelease(&m_strokeStyle);
    SafeRelease(&m_deviceContext);
}


//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::Fill
//
//------------------------------------------------------------------------------
STDMETHODIMP GeometryRealization::Fill(
    ID2D1DeviceContext *deviceContext,
    ID2D1Brush *brush,
    REALIZATION_RENDER_MODE mode
    )
{
    return RenderToTarget(
        true, // => fill
        deviceContext,
        brush,
        mode
        );
}


//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::Draw
//
//------------------------------------------------------------------------------
STDMETHODIMP GeometryRealization::Draw(
    ID2D1DeviceContext *deviceContext,
    ID2D1Brush *brush,
    REALIZATION_RENDER_MODE mode
    )
{
    return RenderToTarget(
        false, // => stroke
        deviceContext,
        brush,
        mode
        );
}


//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::Update
//
//  Description:
//      Discard the current realization's contents and replace with new
//      contents.
//
//      Note: This method attempts to reuse the existing bitmaps (but will
//      replace the bitmaps if they aren't large enough). Since the cost of
//      destroying a texture can be surprisingly astronomical, using this
//      method can be substantially more performant than recreating a new
//      realization every time.
//
//      Note: Here, pWorldTransform is the transform that the realization will
//      be optimized for. If, at the time of rendering, the render target's
//      transform is the same as the pWorldTransform passed in here then the
//      realization will look identical to the unrealized version. Otherwise,
//      quality will be degraded.
//
//------------------------------------------------------------------------------
STDMETHODIMP GeometryRealization::Update(
    ID2D1Geometry *geometry,
    REALIZATION_CREATION_OPTIONS options,
    CONST D2D1_MATRIX_3X2_F *worldTransform,
    float strokeWidth,
    ID2D1StrokeStyle *strokeStyle
    )
{
    HRESULT hr = S_OK;

    if (worldTransform)
    {
        m_realizationTransform = *D2D1::Matrix3x2F::ReinterpretBaseType(worldTransform);
        m_realizationTransformIsIdentity = (m_realizationTransform.IsIdentity());
    }
    else
    {
        m_realizationTransform = D2D1::Matrix3x2F::Identity();
        m_realizationTransformIsIdentity = true;
    }

    //
    // We're about to create our realizations with the world transform applied
    // to them.  When we go to actually render the realization, though, we'll
    // want to "undo" this realization and instead apply the render target's
    // current transform.
    //
    // Note: we keep track to see if the passed in realization transform is the
    // identity.  This is a small optimization that saves us from having to
    // multiply matrices when we go to draw the realization.
    //

    m_realizationTransformInv = m_realizationTransform;
    m_realizationTransformInv.Invert();

    if ((options & REALIZATION_CREATION_OPTIONS_UNREALIZED) || m_swRT)
    {
        SafeReplace(&m_geometry, geometry);
        SafeReplace(&m_strokeStyle, strokeStyle);
        m_strokeWidth = strokeWidth;
    }

    if (options & REALIZATION_CREATION_OPTIONS_ANTI_ALIASED)
    {
        //
        // Antialiased realizations are implemented using opacity masks.
        //

        if (options & REALIZATION_CREATION_OPTIONS_FILLED)
        {
            hr = GenerateOpacityMask(
                true, // => filled
                m_deviceContext,
                m_maxRealizationDimension,
                IN OUT &m_fillRT,
                geometry,
                worldTransform,
                strokeWidth,
                strokeStyle,
                &m_fillMaskDestBounds,
                &m_fillMaskSourceBounds
                );
        }

        if (SUCCEEDED(hr) && options & REALIZATION_CREATION_OPTIONS_STROKED)
        {
            hr = GenerateOpacityMask(
                false, // => stroked
                m_deviceContext,
                m_maxRealizationDimension,
                IN OUT &m_strokeRT,
                geometry,
                worldTransform,
                strokeWidth,
                strokeStyle,
                &m_strokeMaskDestBounds,
                &m_strokeMaskSourceBounds
                );
        }
    }

    if (SUCCEEDED(hr) && options & REALIZATION_CREATION_OPTIONS_ALIASED)
    {
        //
        // Aliased realizations are implemented using meshes.
        //

        if (options & REALIZATION_CREATION_OPTIONS_FILLED)
        {
            ID2D1Mesh *pMesh = nullptr;
            hr = m_deviceContext->CreateMesh(&pMesh);
            if (SUCCEEDED(hr))
            {
                ID2D1TessellationSink *pSink = nullptr;
                hr = pMesh->Open(&pSink);
                if (SUCCEEDED(hr))
                {
                    hr = geometry->Tessellate(worldTransform, pSink);
                    if (SUCCEEDED(hr))
                    {
                        hr = pSink->Close();
                        if (SUCCEEDED(hr))
                        {
                            SafeReplace(&m_fillMesh, pMesh);
                        }
                    }
                    pSink->Release();
                }
                pMesh->Release();
            }
        }

        if (SUCCEEDED(hr) && options & REALIZATION_CREATION_OPTIONS_STROKED)
        {
            //
            // In order generate the mesh corresponding to the stroke of a
            // geometry, we first "widen" the geometry and then tessellate the
            // result.
            //

            ID2D1Factory *pFactory = nullptr;
            m_deviceContext->GetFactory(&pFactory);

            ID2D1PathGeometry *pPathGeometry = nullptr;
            hr = pFactory->CreatePathGeometry(&pPathGeometry);
            if (SUCCEEDED(hr))
            {
                ID2D1GeometrySink *pGeometrySink = nullptr;
                hr = pPathGeometry->Open(&pGeometrySink);
                if (SUCCEEDED(hr))
                {
                    hr = geometry->Widen(
                        strokeWidth,
                        strokeStyle,
                        worldTransform,
                        pGeometrySink
                        );

                    if (SUCCEEDED(hr))
                    {
                        hr = pGeometrySink->Close();
                        if (SUCCEEDED(hr))
                        {
                            ID2D1Mesh *pMesh = nullptr;
                            hr = m_deviceContext->CreateMesh(&pMesh);
                            if (SUCCEEDED(hr))
                            {
                                ID2D1TessellationSink *pSink = nullptr;
                                hr = pMesh->Open(&pSink);
                                if (SUCCEEDED(hr))
                                {
                                    hr = pPathGeometry->Tessellate(
                                        nullptr, // world transform (already handled in Widen)
                                        pSink
                                        );
                                    if (SUCCEEDED(hr))
                                    {
                                        hr = pSink->Close();
                                        if (SUCCEEDED(hr))
                                        {
                                            SafeReplace(&m_strokeMesh, pMesh);
                                        }
                                    }
                                    pSink->Release();
                                }
                                pMesh->Release();
                            }
                        }
                    }
                    pGeometrySink->Release();
                }
                pPathGeometry->Release();
            }
            pFactory->Release();
        }
    }

    return hr;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::RenderToTarget
//
//------------------------------------------------------------------------------
HRESULT GeometryRealization::RenderToTarget(
    bool fill,
    ID2D1DeviceContext *pDeviceContext,
    ID2D1Brush *pBrush,
    REALIZATION_RENDER_MODE mode
    )
{
    HRESULT hr = S_OK;

    D2D1_ANTIALIAS_MODE originalAAMode = pDeviceContext->GetAntialiasMode();
    D2D1_MATRIX_3X2_F originalTransform;

    if (((mode == REALIZATION_RENDER_MODE_DEFAULT) && m_swRT) ||
        (mode == REALIZATION_RENDER_MODE_FORCE_UNREALIZED)
        )
    {
        if (!m_geometry)
        {
            // We're being asked to render the geometry unrealized, but we
            // weren't created with REALIZATION_CREATION_OPTIONS_UNREALIZED.
            hr = E_FAIL;
        }
        if (SUCCEEDED(hr))
        {
            if (fill)
            {
                pDeviceContext->FillGeometry(
                    m_geometry,
                    pBrush
                    );
            }
            else
            {
                pDeviceContext->DrawGeometry(
                    m_geometry,
                    pBrush,
                    m_strokeWidth,
                    m_strokeStyle
                    );
            }
        }
    }
    else
    {
        if (originalAAMode != D2D1_ANTIALIAS_MODE_ALIASED)
        {
            pDeviceContext->SetAntialiasMode(D2D1_ANTIALIAS_MODE_ALIASED);
        }

        if (!m_realizationTransformIsIdentity)
        {
            pDeviceContext->GetTransform(&originalTransform);
            pDeviceContext->SetTransform(m_realizationTransformInv * originalTransform);
        }

        if (originalAAMode == D2D1_ANTIALIAS_MODE_PER_PRIMITIVE)
        {
            if (fill)
            {
                if (!m_fillRT)
                {
                    hr = E_FAIL;
                }
                if (SUCCEEDED(hr))
                {
                    ID2D1Bitmap *pBitmap = nullptr;
                    m_fillRT->GetBitmap(&pBitmap);

                    //
                    // Note: The antialias mode must be set to aliased prior to calling
                    // FillOpacityMask.
                    //
                    pDeviceContext->FillOpacityMask(
                        pBitmap,
                        pBrush,
                        D2D1_OPACITY_MASK_CONTENT_GRAPHICS,
                        &m_fillMaskDestBounds,
                        &m_fillMaskSourceBounds
                        );

                    pBitmap->Release();
                }
            }
            else
            {
                if (!m_strokeRT)
                {
                    hr = E_FAIL;
                }
                if (SUCCEEDED(hr))
                {
                    ID2D1Bitmap *pBitmap = nullptr;
                    m_strokeRT->GetBitmap(&pBitmap);

                    //
                    // Note: The antialias mode must be set to aliased prior to calling
                    // FillOpacityMask.
                    //
                    pDeviceContext->FillOpacityMask(
                        pBitmap,
                        pBrush,
                        D2D1_OPACITY_MASK_CONTENT_GRAPHICS,
                        &m_strokeMaskDestBounds,
                        &m_strokeMaskSourceBounds
                        );

                    pBitmap->Release();
                }
            }
        }
        else
        {
            if (fill)
            {
                if (!m_fillMesh)
                {
                    hr = E_FAIL;
                }
                if (SUCCEEDED(hr))
                {
                    pDeviceContext->FillMesh(
                        m_fillMesh,
                        pBrush
                        );
                }
            }
            else
            {
                if (!m_strokeMesh)
                {
                    hr = E_FAIL;
                }
                if (SUCCEEDED(hr))
                {
                    pDeviceContext->FillMesh(
                        m_strokeMesh,
                        pBrush
                        );
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            pDeviceContext->SetAntialiasMode(originalAAMode);

            if (!m_realizationTransformIsIdentity)
            {
                pDeviceContext->SetTransform(originalTransform);
            }
        }
    }

    return hr;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::Initialize
//
//------------------------------------------------------------------------------
HRESULT GeometryRealization::Initialize(
    ID2D1DeviceContext *deviceContext,
    UINT maxRealizationDimension,
    ID2D1Geometry *geometry,
    REALIZATION_CREATION_OPTIONS options,
    CONST D2D1_MATRIX_3X2_F *worldTransform,
    float strokeWidth,
    ID2D1StrokeStyle *strokeStyle
)
{
    HRESULT hr = S_OK;

    m_deviceContext = deviceContext;
    m_deviceContext->AddRef();

    m_swRT = deviceContext->IsSupported(
        D2D1::RenderTargetProperties(D2D1_RENDER_TARGET_TYPE_SOFTWARE)
        );

    m_maxRealizationDimension = maxRealizationDimension;

    if (geometry)
    {
        hr = Update(
            geometry,
            options,
            worldTransform,
            strokeWidth,
            strokeStyle
            );
    }

    return hr;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::GenerateOpacityMask
//
//  Notes:
//      This method is the trickiest part of doing realizations. Conceptually,
//      we're creating a grayscale bitmap that represents the geometry. We'll
//      reuse an existing bitmap if we can, but if not, we'll create the
//      smallest possible bitmap that contains the geometry. In either, case,
//      though, we'll keep track of the portion of the bitmap we actually used
//      (the source bounds), so when we go to draw the realization, we don't
//      end up drawing a bunch of superfluous transparent pixels.
//
//      We also have to keep track of the "dest" bounds, as more than likely
//      the bitmap has to be translated by some amount before being drawn.
//
//------------------------------------------------------------------------------
/* static */
HRESULT GeometryRealization::GenerateOpacityMask(
    bool fill,
    ID2D1DeviceContext *baseRT,
    UINT maxRealizationDimension,
    ID2D1BitmapRenderTarget **bitmapRT,
    ID2D1Geometry *geometry,
    const D2D1_MATRIX_3X2_F *worldTransform,
    float strokeWidth,
    ID2D1StrokeStyle *strokeStyle,
    D2D1_RECT_F *maskDestBounds,
    D2D1_RECT_F *maskSourceBounds
    )
{
    HRESULT hr = S_OK;

    D2D1_RECT_F bounds;
    D2D1_RECT_F inflatedPixelBounds;
    D2D1_SIZE_U inflatedIntegerPixelSize;
    D2D1_SIZE_U currentRTSize;
    D2D1_MATRIX_3X2_F translateMatrix;
    float dpiX, dpiY;
    float scaleX = 1.0f;
    float scaleY = 1.0f;

    ID2D1BitmapRenderTarget *compatRT = nullptr;
    SafeReplace(&compatRT, *bitmapRT);

    ID2D1SolidColorBrush *brush = nullptr;

    hr = baseRT->CreateSolidColorBrush(
        D2D1::ColorF(1.0f, 1.0f, 1.0f, 1.0f),
        &brush
        );
    if (SUCCEEDED(hr))
    {
        baseRT->GetDpi(&dpiX, &dpiY);
        if (fill)
        {
            hr = geometry->GetBounds(
                worldTransform,
                &bounds
                );
        }
        else
        {
            hr = geometry->GetWidenedBounds(
                strokeWidth,
                strokeStyle,
                worldTransform,
                &bounds
                );
        }

        if (SUCCEEDED(hr))
        {
            //
            // A rect where left > right is defined to be empty.
            //
            // The slightly baroque expression used below is an idiom that also
            // correctly handles NaNs (i.e., if any of the coordinates of the bounds is
            // a NaN, we want to treat the bounds as empty)
            //
            if (!(bounds.left <= bounds.right) ||
                !(bounds.top <= bounds.bottom)
                )
            {
                // Bounds are empty or ill-defined.

                // Make up a fake bounds
                inflatedPixelBounds.top = 0.0f;
                inflatedPixelBounds.left = 0.0f;
                inflatedPixelBounds.bottom = 1.0f;
                inflatedPixelBounds.right = 1.0f;
            }
            else
            {
                //
                // We inflate the pixel bounds by 1 in each direction to ensure we have
                // a border of completely transparent pixels around the geometry.  This
                // ensures that when the realization is stretched the alpha ramp still
                // smoothly falls off to 0 rather than being clipped by the rect.
                //
                inflatedPixelBounds.top = floorf(bounds.top*dpiY/96)-1.0f;
                inflatedPixelBounds.left = floorf(bounds.left*dpiX/96)-1.0f;
                inflatedPixelBounds.bottom = ceilf(bounds.bottom*dpiY/96)+1.0f;
                inflatedPixelBounds.right = ceilf(bounds.right*dpiX/96)+1.0f;
            }


            //
            // Compute the width and height of the underlying bitmap we will need.
            // Note: We round up the width and height to be a multiple of
            // sc_bitmapChunkSize. We do this primarily to ensure that we aren't
            // constantly reallocating bitmaps in the case where a realization is being
            // zoomed in on slowly and updated frequently.
            //

            inflatedIntegerPixelSize = D2D1::SizeU(
                static_cast<UINT>(inflatedPixelBounds.right - inflatedPixelBounds.left),
                static_cast<UINT>(inflatedPixelBounds.bottom - inflatedPixelBounds.top)
                );

            // Round up
            inflatedIntegerPixelSize.width =
                (inflatedIntegerPixelSize.width + sc_bitmapChunkSize - 1)/sc_bitmapChunkSize * sc_bitmapChunkSize;

            // Round up
            inflatedIntegerPixelSize.height =
                (inflatedIntegerPixelSize.height + sc_bitmapChunkSize - 1)/sc_bitmapChunkSize * sc_bitmapChunkSize;

            //
            // Compute the bounds we will pass to FillOpacityMask (which are in Device
            // Independent Pixels).
            //
            // Note: The DIP bounds do *not* use the rounded coordinates, since this
            // would cause us to render superfluous, fully-transparent pixels, which
            // would hurt fill rate.
            //
            D2D1_RECT_F inflatedDipBounds = D2D1::RectF(
                inflatedPixelBounds.left * 96/dpiX,
                inflatedPixelBounds.top * 96/dpiY,
                inflatedPixelBounds.right * 96/dpiX,
                inflatedPixelBounds.bottom * 96/dpiY
                );

            if (compatRT)
            {
                currentRTSize = compatRT->GetPixelSize();
            }
            else
            {
                // This will force the creation of a new target
                currentRTSize = D2D1::SizeU(0, 0);
            }

            //
            // We need to ensure that our desired render target size isn't larger than
            // the max allowable bitmap size. If it is, we need to scale the bitmap
            // down by the appropriate amount.
            //

            if (inflatedIntegerPixelSize.width > maxRealizationDimension)
            {
                scaleX = maxRealizationDimension/static_cast<float>(inflatedIntegerPixelSize.width);
                inflatedIntegerPixelSize.width = maxRealizationDimension;
            }

            if (inflatedIntegerPixelSize.height > maxRealizationDimension)
            {
                scaleY = maxRealizationDimension/static_cast<float>(inflatedIntegerPixelSize.height);
                inflatedIntegerPixelSize.height = maxRealizationDimension;
            }


            //
            // If the necessary pixel dimensions are less than half the existing
            // bitmap's dimensions (in either direction), force the bitmap to be
            // reallocated to save memory.
            //
            // Note: The fact that we use > rather than >= is important for a subtle
            // reason: We'd like to have the property that repeated small changes in
            // geometry size do not cause repeated reallocations of memory. >= does not
            // ensure this property in the case where the geometry size is close to
            // sc_bitmapChunkSize, but > does.
            //
            // Example:
            //
            // Assume sc_bitmapChunkSize is 64 and the initial geometry width is 63
            // pixels. This will get rounded up to 64, and we will allocate a bitmap
            // with width 64. Now, say, we zoom in slightly, so the new geometry width
            // becomes 65 pixels. This will get rounded up to 128 pixels, and a new
            // bitmap will be allocated. Now, say the geometry drops back down to 63
            // pixels. This will get rounded up to 64. If we used >=, this would cause
            // another reallocation.  Since we use >, on the other hand, the 128 pixel
            // bitmap will be reused.
            //

            if (currentRTSize.width > 2*inflatedIntegerPixelSize.width ||
                currentRTSize.height > 2*inflatedIntegerPixelSize.height
                )
            {
                SafeRelease(&compatRT);
                currentRTSize.width = currentRTSize.height = 0;
            }

            if (inflatedIntegerPixelSize.width > currentRTSize.width ||
                inflatedIntegerPixelSize.height > currentRTSize.height
                )
            {
                SafeRelease(&compatRT);
            }

            if (!compatRT)
            {
                //
                // Make sure our new rendertarget is strictly larger than before.
                //
                currentRTSize.width =
                    max(inflatedIntegerPixelSize.width, currentRTSize.width);

                currentRTSize.height =
                    max(inflatedIntegerPixelSize.height, currentRTSize.height);

                D2D1_PIXEL_FORMAT alphaOnlyFormat =
                    D2D1::PixelFormat(
                        DXGI_FORMAT_A8_UNORM,
                        D2D1_ALPHA_MODE_PREMULTIPLIED
                        );

                hr = baseRT->CreateCompatibleRenderTarget(
                    nullptr, // desiredSize
                    &currentRTSize,
                    &alphaOnlyFormat,
                    D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS_NONE,
                    &compatRT
                    );
            }

            if (SUCCEEDED(hr))
            {
                //
                // Translate the geometry so it is flush against the left and top
                // sides of the render target.
                //

                translateMatrix =
                    D2D1::Matrix3x2F::Translation(
                        -inflatedDipBounds.left,
                        -inflatedDipBounds.top
                        ) *
                    D2D1::Matrix3x2F::Scale(
                        scaleX,
                        scaleY
                        );

                if (worldTransform)
                {
                    compatRT->SetTransform(
                        *worldTransform * translateMatrix
                        );
                }
                else
                {
                    compatRT->SetTransform(
                        translateMatrix
                        );
                }

                //
                // Render the geometry.
                //

                compatRT->BeginDraw();

                compatRT->Clear(
                    D2D1::ColorF(0.0f, 0.0f, 0.0f, 0.0f)
                    );

                if (fill)
                {
                    compatRT->FillGeometry(
                        geometry,
                        brush
                        );
                }
                else
                {
                    compatRT->DrawGeometry(
                        geometry,
                        brush,
                        strokeWidth,
                        strokeStyle
                        );
                }

                hr = compatRT->EndDraw();
                if (hr == D2DERR_RECREATE_TARGET)
                {
                    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
                    // is lost. It will be handled during the next call to Present.
                    hr = S_OK;
                }
                if (SUCCEEDED(hr))
                {
                    //
                    // Report back the source and dest bounds (to be used as input parameters
                    // to FillOpacityMask.
                    //
                    *maskDestBounds = inflatedDipBounds;

                    *maskSourceBounds = D2D1::Rect<float>(
                        0.0f,
                        0.0f,
                        static_cast<float>(inflatedDipBounds.right - inflatedDipBounds.left)*scaleX,
                        static_cast<float>(inflatedDipBounds.bottom - inflatedDipBounds.top)*scaleY
                        );

                    if (*bitmapRT != compatRT)
                    {
                        SafeReplace(bitmapRT, compatRT);
                    }
                }
            }
        }
        brush->Release();
    }

    SafeRelease(&compatRT);

    return hr;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::AddRef
//
//------------------------------------------------------------------------------
STDMETHODIMP_(ULONG) GeometryRealizationFactory::AddRef()
{
    return InterlockedIncrement(reinterpret_cast<LONG volatile *>(&m_ref));
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::Release
//
//------------------------------------------------------------------------------
STDMETHODIMP_(ULONG) GeometryRealizationFactory::Release()
{
    ULONG ref = static_cast<ULONG>(
        InterlockedDecrement(reinterpret_cast<LONG volatile *>(&m_ref)));

    if (0 == ref)
    {
        delete this;
    }

    return ref;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealizationFactory::QueryInterface
//
//------------------------------------------------------------------------------
STDMETHODIMP GeometryRealizationFactory::QueryInterface(
    REFIID iid,
    void ** object
    )
{
    HRESULT hr = S_OK;

    if (__uuidof(IUnknown) == iid)
    {
        *object = static_cast<IUnknown*>(this);
        AddRef();
    }
    else if (__uuidof(IGeometryRealizationFactory) == iid)
    {
        *object = static_cast<IGeometryRealizationFactory*>(this);
        AddRef();
    }
    else
    {
        *object = nullptr;
        hr = E_NOINTERFACE;
    }

    return hr;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::AddRef
//
//------------------------------------------------------------------------------
STDMETHODIMP_(ULONG)
GeometryRealization::AddRef()
{
    return InterlockedIncrement(reinterpret_cast<LONG volatile *>(&m_ref));
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::Release
//
//------------------------------------------------------------------------------
STDMETHODIMP_(ULONG)
GeometryRealization::Release()
{
    ULONG ref = static_cast<ULONG>(
        InterlockedDecrement(reinterpret_cast<LONG volatile *>(&m_ref)));

    if (0 == ref)
    {
        delete this;
    }

    return ref;
}

//+-----------------------------------------------------------------------------
//
//  Method:
//      GeometryRealization::QueryInterface
//
//------------------------------------------------------------------------------
STDMETHODIMP
GeometryRealization::QueryInterface(
    REFIID iid,
    void ** object
    )
{
    HRESULT hr = S_OK;

    if (__uuidof(IUnknown) == iid)
    {
        *object = static_cast<IUnknown*>(this);
        AddRef();
    }
    else if (__uuidof(IGeometryRealization) == iid)
    {
        *object = static_cast<IGeometryRealization*>(this);
        AddRef();
    }
    else
    {
        *object = nullptr;
        hr = E_NOINTERFACE;
    }

    return hr;
}

