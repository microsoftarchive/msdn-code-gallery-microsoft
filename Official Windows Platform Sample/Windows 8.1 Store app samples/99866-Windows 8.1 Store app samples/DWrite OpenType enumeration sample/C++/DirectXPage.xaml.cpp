//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace DWriteOpenTypeEnumeration;

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

    m_main = std::unique_ptr<DWriteOpenTypeEnumerationMain>(new DWriteOpenTypeEnumerationMain(m_deviceResources));
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
    if (state->HasKey("FontIndex"))
    {
        state->Remove("FontIndex");
    }
    state->Insert("FontIndex", PropertyValue::CreateUInt32(comboBox->SelectedIndex));

    if (state->HasKey("StylisticSetIndex"))
    {
        state->Remove("StylisticSetIndex");
    }
    state->Insert("StylisticSetIndex", PropertyValue::CreateUInt32(stylisticSet->SelectedIndex));

    m_deviceResources->Trim();
}

// Loads the current state of the app for resume events.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
    if (state->HasKey("FontIndex"))
    {
        comboBox->SelectedIndex = safe_cast<IPropertyValue^>(state->Lookup("FontIndex"))->GetUInt32();
        OnFontSelectionChanged(nullptr, nullptr);
    }
    if (state->HasKey("StylisticSetIndex"))
    {
        stylisticSet->SelectedIndex = safe_cast<IPropertyValue^>(state->Lookup("StylisticSetIndex"))->GetUInt32();
        OnStylisticSetSelectionChanged(nullptr, nullptr);
    }
}

// Called every time XAML decides to render a frame.
void DirectXPage::OnRendering(Object^ sender, Object^ args)
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

void DirectXPage::OnFontSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    Platform::Array<bool>^ temp = m_main->ReturnSupportedFeatures(comboBox->SelectedIndex);
    ResetCheckBoxes();

    if (temp[0] == true)
    {
        style0->IsEnabled = true;
    }
    if (temp[1] == true)
    {
        style1->IsEnabled = true;
    }
    if (temp[2] == true)
    {
        style2->IsEnabled = true;
    }
    if (temp[3] == true)
    {
        style3->IsEnabled = true;
    }
    if (temp[4] == true)
    {
        style4->IsEnabled = true;
    }
    if (temp[5] == true)
    {
        style5->IsEnabled = true;
    }
    if (temp[6] == true)
    {
        style6->IsEnabled = true;
    }
    if (temp[7] == true)
    {
        style7->IsEnabled = true;
    }
    if (!temp[1] && !temp[2] && !temp[3] && !temp[4] && !temp[5] && !temp[6] && !temp[7])
    {
        stylisticSet->Visibility = ::Visibility::Collapsed;
        stylisticSetLabel->Text = "No Stylistic Sets Present in this Font";
    }
}

void DirectXPage::ResetCheckBoxes()
{
    stylisticSet->Visibility = ::Visibility::Visible;
    style0->IsEnabled = false;
    style1->IsEnabled = false;
    style2->IsEnabled = false;
    style3->IsEnabled = false;
    style4->IsEnabled = false;
    style5->IsEnabled = false;
    style6->IsEnabled = false;
    style7->IsEnabled = false;
    stylisticSet->SelectedIndex = 8;
    stylisticSetLabel->Text = "Stylistic Set";
}

void DirectXPage::OnStylisticSetSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    m_main->SetStylisticSet(stylisticSet->SelectedIndex);
}
