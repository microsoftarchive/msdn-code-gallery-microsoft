//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once
#include "Scenario3_DeviceEvents.g.h"

namespace SDKSample
{
    namespace CustomDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceEvents sealed
        {
        public:
            DeviceEvents();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;

            Concurrency::cancellation_token_source cancelSource;
            bool running;
            
            Windows::Foundation::EventRegistrationToken switchChangedEventsHandler;
            Platform::Array<bool>^ previousSwitchValues;

            void deviceEventsGet_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void deviceEventsBegin_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void deviceEventsCancel_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void StartInterruptMessageWorker(Windows::Devices::Custom::CustomDevice^ fx2Device);
            void InterruptMessageWorker(Windows::Devices::Custom::CustomDevice^ fx2Device, 
                                                  Windows::Storage::Streams::IBuffer^ switchMessageBuffer);

            void UpdateRegisterButton(void);
            //void OnSwitchStateChangedEvent(Samples::Devices::Fx2::Fx2Device^ sender, Samples::Devices::Fx2::SwitchStateChangedEventArgs^ e);

            void ClearSwitchStateTable();
            void UpdateSwitchStateTable(Platform::Array<bool>^ SwitchStateArray);

            //void Current_DeviceClosing(Platform::Object^ sender, Samples::Devices::Fx2::Fx2Device^ device);

            Platform::Array<bool>^ CreateSwitchStateArray(byte output [])
            {
                auto switchStateArray = ref new Platform::Array<bool>(8);

                for (auto i = 0; i < 8; i += 1)
                {
                    switchStateArray[i] = (output[0] & (1 << i)) != 0;
                }

                return switchStateArray;
            }
        };
    }
}
