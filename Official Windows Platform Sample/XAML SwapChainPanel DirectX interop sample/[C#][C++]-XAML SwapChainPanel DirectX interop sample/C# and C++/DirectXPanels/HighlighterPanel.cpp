//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
#include "pch.h"
#include "HighlighterPanel.h"
#include <DirectXMath.h>
#include <math.h>
#include <ppltasks.h>
#include <windows.ui.xaml.media.dxinterop.h>
#include "DirectXHelper.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::System::Threading;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Input::Inking;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Interop;
using namespace Concurrency;
using namespace DirectX;
using namespace D2D1;
using namespace DirectXPanels;
using namespace DX;

HighlighterPanel::HighlighterPanel() :
    m_drawingState(DrawingState::None),
    m_activePointerId(0)
{
    // Set alpha mode to premultiplied to enable transparency.
    m_alphaMode = DXGI_ALPHA_MODE_PREMULTIPLIED;
    // Set background color to transparent white.
    m_backgroundColor = ColorF(ColorF::White, 0);
    
    critical_section::scoped_lock lock(m_criticalSection);

    CreateDeviceIndependentResources();
    CreateDeviceResources();
    CreateSizeDependentResources();
}

void HighlighterPanel::StartProcessingInput()
{
    // Initialize the rendering surface and prepares it to receive input.	

    // Create a task to register for independent input and begin processing input messages.
    auto workItemHandler = ref new WorkItemHandler([this](IAsyncAction ^)
    {
        // The CoreIndependentInputSource will raise pointer events for the specified device types on whichever thread it's created on.
        m_coreInput = CreateCoreIndependentInputSource(
            Windows::UI::Core::CoreInputDeviceTypes::Mouse |
            Windows::UI::Core::CoreInputDeviceTypes::Touch |
            Windows::UI::Core::CoreInputDeviceTypes::Pen
            );

        // Register for pointer events, which will be raised on the background thread.
        m_coreInput->PointerPressed += ref new TypedEventHandler<Object^, PointerEventArgs^>(this, &HighlighterPanel::OnPointerPressed);
        m_coreInput->PointerMoved += ref new TypedEventHandler<Object^, PointerEventArgs^>(this, &HighlighterPanel::OnPointerMoved);
        m_coreInput->PointerReleased += ref new TypedEventHandler<Object^, PointerEventArgs^>(this, &HighlighterPanel::OnPointerReleased);

        // Begin processing input messages as they're delivered.
        m_coreInput->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
    });

    // Run task on a dedicated high priority background thread.
    m_inputLoopWorker = ThreadPool::RunAsync(workItemHandler, WorkItemPriority::High, WorkItemOptions::TimeSliced);
}

void HighlighterPanel::StopProcessingInput()
{
    // A call to ProcessEvents() with the ProcessUntilQuit flag will only return by default when the window closes.
    // Calling StopProcessEvents allows ProcessEvents to return even if the window isn't closing so the background thread can exit.
    m_coreInput->Dispatcher->StopProcessEvents();
}

void HighlighterPanel::CreateDeviceResources()
{
    DirectXPanelBase::CreateDeviceResources();

    // Create stroke style.
    ThrowIfFailed(
        m_d2dFactory->CreateStrokeStyle(
        D2D1::StrokeStyleProperties(
        D2D1_CAP_STYLE_ROUND,
        D2D1_CAP_STYLE_ROUND,
        D2D1_CAP_STYLE_ROUND,
        D2D1_LINE_JOIN_ROUND,
        1.0f,
        D2D1_DASH_STYLE_SOLID,
        0.0f),
        nullptr,
        0,
        &m_inkStrokeStyle)
        );

    // Create stroke brush.
    ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(ColorF(ColorF::Yellow), &m_strokeBrush)
        );

    m_loadingComplete = true;
}

void HighlighterPanel::CreateSizeDependentResources()
{
    m_currentBuffer.Reset();
    m_previousBuffer.Reset();

    DirectXPanelBase::CreateSizeDependentResources();

    ThrowIfFailed(
        m_swapChain->GetBuffer(
        0,
        __uuidof(ID3D11Texture2D),
        &m_currentBuffer
        )
        );

    ThrowIfFailed(
        m_swapChain->GetBuffer(
        1,
        __uuidof(ID3D11Texture2D),
        &m_previousBuffer
        )
        );
}

