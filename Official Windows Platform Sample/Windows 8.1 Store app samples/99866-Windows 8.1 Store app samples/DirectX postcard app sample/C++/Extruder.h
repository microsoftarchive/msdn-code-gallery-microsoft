//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "PointSnapper.h"

// The vertex class taken from the Win7 sample and used by Extrude.
struct SimpleVertex
{
    float3 Pos;
    float3 Norm;
};

class Extruder
{
public:
    static void ExtrudeGeometry(
        ID2D1Geometry* pGeometry,
        float height,
        std::vector<SimpleVertex>* vertices
        );

private:
    static void D2DFlatten(ID2D1Geometry* pGeometry, float flatteningTolerance, ID2D1Geometry** ppGeometry);
    static void D2DOutline(ID2D1Geometry* pGeometry, ID2D1Geometry** ppGeometry);

    class ExtrudingSink : public ID2D1SimplifiedGeometrySink, public ID2D1TessellationSink
    {
    public:
        static void CreateExtrudingSink(
            float height,
            _In_ std::vector<SimpleVertex>* pVertices,
            _Outptr_ ExtrudingSink** ppExtrudingSink
            );

        // ID2D1SimplifiedGeometrySink and ID2D1TessellationSink methods.
        STDMETHOD_(void, AddBeziers)(const D2D1_BEZIER_SEGMENT* /*beziers*/, UINT /*beziersCount*/);
        STDMETHOD_(void, AddLines)(const D2D1_POINT_2F* points, UINT pointsCount);
        STDMETHOD_(void, BeginFigure)(D2D1_POINT_2F startPoint, D2D1_FIGURE_BEGIN /*figureBegin*/);
        STDMETHOD(Close)();
        STDMETHOD_(void, EndFigure)(D2D1_FIGURE_END /*figureEnd*/);
        STDMETHOD_(void, SetFillMode)(D2D1_FILL_MODE /*fillMode*/);
        STDMETHOD_(void, SetSegmentFlags)(D2D1_PATH_SEGMENT /*vertexFlags*/);
        STDMETHOD_(void, AddTriangles)(const D2D1_TRIANGLE* triangles, UINT trianglesCount);

        // IUknown methods.
        STDMETHOD(QueryInterface)(REFIID riid, _Outptr_ void** object) override;
        STDMETHOD_(ULONG, AddRef)() override;
        STDMETHOD_(ULONG, Release)() override;

    private:
        // Private constructor used by CreateExtrudingSink method.
        ExtrudingSink(float height, _In_ std::vector<SimpleVertex>* pVertices);

        float2 GetNormal(uint32 i);
        float2 Normalize(float2 pt);

        struct Vertex2D
        {
            float2 pt;
            float2 norm;
            float2 interpNorm1;
            float2 interpNorm2;
        };

        float m_height;
        D2D1_POINT_2F m_lastPoint;
        D2D1_POINT_2F m_startPoint;

        std::vector<SimpleVertex>* m_vertices;
        std::vector<Vertex2D> m_figureVertices;

        ULONG m_ref;    // Reference count for AddRef and Release.
    };
};