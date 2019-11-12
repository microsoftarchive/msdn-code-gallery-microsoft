//
// Page2.xaml.h
// Declaration of the Page2 class
//

#pragma once

#include "pch.h"
#include "GlobalPage.xaml.h"
#include "Page2.g.h"

namespace AppBarControl
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class Page2 sealed
	{
	public:
		Page2();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;

	private:
		GlobalPage^ rootPage;
		Windows::UI::Xaml::Controls::Button^ starButton;
		Windows::UI::Xaml::Controls::StackPanel^ rightPanel;
		void starButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
