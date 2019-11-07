// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2.g.h"

namespace SDKSample
{
    namespace DateAndTimePickers
    {
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();

        private:
            void changeDate_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void changeYearRange_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
