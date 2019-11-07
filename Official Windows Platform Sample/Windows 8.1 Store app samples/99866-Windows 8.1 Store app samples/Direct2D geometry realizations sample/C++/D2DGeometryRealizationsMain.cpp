//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DGeometryRealizationsMain.h"

#include <DirectXColors.h>
#include "DirectXHelper.h"

using namespace D2DGeometryRealizations;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

static const Point DefaultViewPosition = Point(0.0f, 0.0f);
static const float DefaultZoom = 1.0f;
static const float MinZoom = 0.001f;
static const float MaxZoom = 1000.0f;

// Loads and initializes application assets when the application is loaded.
D2DGeometryRealizationsMain::D2DGeometryRealizationsMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_viewPosition(DefaultViewPosition),
    m_zoom(DefaultZoom)
{
    // Register to be notified if the device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    m_sceneRenderer = std::unique_ptr<D2DGeometryRealizationsRenderer>(new D2DGeometryRealizationsRenderer(m_deviceResources));

    // Set the timer to update the scene at 60Hz.
    m_timer.SetFixedTimeStep(true);
    m_timer.SetTargetElapsedSeconds(1.0 / 60);
}

D2DGeometryRealizationsMain::~D2DGeometryRealizationsMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void D2DGeometryRealizationsMain::UpdateForWindowSizeChange()
{
    m_sceneRenderer->CreateWindowSizeDependentResources();
}

// Updates the application state once per frame.
void D2DGeometryRealizationsMain::Update()
{
    // Update scene objects.
    m_timer.Tick([&]()
    {
        m_sceneRenderer->Update(m_timer);
    });
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool D2DGeometryRealizationsMain::Render()
{
    // Don't try to render anything before the first Update.
    if (m_timer.GetFrameCount() == 0)
    {
        return false;
    }

    auto context = m_deviceResources->GetD3DDeviceContext();

    // Reset the viewport to target the whole screen.
    auto viewport = m_deviceResources->GetScreenViewport();
    context->RSSetViewports(1, &viewport);

    // Reset render targets to the screen.
    ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
    context->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());

    // Clear the back buffer and depth stencil view.
    context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), DirectX::Colors::MidnightBlue);
    context->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);

    // Render the scene objects.
    m_sceneRenderer->Render();

    return true;
}

// Notifies renderers that device resources need to be released.
void D2DGeometryRealizationsMain::OnDeviceLost()
{
    m_sceneRenderer->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void D2DGeometryRealizationsMain::OnDeviceRestored()
{
    m_sceneRenderer->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}

// Used to update the framerate counter in the UI.
unsigned int D2DGeometryRealizationsMain::GetFPS()
{
    return m_timer.GetFramesPerSecond();
}

void D2DGeometryRealizationsMain::UpdateZoom(Point position, Point positionDelta, float zoomDelta)
{
    // In this method, we update the transformation variables to reflect how the user is manipulating the scene.

    // Reposition the view to reflect translations.
    m_viewPosition.X += positionDelta.X;
    m_viewPosition.Y += positionDelta.Y;

    // We want to have any zoom operation be "centered" around the pointer position, which
    // requires recalculating the view position based on the new zoom and pointer position.

    // Step 1: Calculate the absolute pointer position (image position).
    D2D1_POINT_2F pointerAbsolutePosition = D2D1::Point2F(
        (m_viewPosition.X - position.X) / m_zoom,
        (m_viewPosition.Y - position.Y) / m_zoom
        );

    // Step 2: Apply the zoom operation and clamp the zoom to minimum and maximum scales.
    // Note that zoomDelta is a coefficient for the change in zoom.
    m_zoom *= zoomDelta;
    m_zoom = max(MinZoom, min(MaxZoom, m_zoom));

    // Step 3: Adjust the view position based on the new m_zoom value.
    m_viewPosition.X = pointerAbsolutePosition.x * m_zoom + position.X;
    m_viewPosition.Y = pointerAbsolutePosition.y * m_zoom + position.Y;

    // Finally, compute the new world matrix and send it to the scene renderer.
    UpdateWorldMatrix();
}

void D2DGeometryRealizationsMain::IncreasePrimitives()
{
    m_sceneRenderer->IncreasePrimitives();
}

void D2DGeometryRealizationsMain::DecreasePrimitives()
{
    m_sceneRenderer->DecreasePrimitives();
}

bool D2DGeometryRealizationsMain::GetRealizationsEnabled()
{
    return m_sceneRenderer->GetRealizationsEnabled();
}

void D2DGeometryRealizationsMain::SetRealizationsEnabled(bool enabled)
{
    m_sceneRenderer->SetRealizationsEnabled(enabled);
}

void D2DGeometryRealizationsMain::RestoreDefaults()
{
    SetViewPosition(DefaultViewPosition);
    SetZoom(DefaultZoom);

    // Delegate further restore operations to the scene renderer.
    m_sceneRenderer->RestoreDefaults();
}

// Saves the current state of the app for suspend and terminate events.
void D2DGeometryRealizationsMain::SaveInternalState(IPropertySet^ state)
{
    // Check to ensure each key is not already in the collection. If it is present, remove
    // it, before storing in the new value.

    if (state->HasKey("ViewPosition"))
    {
        state->Remove("ViewPosition");
    }
    state->Insert("ViewPosition", PropertyValue::CreatePoint(m_viewPosition));

    if (state->HasKey("Zoom"))
    {
        state->Remove("Zoom");
    }
    state->Insert("Zoom", PropertyValue::CreateSingle(m_zoom));

    // Delegate further save operations to the scene renderer.
    m_sceneRenderer->SaveInternalState(state);
}

// Loads the current state of the app for resume events.
void D2DGeometryRealizationsMain::LoadInternalState(IPropertySet^ state)
{
    if (state->HasKey("ViewPosition"))
    {
        Point viewPosition = safe_cast<IPropertyValue^>(state->Lookup("ViewPosition"))->GetPoint();
        SetViewPosition(viewPosition);
    }

    if (state->HasKey("Zoom"))
    {
        float zoom = safe_cast<IPropertyValue^>(state->Lookup("Zoom"))->GetSingle();
        SetZoom(zoom);
    }

    // Delegate further load operations to the scene renderer.
    m_sceneRenderer->LoadInternalState(state);
}

void D2DGeometryRealizationsMain::SetViewPosition(Point viewPosition)
{
    m_viewPosition = viewPosition;

    // Compute the new world matrix and send it to the scene renderer.
    UpdateWorldMatrix();
}

void D2DGeometryRealizationsMain::SetZoom(float zoom)
{
    m_zoom = zoom;

    // Compute the new world matrix and send it to the scene renderer.
    UpdateWorldMatrix();
}

void D2DGeometryRealizationsMain::UpdateWorldMatrix()
{
    // The world matrix is simply a translation followed by a scale.
    D2D1::Matrix3x2F worldMatrix =
        D2D1::Matrix3x2F::Translation(m_viewPosition.X / m_zoom, m_viewPosition.Y / m_zoom) *
        D2D1::Matrix3x2F::Scale(m_zoom, m_zoom);

    m_sceneRenderer->SetWorldMatrix(worldMatrix);
}
