//
// CancelNavigationPage.xaml.h
// Declaration of the CancelNavigationPage class
//

#pragma once

#include "CancelNavigationPage.g.h"

namespace SDKSample
{
	static int _numberCancelNavigationPages;

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class CancelNavigationPage sealed
	{
	public:
		CancelNavigationPage();

		property int ID
		{
			int get()
			{
				return _ID;
			}
			void set(int value)
			{
				_ID = value;
			}
		}
	protected:
		virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;

	private:
		int _ID;
	};
}
