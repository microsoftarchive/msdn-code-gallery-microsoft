// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"

namespace SDKSample
{
    namespace DateAndTimePickers
    {
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        private:
			void datePicker_DateChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::DatePickerValueChangedEventArgs^ e);
			void timePicker_TimeChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TimePickerValueChangedEventArgs^ e);

        };
    }
}
