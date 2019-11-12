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
#include "Scenario1.g.h"

namespace SDKSample
{
    namespace Projection
    {
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        private:
            void StartProjecting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void StopProjecting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            int thisViewId;
        };
    }
}
