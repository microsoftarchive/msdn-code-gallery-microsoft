//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DGeometryRealizationsRenderer.h"

#include "DirectXHelper.h"

using namespace D2DGeometryRealizations;

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

// Constants and default values to define the appearance of the scene.
static const float GridMargin = 150.0f;
static const float StrokeWidth = 1.0f;
static const int DefaultNumberOfRows = 8;
static const int MinNumberOfRows = 4;
static const int MaxNumberOfRows = 64;
static const bool DefaultRealizationsEnabled = true;

// Defines the re-realization threshold. In conjuction with the CheckScaleWindow
// method, this constant defines the change in scale after which the geometry
// realization objects will be recreated. For example, a value of 2 causes the app
// to recreate the realization objects each time the user zooms in or out by an
// additional factor of 2. Larger values make the app recreate the realizations
// less often but cause rendering to be more expensive. Smaller values result in
// less expensive rendering at the cost of recreating the realizations more often.
// Your choice of value depends on the specific needs of your app.
static const float RerealizationThreshold = 2.0f;

// Initializes the renderer-specific resources for this app.
D2DGeometryRealizationsRenderer::D2DGeometryRealizationsRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_numberOfRows(DefaultNumberOfRows),
    m_worldMatrix(D2D1::Matrix3x2F::Identity()),
    m_angle(0),
    m_realizationsEnabled(DefaultRealizationsEnabled),
    m_lastScale(1.0f)
{
    // Create and store the hourglass path geometry.

    ComPtr<ID2D1PathGeometry> pathGeometry;
    ComPtr<ID2D1GeometrySink> sink;

    DX::ThrowIfFailed(
        m_deviceResources->GetD2DFactory()->CreatePathGeometry(&pathGeometry)
        );

    DX::ThrowIfFailed(
        pathGeometry->Open(&sink)
        );

    sink->SetFillMode(D2D1_FILL_MODE_ALTERNATE);

    sink->BeginFigure(
        D2D1::Point2F(-50.0f, -50.0f),
        D2D1_FIGURE_BEGIN_FILLED
        );

    sink->AddLine(D2D1::Point2F(50.0f, -50.0f));

    sink->AddBezier(
        D2D1::BezierSegment(
            D2D1::Point2F(25.0f, -25.0f),
            D2D1::Point2F(25.0f, 25.0f),
            D2D1::Point2F(50.0f, 50.0f)
            )
        );

    sink->AddLine(D2D1::Point2F(-50.0f, 50.0f));

    sink->AddBezier(
        D2D1::BezierSegment(
            D2D1::Point2F(-25.0f, 25.0f),
            D2D1::Point2F(-25.0f, -25.0f),
            D2D1::Point2F(-50.0f, -50.0f)
            )
        );

    sink->EndFigure(D2D1_FIGURE_END_CLOSED);

    DX::ThrowIfFailed(
        sink->Close()
        );

    m_geometry = pathGeometry;

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

void D2DGeometryRealizationsRenderer::CreateDeviceDependentResources()
{
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Blue),
            &m_fillBrush
            )
        );

    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_strokeBrush
            )
        );
}

