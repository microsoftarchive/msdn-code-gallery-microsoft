//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace DWriteColorFontFallback;

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
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;

DirectXPage::DirectXPage():
    m_windowVisible(true),
    m_viewPosition(),
    m_zoom(1.0f)
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

    // Setup Input events to be able to use the gesture recognizer

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerMoved);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerWheelChanged);

    // We will use the gesture recognizer to compute the touch gestures, like pinching and dragging,
    // into transformations we can use in Direct2D. We simply funnel any touch events into it
    // and the gesture recognizer will fire events when the transformations need to be updated or
    // a specific gesture (e.g. double tap) occurs.
    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings =
        GestureSettings::DoubleTap                     |
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationScale             |
        GestureSettings::ManipulationTranslateInertia  |
        GestureSettings::ManipulationScaleInertia;

    m_gestureRecognizer->Tapped +=
        ref new TypedEventHandler<GestureRecognizer^, TappedEventArgs^>(this, &DirectXPage::OnTapped);

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(
            this,
            &DirectXPage::OnManipulationUpdated
            );

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false;
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;
    m_gestureRecognizer->ShowGestureFeedback = false;

    // At this point we have access to the device.
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
    m_deviceResources->SetWindow(Window::Current->CoreWindow, swapChainBackgroundPanel);

    m_main = std::unique_ptr<DWriteColorFontFallbackMain>(new DWriteColorFontFallbackMain(m_deviceResources));

    // Setup fontfallback list
    m_fallbackScenarioList = ref new Platform::Collections::Vector<Object^>();

    // Populate the ComboBox with the list of font fallback scenarios.
    for (unsigned int i = 0; i < SampleConstants::MaxFontFallbackScenarios; ++i)
    {
        ComboBoxItem^ item = ref new ComboBoxItem();
        item->Tag = PropertyValue::CreateUInt32(i);
        item->Content = ref new String(SampleConstants::FontFallbackDescriptions[i]);
        m_fallbackScenarioList->Append(item);
    }

    // Bind the ComboListBox to the scenario list.
    FontFallbackListBox->ItemsSource = m_fallbackScenarioList;

    // Listen for Charms invokation so we can add the setting item to the menu.
    m_commandsRequestedToken = SettingsPane::GetForCurrentView()->CommandsRequested +=
        ref new TypedEventHandler<SettingsPane^, SettingsPaneCommandsRequestedEventArgs^>(this, &DirectXPage::OnCommandsRequested);
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
    m_deviceResources->Trim();

    String^ key = L"ZoomFactor";
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    state->Insert(key, PropertyValue::CreateSingle(m_zoom));

    key = L"Translation";
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    Point point;
    point.X = m_viewPosition.x;
    point.Y = m_viewPosition.y;
    state->Insert(key, PropertyValue::CreatePoint(point));

    key = L"FontFallback";
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    state->Insert(key, PropertyValue::CreateUInt32(m_main->GetFontFallbackId()));

    key = L"ColorGlyph";
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    state->Insert(key, PropertyValue::CreateInt32(m_main->ColorGlyphsEnabled() ? 1 : 0));
}

// Loads the current state of the app for resume events.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
    String^ key = L"ZoomFactor";
    if (state->HasKey(key))
    {
        m_zoom = safe_cast<IPropertyValue^>(state->Lookup(key))->GetSingle();
        m_main->SetZoom(m_zoom);
    }

    key = L"Translation";
    if (state->HasKey(key))
    {
        Point point = safe_cast<IPropertyValue^>(state->Lookup(key))->GetPoint();
        m_viewPosition.x = point.X;
        m_viewPosition.y = point.Y;

        // Pre-divide m_viewPosition by m_zoom to set the right offset translation.
        D2D1_POINT_2F pos = m_viewPosition;
        pos.x /= m_zoom;
        pos.y /= m_zoom;

        m_main->SetTranslation(pos);
    }

    key = L"FontFallback";
    if (state->HasKey(key))
    {
        unsigned int fallback = (safe_cast<IPropertyValue^>(state->Lookup(key))->GetUInt32());
        m_main->SetFontFallbackId(fallback);
    }

    key = L"ColorGlyph";
    if (state->HasKey(key))
    {
        int32 logical = safe_cast<IPropertyValue^>(state->Lookup(key))->GetInt32();

        m_main->UseColorGlyphs(logical ? true : false);
    }
}

// Called each time the DirectX content needs to be updated.
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
    m_deviceResources->ValidateDevice();
    RenderDXContent();
}

void DirectXPage::ShowSettingsMenu()
{
   // Make sure the ComboList is showing the correct scenario
    unsigned int fallback = m_main->GetFontFallbackId();

    auto size = FontFallbackListBox->Items->Size;
    for (unsigned int i = 0; i < size; i++)
    {
        ComboBoxItem^ x = dynamic_cast<ComboBoxItem^>(FontFallbackListBox->Items->GetAt(i));
        if (dynamic_cast<IPropertyValue^>(x->Tag)->GetUInt32() == fallback)
        {
            FontFallbackListBox->SelectedItem = x;
        }
    }

    SettingsGrid->Height = Window::Current->Bounds.Height;
    SettingsFlyout->HorizontalOffset = Window::Current->Bounds.Width - SettingsGrid->Width;
    ColorFontCheckBox->IsChecked = m_main->ColorGlyphsEnabled();
    SettingsFlyout->IsOpen = true;
    SettingsFlyout->Visibility = ::Visibility::Visible;
}

