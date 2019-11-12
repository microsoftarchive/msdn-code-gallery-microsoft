//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2_DeviceIO.g.h"

namespace SDKSample
{
    namespace CustomDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceIO sealed
        {
        public:
            DeviceIO();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void DeviceIOGet_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeviceIOSet_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
