//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1_DeviceConnect.g.h"

namespace SDKSample
{
    namespace CustomDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceConnect sealed
        {
        public:
            DeviceConnect();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;

            void deviceConnectStart_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void deviceConnectStop_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void deviceConnectDevices_SelectionChanged_1(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

            void UpdateStartStopButtons(void);

            Concurrency::task<void> OpenFx2DeviceAsync(Platform::String^ Id);
            void CloseFx2Device(void);

        };
    }
}
