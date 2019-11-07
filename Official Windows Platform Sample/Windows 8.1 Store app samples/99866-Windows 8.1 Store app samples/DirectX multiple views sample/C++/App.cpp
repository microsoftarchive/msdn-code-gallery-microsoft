//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "App.h"

using namespace MultiViewSample;

using namespace concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

// The main function is only used to initialize our IFrameworkView class.
[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXApplicationSource = ref new DirectXApplicationSource();
    CoreApplication::Run(directXApplicationSource);
    return 0;
}

IFrameworkView^ DirectXApplicationSource::CreateView()
{
    return ref new App();
}

App::App() :
    m_windowClosed(false),
    m_windowVisible(true)
{
}

// The first method called when the IFrameworkView is being created.
void App::Initialize(CoreApplicationView^ applicationView)
{
    // Register event handlers for app lifecycle. Only the main view's thread needs to handle this in this sample
    // Note that "Activated" events will always fire on the main view's thread for activations other than hosted contracts
    // like Share and Picker
    if (CoreApplication::GetCurrentView()->IsMain)
    {
        applicationView->Activated +=
            ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &App::OnActivated);

        CoreApplication::Suspending +=
            ref new EventHandler<SuspendingEventArgs^>(this, &App::OnSuspending);

        CoreApplication::Resuming +=
            ref new EventHandler<Platform::Object^>(this, &App::OnResuming);
    }

    // At this point we have access to the device.
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
}

// Called when the CoreWindow object is created (or re-created).
void App::SetWindow(CoreWindow^ window)
{
    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &App::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &App::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &App::OnWindowClosed);

    DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();

    // Save event registration tokens for DisplayInformation-based events.
    // These will be deregistered when the view is closed in order to prevent
    // handling these events during uninitialization.

    m_dpiChangedToken = currentDisplayInformation->DpiChanged +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDpiChanged);

    m_orientationChangedToken = currentDisplayInformation->OrientationChanged +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnOrientationChanged);

    m_displayContentsToken = DisplayInformation::DisplayContentsInvalidated +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDisplayContentsInvalidated);

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false;
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    if (CoreApplication::GetCurrentView()->IsMain)
    {
        // Only the main view in this sample should offer the ability to create new views. It does so 
        // when the user taps or clicks the window
        window->PointerPressed +=
            ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &App::OnPointerPressed);
    }
    else
    {
        // When any window falls out of the list of recently used apps (and leaves the screen),
        // it becomes "consolidated" along with the other views. That mean the view and window still exist,
        // but the user can no longer access it. It's a good time to go ahead and release the view by closing it.
        // Note that you shouldn't ever close the window on the main view's thread, as that will tear down your process
        ApplicationView::GetForCurrentView()->Consolidated +=
            ref new Windows::Foundation::TypedEventHandler<ApplicationView ^, ApplicationViewConsolidatedEventArgs ^>(this, &App::OnConsolidated);
    }

    m_deviceResources->SetWindow(window);
}

// Initializes scene resources, or loads a previously saved app state.
void App::Load(Platform::String^ entryPoint)
{
    std::wstring titleText = (entryPoint + " - DX Sample")->Data();
    Platform::String^ textToDraw; // String that contains text specific to the view.
    DirectX::XMVECTORF32 windowColor; // Window background color.

    auto appView = ApplicationView::GetForCurrentView();
    appView->Title = entryPoint;

    if (entryPoint == "Main View")
    {
        m_mainViewId = appView->Id;
        windowColor = DirectX::Colors::DarkSlateGray;
        textToDraw = "I'm main! \n \n \tPlease click in this area to create/destroy new views \n \n \tIn the absence of a secondary monitor the projection view will not be created";
    }
    else if (entryPoint == "Projection View" && ProjectionManager::ProjectionDisplayAvailable)
    {
        m_projectionViewId = appView->Id;
        ProjectionManager::StartProjectingAsync(m_projectionViewId, m_mainViewId);
        windowColor = DirectX::Colors::DarkGreen;
        textToDraw = "I'm projected!";
    }
    else if (entryPoint == "Secondary View")
    {
        m_secondaryViewId = appView->Id;
        ApplicationViewSwitcher::TryShowAsStandaloneAsync(m_secondaryViewId, ViewSizePreference::UseHalf, m_mainViewId, ViewSizePreference::UseHalf);
        windowColor = DirectX::Colors::DarkOrchid;
        textToDraw = "I'm secondary!";
    }

    m_main = std::unique_ptr<MultiViewSampleMain>(new MultiViewSampleMain(m_deviceResources, titleText, textToDraw, windowColor));
}

