//
// SimplePageType2Cacheable.xaml.h
// Declaration of the SimplePageType2Cacheable class
//

#pragma once

#include "SimplePageType2Cacheable.g.h"

namespace SDKSample
{
	static int _numberSimplePageType2Cacheable;

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class SimplePageType2Cacheable sealed
	{
	public:
		SimplePageType2Cacheable();

		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		bool _newPage;
	};
}
