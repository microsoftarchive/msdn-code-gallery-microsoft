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
#include "S3_ResetPin.g.h"

namespace SDKSample
{
    namespace SmartCardSample
    {
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();

        private:
            void ResetPin_Click(Platform::Object^ sender,
                                Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
