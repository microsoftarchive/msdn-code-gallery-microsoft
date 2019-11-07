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
#include "Scenario2_CustomizeColor.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace AppBarControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario2 sealed
        {
        public:
            Scenario2();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

		private:
			MainPage^ rootPage;
			Windows::UI::Xaml::Controls::StackPanel^ leftPanel;
			Windows::UI::Xaml::Controls::StackPanel^ rightPanel;
			Windows::UI::Xaml::Media::Brush^ originalBackgroundBrush;
			Windows::UI::Xaml::Media::Brush^ originalSeparatorBrush;
			Windows::UI::Xaml::Style^ originalButtonStyle;
			void ColorButtons(Windows::UI::Xaml::Controls::StackPanel^ panel);
			void RestoreButtons(Windows::UI::Xaml::Controls::StackPanel^ panel);
		};
    }
}