// Called when the window size changes or when the system DPI changes. Because the
// system DPI can affect the visual quality of the geometry realization objects, the
// app re-creates them any time the system DPI changes, to ensure they are created
// based on the current DPI. This prevents scaling artifacts from appearing after a
// change in DPI.
void D2DGeometryRealizationsRenderer::CreateWindowSizeDependentResources()
{
    // Compute the scaling factor applied by the current world transform.
    float newScale = D2D1ComputeMaximumScaleFactor(&m_worldMatrix);

    // Recenter the scale window to the new scale.
    m_lastScale = newScale;

    // Recreate the geometry realization objects. The CreateScaleDependentResources
    // method will compute the recommended flattening tolerance based on the current
    // DPI and the current world transform.
    CreateScaleDependentResources();
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void D2DGeometryRealizationsRenderer::ReleaseDeviceDependentResources()
{
    m_filledGeometryRealization.Reset();
    m_strokedGeometryRealization.Reset();
    m_fillBrush.Reset();
    m_strokeBrush.Reset();
}

// Creates geometry realization objects for the fill and stroke of the hourglass geometry.
// This method must be called any time the scene is scaled up or down by more than the
// RerealizationThreshold constant.
void D2DGeometryRealizationsRenderer::CreateScaleDependentResources()
{
    ID2D1DeviceContext1* deviceContext = m_deviceResources->GetD2DDeviceContext();

    // Get the DPI (pixel density) of the render target. On higher-DPI screens, geometry
    // realizations must be created at lower tolerances to give the same visual quality.
    float dpiX, dpiY;
    deviceContext->GetDpi(&dpiX, &dpiY);

    // Compute the recommended flattening tolerance for the current world transform and
    // DPI. Although other transforms are used when the realizations are rendered, only
    // the world transform applies a scale, so only it is required for this calculation.
    // By passing RerealizationThreshold as the maxZoomFactor parameter, the app
    // receives a stricter tolerance than is necessary for the current zoom level. This
    // stricter tolerance allows the resulting realizations to be scaled up by an
    // additional factor of RerealizationThreshold without visible degradation of quality.
    float flatteningTolerance = D2D1::ComputeFlatteningTolerance(m_worldMatrix, dpiX, dpiY, RerealizationThreshold);

    // Create a realization of the filled interior of the path geometry, using the
    // recommended flattening tolerance.
    DX::ThrowIfFailed(
        deviceContext->CreateFilledGeometryRealization(
            m_geometry.Get(),
            flatteningTolerance,
            &m_filledGeometryRealization
            )
        );

    // Create a realization of the stroke (outline) of the path geometry, using the
    // recommended flattening tolerance.
    DX::ThrowIfFailed(
        deviceContext->CreateStrokedGeometryRealization(
            m_geometry.Get(),
            flatteningTolerance,
            StrokeWidth,
            nullptr,
            &m_strokedGeometryRealization
            )
        );
}

// Called once per frame.
void D2DGeometryRealizationsRenderer::Update(DX::StepTimer const& timer)
{
    // Update the base angle that defines how each rendered shape is rotated.
    m_angle = (m_angle + 1) % 360;
}

// Renders one frame.
void D2DGeometryRealizationsRenderer::Render()
{
    ComPtr<ID2D1DeviceContext1> deviceContext = m_deviceResources->GetD2DDeviceContext();

    deviceContext->BeginDraw();
    deviceContext->Clear();
    
    // Retrieve the 2D transform that accounts for device orientation.
    D2D1::Matrix3x2F orientationTransform = m_deviceResources->GetOrientationTransform2D();

    // Retrieve the size of the window in DIPs.
    Size size = m_deviceResources->GetLogicalSize();

    // Compute the arrangement of the grid of shapes.
    float gridWidth = size.Width - (2 * GridMargin);
    float gridHeight = size.Height - (2 * GridMargin);
    float cellWidth = gridWidth / (m_numberOfRows - 1.0f);
    float cellHeight = gridHeight / (m_numberOfRows - 1.0f);

    for (int row = 0; row < m_numberOfRows; row++)
    {
        for (int col = 0; col < m_numberOfRows; col++)
        {
            // For each cell in the grid, define a transform that rotates the shape
            // and translates it to the appropriate location in the scene.
            D2D1::Matrix3x2F cellTransform =
                D2D1::Matrix3x2F::Rotation(static_cast<float>((m_angle + row * 5 + col * 5))) *
                D2D1::Matrix3x2F::Translation(col * cellWidth + GridMargin, row * cellHeight + GridMargin);

            // Apply the per-cell transform on top of the world transform and the orientation transform.
            deviceContext->SetTransform(cellTransform * m_worldMatrix * orientationTransform);

            // For each cell, define a different color for the fill of the shape.
            float r = static_cast<float>(row) / m_numberOfRows;
            float g = static_cast<float>(col) / m_numberOfRows;
            float b = 1.0f;

            m_fillBrush->SetColor(D2D1::ColorF(r, g, b));

            if (m_realizationsEnabled)
            {
                // If the user has enabled geometry realizations, draw the realization
                // objects (one call for the realized fill, one call for the realized
                // stroke).
                deviceContext->DrawGeometryRealization(
                    m_filledGeometryRealization.Get(),
                    m_fillBrush.Get()
                    );

                deviceContext->DrawGeometryRealization(
                    m_strokedGeometryRealization.Get(),
                    m_strokeBrush.Get()
                    );
            }
            else
            {
                // If the user has disabled geometry realizations, simply draw
                // the geometry object itself (one call for the fill, one call
                // for the stroke).
                deviceContext->FillGeometry(
                    m_geometry.Get(),
                    m_fillBrush.Get()
                    );

                deviceContext->DrawGeometry(
                    m_geometry.Get(),
                    m_strokeBrush.Get(),
                    StrokeWidth
                    );
            }
        }
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = deviceContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

// Called when the user updates the view position or zoom level by interacting with the UI.
// The main object computes a new world transform matrix and sends it to the scene renderer.
void D2DGeometryRealizationsRenderer::SetWorldMatrix(D2D1::Matrix3x2F worldMatrix)
{
    m_worldMatrix = worldMatrix;

    if (m_realizationsEnabled)
    {
        // In case the user has zoomed in or out beyond the re-realization threshold, the
        // app must check whether it needs to recreate the geometry realizations.
        CheckScaleWindow();
    }
}

// Called whenever the view of the scene has changed. This method determines whether the
// geometry realizations must be recreated, based on the scale at which they were last
// created (m_lastScale) and the new scale (newScale) computed from m_worldMatrix.
//
// This method works by defining a "window" of acceptable scaling factors. The window is
// always centered on m_lastScale, and its width is defined by the RerealizationThreshold
// constant.
//
// Any time the new scale falls outside that window, the window is recentered on the new
// scale and the geometry is re-realized at a new flattening tolerance computed from the
// world transform. This may happen when zooming in or when zooming out.
void D2DGeometryRealizationsRenderer::CheckScaleWindow()
{
    // Compute the scaling factor applied by the world transform.
    float newScale = D2D1ComputeMaximumScaleFactor(&m_worldMatrix);

    // Compute the upper and lower bounds of the scale window defined by the last scale
    // and the RerealizationThreshold factor.
    float scaleLowerBound = m_lastScale / RerealizationThreshold;
    float scaleUpperBound = m_lastScale * RerealizationThreshold;

    // If the new scale falls outside the window, then either the user has zoomed in too
    // far (so the app should re-realize to preserve quality) or the user has zoomed out
    // too far (so the app should re-realize to preserve performance).
    if (newScale < scaleLowerBound || newScale > scaleUpperBound)
    {
        // Recenter the scale window.
        m_lastScale = newScale;

        // Recreate the geometry realization objects. The CreateScaleDependentResources
        // method will compute the recommended flattening tolerance based on the current
        // DPI and the current world transform.
        CreateScaleDependentResources();
    }
}

void D2DGeometryRealizationsRenderer::IncreasePrimitives()
{
    m_numberOfRows = min(m_numberOfRows + 8, MaxNumberOfRows);
}

void D2DGeometryRealizationsRenderer::DecreasePrimitives()
{
    m_numberOfRows = max(m_numberOfRows - 8, MinNumberOfRows);
}

bool D2DGeometryRealizationsRenderer::GetRealizationsEnabled()
{
    return m_realizationsEnabled;
}

void D2DGeometryRealizationsRenderer::SetRealizationsEnabled(bool enabled)
{
    m_realizationsEnabled = enabled;

    // Because m_lastScale is not updated while realizations are disabled, the app must
    // update it when realizations are enabled, in case ther user performed any zooming
    // while realizations were disabled.
    if (m_realizationsEnabled)
    {
        CheckScaleWindow();
    }
}

void D2DGeometryRealizationsRenderer::RestoreDefaults()
{
    m_numberOfRows = DefaultNumberOfRows;
    SetRealizationsEnabled(DefaultRealizationsEnabled);
}

// Saves the current state of the app for suspend and terminate events.
void D2DGeometryRealizationsRenderer::SaveInternalState(IPropertySet^ state)
{
    // Check to ensure each key is not already in the collection. If it is present, remove
    // it, before storing in the new value.

    if (state->HasKey("NumberOfRows"))
    {
        state->Remove("NumberOfRows");
    }
    state->Insert("NumberOfRows", PropertyValue::CreateInt32(m_numberOfRows));

    if (state->HasKey("RealizationsEnabled"))
    {
        state->Remove("RealizationsEnabled");
    }
    state->Insert("RealizationsEnabled", PropertyValue::CreateBoolean(m_realizationsEnabled));
}

// Loads the current state of the app for resume events.
void D2DGeometryRealizationsRenderer::LoadInternalState(IPropertySet^ state)
{
    if (state->HasKey("NumberOfRows"))
    {
        int numRows = safe_cast<IPropertyValue^>(state->Lookup("NumberOfRows"))->GetInt32();
        m_numberOfRows = numRows;
    }

    if (state->HasKey("RealizationsEnabled"))
    {
        bool enabled = safe_cast<IPropertyValue^>(state->Lookup("RealizationsEnabled"))->GetBoolean();
        SetRealizationsEnabled(enabled);
    }
}
