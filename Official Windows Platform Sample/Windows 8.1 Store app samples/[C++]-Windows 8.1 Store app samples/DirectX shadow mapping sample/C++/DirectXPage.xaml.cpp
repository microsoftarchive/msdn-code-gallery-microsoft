//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace ShadowMapping;

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

    // Whenever this screen is not being used anymore, you can unregister this event with the following line:
    // CompositionTarget::Rendering::remove(m_eventToken);

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false;
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    // At this point we have access to the device.
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
    m_deviceResources->SetWindow(Window::Current->CoreWindow, swapChainBackgroundPanel);

    m_main = std::unique_ptr<ShadowMapSampleMain>(new ShadowMapSampleMain(m_deviceResources));

    UpdateUI();
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
    // DirectX apps call Trim() to reduce memory footprint before suspend.
    m_deviceResources->Trim();

    // Delegate save operations to the main class.
    m_main->SaveInternalState(state);
}

// Loads the current state of the app for resume events.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
    // Delegate load operations to the main class.
    m_main->LoadInternalState(state);

    UpdateUI();
}

// Convenience method to synchronize the state of the UI controls with the state of the app.
void DirectXPage::UpdateUI()
{
    // Update filtering checkbox.
    bool filtering = m_main->GetFiltering();
    EnableLinearFiltering->IsChecked = filtering;
    
    // Update size slider and text indicator.
    float size = m_main->GetShadowSize();
    ShadowBufferSizeChanger->Value = size;
    SetShadowSizeText(size);
    
    // Check feature support for warning.
    if (!m_main->GetD3D9ShadowsSupported())
    {
        ErrorText->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
}

// Convenience method for updating UI text.
void DirectXPage::SetShadowSizeText(float val)
{
    ShadowSizeText->Text = "Shadow buffer size (" + val.ToString() + "): ";
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

void DirectXPage::OnEnableLinearFiltering(Object^ sender, RoutedEventArgs^ args)
{
    if (m_main != nullptr)
    {
        m_main->SetFiltering(true);
    }
}

void DirectXPage::OnDisableLinearFiltering(Object^ sender, RoutedEventArgs^ args)
{
    if (m_main != nullptr)
    {
        m_main->SetFiltering(false);
    }
}

void DirectXPage::OnShadowBufferSizeChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    if (m_main != nullptr)
    {
        float val = static_cast<float>(e->NewValue);
        m_main->SetShadowSize(val);   
        SetShadowSizeText(val);
    }
}