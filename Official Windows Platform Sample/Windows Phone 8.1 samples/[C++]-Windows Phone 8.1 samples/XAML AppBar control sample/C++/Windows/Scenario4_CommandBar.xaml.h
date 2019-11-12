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
#include "Scenario4_CommandBar.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace AppBarControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario4 sealed
        {
        public:
            Scenario4();

        private:
			MainPage^ rootPage;
			Windows::UI::Xaml::Controls::AppBar^ originalAppBar;

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
