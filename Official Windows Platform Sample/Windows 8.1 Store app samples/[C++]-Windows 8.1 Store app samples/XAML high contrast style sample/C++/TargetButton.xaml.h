//
// TargetButton.xaml.h
// Declaration of the TargetButton class
//

#pragma once

#include "TargetButton.g.h"

namespace SDKSample
{
	namespace HighContrast
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class TargetButton sealed 
		{
		public:
			TargetButton();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);
			virtual void OnApplyTemplate() override;
		};
	}
}
