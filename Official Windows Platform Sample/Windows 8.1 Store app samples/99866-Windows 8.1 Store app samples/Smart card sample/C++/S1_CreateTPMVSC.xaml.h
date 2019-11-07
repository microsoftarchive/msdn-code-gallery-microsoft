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
#include "S1_CreateTPMVSC.g.h"

namespace SDKSample
{
    namespace SmartCardSample
    {
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        private:
            static const int pinPolicyDisallowedIndex = 0;
            static const int pinPolicyAllowedIndex = 1;
            static const int pinPolicyRequireOneIndex = 2;

            void Create_Click(Platform::Object^ sender,
                              Windows::UI::Xaml::RoutedEventArgs^ e);
            void HandleCardAdded(
                Windows::Devices::SmartCards::SmartCardReader^ evtReader,
                Windows::Devices::SmartCards::CardAddedEventArgs^ args);
            Windows::Devices::SmartCards::SmartCardPinPolicy^ ParsePinPolicy();

            Windows::Devices::SmartCards::SmartCardReader^ reader;
        };
    }
}
