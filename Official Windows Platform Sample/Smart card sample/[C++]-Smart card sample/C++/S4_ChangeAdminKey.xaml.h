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
#include "S4_ChangeAdminKey.g.h"

namespace SDKSample
{
    namespace SmartCardSample
    {
        public ref class Scenario4 sealed
        {
        public:
            Scenario4();

        private:
            void ChangeAdminKey_Click(Platform::Object^ sender,
                                      Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
