//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace DirectX;

#include "DirectXApp.h"
#include "BasicTimer.h"

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace concurrency;

DirectXApp::DirectXApp() :
    m_windowClosed(false),
    m_windowVisible(false)
{
}

// <snippet3>
void DirectXApp::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DirectXApp::OnActivated);

    // <snippet13>
    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &DirectXApp::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &DirectXApp::OnResuming);
    // </snippet13>

    m_renderer = ref new MarbleMaze();
}
// </snippet3>

// <snippet4>
void DirectXApp::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    // <snippet8>
    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXApp::OnWindowSizeChanged);
    // </snippet8>

    // <snippet113>

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXApp::OnVisibilityChanged);
    // </snippet113>

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &DirectXApp::OnWindowClosed);

    // <snippet52>
    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXApp::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXApp::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXApp::OnPointerMoved);
    // </snippet52>

    window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &DirectXApp::OnKeyDown);

    window->KeyUp +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &DirectXApp::OnKeyUp);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXApp::OnLogicalDpiChanged);

    m_renderer->Initialize(window, DisplayProperties::LogicalDpi);
}
// </snippet4>

// <snippet112>
void DirectXApp::Load(
    _In_ Platform::String^ entryPoint
    )
{
    task<void>([=]()
    {
        m_renderer->LoadDeferredResources(true, false);
    });
}
// </snippet112>


// <snippet6>
void DirectXApp::Run()
{
    BasicTimer^ timer = ref new BasicTimer();

    while (!m_windowClosed)
    {
        timer->Update();

        if (m_windowVisible)
        {
            // <snippet67>
            // Process windowing events.
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            // </snippet67>

            // Update and render this frame.
            m_renderer->Update(timer->Total, timer->Delta);
            m_renderer->Render();

            // Present the frame. This call is synchronized to the display frame rate.
            m_renderer->Present();
        }
        else
        {
            // The window is not visible, so just wait for next event and respond to it.
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }

    // The app is exiting so do the same thing as would if app was being suspended.
    m_renderer->OnSuspending();
}
// </snippet6>

void DirectXApp::Uninitialize()
{
}

// <snippet9>
void DirectXApp::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    // Query for the current ApplicationView state to make sure the most current
    // ApplicationViewState is known before resizing all the resources.

    m_renderer->OnViewChange(ApplicationView::Value);
    m_renderer->UpdateForWindowSizeChange();
}
// </snippet9>

// <snippet114>
void DirectXApp::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}
// </snippet114>


void DirectXApp::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void DirectXApp::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
}

void DirectXApp::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    CoreWindow::GetForCurrentThread()->Activated +=
        ref new TypedEventHandler<CoreWindow^, WindowActivatedEventArgs^>(this, &DirectXApp::OnWindowActivationChanged);
    CoreWindow::GetForCurrentThread()->Activate();
    m_windowVisible = true;
}

void DirectXApp::OnWindowActivationChanged(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::WindowActivatedEventArgs^ args
    )
{
    if (args->WindowActivationState == CoreWindowActivationState::Deactivated)
    {
        m_renderer->OnFocusChange(false);
    }
    else if (args->WindowActivationState == CoreWindowActivationState::CodeActivated
        || args->WindowActivationState == CoreWindowActivationState::PointerActivated)
    {
        m_renderer->OnFocusChange(true);
    }
}

// <snippet68>
void DirectXApp::OnPointerPressed(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    m_renderer->AddTouch(args->CurrentPoint->PointerId, args->CurrentPoint->Position);
}

void DirectXApp::OnPointerReleased(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    m_renderer->RemoveTouch(args->CurrentPoint->PointerId);
}

void DirectXApp::OnPointerMoved(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    m_renderer->UpdateTouch(args->CurrentPoint->PointerId, args->CurrentPoint->Position);
}
// </snippet68>

void DirectXApp::OnKeyDown(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::KeyEventArgs^ args
    )
{
    m_renderer->KeyDown(args->VirtualKey);
}

void DirectXApp::OnKeyUp(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::KeyEventArgs^ args
    )
{
    m_renderer->KeyUp(args->VirtualKey);
}

// <snippet14>
void DirectXApp::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    m_renderer->OnSuspending();
}
// </snippet14>

// <snippet15>
void DirectXApp::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
    m_renderer->OnResuming();
}
// </snippet15>

// <snippet2>
IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DirectXApp();
}
// </snippet2>

// <snippet5>
[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
// </snippet5>