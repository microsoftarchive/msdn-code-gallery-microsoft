//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LaunchFile.xaml.h
// Declaration of the LaunchFile class
//

#pragma once
#include "Scenario1_LaunchFile.g.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
#endif

namespace SDKSample
{
    namespace AssociationLaunching
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class LaunchFile sealed
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
            : IFileOpenPickerContinuable
#endif
        {
        public:
            LaunchFile();
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
            virtual void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args);
#endif
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void LaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchFileWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchFileOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchFileSplitScreenButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void PickAndLaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            static Windows::Foundation::Point GetOpenWithPosition(Windows::UI::Xaml::FrameworkElement^ element);
            static Platform::String^ fileToLaunch;
            void OnPage_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
        };
    }
}
