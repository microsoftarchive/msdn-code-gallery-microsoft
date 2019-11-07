//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXMapDefaultBuffersMain.h"

#include <DirectXColors.h>
#include "DirectXHelper.h"

using namespace DirectXMapDefaultBuffers;

// Loads and initializes application assets when the application is loaded.
DirectXMapDefaultBuffersMain::DirectXMapDefaultBuffersMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources)
{
    // Register to be notified if the device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    m_sampleOverlay = std::shared_ptr<SampleOverlay>(new SampleOverlay(m_deviceResources, L"DirectX Mappable Default Buffers Sample"));
    m_sceneRenderer = std::unique_ptr<DirectXMapDefaultBuffersRenderer>(new DirectXMapDefaultBuffersRenderer(m_deviceResources, m_sampleOverlay));
}

DirectXMapDefaultBuffersMain::~DirectXMapDefaultBuffersMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void DirectXMapDefaultBuffersMain::UpdateForWindowSizeChange()
{
    m_sampleOverlay->CreateWindowSizeDependentResources();
}

// Updates the application state once per frame.
void DirectXMapDefaultBuffersMain::Update()
{
    // Update scene objects.
    m_timer.Tick([&]()
    {
        m_sceneRenderer->Update(m_timer);
    });
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool DirectXMapDefaultBuffersMain::Render()
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
    m_sampleOverlay->Render();

    return true;
}

// Notifies renderers that device resources need to be released.
void DirectXMapDefaultBuffersMain::OnDeviceLost()
{
    m_sceneRenderer->ReleaseDeviceDependentResources();
    m_sampleOverlay->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void DirectXMapDefaultBuffersMain::OnDeviceRestored()
{
    m_sceneRenderer->CreateDeviceDependentResources();
    m_sampleOverlay->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}
