//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"

class PointSnapper
{
public:
    static void SnapGeometry(ID2D1Geometry* pGeometry, ID2D1Geometry** ppGeometry);
    static float SnapCoordinate(float x);
    static D2D1_POINT_2F SnapPoint(D2D1_POINT_2F pt);
    static float2 SnapPoint(float2 pt);

private:
    class PointSnappingSink : public ID2D1SimplifiedGeometrySink
    {
    public:
        static void CreatePointSnappingSink(
            _In_ ID2D1SimplifiedGeometrySink* pSink,
            _Outptr_ PointSnappingSink** ppPointSnappingSink
            );

        // ID2D1SimplifiedGeometrySink methods.
        STDMETHOD_(void, AddBeziers)(const D2D1_BEZIER_SEGMENT* /*beziers*/, UINT /*beziersCount*/);
        STDMETHOD_(void, AddLines)(const D2D1_POINT_2F* points, UINT pointsCount);
        STDMETHOD_(void, BeginFigure)(D2D1_POINT_2F startPoint, D2D1_FIGURE_BEGIN figureBegin);
        STDMETHOD_(void, EndFigure)(D2D1_FIGURE_END figureEnd);
        STDMETHOD_(void, SetFillMode)(D2D1_FILL_MODE fillMode);
        STDMETHOD_(void, SetSegmentFlags)(D2D1_PATH_SEGMENT vertexFlags);
        STDMETHOD(Close)();

        // IUknown methods.
        STDMETHOD(QueryInterface)(REFIID riid, _Outptr_ void** object) override;
        STDMETHOD_(ULONG, AddRef)() override;
        STDMETHOD_(ULONG, Release)() override;

    private:
        // Private constructor used by CreatePointSnappingSink method.
        PointSnappingSink(_In_ ID2D1SimplifiedGeometrySink* pSink);

        ID2D1SimplifiedGeometrySink* m_pSinkNoRef;      // The underlying ID2D1SimplifiedGeometrySink.
        ULONG m_ref;                                    // Reference count for AddRef and Release.
    };
};