//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PointSnapper.h"

using namespace Microsoft::WRL;

// Utility class for snapping points and vertices to grid points.
// This is useful when preparing and consuming tessellation data.
// Grid-points are spaced at 1/16 intervals and aligned on integer
// boundaries.
void PointSnapper::SnapGeometry(ID2D1Geometry* pGeometry, ID2D1Geometry** ppGeometry)
{
    ComPtr<ID2D1Factory> pFactory;
    pGeometry->GetFactory(&pFactory);

    ComPtr<ID2D1PathGeometry> pPathGeometry;
    DX::ThrowIfFailed(
        pFactory->CreatePathGeometry(&pPathGeometry)
        );

    ComPtr<ID2D1GeometrySink> pSink;
    DX::ThrowIfFailed(
        pPathGeometry->Open(&pSink)
        );

    ComPtr<PointSnappingSink> snapper;
    PointSnappingSink::CreatePointSnappingSink(
        pSink.Get(),
        &snapper
        );

    DX::ThrowIfFailed(
        pGeometry->Simplify(
            D2D1_GEOMETRY_SIMPLIFICATION_OPTION_LINES,
            nullptr, // world transform
            snapper.Get()
            )
        );

    DX::ThrowIfFailed(
        snapper->Close()
        );

    *ppGeometry = pPathGeometry.Get();
    (*ppGeometry)->AddRef();
}

float PointSnapper::SnapCoordinate(float x)
{
    return floorf((16.0f * x) + 0.5f) / 16.0f;
}

D2D1_POINT_2F PointSnapper::SnapPoint(D2D1_POINT_2F pt)
{
    return D2D1::Point2F(
        SnapCoordinate(pt.x),
        SnapCoordinate(pt.y)
        );
}

float2 PointSnapper::SnapPoint(float2 pt)
{
    return float2(
        SnapCoordinate(pt.x),
        SnapCoordinate(pt.y)
        );
}

// Internal sink used to implement SnapGeometry.
// Note: This class makes certain assumptions about its usage
// (e.g. no Beziers), which is why it's a private class.
void PointSnapper::PointSnappingSink::CreatePointSnappingSink(
    _In_ ID2D1SimplifiedGeometrySink* pSink,
    _Outptr_ PointSnapper::PointSnappingSink** ppPointSnappingSink
    )
{
    *ppPointSnappingSink = new PointSnappingSink(pSink);
    (*ppPointSnappingSink)->AddRef();
}

PointSnapper::PointSnappingSink::PointSnappingSink(_In_ ID2D1SimplifiedGeometrySink* pSink) :
    m_pSinkNoRef(pSink),
    m_ref(0)
{
}

void PointSnapper::PointSnappingSink::AddBeziers(const D2D1_BEZIER_SEGMENT* /*beziers*/, UINT /*beziersCount*/)
{
    //
    // Users should be sure to flatten their geometries prior to passing
    // through a PointSnappingSink. It makes little sense snapping
    // the control points of a Bezier, as the vertices from the
    // flattened Bezier will almost certainly not be snapped.
    //
}

void PointSnapper::PointSnappingSink::AddLines(const D2D1_POINT_2F* points, UINT pointsCount)
{
    for (UINT i = 0; i < pointsCount; ++i)
    {
        D2D1_POINT_2F pt = SnapPoint(points[i]);
        m_pSinkNoRef->AddLines(&pt, 1);
    }
}

void PointSnapper::PointSnappingSink::BeginFigure(D2D1_POINT_2F startPoint, D2D1_FIGURE_BEGIN figureBegin)
{
    D2D1_POINT_2F pt = SnapPoint(startPoint);
    m_pSinkNoRef->BeginFigure(pt, figureBegin);
}

void PointSnapper::PointSnappingSink::EndFigure(D2D1_FIGURE_END figureEnd)
{
    m_pSinkNoRef->EndFigure(figureEnd);
}

void PointSnapper::PointSnappingSink::SetFillMode(D2D1_FILL_MODE fillMode)
{
    m_pSinkNoRef->SetFillMode(fillMode);
}

void PointSnapper::PointSnappingSink::SetSegmentFlags(D2D1_PATH_SEGMENT vertexFlags)
{
    m_pSinkNoRef->SetSegmentFlags(vertexFlags);
}

HRESULT PointSnapper::PointSnappingSink::Close()
{
    return m_pSinkNoRef->Close();
}

//
// IUnknown methods
//
// These use a basic, non-thread-safe implementation of the
// standard reference-counting logic.
//
HRESULT PointSnapper::PointSnappingSink::QueryInterface(REFIID riid, _Outptr_ void** object)
{
    *object = nullptr;
    return E_NOTIMPL;
}

ULONG PointSnapper::PointSnappingSink::AddRef()
{
    m_ref++;
    return m_ref;
}

ULONG PointSnapper::PointSnappingSink::Release()
{
    m_ref--;

    if (m_ref == 0)
    {
        delete this;
        return 0;
    }

    return m_ref;
}
