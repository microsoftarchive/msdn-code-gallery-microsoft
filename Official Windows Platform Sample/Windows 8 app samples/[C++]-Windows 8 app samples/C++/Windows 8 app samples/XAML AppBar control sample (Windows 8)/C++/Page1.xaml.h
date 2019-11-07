//
// Page1.xaml.h
// Declaration of the Page1 class
//

#pragma once

#include "pch.h"
#include "GlobalPage.xaml.h"
#include "Page1.g.h"

namespace AppBarControl
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class Page1 sealed
	{
	public:
		Page1();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
	private:
		void PageTwo_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		static GlobalPage^ rootPage;
	};
}
