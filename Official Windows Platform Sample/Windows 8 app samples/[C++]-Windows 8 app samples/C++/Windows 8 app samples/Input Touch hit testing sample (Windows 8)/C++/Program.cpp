//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include <stdio.h>
#include "Program.h"

using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::Devices::Input;
using namespace Windows::System;
using namespace Windows::Foundation;

//
// Definitions
//
int const CONTROLS_AREA_SIZE = 300; // Relative size of area where regular shapes are initially drawn

Program::Program():
    m_shapes(nullptr),
    m_width(-1),
    m_height(-1)
{
}

Program::~Program()
{
}

// Add new shape to the top of the list
void Program::AddShape(_In_reads_(numPoints) Windows::Foundation::Point const* points, _In_ int numPoints,
                       _In_ D2D1::ColorF color)
{
    // Create new record for this shape
    std::shared_ptr<ShapeRecord> shapeRecord = std::shared_ptr<ShapeRecord>(new ShapeRecord());
    shapeRecord->shape = std::shared_ptr<Shape>(new Shape());
    shapeRecord->shape->InitializeObject(points, numPoints, color);
    shapeRecord->next = nullptr;
    shapeRecord->cursorId = CURSOR_ID_NONE;

    if (m_shapes == nullptr)
    {
        // The first entry
        m_shapes = shapeRecord;
    }
    else
    {
        // Add to top
        shapeRecord->next = m_shapes;
        m_shapes = shapeRecord;
    }
}

// Move selected shape to the top
void Program::MoveToFront(_Inout_ std::shared_ptr<ShapeRecord>& shapeRec)
{
    // Find previos shape
    std::shared_ptr<ShapeRecord> shapeRecPrev = m_shapes;
    while (shapeRecPrev != nullptr && (shapeRecPrev->next != shapeRec))
    {
        shapeRecPrev = shapeRecPrev->next;
    }

    if (shapeRecPrev != nullptr)
    {
        // It's not on top yet, move it to front
        shapeRecPrev->next = shapeRec->next;
        shapeRec->next = m_shapes;
        m_shapes = shapeRec;
    }
}

// Initialize and place all controls on the page
void Program::SetupControls()
{
    Windows::Foundation::Point poly[MAX_POLY_VERTICES];

    // Drawing parameters
    // Triangle side
    float triangleSide = CONTROLS_AREA_SIZE;
    // Triangle height
    float triangleHeight = triangleSide * 87 / 100;

    // 1st control
    // rectangle
    poly[0].X = triangleSide   / 3;
    poly[0].Y = triangleHeight * 2 / 3;
    poly[1].X = triangleSide   * 2 / 3;
    poly[1].Y = triangleHeight;
    AddShape(poly, 2, D2D1::ColorF::Blue);

    // 2nd control
    // Triangle
    poly[0].X = triangleSide   / 3;
    poly[0].Y = triangleHeight * 2 / 3;
    poly[1].X = triangleSide   * 2 / 3;
    poly[1].Y = triangleHeight * 2 / 3;
    poly[2].X = triangleSide   / 2;
    poly[2].Y = triangleHeight / 3;
    AddShape(poly, 3, D2D1::ColorF::Orange);
}

// Initialize page
void Program::Initialize(_In_ Windows::UI::Core::CoreWindow^ window)
{
    m_window = window;

    // Enable Touch Hit Test
    window->TouchHitTesting +=
        ref new TypedEventHandler<CoreWindow^, TouchHitTestingEventArgs^>(this, &Program::OnTouchHitTesting);

    // Create and initialize core objects
    SetupControls();
}

// Draw shapes in backward order.
// First draw shape with the lowest z-order and so on.
// The last drawn shape has the highest z-order.
void Program::DrawBackward(_In_ std::shared_ptr<ShapeRecord> const& shapeRec)
{
    if (shapeRec != nullptr)
    {
        DrawBackward(shapeRec->next);
        shapeRec->shape->Paint(m_d2dContext.Get());
    }
}

// Render whole page.
void Program::RenderObjects()
{
    DrawBackward(m_shapes);
}

void Program::CreateWindowSizeDependentResources()
{
    m_width = m_window->Bounds.Width;
    m_height = m_window->Bounds.Height;

    // Scaling factor
    float minWH = m_width > m_height ? m_height : m_width;
    float scale = minWH / (2 * CONTROLS_AREA_SIZE);

    // Origin of the objects
    Windows::Foundation::Point orig;

    orig.X = (m_width  - CONTROLS_AREA_SIZE * scale) / 2.0F;
    orig.Y = (m_height - CONTROLS_AREA_SIZE * scale) / 2.0F;

    // Assign the setup defined above to each of the core objects
    std::shared_ptr<ShapeRecord> shapeRec = m_shapes;
    while (shapeRec != nullptr)
    {
        shapeRec->shape->ResetState(orig.X, orig.Y, m_width, m_height, scale);
        shapeRec = shapeRec->next;
    }
}

void Program::SetD2DContext(_In_ ID2D1DeviceContext*  d2dContext)
{
    m_d2dContext = d2dContext;
}

