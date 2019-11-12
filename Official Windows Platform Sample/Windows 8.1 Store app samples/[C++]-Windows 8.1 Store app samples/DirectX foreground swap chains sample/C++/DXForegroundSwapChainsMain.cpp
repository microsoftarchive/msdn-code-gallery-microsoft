//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DXForegroundSwapChainsMain.h"

#include <DirectXColors.h>
#include "DirectXHelper.h"

using namespace DXForegroundSwapChains;

// Loads and initializes application assets when the application is loaded.
DXForegroundSwapChainsMain::DXForegroundSwapChainsMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources)
{
    // Register to be notified if the device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    m_sceneRenderer = std::unique_ptr<DXForegroundSwapChainsRenderer>(new DXForegroundSwapChainsRenderer(m_deviceResources));
    m_sampleOverlay = std::unique_ptr<SampleOverlay>(new SampleOverlay(m_deviceResources, L"DirectX Foreground Swap Chains"));

    // To demonstrate scaling behavior, set initial rendering scale to 75% of the window size.
    m_deviceResources->SetRenderSize(0.75f);
}

DXForegroundSwapChainsMain::~DXForegroundSwapChainsMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void DXForegroundSwapChainsMain::UpdateForWindowSizeChange()
{
    m_sceneRenderer->CreateWindowSizeDependentResources();
    m_sampleOverlay->CreateWindowSizeDependentResources();
}

// Updates the application state once per frame.
void DXForegroundSwapChainsMain::Update()
{
    // Update scene objects.
    m_timer.Tick([&]()
    {
        m_sceneRenderer->Update(m_timer);
    });
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool DXForegroundSwapChainsMain::Render()
{
    // Don't try to render anything before the first Update.
    if (m_timer.GetFrameCount() == 0)
    {
        return false;
    }

    auto context = m_deviceResources->GetD3DDeviceContext();

    // Reset render targets to the screen.
    ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
    context->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());

    // Clear the back buffer and depth stencil view.
    context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), DirectX::Colors::MidnightBlue);

    if (m_deviceResources->GetForegroundRenderTargetView())
    {
        context->ClearRenderTargetView(m_deviceResources->GetForegroundRenderTargetView(), DirectX::Colors::Transparent);
    }

    context->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);

    // If overlays are not available, the 2D resources will be drawn to the (potentially) scaled
    // swap chain. Apply a D2D transform to account for the current size of the swap chain to
    // ensure the resources are drawn proportionally.
    if (!m_deviceResources->GetOverlaySupportExists())
    {
        m_deviceResources->GetD2DDeviceContext()->SetTransform(
            m_deviceResources->GetRenderSizeTransform() *
            m_deviceResources->GetOrientationTransform2D()
            );
    }
    else
    {
        m_deviceResources->GetD2DDeviceContext()->SetTransform(
            m_deviceResources->GetOrientationTransform2D()
            );
    }

    // Render the scene objects.
    m_sceneRenderer->Render();

    // Render overlay text. If overlays are available, this will be rendered onto the foreground swap chain since
    // that will have been set as the Direct2D Target in DeviceResources::CreateWindowSizeDependentResources.
    m_sampleOverlay->Render();

    return true;
}

// Notifies renderers that device resources need to be released.
void DXForegroundSwapChainsMain::OnDeviceLost()
{
    m_sceneRenderer->ReleaseDeviceDependentResources();
    m_sampleOverlay->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void DXForegroundSwapChainsMain::OnDeviceRestored()
{
    m_sceneRenderer->CreateDeviceDependentResources();
    m_sampleOverlay->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}
