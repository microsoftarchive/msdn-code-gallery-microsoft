//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "App.h"

#include <ppltasks.h>

using namespace JigsawPuzzle;

using namespace concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

// The main function is only used to initialize our IFrameworkView class.
[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto direct3DApplicationSource = ref new Direct3DApplicationSource();
    CoreApplication::Run(direct3DApplicationSource);
    return 0;
}

IFrameworkView^ Direct3DApplicationSource::CreateView()
{
    return ref new App();
}

App::App() :
    m_lastPosition(-1.0f, -1.0f),
    m_pointerId(Constants::InvalidPointerId)
{
}

// The first method called when the IFrameworkView is being created.
void App::Initialize(CoreApplicationView^ applicationView)
{
    // Register event handlers for app lifecycle. This example includes Activated, so that we
    // can make the CoreWindow active and start rendering on the window.
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &App::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &App::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &App::OnResuming);

    // At this point we have access to the device. 
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
}

// Called when the CoreWindow object is created (or re-created).
void App::SetWindow(CoreWindow^ window)
{
    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &App::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &App::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &App::OnPointerMoved);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &App::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &App::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &App::OnWindowClosed);

    DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();

    currentDisplayInformation->DpiChanged +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDpiChanged);

    currentDisplayInformation->OrientationChanged +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnOrientationChanged);

    DisplayInformation::DisplayContentsInvalidated +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDisplayContentsInvalidated);

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false; 
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    m_deviceResources->SetWindow(window);
}

// Initializes scene resources, or loads a previously saved app state.
void App::Load(Platform::String^ entryPoint)
{
    m_state = std::make_shared<GameState>();
    m_main = std::unique_ptr<JigsawPuzzleMain>(new JigsawPuzzleMain(m_deviceResources, m_state));
}

// This method is called after the window becomes active.
void App::Run()
{
    // 4. Start a thread dedicated to rendering and dedicate the UI thread to input processing.
    m_main->StartRenderThread();

    // ProcessUntilQuit will block the thread and process events as they appear until the App terminates.
    CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

// Required for IFrameworkView.
// Terminate events do not cause Uninitialize to be called. It will be called if your IFrameworkView
// class is torn down while the app is in the foreground.
void App::Uninitialize()
{
}

// Application lifecycle event handlers.

void App::OnActivated(CoreApplicationView^ applicationView, IActivatedEventArgs^ args)
{
    // Run() won't start until the CoreWindow is activated.
    CoreWindow::GetForCurrentThread()->Activate();
}

void App::OnSuspending(Platform::Object^ sender, SuspendingEventArgs^ args)
{
    // Save app state asynchronously after requesting a deferral. Holding a deferral
    // indicates that the application is busy performing suspending operations. Be
    // aware that a deferral may not be held indefinitely. After about five seconds,
    // the app will be forced to exit.
    SuspendingDeferral^ deferral = args->SuspendingOperation->GetDeferral();

    // 4. Schedule a work item that handles suspending operations.
    m_main->BeginInvoke([this, deferral]()
    {
        m_deviceResources->Trim();
        m_main->StopRenderThread();

        // Save the current state of the puzzle, pause the game, or do whatever is
        // necessary to ensure a seamless transition back to the game in the event
        // that the app is terminated while it is in the suspended state.

        deferral->Complete();
    });
}

void App::OnResuming(Platform::Object^ sender, Platform::Object^ args)
{
    // Restore any data or state that was unloaded on suspend. By default, data
    // and state are persisted when resuming from suspend. Note that this event
    // does not occur if the app was previously terminated.

    // 4. Start a new rendering thread since the previous one was terminated during
    // the suspend operation.
    m_main->StartRenderThread();
}

// Window event handlers.

void App::OnPointerPressed(CoreWindow^ sender, PointerEventArgs^ args)
{
    if (m_pointerId != Constants::InvalidPointerId)
    {
        // Only track one pointer at a time.
        return;
    }

    m_state->OnPointerPressed(args->CurrentPoint->Position);
    m_lastPosition = args->CurrentPoint->Position;
    m_pointerId = args->CurrentPoint->PointerId;

#if MEASURE_LATENCY
    m_state->ToggleBackgroundColor();
#endif
}

void App::OnPointerReleased(CoreWindow^ sender, PointerEventArgs^ args)
{
    if (args->CurrentPoint->PointerId != m_pointerId)
    {
        // We're not tracking this pointer. Ignore the event.
        return;
    }

    m_state->OnPointerReleased(args->CurrentPoint->Position);
    m_lastPosition = args->CurrentPoint->Position;
    m_pointerId = Constants::InvalidPointerId;
}

void App::OnPointerMoved(CoreWindow^ sender, PointerEventArgs^ args)
{
    if (args->CurrentPoint->PointerId != m_pointerId)
    {
        // We're not tracking this pointer. Ignore the event.
        return;
    }

    // On touch-enabled devices, PointerMoved will continuously fire while the display is touched.
    // For this sample we do not want to process these additional events as "moves."
    if (args->CurrentPoint->Position == m_lastPosition)
    {
        return;
    }

    m_state->OnPointerMoved(args->CurrentPoint->Position);
    m_lastPosition = args->CurrentPoint->Position;
}

void App::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
    // 4. Schedule a work item that resizes the device resources.
    Size size = Size(sender->Bounds.Width, sender->Bounds.Height);
    m_main->BeginInvoke([this, size]()
    {
        m_deviceResources->SetLogicalSize(size);
        m_main->CreateWindowSizeDependentResources();
    });
}

void App::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
    // 4. If the window is not visible, there is no need to render the app content.
    // When the window becomes visible again, the rendering thread should be restarted.
    if (args->Visible)
    {
        m_main->StartRenderThread();
    }
    else
    {
        m_main->StopRenderThread();
    }
}

void App::OnWindowClosed(CoreWindow^ sender, CoreWindowEventArgs^ args)
{
    // 4. Terminate the render thread if it has not already been terminated yet.
    m_main->StopRenderThread();
}

// DisplayInformation event handlers.

void App::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
    // 4. Schedule a work item that handles the change in DPI.
    float dpi = sender->LogicalDpi;
    auto bounds = CoreWindow::GetForCurrentThread()->Bounds;
    Size logicalSize = Size(bounds.Width, bounds.Height);
    m_main->BeginInvoke([=]()
    {
        m_deviceResources->SetDpi(dpi, logicalSize);
        m_main->CreateWindowSizeDependentResources();
    });
}

void App::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
    // 4. Schedule a work item that handles the change in device orientation.
    DisplayOrientations orientation = sender->CurrentOrientation;
    m_main->BeginInvoke([this, orientation]()
    {
        m_deviceResources->SetCurrentOrientation(orientation);
        m_main->CreateWindowSizeDependentResources();
    });
}

void App::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
    // 4. Schedule a work item that validates the D3D device.
    m_main->BeginInvoke([this]()
    {
        m_deviceResources->ValidateDevice();
    });
}
