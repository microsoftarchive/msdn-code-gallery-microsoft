//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "InkRenderer.h"

using namespace Microsoft::WRL;
using namespace Windows::UI::Input;
using namespace Windows::UI::Input::Inking;
using namespace Windows::UI::Xaml::Input;

InkRenderer::InkRenderer() :
    m_inkState(InkState::StrokeCompleted),
    m_pointerId(0)
{
    m_inkManager = ref new InkManager();

    auto drawingAttributes = ref new InkDrawingAttributes();
    drawingAttributes->FitToCurve = true;
    drawingAttributes->Color = Windows::UI::Colors::Black;     // Opaque strokes.
    m_inkManager->SetDefaultDrawingAttributes(drawingAttributes);

    m_inkManager->Mode = InkManipulationMode::Inking;
}

void InkRenderer::CreateDeviceIndependentResources(ComPtr<ID2D1Factory1> d2dFactory)
{
    // Save shared factory.
    m_d2dFactory = d2dFactory;
}

void InkRenderer::CreateDeviceResources(ComPtr<ID2D1DeviceContext> d2dContext)
{
    // Save shared device context.
    m_d2dContext = d2dContext;

    // Create a solid black brush to render the ink.
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black), &m_blackBrush)
        );
}

void InkRenderer::CreateWindowSizeDependentResources(float dpi)
{
    // Save shared DPI.
    m_dpi = dpi;
}

void InkRenderer::DrawInk()
{
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    // Render the strokes stored in the stroke geometry.
    if (!m_strokes.empty())
    {
        for (ComPtr<ID2D1PathGeometry> stroke : m_strokes)
        {
            m_d2dContext->DrawGeometry(
                stroke.Get(),
                m_blackBrush.Get(),
                5.0f    // Stroke width.
                );
        }
    }

    // Render the current stroke.
    if (!m_currentStroke.empty())
    {
        auto previousPoint = m_currentStroke.at(0);
        D2D1_POINT_2F currentPoint;
        for (auto i = m_currentStroke.begin() + 1; i != m_currentStroke.end(); i++)
        {
            currentPoint = *i;
            m_d2dContext->DrawLine(
                previousPoint,
                currentPoint,
                m_blackBrush.Get(),
                5.0f
                );
            previousPoint = currentPoint;
        }
    }
}

void InkRenderer::StartSignature()
{
    m_inkState = InkState::StrokeCompleted;
}

void InkRenderer::Reset()
{
    DeleteAllStrokes();
    m_inkState = InkState::StrokeCompleted;
}

void InkRenderer::OnPointerPressed(_In_ PointerRoutedEventArgs^ args)
{
    if (m_inkState != InkState::StrokeCompleted)
    {
        return;
    }

    m_inkState = InkState::StrokeDrawing;

    PointerPoint^ point = args->GetCurrentPoint(nullptr);
    m_pointerId = point->PointerId;

    m_inkManager->ProcessPointerDown(point);

    m_currentStroke.clear();
    m_currentStroke.push_back(D2D1::Point2F(point->Position.X, point->Position.Y));
}

void InkRenderer::OnPointerReleased(_In_ PointerRoutedEventArgs^ args)
{
    PointerPoint^ point = args->GetCurrentPoint(nullptr);

    if (m_inkState != InkState::StrokeDrawing  || m_pointerId != point->PointerId)
    {
        return;
    }

    m_inkState = InkState::StrokeCompleted;

    auto rect = m_inkManager->ProcessPointerUp(point);

    m_currentStroke.clear();

    // Convert the last stroke in the ink manager to a path geometry for fast rendering.
    auto strokeView = m_inkManager->GetStrokes();
    auto strokeViewSize = strokeView->Size;
    if (strokeViewSize > 0)
    {
        auto lastStroke = strokeView->GetAt(strokeViewSize-1);
        ComPtr<ID2D1PathGeometry> strokeGeometry;
        ConvertStrokeToGeometry(lastStroke, &strokeGeometry);
        if (strokeGeometry != nullptr)
        {
            m_strokes.push_back(strokeGeometry);
        }
    }
}

void InkRenderer::OnPointerMoved(_In_ PointerRoutedEventArgs^ args)
{
    PointerPoint^ point = args->GetCurrentPoint(nullptr);

    if (m_inkState == InkState::StrokeDrawing && m_pointerId == point->PointerId)
    {
        m_inkManager->ProcessPointerUpdate(point);
        m_currentStroke.push_back(D2D1::Point2F(point->Position.X, point->Position.Y));
    }
}

void InkRenderer::DeleteAllStrokes()
{
    auto strokes = m_inkManager->GetStrokes();
    for (auto stroke : strokes)
    {
        stroke->Selected = true;
    }
    m_inkManager->DeleteSelected();
    m_strokes.clear();
    m_currentStroke.clear();
}

// Create a new path geometry and open a geometry sink.
void InkRenderer::ConvertStrokeToGeometry(
    _In_ IInkStroke^ stroke,
    _Outptr_result_maybenull_ ID2D1PathGeometry** geometry
    )
{
    *geometry = nullptr;

    auto segments = stroke->GetRenderingSegments();
    auto segmentSize = segments->Size;

    if (segmentSize == 0)
    {
        return;
    }

    DX::ThrowIfFailed(
        m_d2dFactory->CreatePathGeometry(geometry)
        );

    ComPtr<ID2D1GeometrySink> sink;
    DX::ThrowIfFailed(
        (*geometry)->Open(&sink)
        );

    sink->SetSegmentFlags(D2D1_PATH_SEGMENT_FORCE_ROUND_LINE_JOIN);
    sink->SetFillMode(D2D1_FILL_MODE_ALTERNATE);

    // Convert points and Bezier control points in each segment to a Bezier curve in the path geometry.
    auto segment = segments->GetAt(0);
    auto point = segment->Position;
    sink->BeginFigure(
        D2D1::Point2F(point.X, point.Y),
        D2D1_FIGURE_BEGIN_FILLED
        );
    for (uint32 j = 1; j < segmentSize; j++)
    {
        segment = segments->GetAt(j);
        point = segments->GetAt(j)->Position;
        sink->AddBezier(
            D2D1::BezierSegment(
                D2D1::Point2F(segment->BezierControlPoint1.X, segment->BezierControlPoint1.Y),
                D2D1::Point2F(segment->BezierControlPoint2.X, segment->BezierControlPoint2.Y),
                D2D1::Point2F(point.X, point.Y)
                )
            );
    }
    sink->EndFigure(D2D1_FIGURE_END_OPEN);

    DX::ThrowIfFailed(
        sink->Close()
        );
}