// This method is called after the window becomes active.
void App::Run()
{
    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            // Process any UI events in the queue.
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);

            if (m_main->Render())
            {
                m_deviceResources->Present();
            }
        }
        else
        {
            // The window is hidden. Block until a UI event occurs.
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

// Required for IFrameworkView.
// This method will never be called by the main view since the main view 
// will be closed only after the process has been suspended. Secondary views, though
// are actually closed in this sample (see OnConsolidated), so you can use this method 
// to do any necessary cleanup
void App::Uninitialize()
{
}

// Application lifecycle event handlers. In this sample, only the main view
// registers for these events
void App::OnActivated(CoreApplicationView^ applicationView, IActivatedEventArgs^ args)
{
    // When the application is first launched, a splash screen is shown for the app.
    // This splash screen is not dismissed until CoreWindow::Activate is called.
    CoreWindow::GetForCurrentThread()->Activate();
}

void App::OnSuspending(Object^ sender, SuspendingEventArgs^ args)
{
    m_deviceResources->Trim();
}

void App::OnResuming(Object^ sender, Object^ args)
{
    // Restore any data or state that was unloaded on suspend. By default, data
    // and state are persisted when resuming from suspend. Note that this event
    // does not occur if the app was previously terminated.
}

// Window event handlers.

void App::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
    m_deviceResources->SetLogicalSize(Size(sender->Bounds.Width, sender->Bounds.Height));
    m_main->UpdateForWindowSizeChange();
}

void App::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
    m_windowVisible = args->Visible;
}

void App::OnWindowClosed(CoreWindow^ sender, CoreWindowEventArgs^ args)
{
    m_windowClosed = true;
}

// This event is fired whenever a window stops being accessible by the user (e.g. it leaves the list of recently used apps and the screen).
// This can happen as a result of going unused some time, being closed by the app using the "StopProjectingAsync" method, the user doing
// the close gesture, etc.
void App::OnConsolidated(ApplicationView^ sender, ApplicationViewConsolidatedEventArgs^ args)
{
    if (sender->Id == m_projectionViewId)
    {
        m_projectionViewId = 0;
    }
    else if (sender->Id == m_secondaryViewId)
    {
        m_secondaryViewId = 0;
    }

    // Deregister the event handlers to prevent handling events that are no longer relevant.
    DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();

    currentDisplayInformation->DpiChanged -= m_dpiChangedToken;
    currentDisplayInformation->OrientationChanged -= m_orientationChangedToken;
    DisplayInformation::DisplayContentsInvalidated -= m_displayContentsToken;

    CoreWindow::GetForCurrentThread()->Close(); // Note that calling this method does not trigger "OnWindowClosed".
    m_windowClosed = true; // Cause this object to leave the loop in "Run" and trigger tear-down of this object.
}

// Registered by the main view to allow creation or destruction of secondary views.
void App::OnPointerPressed(CoreWindow^ sender, PointerEventArgs^ args)
{
    // Disallow creating/destroying views until the async tasks are completed.
    static bool projectionTaskRunning = false;
    static bool secondaryViewTaskRunning = false;

    if (projectionTaskRunning || secondaryViewTaskRunning)
    {
        return;
    }

    // Create new views for the current display and optionally a secondary display if one is available.
    // The code directing the views to the appropriate display is in App::Load.
    if (m_projectionViewId == 0)
    {
        projectionTaskRunning = true;
        // Create a view for the secondary display if one is available.
        if (ProjectionManager::ProjectionDisplayAvailable)
        {
            CoreApplication::CreateNewView(nullptr, "Projection View")->Dispatcher->RunAsync(
                CoreDispatcherPriority::Normal,
                ref new DispatchedHandler([this]()
                {
                    projectionTaskRunning = false;
                }));
        }
        else
        {
            projectionTaskRunning = false;
        }
    } 
    else
    {
        // Destroy the projection view if it's still around.
        projectionTaskRunning = true;
        create_task(ProjectionManager::StopProjectingAsync(m_projectionViewId, m_mainViewId)).then([this]()
        {
            projectionTaskRunning = false;
        });
    }
    
    if (m_secondaryViewId == 0)
    {
        // Create an additional view on the same display as the main view.
        secondaryViewTaskRunning = true;
        CoreApplication::CreateNewView(nullptr, "Secondary View")->Dispatcher->RunAsync(
            CoreDispatcherPriority::Normal,
            ref new DispatchedHandler([this]()
            {
                secondaryViewTaskRunning = false;
            }));
    }
    else
    {
        // Destroy the other view if it's still around.
        secondaryViewTaskRunning = true;
        create_task(
            ApplicationViewSwitcher::SwitchAsync(m_mainViewId, m_secondaryViewId, ApplicationViewSwitchingOptions::ConsolidateViews)
            ).then([this]()
            {
                secondaryViewTaskRunning = false;
            });
    }
}

// DisplayInformation event handlers.

void App::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->SetDpi(sender->LogicalDpi);
    m_main->UpdateForWindowSizeChange();
}

void App::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->SetCurrentOrientation(sender->CurrentOrientation);
    m_main->UpdateForWindowSizeChange();
}

void App::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->ValidateDevice();
}

int App::m_mainViewId = 0;
int App::m_projectionViewId = 0;
int App::m_secondaryViewId = 0;
