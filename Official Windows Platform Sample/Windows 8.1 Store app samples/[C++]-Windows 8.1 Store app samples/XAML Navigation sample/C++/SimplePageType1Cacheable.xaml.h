//
// SimplePageType1Cacheable.xaml.h
// Declaration of the SimplePageType1Cacheable class
//

#pragma once

#include "SimplePageType1Cacheable.g.h"

namespace SDKSample
{
	static int _numberSimplePageType1Cacheable;

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class SimplePageType1Cacheable sealed
	{
	public:
		SimplePageType1Cacheable();

		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		
	private:
		 bool _newPage;
	};
}