void HighlighterPanel::Render()
{
    if (!m_loadingComplete)
    {
        return;
    }

    // Render and present the DirectX content.

    m_d2dContext->BeginDraw();
 
    // Note that in this simple example, the strokes the user has drawn are not preserved when the panel's size changes or when the device is lost.  
    // For an example of preserving strokes, see the DrawingPanel implemention used in Scenario2.

    // Clear the surface with a transparent background color.
    m_d2dContext->Clear(m_backgroundColor);

    HRESULT hr = m_d2dContext->EndDraw();

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    if (hr != D2DERR_RECREATE_TARGET)
    {
        ThrowIfFailed(hr);
    }
    Present();
}

void HighlighterPanel::OnPointerPressed(Object^ sender, PointerEventArgs^ e)
{
    // Handle the PointerPressed event, which will be raised on a background thread.

    if (e->CurrentPoint->Properties->PointerUpdateKind == PointerUpdateKind::LeftButtonPressed && 
        m_drawingState == DrawingState::None)
    {
        m_drawingState = DrawingState::Inking;
        m_previousPoint = e->CurrentPoint->Position;
        // Store active pointer ID: only one contact can be inking at a time.
        m_activePointerId = e->CurrentPoint->PointerId;
    }
}

void HighlighterPanel::OnPointerMoved(Object^ sender, PointerEventArgs^ e)
{
    if (!m_loadingComplete)
    {
        return;
    }

    // Handle the PointerMoved event, which will be raised on a background thread.

    if (m_drawingState == DrawingState::Inking && e->CurrentPoint->PointerId == m_activePointerId)
    {
        critical_section::scoped_lock lock(m_criticalSection);

        auto pointerPosition = e->CurrentPoint->Position;

        // While actively inking, copy the back buffer to the active buffer then draw the current stroke segment.
        m_d3dContext->CopyResource(m_currentBuffer.Get(), m_previousBuffer.Get());

        m_d2dContext->BeginDraw();

        m_d2dContext->DrawLine(
            D2D1::Point2F(m_previousPoint.X, m_previousPoint.Y),
            D2D1::Point2F(pointerPosition.X, pointerPosition.Y),
            m_strokeBrush.Get(),
            10.0f,
            m_inkStrokeStyle.Get()
            );

        m_previousPoint = pointerPosition;

        HRESULT hr = m_d2dContext->EndDraw();

        // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
        // is lost. It will be handled during the next call to Present.
        if (hr != D2DERR_RECREATE_TARGET)
        {
            ThrowIfFailed(hr);
        }
        Present();
    }
}

void HighlighterPanel::OnPointerReleased(Object^ sender, PointerEventArgs^ e)
{
    // Handle the PointerReleased event, which will be raised on a background thread.

    if (e->CurrentPoint->Properties->PointerUpdateKind == PointerUpdateKind::RightButtonReleased)
    {
        // When right-clicks are unhandled on the background thread, the platform can use them for AppBar invocation.
        e->Handled = false;
    }
    else if (m_drawingState == DrawingState::Inking)
    {
        m_drawingState = DrawingState::None;
        // Reset active pointer ID.
        m_activePointerId = 0;
    }
}

void HighlighterPanel::OnDeviceLost()
{
    // Handle device lost, then re-render.
    DirectXPanelBase::OnDeviceLost();
    Render();
}

void HighlighterPanel::OnSizeChanged(Platform::Object^ sender, SizeChangedEventArgs^ e)
{
    // Process SizeChanged event, then re-render at the new size.
    DirectXPanelBase::OnSizeChanged(sender, e);
       
    critical_section::scoped_lock lock(m_criticalSection);
    Render();
}

void HighlighterPanel::OnCompositionScaleChanged(SwapChainPanel ^sender, Object ^args)
{
    // Process CompositionScaleChanged event, then re-render at the new scale.
    DirectXPanelBase::OnCompositionScaleChanged(sender, args);
    
    critical_section::scoped_lock lock(m_criticalSection);
    Render();
}

void HighlighterPanel::OnResuming(Object^ sender, Object^ args)
{
    critical_section::scoped_lock lock(m_criticalSection);
    Render();
}
