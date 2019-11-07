//
// Page2.xaml.h
// Declaration of the Page2 class
//

#pragma once

#include "Page2.g.h"
#include "SampleData/SampleDataSource.h"

using namespace SDKSample::ExpressionBlendSampleDataSampleDataSource;

namespace SDKSample
{
namespace PersonalityAnimations
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class Page2 sealed
	{
	public:
		Page2();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		MessageData^ messageData;
	};
}
}
