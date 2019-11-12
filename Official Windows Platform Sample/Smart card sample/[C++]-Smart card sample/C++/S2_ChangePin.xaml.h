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
#include "S2_ChangePin.g.h"

namespace SDKSample
{
    namespace SmartCardSample
    {
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();

        private:
            void ChangePin_Click(Platform::Object^ sender,
                                 Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
