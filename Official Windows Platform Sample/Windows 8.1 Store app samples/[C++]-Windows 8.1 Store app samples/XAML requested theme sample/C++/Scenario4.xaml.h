//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once
#include "Scenario4.g.h"

namespace SDKSample
{
    namespace RequestedThemeCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario4 sealed
        {
        public:
            Scenario4();

        private:
            void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RevertRequestedTheme(Windows::UI::Xaml::FrameworkElement^ fe);
        };
    }
}
