// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


#pragma once

#include "pch.h"
#include "ExtendedSplash.g.h"

namespace SDKSample
{
    namespace SplashScreen
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ExtendedSplash sealed
        {
        public:
            ExtendedSplash(Windows::ApplicationModel::Activation::SplashScreen^ splash, bool loadState);

        private:
            Windows::ApplicationModel::Activation::SplashScreen^ splash; // Variable to hold the splash screen object.
            void DismissedEventHandler(Windows::ApplicationModel::Activation::SplashScreen^ sender, Object^ e);
            void LearnMoreButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ExtendedSplash_OnResize(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
            void PositionImage();

        internal:
            Windows::Foundation::Rect splashImageRect; // Rect to store splash screen image coordinates.
            bool dismissed; // Variable to track splash screen dismissal status.
            Windows::UI::Xaml::Controls::Frame^ rootFrame;
        };
    }
}
