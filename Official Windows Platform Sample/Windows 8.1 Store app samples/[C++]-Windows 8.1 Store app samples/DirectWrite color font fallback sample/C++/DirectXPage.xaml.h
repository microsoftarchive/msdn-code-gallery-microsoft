//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXPage.g.h"

#include "DeviceResources.h"
#include "DWriteColorFontFallbackMain.h"
#include "DWriteColorFontFallback.h"

namespace DWriteColorFontFallback
{
    /// <summary>
    /// A page that hosts a DirectX SwapChainBackgroundPanel.
    /// This page must be the root of the Window content (it cannot be hosted on a Frame).
    /// </summary>
    public ref class DirectXPage sealed
    {
    public:
        DirectXPage();

        void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

    private:
        // Invoke the renderer to update the DX SwapChainPanel content.
        void RenderDXContent();

        // Window event handlers.
        void OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
        void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);

        // DisplayInformation event handlers.
        void OnDpiChanged(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
        void OnOrientationChanged(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
        void OnDisplayContentsInvalidated(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);

        // Input CoreWindwo event handlers
        void OnPointerPressed(
            Windows::UI::Core::CoreWindow^ window,
            Windows::UI::Core::PointerEventArgs^ args
            );

        void OnPointerReleased(
            Windows::UI::Core::CoreWindow^ window,
            Windows::UI::Core::PointerEventArgs^ args
            );

        void OnPointerMoved(
            Windows::UI::Core::CoreWindow^ window,
            Windows::UI::Core::PointerEventArgs^ args
            );

        void OnPointerWheelChanged(
            Windows::UI::Core::CoreWindow^ window,
            Windows::UI::Core::PointerEventArgs^ args
            );

        // Declare GestureRecognizer Event Handlers
        void OnManipulationUpdated(
            Windows::UI::Input::GestureRecognizer^ recognizer,
            Windows::UI::Input::ManipulationUpdatedEventArgs^ args
            );

        void OnTapped(
            Windows::UI::Input::GestureRecognizer^ recognizer,
            Windows::UI::Input::TappedEventArgs^ args
            );

        // Event handlers for AppBar
        void OnAppBarOpened(
            Platform::Object^ sender,
            Platform::Object^ args
            );

        void OnSettingsButtonClicked(
            Platform::Object^ sender,
            Windows::UI::Xaml::RoutedEventArgs^ args
            );

        void OnSettingsCommand(
            Windows::UI::Popups::IUICommand^ command
            );

        void OnCommandsRequested(
            Windows::UI::ApplicationSettings::SettingsPane^ settingsPane,
            Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs^ eventArgs
            );

        // Settings Menu Event handlers
        void ShowSettingsMenu();
        void OnColorFontChecked();
        void OnColorFontUnchecked();
        void OnScenarioSelectionChanged(
            Platform::Object^ sender,
            Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e
            );

        // Resources used to render the DirectX content in the XAML page background.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        std::unique_ptr<DWriteColorFontFallbackMain> m_main;
        bool m_windowVisible;

        // Resource used to keep track of the Settings Charm
        Windows::Foundation::EventRegistrationToken m_commandsRequestedToken;

        Platform::Collections::Vector<Object^>^ m_fallbackScenarioList;

        // Declare the gesture recognizer
        Platform::Agile<Windows::UI::Input::GestureRecognizer> m_gestureRecognizer;
        D2D1_POINT_2F m_viewPosition;
        float m_zoom;

        // Private helper functions for use in computing Pan and Zoom
        inline static float Clamp(float v, float low, float high)
        {
            return (v < low) ? low : (v > high) ? high : v;
        }

        inline static float LinearInterpolate(float start, float end, float weight)
        {
            return start * (1.0f - weight) + end * weight;
        }

        inline static void ClampViewPosition(
            D2D1_POINT_2F *viewPosition,
            Windows::Foundation::Size windowSize,
            float zoom
            )
        {
            viewPosition->x = Clamp(viewPosition->x, windowSize.Width * (1.0f - zoom), 0);
            viewPosition->y = Clamp(viewPosition->y, windowSize.Height * (1.0f - zoom), 0);
        }
    };
}
