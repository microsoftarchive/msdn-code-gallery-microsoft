//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MainPage.g.h"
#include "PostcardRenderer.h"

namespace Postcard
{
    public ref class MainPage sealed
    {
    public:
        MainPage();

    private:
        void OnLogicalDpiChanged(
            Platform::Object^ sender
            );
        void OnWindowSizeChanged(
            Windows::UI::Core::CoreWindow^ sender,
            Windows::UI::Core::WindowSizeChangedEventArgs^ args
            );
        void OnPointerPressed(
            Platform::Object^ sender,
            Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
            );
        void OnPointerReleased(
            Platform::Object^ sender,
            Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
            );
        void OnPointerMoved(
            Platform::Object^ sender,
            Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
            );

        void AddImage();
        void AddEffectsClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void EffectIntensitySliderChanged(Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e);
        void AddConstructionPaperClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PaperFlyoutAddPaperClicked();
        void PaperFlyoutDeletePaperClicked();
        void PaperFlyoutMoveClicked();
        void PaperFlyoutStampClicked();
        void AddTextClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void TextSubmitted(Platform::String^ text);
        void AddSignatureClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SignClicked();
        void EraseClicked();
        void SaveClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ShareClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ResetClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        PostcardRenderer^ m_renderer;
        bool m_atStartScreen;
    };
}
