//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace JpegYCbCrOptimizations;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

using namespace concurrency;

DirectXPage::DirectXPage() :
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

    m_invalidateEventToken = DisplayInformation::DisplayContentsInvalidated::add(
        ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDisplayContentsInvalidated)
        );

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false;
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    // At this point we have access to the device.
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
    m_deviceResources->SetWindow(Window::Current->CoreWindow, swapChainBackgroundPanel);

    // Defer creating JpegYCbCrOptimizationsMain until LoadInternalState is called; this lets us
    // determine the correct creation settings.
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
    m_deviceResources->Trim();

    String^ key = L"IsBgraForced";
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    state->Insert(key, PropertyValue::CreateBoolean(m_main->GetIsBgraForced()));
}

// Loads the current state of the app for resume events.
// This method is used to initialize JpegYCbCrOptimizationsMain regardless of whether there actually is
// any persisted state to restore, and provides default values for any missing state.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
    // Set up default values.
    bool isBgraForced = false;

    // Read persisted state, if any.
    String^ key = L"IsBgraForced";
    if (state->HasKey(key))
    {
        isBgraForced = safe_cast<IPropertyValue^>(state->Lookup(key))->GetBoolean();
    }

    ForceBgraCheckBox->IsChecked = isBgraForced;

    // Initialize JpegYCbCrOptimizationsMain with settings.
    m_main = std::unique_ptr<JpegYCbCrOptimizationsMain>(
        new JpegYCbCrOptimizationsMain(
            m_deviceResources,
            ref new ResourcesLoadedHandler(this, &DirectXPage::OnResourcesLoaded),
            isBgraForced
            )
        );
}

// Called when DirectX content needs to be rendered.
void DirectXPage::RenderDXContent()
{
    if (m_windowVisible)
    {
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
    RenderDXContent();
}

void DirectXPage::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
    m_windowVisible = args->Visible;
    RenderDXContent();
}

// DisplayInformation event handlers.

void DirectXPage::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->SetDpi(sender->LogicalDpi);
    m_main->UpdateForWindowSizeChange();
    RenderDXContent();
}

void DirectXPage::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
    m_deviceResources->SetCurrentOrientation(sender->CurrentOrientation);
    m_main->UpdateForWindowSizeChange();
    RenderDXContent();
}

void DirectXPage::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
    // Suppress further events until the invalidated handler has completed its work.
    DisplayInformation::DisplayContentsInvalidated::remove(m_invalidateEventToken);

    m_deviceResources->ValidateDeviceAsync().then([this]()
    {
        m_invalidateEventToken = DisplayInformation::DisplayContentsInvalidated::add(
            ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDisplayContentsInvalidated)
            );

        RenderDXContent();
    }, task_continuation_context::use_current());
}

// Once JpegYCbCrOptimizationsRenderer has finished resource creation, update the UI text and render.
void DirectXPage::OnResourcesLoaded(YCbCrSupportMode mode)
{
    switch (mode)
    {
    case YCbCrSupportMode::DisabledFallback:
        TitleText->Text = String::Concat(
            SampleConstants::TitleString,
            SampleConstants::SampleModeString_DisabledFallback
            );

        break;

    case YCbCrSupportMode::DisabledForced:
        TitleText->Text = String::Concat(
            SampleConstants::TitleString,
            SampleConstants::SampleModeString_DisabledForced
            );

        break;

    case YCbCrSupportMode::Enabled:
        TitleText->Text = String::Concat(
            SampleConstants::TitleString,
            SampleConstants::SampleModeString_Enabled
            );

        break;

    default:
        TitleText->Text = SampleConstants::TitleString;
    }

    RenderDXContent();
}

void JpegYCbCrOptimizations::DirectXPage::OnForceBgraClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    m_main->SetIsBgraForced(!m_main->GetIsBgraForced());
}
