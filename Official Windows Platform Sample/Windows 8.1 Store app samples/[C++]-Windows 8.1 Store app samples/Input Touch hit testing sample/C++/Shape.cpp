//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXSample.h"
#include <math.h>
#include "Shape.h"

// Shape Constructor
Shape::Shape() :
    m_color(D2D1::ColorF::Blue)
{
}

// Shape Descructor
Shape::~Shape()
{
}

// Initialize shape object
void Shape::InitializeObject(_In_reads_(numPoints) Windows::Foundation::Point const* points, _In_ int numPoints,
                             _In_ D2D1::ColorF color)
{
    m_polygonRelative.numPoints = numPoints;
    m_color = color;

    if (numPoints == 2)
    {
        m_polygonRelative.isRect = true;
        m_polygonRelative.rcBound.X = points[0].X;
        m_polygonRelative.rcBound.Y = points[0].Y;
        m_polygonRelative.rcBound.Width = points[1].X - points[0].X;
        m_polygonRelative.rcBound.Height = points[1].Y - points[0].Y;

        // Initialize points
        m_polygonRelative.points[0] = points[0];
        m_polygonRelative.points[1].X = points[1].X;
        m_polygonRelative.points[1].Y = points[0].Y;
        m_polygonRelative.points[2] = points[1];
        m_polygonRelative.points[3].X = points[0].X;
        m_polygonRelative.points[3].Y = points[1].Y;
        m_polygonRelative.points[4] = points[0];
        m_polygonRelative.numPoints = 5;
    }
    else
    {
        m_polygonRelative.isRect = false;

        // calculate width and height
        if (numPoints > 0)
        {
            m_polygonRelative.points[0] = points[0];

            // Copy polygon points to m_polygonRelative object and calculate polygon bounding box
            float left, right, top, bottom;
            left = right = points[0].X;
            top = bottom = points[0].Y;
            for (int i = 1; i < numPoints; i++)
            {
                m_polygonRelative.points[i] = points[i];
                left = min(left, points[i].X);
                top = min(top, points[i].Y);
                right = max(right, points[i].X);
                bottom = max(bottom, points[i].Y);
            }
            m_polygonRelative.rcBound.X = left;
            m_polygonRelative.rcBound.Y = top;
            m_polygonRelative.rcBound.Width = right - left;
            m_polygonRelative.rcBound.Height = bottom - top;
        }
    }
    m_polygonAbsolute = m_polygonRelative;
}

// Sets the default position, dimensions and color for the drawing object
void Shape::ResetState(_In_ float startX, _In_ float startY,
                       _In_ float width, _In_ float height,
                       _In_ float scale)
{
    // Set width and height of the client area
    // must adjust for dpi aware
    m_width = width;
    m_height = height;

    // Set coordinates used for rendering
    m_topLeftX = startX;
    m_topLeftY = startY;

    // scaling factor
    m_scale = scale;
}

// Draw shape object
void Shape::Paint(_In_ ID2D1DeviceContext* d2dDeviceContext)
{
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> brush;
    Microsoft::WRL::ComPtr<ID2D1Factory> d2dFactory;
    DX::ThrowIfFailed(d2dDeviceContext->CreateSolidColorBrush(D2D1::ColorF(m_color), brush.GetAddressOf()));
    d2dDeviceContext->GetFactory(d2dFactory.GetAddressOf());

    // Create path to draw
    m_pathGeometry = nullptr;
    DX::ThrowIfFailed(d2dFactory->CreatePathGeometry(&m_pathGeometry));

    Microsoft::WRL::ComPtr<ID2D1GeometrySink> sink;
    DX::ThrowIfFailed(m_pathGeometry->Open(&sink));

    sink->BeginFigure(
        UnscaledToClient(m_polygonRelative.points[0]),
        D2D1_FIGURE_BEGIN_FILLED);

    // Draw polygon
    for (int i = 1; i < m_polygonRelative.numPoints; i++)
    {
        sink->AddLine(UnscaledToClient(m_polygonRelative.points[i]));
    }

    sink->EndFigure(D2D1_FIGURE_END_CLOSED);
    sink->Close();

    // Fill the geometry that was created
    d2dDeviceContext->FillGeometry(m_pathGeometry.Get(), brush.Get());

    // Create absolute polygon
    for (int i = 0; i < m_polygonRelative.numPoints; i++)
    {
        UnscaledToClient(m_polygonRelative.points[i], &m_polygonAbsolute.points[i]);
    }
    UnscaledToClient(m_polygonRelative.rcBound, &m_polygonAbsolute.rcBound);
}

