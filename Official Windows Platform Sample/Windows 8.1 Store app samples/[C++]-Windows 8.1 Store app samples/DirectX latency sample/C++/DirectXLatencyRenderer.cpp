//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXLatencyRenderer.h"

#include "DirectXHelper.h"

using namespace DirectXLatency;

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

// Initialization.
DirectXLatencyRenderer::DirectXLatencyRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_currentPosition(),
    m_ellipse()
{
    CreateDeviceDependentResources();

    // Start the circle in the middle of the screen.
    D2D1_SIZE_F size = m_deviceResources->GetD2DDeviceContext()->GetSize();
    SetCirclePosition(Point(size.width/2, size.height/2));
}

// Initialization.
void DirectXLatencyRenderer::CreateDeviceDependentResources()
{
    // Create a brush with which to draw the circle.
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Red),
            &m_brush
            )
        );
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DirectXLatencyRenderer::ReleaseDeviceDependentResources()
{
    m_brush.Reset();
}

// Updates any state in the renderer based on UI events that have occurred.
void DirectXLatencyRenderer::Update(DX::StepTimer const& timer)
{
    // Create a circle at the last given position.
    m_ellipse = D2D1::Ellipse(
        D2D1::Point2F(m_currentPosition.X, m_currentPosition.Y),
        50.0f,
        50.0f
        );
}

// Renders one frame.
void DirectXLatencyRenderer::Render()
{
    ComPtr<ID2D1DeviceContext1> deviceContext = m_deviceResources->GetD2DDeviceContext();

    deviceContext->BeginDraw();

    // Rotate the rendered scene based on the current orientation of the device.
    deviceContext->SetTransform(m_deviceResources->GetOrientationTransform2D());

    // Draw the circle at its current position.
    deviceContext->FillEllipse(
        m_ellipse,
        m_brush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = deviceContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

// Update the position of the circle based on UI input.
void DirectXLatencyRenderer::SetCirclePosition(Point newPosition)
{
    m_currentPosition = newPosition;
}

// Saves the current state of the app for suspend and terminate events.
void DirectXLatencyRenderer::SaveInternalState(IPropertySet^ state)
{
    // Check to ensure each key is not already in the collection. If it is present, remove
    // it, before storing in the new value.
    if (state->HasKey("CirclePosition"))
    {
        state->Remove("CirclePosition");
    }
    state->Insert("CirclePosition", PropertyValue::CreatePoint(m_currentPosition));
}

// Loads the current state of the app for resume events.
void DirectXLatencyRenderer::LoadInternalState(IPropertySet^ state)
{
    if (state->HasKey("CirclePosition"))
    {
        Point circlePosition = safe_cast<IPropertyValue^>(state->Lookup("CirclePosition"))->GetPoint();
        SetCirclePosition(circlePosition);
    }
}
