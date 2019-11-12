//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "Scenario2.g.h"

namespace SDKSample
{
	namespace Navigation
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario2 sealed
		{
		public:
			Scenario2();
		protected:
			void NavigateButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		};
	}
}
