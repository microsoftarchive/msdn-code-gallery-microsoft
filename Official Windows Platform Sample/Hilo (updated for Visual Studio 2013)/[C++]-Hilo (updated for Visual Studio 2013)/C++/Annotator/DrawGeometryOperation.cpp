//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "DrawGeometryOperation.h"
#include "direct2dutility.h"

DrawGeometryOperation::DrawGeometryOperation() : m_strokeSize(1)
{
}

DrawGeometryOperation::~DrawGeometryOperation()
{
}

HRESULT DrawGeometryOperation::DrawToRenderTarget(__in ID2D1RenderTarget* renderTarget, D2D1_RECT_F /*imageRect*/)
{
    ComPtr<ID2D1Factory> factory;
    renderTarget->GetFactory(&factory);

    HRESULT hr = S_OK;
    if (!m_strokeStyle)
    {
        hr = factory->CreateStrokeStyle(
            D2D1::StrokeStyleProperties(
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_LINE_JOIN_ROUND),
                nullptr, 0, &m_strokeStyle);
    }

    if (!m_brush && SUCCEEDED(hr))
    {
        hr = renderTarget->CreateSolidColorBrush(m_brushColor, &m_brush);
    }

    if (SUCCEEDED(hr))
    {
        renderTarget->DrawGeometry(m_geometry, m_brush, m_strokeSize, m_strokeStyle);
    }

    return hr;
}

HRESULT DrawGeometryOperation::AppendPoint(__in ID2D1RenderTarget* renderTarget, __in D2D1_POINT_2F point)
{
    m_points.push_back(point);

    return UpdateGeometry(renderTarget);
}

HRESULT DrawGeometryOperation::SetBrushColor(__in D2D1_COLOR_F brushColor)
{
    m_brush = nullptr;
    m_brushColor = brushColor;

    return S_OK;
}

HRESULT DrawGeometryOperation::SetStrokeSize(__in float strokeSize)
{
    m_strokeSize = strokeSize;
    return S_OK;
}

HRESULT DrawGeometryOperation::UpdateGeometry(ID2D1RenderTarget* renderTarget)
{
    // Use the render target's factory
    ComPtr<ID2D1Factory> factory;
    renderTarget->GetFactory(&factory);

    ComPtr<ID2D1PathGeometry> pathGeometry;
    HRESULT hr = factory->CreatePathGeometry(&pathGeometry);

    ComPtr<ID2D1GeometrySink> sink;
    if (SUCCEEDED(hr))
    {
        hr = pathGeometry->Open(&sink);
    }

    if (SUCCEEDED(hr))
    {
        sink->BeginFigure(m_points[0], D2D1_FIGURE_BEGIN_HOLLOW);

        for (size_t i = 1; i < m_points.size(); i++)
        {
            // Smoothing
            if (i > 1 && i < m_points.size() - 1)
            {
                D2D1_POINT_2F point1;
                D2D1_POINT_2F point2;
                GetSmoothingPoints(static_cast<int>(i), &point1, &point2);
                sink->AddBezier(
                    D2D1::BezierSegment(
                        point1,
                        point2,
                        m_points[i]));
            }
            else
            {
                sink->AddLine(m_points[i]);
            }
        }
        sink->EndFigure(D2D1_FIGURE_END_OPEN);
    }

    if (SUCCEEDED(hr))
    {
        hr = sink->Close();
    }
    
    if (SUCCEEDED(hr))
    {
        m_geometry = pathGeometry;
    }

    return hr;
}

void DrawGeometryOperation::GetSmoothingPoints(int i, D2D1_POINT_2F* point1, D2D1_POINT_2F* point2)
{
    static const float smoothing = 0.25f; // 0 = no smoothing

    // Calculate distance between previous point and current point
    float dx = m_points[i].x - m_points[i - 1].x;
    float dy = m_points[i].y - m_points[i - 1].y;
    float currentDelta = std::sqrt(dx * dx + dy * dy); 

    // Calculate distance between two points back and current point
    float prevDx = m_points[i].x - m_points[i - 2].x;
    float prevDy = m_points[i].y - m_points[i - 2].y;    
    float prevDelta = std::sqrt(prevDx * prevDx + prevDy * prevDy);

    // Calculate distance between previous point and the next point
    float nextDx = m_points[i + 1].x - m_points[i - 1].x;
    float nextDy = m_points[i + 1].y - m_points[i - 1].y;    
    float nextDelta = std::sqrt(nextDx * nextDx + nextDy * nextDy); 

    // Store point 1
    *point1 = D2D1::Point2F(
        m_points[i - 1].x + (prevDx == 0 ? 0 : (smoothing * currentDelta * prevDx / prevDelta)),
        m_points[i - 1].y + (prevDy == 0 ? 0 : (smoothing * currentDelta * prevDy / prevDelta))
        );

    // Store point 2
    *point2 = D2D1::Point2F(
        m_points[i].x - (nextDx == 0 ? 0 : (smoothing * currentDelta * nextDx / nextDelta)),
        m_points[i].y - (nextDy == 0 ? 0 : (smoothing * currentDelta * nextDy / nextDelta))
        );
}
