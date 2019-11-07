//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

//
// Definitions
//
int const MAX_POLY_VERTICES = 50;  // Maximal number of points in polygonal shape
int const CURSOR_ID_NONE = -1;     // Identifier of no input

// Polygonal shape structure
struct Polygon
{
    bool isRect;   // Is the polygon rectangle
    int numPoints; // Number of vertices in polgyon
    Windows::Foundation::Point points[MAX_POLY_VERTICES]; // Polygon vertices
    Windows::Foundation::Rect rcBound;                    // Polygon bounding box
};

// Shape class represents a windowless control that are touch interactable.
//
// Shape has following properties:
//     1) It can be control or regular object
//     2) It can be draggable with pointer
//     3) It has polygonal shape
//
// Shape can be scaled and translated on the screen.
class Shape
{
public:
    Shape();
    ~Shape();

    // Initialize shape
    void InitializeObject(_In_reads_(numPoints) Windows::Foundation::Point const* points, _In_ int numPoints,
                          _In_ D2D1::ColorF color);

    // Reset shape to it's start position
    void ResetState(_In_ float startX, _In_ float startY,
                    _In_ float width, _In_ float height,
                    _In_ float scale);

    // Draw the shape
    void Paint(_In_ ID2D1DeviceContext* d2dDeviceContext);

    // Hit test point - returns true if point is within shape
    bool InRegion(Windows::Foundation::Point pt);

    // Respond to input event
    void OnPointerEvent(_In_ Windows::Foundation::Point pt, bool shouldMove);

    // Properties
    Polygon* GetPolygon();

private:
    // Rendering
    Microsoft::WRL::ComPtr<ID2D1PathGeometry> m_pathGeometry;

    // Rendered top, left coordinates of object
    float m_topLeftX;
    float m_topLeftY;

    // Client (parent window) width and height
    float m_width;
    float m_height;

    // How much shape is scaled
    float m_scale;

    // Object shape in relative coordinates (before scaling and translation)
    Polygon m_polygonRelative;
    // Object shape in absolute coordinates (scaling and translation applied)
    Polygon m_polygonAbsolute;

    // Object colors
    D2D1::ColorF m_color;

    // Pixel location where last pointer event occurred
    Windows::Foundation::Point m_ptPointerPosition;

    // Move the object on the screen
    void EnsureVisible();
    void Translate(_In_ float fdx, _In_ float fdy);

    // Helper functions to deal with scaled object
    void UnscaledToClient(_In_ Windows::Foundation::Point ptUnscaled, _Inout_ Windows::Foundation::Point* pptClient);
    void UnscaledToClient(_In_ Windows::Foundation::Rect rcUnscaled, _Inout_ Windows::Foundation::Rect* prcClient);
    D2D1_POINT_2F UnscaledToClient(_In_ Windows::Foundation::Point ptUnscaled);
    D2D1_RECT_F UnscaledToClient(_In_ Windows::Foundation::Rect rcUnscaled);
};
