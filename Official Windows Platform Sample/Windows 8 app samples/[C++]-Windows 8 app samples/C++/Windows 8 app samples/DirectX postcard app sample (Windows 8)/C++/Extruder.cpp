//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Extruder.h"

using namespace Microsoft::WRL;

static const float FlatteningTolerance = 0.1f;

void Extruder::ExtrudeGeometry(ID2D1Geometry* pGeometry, float height, std::vector<SimpleVertex>* vertices)
{
    //
    // The basic idea here is to generate the side faces by walking the
    // geometry and constructing quads, and use ID2D1Geometry::Tessellate
    // to generate the front and back faces.
    //

    // Flatten our geometry first so we don't have to worry about stitching
    // together seams of Beziers.
    ComPtr<ID2D1Geometry> pFlattenedGeometry;
    D2DFlatten(pGeometry, FlatteningTolerance, &pFlattenedGeometry);

    // D2DOutline will remove any self-intersections. This is important to
    // ensure that the tessellator doesn't introduce new vertices (which
    // can cause T-junctions).
    ComPtr<ID2D1Geometry> pOutlinedGeometry;
    D2DOutline(pFlattenedGeometry.Get(), &pOutlinedGeometry);

    ComPtr<ID2D1Geometry> pSnappedGeometry;
    PointSnapper::SnapGeometry(pOutlinedGeometry.Get(), &pSnappedGeometry);

    ComPtr<ExtrudingSink> helper;
    ExtrudingSink::CreateExtrudingSink(height, vertices, &helper);

    DX::ThrowIfFailed(
        pSnappedGeometry->Tessellate(nullptr, helper.Get())
        );

    // Simplify is a convenient API for extracting the data out of a geometry.
    DX::ThrowIfFailed(
        pOutlinedGeometry->Simplify(
            D2D1_GEOMETRY_SIMPLIFICATION_OPTION_LINES,
            nullptr, // world transform
            helper.Get()
            )
        );

    // This Close() call is a little ambiguous, since it refers both to the
    // ID2D1TessellationSink and to the ID2D1SimplifiedGeometrySink.
    // Thankfully, it really doesn't matter with our ExtrudingSink.
    DX::ThrowIfFailed(
        helper->Close()
        );
}

// Helper function for performing "flattening" -- transforming
// a geometry with Beziers into one containing only line segments.
void Extruder::D2DFlatten(ID2D1Geometry* pGeometry, float flatteningTolerance, ID2D1Geometry** ppGeometry)
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

    DX::ThrowIfFailed(
        pGeometry->Simplify(
            D2D1_GEOMETRY_SIMPLIFICATION_OPTION_LINES,
            nullptr, // world transform
            flatteningTolerance,
            pSink.Get()
            )
        );

    DX::ThrowIfFailed(
        pSink->Close()
        );

    *ppGeometry = pPathGeometry.Get();
    (*ppGeometry)->AddRef();
}

// Helper function for performing "outlining" -- constructing an
// equivalent geometry with no self-intersections. Note: This
// uses the default flattening tolerance and hence should not be
// used with very small geometries.
void Extruder::D2DOutline(ID2D1Geometry* pGeometry, ID2D1Geometry** ppGeometry)
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

    DX::ThrowIfFailed(
        pGeometry->Outline(nullptr, pSink.Get())
        );

    DX::ThrowIfFailed(
        pSink->Close()
        );

    *ppGeometry = pPathGeometry.Get();
    (*ppGeometry)->AddRef();
}

// Internal sink used to implement Extruder.
// Note: This class makes certain assumptions about its usage
// (e.g. no Beziers), which is why it's a private class.
// Note 2: Both ID2D1SimplifiedGeometrySink and
// ID2D1TessellationSink define a Close() method, which is
// bending the rules a bit. This is another reason why we are
// significantly limiting its usage.
void Extruder::ExtrudingSink::CreateExtrudingSink(
    float height,
    _In_ std::vector<SimpleVertex>* pVertices,
    _Outptr_ ExtrudingSink** ppExtrudingSink
    )
{
    *ppExtrudingSink = new ExtrudingSink(height, pVertices);
    (*ppExtrudingSink)->AddRef();
}

Extruder::ExtrudingSink::ExtrudingSink(float height, _In_ std::vector<SimpleVertex>* pVertices) :
    m_height(height),
    m_vertices(pVertices),
    m_ref(0)
{
}

