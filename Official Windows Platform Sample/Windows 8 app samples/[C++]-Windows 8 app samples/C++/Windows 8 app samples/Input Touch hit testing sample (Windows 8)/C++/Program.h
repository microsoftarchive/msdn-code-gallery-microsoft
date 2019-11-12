//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <new>
#include <memory>
#include "Shape.h"

// Shapes list
struct ShapeRecord
{
    std::shared_ptr<Shape> shape;      // Internal shape object
    int cursorId;                      // Cursor (mouse or pointer) identifier that 'holds' the object
    std::shared_ptr<ShapeRecord> next; // Next object
};

ref class Program : public Platform::Object
{
internal:
    Program();

    // Initializes Core Objects, Manipulation Processors and Inertia Processors
    void Initialize(_In_ Windows::UI::Core::CoreWindow^ window);

    // Event handlers
    void OnPointerPressed(_In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerMoved(_In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerReleased(_In_ Windows::UI::Core::PointerEventArgs^ args);

    void RenderObjects();
    void SetD2DContext(_In_ ID2D1DeviceContext* d2dContext);
    void CreateWindowSizeDependentResources();

private:
    ~Program();
    // Touch hit testing events
    void OnTouchHitTesting(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::TouchHitTestingEventArgs^ args);

    void SetupControls();

    // List of shapes (includes regular shapes and controls)
    // List contains shapes in their z-order
    std::shared_ptr<ShapeRecord> m_shapes;

    // Add new shape to the list
    void AddShape(_In_reads_(numPoints) Windows::Foundation::Point const* points, _In_ int numPoints,
                  _In_ D2D1::ColorF color);
    // Move shape to the top of the list
    void MoveToFront(_Inout_ std::shared_ptr<ShapeRecord>& shapeRec);
    // Drawing
    void DrawBackward(_In_ std::shared_ptr<ShapeRecord> const& shapeRec);

    // Main window
    Platform::Agile<Windows::UI::Core::CoreWindow> m_window;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> m_d2dContext;

    // The client width and height
    float m_width;
    float m_height;

    // Hit test method
    std::shared_ptr<ShapeRecord> PointHitTest(_In_ Windows::Foundation::Point pt);
};
