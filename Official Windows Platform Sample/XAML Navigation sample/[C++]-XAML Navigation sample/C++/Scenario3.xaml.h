//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "Scenario3.g.h"

namespace SDKSample
{
	namespace Navigation
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario3 sealed
		{
		public:
			Scenario3();
		protected:
			void NavigateButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		};
	}
}
