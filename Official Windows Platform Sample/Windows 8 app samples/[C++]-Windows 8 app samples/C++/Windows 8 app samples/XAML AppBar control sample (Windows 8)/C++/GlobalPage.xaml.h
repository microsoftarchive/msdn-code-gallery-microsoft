//
// GlobalPage.xaml.h
// Declaration of the GlobalPage class
//

#pragma once

#include "pch.h"
#include "MainPage.xaml.h"
#include "GlobalPage.g.h"

namespace AppBarControl
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class GlobalPage sealed
	{
	public:
		GlobalPage();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		void Back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

	//internal:
        static MainPage^ rootPage;
	};

	
}
