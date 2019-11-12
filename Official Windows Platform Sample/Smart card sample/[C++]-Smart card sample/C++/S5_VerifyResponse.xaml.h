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
#include "S5_VerifyResponse.g.h"

namespace SDKSample
{
    namespace SmartCardSample
    {
        public ref class Scenario5 sealed
        {
        public:
            Scenario5();

        private:
            void VerifyResponse_Click(Platform::Object^ sender,
                                      Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
