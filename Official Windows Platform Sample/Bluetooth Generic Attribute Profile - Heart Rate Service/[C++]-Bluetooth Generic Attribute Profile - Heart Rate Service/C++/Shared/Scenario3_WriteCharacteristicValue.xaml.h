//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3_WriteCharacteristicValue.xaml.h
// Declaration of the Scenario3_WriteCharacteristicValue class
//

#pragma once
#include "Scenario3_WriteCharacteristicValue.g.h"

namespace SDKSample
{
    namespace BluetoothGattHeartRate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario3_WriteCharacteristicValue sealed
        {
        public:
            Scenario3_WriteCharacteristicValue();

        protected:
            void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void WriteCharacteristicValue_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Instance_ValueChangeCompleted(HeartRateMeasurement^ heartRateMeasurementValue);
            
            Windows::Foundation::EventRegistrationToken valueChangeCompletedRegistrationToken;
        };
    }
}