void Extruder::ExtrudingSink::AddBeziers(const D2D1_BEZIER_SEGMENT* /*beziers*/, UINT /*beziersCount*/)
{
    //
    // ExtrudingSink only handles line segments. Users should flatten
    // their geometry prior to passing through an ExtrudingSink.
    //
}

void Extruder::ExtrudingSink::AddLines(const D2D1_POINT_2F* points, UINT pointsCount)
{
    for (UINT i = 0; i < pointsCount; ++i)
    {
        Vertex2D v;

        v.pt = float2(points[i].x, points[i].y);

        //
        // Take care to ignore degenerate segments, as we will be
        // unable to compute proper normals for them.
        //
        // Note: This doesn't handle near-degenerate segments, which
        // should probably also be removed. The one complication here
        // is that the segments should be removed from both the outline
        // and the front/back tessellations.
        //
        if ((m_figureVertices.size() == 0) ||
            (v.pt.x != m_figureVertices.back().pt.x) ||
            (v.pt.y != m_figureVertices.back().pt.y)
            )
        {
            m_figureVertices.push_back(v);
        }
    }
}

void Extruder::ExtrudingSink::BeginFigure(D2D1_POINT_2F startPoint, D2D1_FIGURE_BEGIN /*figureBegin*/)
{
    m_figureVertices.clear();

    Vertex2D v = {
        float2(startPoint.x, startPoint.y),
        float2(0, 0), // dummy
        float2(0, 0), // dummy
        float2(0, 0) // dummy
    };

    m_figureVertices.push_back(v);
}

HRESULT Extruder::ExtrudingSink::Close()
{
    return S_OK;
}

void Extruder::ExtrudingSink::EndFigure(D2D1_FIGURE_END /*figureEnd*/)
{
    float2 front =  m_figureVertices.front().pt;
    float2 back =  m_figureVertices.back().pt;

    if (front.x == back.x && front.y == back.y)
    {
        m_figureVertices.pop_back();
    }

    // If we only have one vertex, then there is nothing to draw!
    if (m_figureVertices.size() > 1)
    {
        //
        // We construct the triangles corresponding to the sides of
        // the extruded object in 3 steps:
        //

        //
        // Step 1:
        //
        // Snap vertices and calculate normals.
        //
        //
        // Note: it is important that we compute normals *before*
        // snapping the vertices, otherwise, the normals will become
        // discretized, which will manifest itself as faceting.
        //
        for (UINT i = 0; i < m_figureVertices.size(); ++i)
        {
            m_figureVertices[i].norm = GetNormal(i);
            m_figureVertices[i].pt = PointSnapper::SnapPoint(m_figureVertices[i].pt);
        }

        //
        // Step 2:
        //
        // Interpolate normals as appropriate.
        //
        for (UINT i = 0; i < m_figureVertices.size(); ++i)
        {
            UINT h = (i+m_figureVertices.size()-1)%m_figureVertices.size();

            float2 n1 =  m_figureVertices[h].norm;
            float2 n2 =  m_figureVertices[i].norm;

            //
            // Take a dot-product to determine if the angle between
            // the normals is small. If it is, then average them so we
            // get a smooth transition from one face to the next.
            //
            if ((n1.x * n2.x + n1.y * n2.y) > 0.5f)
            {
                float2 sum = m_figureVertices[i].norm + m_figureVertices[h].norm;

                m_figureVertices[i].interpNorm1 = m_figureVertices[i].interpNorm2 = Normalize(sum);
            }
            else
            {
                m_figureVertices[i].interpNorm1 = m_figureVertices[h].norm;
                m_figureVertices[i].interpNorm2 = m_figureVertices[i].norm;
            }
        }

        //
        // Step 3:
        //
        // Output the triangles.
        //

        // interpNorm1 == end normal of previous segment
        // interpNorm2 == begin normal of next segment
        for (UINT i = 0; i < m_figureVertices.size(); ++i)
        {
            UINT j = (i+1) % m_figureVertices.size();

            float2 pt =  m_figureVertices[i].pt;
            float2 nextPt = m_figureVertices[j].pt;

            float2 ptNorm3 =  m_figureVertices[i].interpNorm2;
            float2 nextPtNorm2 =  m_figureVertices[j].interpNorm1;

            //
            // These 6 vertices define two adjacent triangles that
            // together form a quad.
            //
            SimpleVertex newVertices[6] =
            {
                {float3(pt.x, pt.y, m_height/2), float3(ptNorm3.x, ptNorm3.y, 0.0f)},
                {float3(pt.x, pt.y, -m_height/2), float3(ptNorm3.x, ptNorm3.y, 0.0f)},
                {float3(nextPt.x, nextPt.y, -m_height/2), float3(nextPtNorm2.x, nextPtNorm2.y, 0.0f)},
                {float3(nextPt.x, nextPt.y, -m_height/2), float3(nextPtNorm2.x, nextPtNorm2.y, 0.0f)},
                {float3(nextPt.x, nextPt.y, m_height/2), float3(nextPtNorm2.x, nextPtNorm2.y, 0.0f)},
                {float3(pt.x, pt.y, m_height/2), float3(ptNorm3.x, ptNorm3.y, 0.0f)},
            };

            for (UINT n = 0; n < ARRAYSIZE(newVertices); ++n)
            {
                m_vertices->push_back(newVertices[n]);
            }
        }
    }
}

