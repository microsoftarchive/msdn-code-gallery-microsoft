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
#include "MainPage.xaml.h"

namespace SDKSample
{
	namespace Navigation
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario1 sealed
		{
		public:
			Scenario1();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			void NavigateButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void GoBackButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void GoForwardButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void GoHomeButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ClearStacksButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ShowInfo();
			void UpdateUI();
	
		};

	}
}
