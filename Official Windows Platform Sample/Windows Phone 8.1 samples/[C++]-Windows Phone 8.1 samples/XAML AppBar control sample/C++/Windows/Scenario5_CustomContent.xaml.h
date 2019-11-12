//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario5.xaml.h
// Declaration of the Scenario5 class
//

#pragma once
#include "Scenario5_CustomContent.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace AppBarControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario5 sealed
        {
        public:
            Scenario5();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		private:
			MainPage^ rootPage;
			Windows::UI::Xaml::Controls::StackPanel^ leftPanel;
			Platform::Collections::Vector<UIElement^>^ leftItems;
			void tb_GotFocus(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void b_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
