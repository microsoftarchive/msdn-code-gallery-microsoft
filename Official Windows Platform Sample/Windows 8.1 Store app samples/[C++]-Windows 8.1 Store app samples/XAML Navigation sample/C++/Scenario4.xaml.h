//
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once

#include "Scenario4.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	namespace Navigation
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario4 sealed
		{
		public:
			Scenario4();
			
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			void Navigate1ButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Navigate2ButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Navigate3ButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void GoBackButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void GoForwardButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void GoHomeButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ClearStacksButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ShowInfo();
			void UpdateUI();
		};
	}
}
