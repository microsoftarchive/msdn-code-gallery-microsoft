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
#include "Scenario3.g.h"
#include "UserSettings.h"

using namespace SDKSample::RequestedThemeCPP;


namespace SDKSample
{
    namespace RequestedThemeCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();
		

        private:
			UserSettings^ _userSettings;
            void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void RevertRequestedTheme();
        };
    }
}
