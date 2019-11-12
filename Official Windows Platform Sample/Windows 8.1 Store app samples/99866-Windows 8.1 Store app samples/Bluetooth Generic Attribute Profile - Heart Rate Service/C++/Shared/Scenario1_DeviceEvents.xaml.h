//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_DeviceEvents.xaml.h
// Declaration of the Scenario1_DeviceEvents class
//

#pragma once
#include "Scenario1_DeviceEvents.g.h"

namespace SDKSample
{
    namespace BluetoothGattHeartRate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario1_DeviceEvents sealed
        {
        public:
            Scenario1_DeviceEvents();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void Instance_ValueChangeCompleted(HeartRateMeasurement^ heartRateMeasurement);

            void RunButton_Click(
                Platform::Object^ sender, 
                Windows::UI::Xaml::RoutedEventArgs^ e);

            void DevicesListBox_Tapped(
                Platform::Object^ sender, 
                Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);

            void OutputDataChart_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
            void OnDeviceConnectionUpdated(boolean isConnected);

            Windows::Devices::Enumeration::DeviceInformationCollection^ devices;
            Windows::Foundation::EventRegistrationToken valueChangeCompletedRegistrationToken;
        };
    }
}
