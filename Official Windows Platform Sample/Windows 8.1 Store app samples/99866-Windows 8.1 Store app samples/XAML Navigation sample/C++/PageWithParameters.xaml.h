//
// PageWithParameters.xaml.h
// Declaration of the PageWithParameters class
//

#pragma once

#include "PageWithParameters.g.h"
#include "PageWithParametersConfiguration.h"

namespace SDKSample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class PageWithParameters sealed
	{
	public:
		PageWithParameters();
	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
	
	private:
		SDKSample::Navigation::PageWithParametersConfiguration^ _pageParameters;

	};
}
