//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_ReadCharacteristicValue.xaml.h
// Declaration of the Scenario2_ReadCharacteristicValue class
//

#pragma once
#include "Scenario2_ReadCharacteristicValue.g.h"

namespace SDKSample
{
    namespace BluetoothGattHeartRate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario2_ReadCharacteristicValue sealed
        {
        public:
            Scenario2_ReadCharacteristicValue();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void ReadValueButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