// Hit testing method handled with Direct2D
bool Shape::InRegion(Windows::Foundation::Point pt)
{
    BOOL b = FALSE;
    // The adjusted point returned from TouchHitTesting isn't quite equal to
    // the point passed to the corresponding PointerPressed, due to conversion
    // between DIPs and screen pixels. To compensate for this difference, it's
    // necessary to increase the tolerance passed to FillContainsPoint(). 1.5F
    // works for this sample, but some testing is needed to determine the best
    // value for other scenarios.
    m_pathGeometry->FillContainsPoint(
        D2D1::Point2F(pt.X, pt.Y),
        D2D1::Matrix3x2F::Identity(),
        1.5F,
        &b
    );

    return !!b;
}

// Pointer event handler
void Shape::OnPointerEvent(_In_ Windows::Foundation::Point pt, bool shouldMove)
{
    if (shouldMove)
    {
        Translate(pt.X - m_ptPointerPosition.X, pt.Y - m_ptPointerPosition.Y);
    }

    m_ptPointerPosition = pt;
}

// Shape polygonal representation
Polygon* Shape::GetPolygon()
{
    return &m_polygonAbsolute;
}

// Update shape position to ensure that it stays within client screen
void Shape::EnsureVisible()
{
    // Initialize width height of object
    float width  = m_polygonRelative.rcBound.Width  * m_scale;
    float height = m_polygonRelative.rcBound.Height * m_scale;

    float x = m_topLeftX + (m_polygonRelative.rcBound.Left * m_scale);
    x = max(0, min(x, m_width - width));
    m_topLeftX = x - (m_polygonRelative.rcBound.Left * m_scale);

    float y = m_topLeftY + (m_polygonRelative.rcBound.Top * m_scale);
    y = max(0, min(y, m_height - height));
    m_topLeftY = y - (m_polygonRelative.rcBound.Top * m_scale);
}

// _Translate shape within client screen
void Shape::Translate(_In_ float dx, _In_ float dy)
{
    m_topLeftX += dx;
    m_topLeftY += dy;

    // Make sure object stays on screen
    EnsureVisible();
}

// Convert unscaled point to client coordinates
void Shape::UnscaledToClient(_In_ Windows::Foundation::Point ptUnscaled, _Inout_ Windows::Foundation::Point* pptClient)
{
    pptClient->X = m_topLeftX + (ptUnscaled.X * m_scale);
    pptClient->Y = m_topLeftY + (ptUnscaled.Y * m_scale);
}

// Convert unscaled point to client coordinates
void Shape::UnscaledToClient(_In_ Windows::Foundation::Rect rcUnscaled, _Inout_ Windows::Foundation::Rect* prcClient)
{
    prcClient->X = m_topLeftX + (rcUnscaled.X * m_scale);
    prcClient->Y = m_topLeftY + (rcUnscaled.Y * m_scale);
    prcClient->Width  = rcUnscaled.Width      * m_scale;
    prcClient->Height = rcUnscaled.Height     * m_scale;
}

// Convert unscaled point to client coordinates
D2D1_POINT_2F Shape::UnscaledToClient(_In_ Windows::Foundation::Point ptUnscaled)
{
    return D2D1::Point2F(m_topLeftX + ptUnscaled.X * m_scale, m_topLeftY + ptUnscaled.Y * m_scale);
}

// Convert unscaled rectangle to client coordinates
D2D1_RECT_F Shape::UnscaledToClient(_In_ Windows::Foundation::Rect rcUnscaled)
{
    return D2D1::RectF(
        m_topLeftX + rcUnscaled.Left   * m_scale,
        m_topLeftY + rcUnscaled.Top    * m_scale,
        m_topLeftX + rcUnscaled.Right  * m_scale,
        m_topLeftY + rcUnscaled.Bottom * m_scale
    );
}
