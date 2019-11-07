//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3-LastPrefetchTime.xaml.h
// Declaration of the LastPrefetchTimeScenario class
//

#pragma once
#include "S3-LastPrefetchTime.g.h"

namespace SDKSample
{
    namespace SDKTemplate
    {
        public ref class LastPrefetchTimeScenario sealed
        {
        public:
            LastPrefetchTimeScenario();

        private:
            void GetLastPrefetchTime_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
