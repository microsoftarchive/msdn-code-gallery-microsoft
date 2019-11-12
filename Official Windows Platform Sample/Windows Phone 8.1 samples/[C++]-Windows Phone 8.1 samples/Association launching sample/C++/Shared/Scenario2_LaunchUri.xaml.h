//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LaunchUri.xaml.h
// Declaration of the LaunchUri class
//

#pragma once
#include "Scenario2_LaunchUri.g.h"

namespace SDKSample
{
    namespace AssociationLaunching
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class LaunchUri sealed
        {
        public:
            LaunchUri();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void LaunchUriButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchUriWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchUriOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchUriSplitScreenButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            static Windows::Foundation::Point GetOpenWithPosition(Windows::UI::Xaml::FrameworkElement^ element);
            static Platform::String^ uriToLaunch;
            void OnPage_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
        };
    }
}