void DirectXPage::OnCommandsRequested(SettingsPane^ settingsPane, SettingsPaneCommandsRequestedEventArgs^ eventArgs)
{
    UICommandInvokedHandler^ handler = ref new UICommandInvokedHandler(this, &DirectXPage::OnSettingsCommand);

    SettingsCommand^ generalCommand = ref new SettingsCommand("generalSettings", "Settings", handler);
    eventArgs->Request->ApplicationCommands->Append(generalCommand);
}

void DirectXPage::OnSettingsCommand(IUICommand^ command)
{
    SettingsCommand^ settingsCommand = safe_cast<SettingsCommand^>(command);

    ShowSettingsMenu();
}

void DirectXPage::OnSettingsButtonClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
    ShowSettingsMenu();
    SampleAppBar->IsOpen = false;
}

void DirectXPage::OnColorFontChecked()
{
    m_main->UseColorGlyphs(true);
    RenderDXContent();
}

void DirectXPage::OnColorFontUnchecked()
{
    m_main->UseColorGlyphs(false);
    RenderDXContent();
}

void DirectXPage::OnScenarioSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    auto added = e->AddedItems;

    if (added->Size > 0)
    {
        m_main->SetFontFallbackId(
            dynamic_cast<IPropertyValue^>((dynamic_cast<ComboBoxItem^>(added->GetAt(0))->Tag))->GetUInt32()
            );
    }
    RenderDXContent();
}

// Input Event Handlers
void DirectXPage::OnPointerPressed(CoreWindow^ window, PointerEventArgs^ args)
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void DirectXPage::OnPointerReleased(CoreWindow^ window, PointerEventArgs^ args)
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void DirectXPage::OnPointerMoved(CoreWindow^ window, PointerEventArgs^ args)
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void DirectXPage::OnPointerWheelChanged(CoreWindow^ window, PointerEventArgs^ args)
{
    // We are ignoring the gesture that wheel movement means panning.
    // Zooming is indicated by holding the Control key down, so we
    // override this method to always detect a zoom wheel event.
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, FALSE, TRUE);
}

// In this method, we use the gesture recognizer data to update the transformation variables
// to reflect how the user is manipulating the image.
// The gesture recognizer provides the following data:
// 1. position (pixel offset from the screen origin to the pointer's current position)
// 2. positionDelta (pixel offset from the previous event's pointer to the current pointer)
// 3. zoomDelta (coefficient indicating the change in zoom)
void DirectXPage::OnManipulationUpdated(GestureRecognizer^ recognizer, ManipulationUpdatedEventArgs^ args)
{
    if (SettingsFlyout->IsOpen)
    {
        return;
    }

    Point position = args->Position;
    Point positionDelta = args->Delta.Translation;
    float zoomDelta = args->Delta.Scale;

    // Reposition the view to reflect translations.
    m_viewPosition.x += positionDelta.X;
    m_viewPosition.y += positionDelta.Y;

    // We want to have any zoom operation be "centered" around the pointer position, which
    // requires recalculating the view position based on the new zoom and pointer position.
    // Step 1: Calculate the absolute pointer position (image position).
    D2D1_POINT_2F pointerAbsolutePosition = D2D1::Point2F(
        (m_viewPosition.x - position.X) / m_zoom,
        (m_viewPosition.y - position.Y) / m_zoom
        );

    // Step 2: Apply the zoom operation and clamp to the min/max zoom.
    // zoomDelta is a coefficient for the change in zoom.
    m_zoom *= zoomDelta;
    m_zoom = Clamp(m_zoom, SampleConstants::UIMinZoom, SampleConstants::UIMaxZoom);

    // Step 3: Adjust the view position based on the new m_zoom value.
    m_viewPosition.x = pointerAbsolutePosition.x * m_zoom + position.X;
    m_viewPosition.y = pointerAbsolutePosition.y * m_zoom + position.Y;

    auto windowSize = m_deviceResources->GetLogicalSize();

    // Prevent the view from going outside the image boundaries.
    ClampViewPosition(&m_viewPosition, windowSize, m_zoom);

    m_main->SetZoom(m_zoom);

    // Pre-divide m_viewPosition by m_zoom to set the right offset translation.
    D2D1_POINT_2F pos = m_viewPosition;
    pos.x /= m_zoom;
    pos.y /= m_zoom;

    m_main->SetTranslation(pos);
    RenderDXContent();
}

void DirectXPage::OnTapped(GestureRecognizer^ recognizer, TappedEventArgs^ args)
{
    if (SettingsFlyout->IsOpen)
    {
        return;
    }

    if (args->TapCount == 2)
    {
        m_main->SetZoom(m_zoom = 1.0f);
        m_viewPosition.x = 0.0f;
        m_viewPosition.y = 0.0f;
        m_main->SetTranslation(m_viewPosition);
        RenderDXContent();
    }
}