// Return shape that is hit with given point.
std::shared_ptr<ShapeRecord> Program::PointHitTest(_In_ Windows::Foundation::Point pt)
{
    std::shared_ptr<ShapeRecord> shapeRec = m_shapes;
    while (shapeRec != nullptr)
    {
        if (shapeRec->shape->InRegion(pt))
        {
            return shapeRec;
        }
        shapeRec = shapeRec->next;
    }

    return nullptr;
}

// On pointer down event select target shape (associate shape with cursor)
// and move shape to the front.
void Program::OnPointerPressed(_In_ Windows::UI::Core::PointerEventArgs^ args)
{
    std::shared_ptr<ShapeRecord> shapeRec = PointHitTest(args->CurrentPoint->Position);
    if (shapeRec != nullptr)
    {
        shapeRec->cursorId = args->CurrentPoint->PointerId;
        shapeRec->shape->OnPointerEvent(args->CurrentPoint->Position, false);

        MoveToFront(shapeRec);
    }
}

// On pointer move event, translate the shape that is associated with
// current cursor (if any).
void Program::OnPointerMoved(_In_ Windows::UI::Core::PointerEventArgs^ args)
{
    // find object held with this cursor
    std::shared_ptr<ShapeRecord> shapeRec = m_shapes;
    while (shapeRec != nullptr && (shapeRec->cursorId != args->CurrentPoint->PointerId))
    {
        shapeRec = shapeRec->next;
    }

    // move object
    if (shapeRec != nullptr)
    {
        // move the object
        shapeRec->shape->OnPointerEvent(args->CurrentPoint->Position, true);
    }
}

// On pointer up event, release the associated shape (if any).
void Program::OnPointerReleased(_In_ Windows::UI::Core::PointerEventArgs^ args)
{
    // find object held with this cursor
    std::shared_ptr<ShapeRecord> shapeRec = m_shapes;
    while (shapeRec != nullptr && (shapeRec->cursorId != args->CurrentPoint->PointerId))
    {
        shapeRec = shapeRec->next;
    }

    if (shapeRec != nullptr)
    {
        shapeRec->shape->OnPointerEvent(args->CurrentPoint->Position, true);

        // Release cursor from shape
        shapeRec->cursorId = CURSOR_ID_NONE;
    }
}

// Intelligent Touch Hit Test handler - sample implementation of the Touch Hit Test
// Algorithm.
void Program::OnTouchHitTesting(_In_ Windows::UI::Core::CoreWindow^ /*sender*/,
                                _In_ Windows::UI::Core::TouchHitTestingEventArgs^ touchHitTestingArgs)
{
    // Current and best Touch Hit Test results
    Windows::UI::Core::CoreProximityEvaluation bestResult;
    Windows::UI::Core::CoreProximityEvaluation currentResult;

    // Initialize best result from input arguments.
    // In theory there may be more than one listener to this event.
    bestResult = touchHitTestingArgs->ProximityEvaluation;

    // Best Touch Hit Test target
    std::shared_ptr<Shape> bestTarget = nullptr;

    bestResult.AdjustedPoint = touchHitTestingArgs->Point;

    // Iterate through all shapes by their z-order (starting from shape with highest z-order)
    // Stop early if there's no chance to find better target.
    std::shared_ptr<ShapeRecord> shapeRec = m_shapes;
    while (shapeRec != nullptr && bestResult.Score > static_cast<int>(Windows::UI::Core::CoreProximityEvaluationScore::Closest))
    {
        Polygon* poly = shapeRec->shape->GetPolygon();

        // Hit test current shape
        if (poly->isRect)
        {
            currentResult = touchHitTestingArgs->EvaluateProximity(poly->rcBound);
        }
        else
        {
            // Create correctly sized array of Points
            auto controlVertices = ref new Platform::Array<Windows::Foundation::Point>(poly->points, poly->numPoints);
            currentResult = touchHitTestingArgs->EvaluateProximity(controlVertices);
        }

        // Does the shape intersect touch contact and is it the best target so far?
        if (currentResult.Score < bestResult.Score)
        {
            // Even if this shape has better score than the score of the shape with higher z-order,
            // don't select this shape if previous adjusted point belongs to this shape.
            // In other words, if this shape overlaps shape with higher z-order, favor the shape with
            // higher z-order.
            if (bestTarget == nullptr || !shapeRec->shape->InRegion(currentResult.AdjustedPoint))
            {
                // Finally, don't select target if adjusted touch point is not on the control.
                // That may happen if another shape with higher z-order overlaps this shape at adjusted touch
                // point.
                if (PointHitTest(currentResult.AdjustedPoint) == shapeRec)
                {
                    // This is the best touch target so far.
                    bestTarget = shapeRec->shape;
                    bestResult = currentResult;
                }
            }
        }

        // Evaluate next shape.
        shapeRec = shapeRec->next;
    }

    // Set best target
    touchHitTestingArgs->ProximityEvaluation = bestResult;

    // Event is handled
    touchHitTestingArgs->Handled = true;
}
