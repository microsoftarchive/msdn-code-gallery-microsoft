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
#include "S6_DeleteTPMVSC.g.h"

namespace SDKSample
{
    namespace SmartCardSample
    {
        public ref class Scenario6 sealed
        {
        public:
            Scenario6();

        private:
            void Delete_Click(Platform::Object^ sender,
                              Windows::UI::Xaml::RoutedEventArgs^ e);
            void HandleCardRemoved(
                Windows::Devices::SmartCards::SmartCardReader^ evtReader,
                Windows::Devices::SmartCards::CardRemovedEventArgs^ args);

            Windows::Devices::SmartCards::SmartCardReader ^reader;
        };
    }
}
