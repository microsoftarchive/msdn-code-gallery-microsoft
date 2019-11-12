//
// Scenario10.xaml.h
// Declaration of the Scenario10 class
//

#pragma once

#include "Scenario10_FallbackValueAndTargetNullValue.g.h"
#include "MainPage.xaml.h"
#include "Employee.h"

namespace SDKSample
{
	namespace DataBinding
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario10 sealed
		{
		public:
			Scenario10();

		private:
			property Employee^ _employee;
			MainPage^ rootPage;
			void ScenarioReset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void EmployeeChanged(Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e);
			void ShowEmployeeInfo(Platform::String^ content);
			void AgeTextBoxLostFocus(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		protected:
		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		};
	}
}
