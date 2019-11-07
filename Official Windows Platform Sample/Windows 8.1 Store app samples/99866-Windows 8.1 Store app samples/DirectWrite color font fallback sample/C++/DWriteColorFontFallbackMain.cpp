//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteColorFontFallbackMain.h"

#include <DirectXColors.h>
#include "DirectXHelper.h"

using namespace DWriteColorFontFallback;

// Loads and initializes application assets when the application is loaded.
DWriteColorFontFallbackMain::DWriteColorFontFallbackMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_fontFallbackId(SampleConstants::FontFallbackEmoji),
    m_colorGlyphs(true),
    m_zoom(1.0f),
    m_viewPosition()
{
    // Register to be notified if the device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    m_sceneRenderer = std::unique_ptr<DWriteColorFontFallbackRenderer>(new DWriteColorFontFallbackRenderer(m_deviceResources));
}

DWriteColorFontFallbackMain::~DWriteColorFontFallbackMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void DWriteColorFontFallbackMain::UpdateForWindowSizeChange()
{
    m_sceneRenderer->CreateWindowSizeDependentResources();
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool DWriteColorFontFallbackMain::Render()
{
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
    m_sceneRenderer->Render(m_fontFallbackId, m_colorGlyphs, m_zoom, m_viewPosition);

    return true;
}

// Notifies renderers that device resources need to be released.
void DWriteColorFontFallbackMain::OnDeviceLost()
{
    m_sceneRenderer->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void DWriteColorFontFallbackMain::OnDeviceRestored()
{
    m_sceneRenderer->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}