void Extruder::ExtrudingSink::SetFillMode(D2D1_FILL_MODE /*fillMode*/)
{
    // Do nothing
}

void Extruder::ExtrudingSink::SetSegmentFlags(D2D1_PATH_SEGMENT /*vertexFlags*/)
{
    // Do nothing
}

void Extruder::ExtrudingSink::AddTriangles(const D2D1_TRIANGLE* triangles, UINT trianglesCount)
{
    //
    // These triangles reprent the front and back faces of the extrusion.
    //
    for (UINT i = 0; i < trianglesCount; ++i)
    {
        D2D1_TRIANGLE tri = triangles[i];

        D2D1_POINT_2F d1 = {tri.point2.x - tri.point1.x, tri.point2.y - tri.point1.y};
        D2D1_POINT_2F d2 = {tri.point3.x - tri.point2.x, tri.point3.y - tri.point2.y};

        tri.point1 = PointSnapper::SnapPoint(tri.point1);
        tri.point2 = PointSnapper::SnapPoint(tri.point2);
        tri.point3 = PointSnapper::SnapPoint(tri.point3);

        //
        // Currently, Tessellate does not guarantee the orientation
        // of the triangles it produces, so we must check here.
        //

        float cross = d1.x * d2.y - d1.y*d2.x;
        if (cross < 0)
        {
            D2D1_POINT_2F tmp = tri.point1;

            tri.point1 = tri.point2;
            tri.point2 = tmp;
        }

        SimpleVertex newVertices[] =
        {
            {float3(tri.point1.x, tri.point1.y, m_height/2), float3(0.0f, 0.0f, 1.0f)},
            {float3(tri.point2.x, tri.point2.y, m_height/2), float3(0.0f, 0.0f, 1.0f)},
            {float3(tri.point3.x, tri.point3.y, m_height/2), float3(0.0f, 0.0f, 1.0f)},

            //
            // Note: these points are listed in a different order since the orientation of the back
            // face should be the opposite of the front face.
            //
            {float3(tri.point2.x, tri.point2.y, -m_height/2), float3(0.0f, 0.0f, -1.0f)},
            {float3(tri.point1.x, tri.point1.y, -m_height/2), float3(0.0f, 0.0f, -1.0f)},
            {float3(tri.point3.x, tri.point3.y, -m_height/2), float3(0.0f, 0.0f, -1.0f)},
        };

        for (UINT i = 0; i < ARRAYSIZE(newVertices); ++i)
        {
            m_vertices->push_back(newVertices[i]);
        }
    }
}

float2 Extruder::ExtrudingSink::GetNormal(uint32 i)
{
    uint32 j = (i+1) % m_figureVertices.size();

    float2 pti = m_figureVertices[i].pt;
    float2 ptj = m_figureVertices[j].pt;
    float2 vecij = ptj - pti;

    return Normalize(float2(vecij.y, vecij.x));
}

float2 Extruder::ExtrudingSink::Normalize(float2 pt)
{
    return pt / sqrtf(pt.x*pt.x+pt.y*pt.y);
}

//
// IUnknown methods
//
// These use a basic, non-thread-safe implementation of the
// standard reference-counting logic.
//
HRESULT Extruder::ExtrudingSink::QueryInterface(REFIID riid, _Outptr_ void** object)
{
    *object = nullptr;
    return E_NOTIMPL;
}

ULONG Extruder::ExtrudingSink::AddRef()
{
    m_ref++;
    return m_ref;
}

ULONG Extruder::ExtrudingSink::Release()
{
    m_ref--;

    if (m_ref == 0)
    {
        delete this;
        return 0;
    }

    return m_ref;
}
