//
// GridViewPage.xaml.h
// Declaration of the GridViewPage class
//

#pragma once

#include "pch.h"
#include "GridViewPage.g.h"
#include "MainPage.xaml.h"
#include "IceCream.h"

namespace SDKSample
{
	namespace AppBarControl
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class GridViewPage sealed
		{
		public:
			GridViewPage();

		private:
			MainPage^ rootPage;
			Platform::Collections::Vector<IceCream^>^ Flavors;
			Platform::Collections::Vector<IceCream^>^ items;
			IceCream^ GenerateItem();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

			void OnSelectionChanged(Platform::Object ^sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs ^e);
		private:
			void SelectAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Clear_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Delete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Add_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		};
	}
}
