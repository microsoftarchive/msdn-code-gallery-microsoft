//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace D2DGeometryRealizations;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::System::Threading;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

DirectXPage::DirectXPage():
    m_windowVisible(true)
{
    InitializeComponent();

    // Register event handlers for page lifecycle.
    CoreWindow^ window = Window::Current->CoreWindow;

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXPage::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXPage::OnVisibilityChanged);

    DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();

    currentDisplayInformation->DpiChanged +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDpiChanged);

    currentDisplayInformation->OrientationChanged +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnOrientationChanged);

    DisplayInformation::DisplayContentsInvalidated +=
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDisplayContentsInvalidated);

    // Register the rendering event, called every time XAML renders the screen.
    m_eventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &DirectXPage::OnRendering));

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false;
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    // At this point we have access to the device.
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
    m_deviceResources->SetWindow(Window::Current->CoreWindow, swapChainBackgroundPanel);

    m_main = std::unique_ptr<D2DGeometryRealizationsMain>(new D2DGeometryRealizationsMain(m_deviceResources));

    // Set a timer object to update the framerate counter each second.
    TimeSpan period;
    period.Duration = 10000000;

    ThreadPoolTimer^ periodicTimer = ThreadPoolTimer::CreatePeriodicTimer(
        ref new TimerElapsedHandler([this](ThreadPoolTimer^ source)
        {
            unsigned int fps = m_main->GetFPS();

            // Update the UI thread by using the UI core dispatcher.
            Dispatcher->RunAsync(CoreDispatcherPriority::High,
                ref new DispatchedHandler([this, fps]()
                {
                    // Update the UI from this thread.
                    FramerateTextBlock->Text = fps + " FPS";
                }));

        }), period);
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
    m_deviceResources->Trim();

    // Delegate save operations to the main class.
    m_main->SaveInternalState(state);
}

// Loads the current state of the app for resume events.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
    // Delegate load operations to the main class.
    m_main->LoadInternalState(state);

    // Synchronize the UI with the newly loaded application state.
    UpdateUI();
}

// Called every time XAML decides to render a frame.
void DirectXPage::OnRendering(Object^ sender, Object^ args)
{
    if (m_windowVisible)
    {
        m_main->Update();

        if (m_main->Render())
        {
            m_deviceResources->Present();
        }
    }
}

// Window event handlers.

void DirectXPage::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
    m_deviceResources->SetLogicalSize(Size(sender->Bounds.Width, sender->Bounds.Height));
    m_main->UpdateForWindowSizeChange();
}

void DirectXPage::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
    m_windowVisible = args->Visible;
}

// DisplayInformation event handlers.

void DirectXPage::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->SetDpi(sender->LogicalDpi);
    m_main->UpdateForWindowSizeChange();
}

void DirectXPage::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->SetCurrentOrientation(sender->CurrentOrientation);
    m_main->UpdateForWindowSizeChange();
}

void DirectXPage::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->ValidateDevice();
}

// Handlers for UI interaction.

// Panning, pinching, and stretching cause the view position to move and the zoom factor
// to increase and decrease.
void DirectXPage::OnManipulationUpdated(Object^ sender, ManipulationDeltaRoutedEventArgs^ e)
{
    Point position = e->Position;
    Point positionDelta = e->Delta.Translation;
    float zoomDelta = e->Delta.Scale;

    m_main->UpdateZoom(position, positionDelta, zoomDelta);
}

// Scrolling the mouse wheel up and down cause the zoom factor to increase and decrease.
void DirectXPage::OnPointerWheelChanged(Object^ sender, PointerRoutedEventArgs^ e)
{
    PointerPoint^ p = e->GetCurrentPoint(this);

    Point position = p->Position;
    Point positionDelta = Point(0.0f, 0.0f);
    float zoomDelta = 1 + (p->Properties->MouseWheelDelta / WHEEL_DELTA) * 0.05f;

    m_main->UpdateZoom(position, positionDelta, zoomDelta);
}

void DirectXPage::OnUseRealizationsChanged(Object^ sender, RoutedEventArgs^ e)
{
    // This handler is called by the XAML infrastructure before the m_main object is
    // set up. Avoid passing the call while the m_main object is still null.
    if (m_main != nullptr)
    {
        bool enabled = RealizationsEnabledCheckBox->IsChecked->Value;
        m_main->SetRealizationsEnabled(enabled);
    }
}

void DirectXPage::OnMorePrimitivesTapped(Object^ sender, TappedRoutedEventArgs^ e)
{
    m_main->IncreasePrimitives();
}

void DirectXPage::OnFewerPrimitivesTapped(Object^ sender, TappedRoutedEventArgs^ e)
{
    m_main->DecreasePrimitives();
}

void DirectXPage::OnRestoreDefaultsTapped(Object^ sender, TappedRoutedEventArgs^ e)
{
    // Delegate restore operations to the main class.
    m_main->RestoreDefaults();

    // Synchronize the UI with the newly loaded application state.
    UpdateUI();
}

// Convenience method to synchronize the state of the UI controls with the state of the app.
void DirectXPage::UpdateUI()
{
    RealizationsEnabledCheckBox->IsChecked = m_main->GetRealizationsEnabled();
}
