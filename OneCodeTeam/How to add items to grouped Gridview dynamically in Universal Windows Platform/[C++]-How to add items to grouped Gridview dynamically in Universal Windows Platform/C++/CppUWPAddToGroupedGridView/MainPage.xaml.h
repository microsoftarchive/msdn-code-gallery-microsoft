//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"
#include "StoreData.h"
using namespace CppUWPAddToGroupedGridView::SampleData;


namespace CppUWPAddToGroupedGridView
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

	private:
		/// <summary>
		/// The data source for the grouped grid view.
		/// </summary>
		Windows::Foundation::Collections::IObservableVector<GroupInfoCollection^>^  source;		

		void linkFooter_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void AddButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void GroupComboBox_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
		void ItemsByCategory_LayoutUpdated(Platform::Object^ sender, Platform::Object^ e);
	};
}
