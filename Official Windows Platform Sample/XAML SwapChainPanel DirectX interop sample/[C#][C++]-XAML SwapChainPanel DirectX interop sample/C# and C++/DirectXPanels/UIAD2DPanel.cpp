//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
#include "pch.h"
#include "UIAD2DPanel.h"
#include "DirectXHelper.h"

#include <windows.ui.xaml.media.dxinterop.h>

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Automation;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Interop;
using namespace DirectX;
using namespace D2D1;
using namespace DirectXPanels;

#pragma region Panel and Panel Item peers
UIAD2DPanelItemPeer::UIAD2DPanelItemPeer(UIAD2DPanel^ owner) :
    m_owner(owner)
{
}

Windows::Foundation::Rect UIAD2DPanelItemPeer::GetBoundingRectangleCore()
{
    return m_owner->ContentGlobalBoundingRect;
}

Windows::Foundation::Point UIAD2DPanelItemPeer::GetClickablePointCore()
{
    Windows::Foundation::Rect bounds = m_owner->ContentGlobalBoundingRect;
    return Windows::Foundation::Point(
        bounds.X + bounds.Width / 2,
        bounds.Y + bounds.Height / 2);
}

UIAD2DPanelPeer::UIAD2DPanelPeer(UIAD2DPanel^ owner) :
    FrameworkElementAutomationPeer(owner)
{
    // Create a peer for the custom DirectX child element. Unlike FrameworkElementAutomationPeers for XAML elements derived
    // from FrameworkElement, AutomationPeers for DirectX content must be manually created and stored.
    m_childPeer = ref new UIAD2DPanelItemPeer(owner);
}

AutomationPeer^ UIAD2DPanelPeer::GetPeerFromPointCore(Windows::Foundation::Point point)
{
    // Find the correct XAML element peer.
    AutomationPeer^ peer = FrameworkElementAutomationPeer::GetPeerFromPointCore(point);

    // If the XAML peer found was the SwapChainPanel's, then look for the topmost element in its DirectX content.
    if (peer == this)
    {
        if (m_childPeer->GetBoundingRectangle().Contains(point))
        {
            peer = m_childPeer;
        }
    }

    return peer;
}

IVector<AutomationPeer^>^ UIAD2DPanelPeer::GetChildrenCore()
{
    // Retrieve existing XAML child element peers.
    IVector<AutomationPeer^>^ children = FrameworkElementAutomationPeer::GetChildrenCore();

    // Add DirectX element peer.
    children->Append(m_childPeer);

    // Return combined list of XAML and DirectX content peers.
    return children;
}

#pragma endregion

UIAD2DPanel::UIAD2DPanel() :
    m_contentRect(D2D1::RectF(100, 100, 300, 300))
{
    m_backgroundColor = ColorF(ColorF::AliceBlue);
    
    CreateDeviceIndependentResources();
    CreateDeviceResources();
    CreateSizeDependentResources();
}

void UIAD2DPanel::Render()
{
    if (!m_loadingComplete)
    {
        return;
    }

    m_d2dContext->BeginDraw();
    m_d2dContext->Clear(m_backgroundColor);

    // Draw a content rectangle which will be exposed via UIA.
    m_d2dContext->FillRectangle(m_contentRect, m_fillBrush.Get());
    m_d2dContext->DrawRectangle(m_contentRect, m_strokeBrush.Get());

    m_d2dContext->EndDraw();
    
    Present();
}

void UIAD2DPanel::CreateDeviceResources()
{
    DirectXPanelBase::CreateDeviceResources();

    m_d2dContext->CreateSolidColorBrush(ColorF(ColorF::Black), &m_strokeBrush);
    m_d2dContext->CreateSolidColorBrush(ColorF(ColorF::Green), &m_fillBrush);

    m_loadingComplete = true;
}

void UIAD2DPanel::OnDeviceLost()
{
    // Handle device lost, then re-render.
    DirectXPanelBase::OnDeviceLost();
    Render();
}

void UIAD2DPanel::OnSizeChanged(Platform::Object^ sender, SizeChangedEventArgs^ e)
{
    // Process SizeChanged event, then re-render at the new size.
    DirectXPanelBase::OnSizeChanged(sender, e);
    Render();
}

void UIAD2DPanel::OnCompositionScaleChanged(SwapChainPanel ^sender, Object ^args)
{
    // Process CompositionScaleChanged event, then re-render at the new scale.
    DirectXPanelBase::OnCompositionScaleChanged(sender, args);
    Render();
}

void UIAD2DPanel::OnResuming(Object^ sender, Object^ args)
{
    // Ensure content is rendered when the app is resumed.
    Render();
}

