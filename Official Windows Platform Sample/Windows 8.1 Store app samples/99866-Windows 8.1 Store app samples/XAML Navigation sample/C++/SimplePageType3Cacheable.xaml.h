//
// SimplePageType3Cacheable.xaml.h
// Declaration of the SimplePageType3Cacheable class
//

#pragma once

#include "SimplePageType3Cacheable.g.h"

namespace SDKSample
{
	static int _numberSimplePageType3Cacheable;

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class SimplePageType3Cacheable sealed
	{
	public:
		SimplePageType3Cacheable();

		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		bool _newPage;

	};
}
