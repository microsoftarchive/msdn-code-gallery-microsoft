//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace CppUWPAddToGroupedGridView;
using namespace CppUWPAddToGroupedGridView::SampleData;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409



MainPage::MainPage()
{
	InitializeComponent();
	source = (ref new StoreData())->GetGroupsByCategory();
	CollectionViewSource->Source = source;

	Vector<Platform::String^>^ pictureOptions = ref new Vector<Platform::String^>();
	pictureOptions->Append("Banana");
	pictureOptions->Append("Lemon");
	pictureOptions->Append("Mint");
	pictureOptions->Append("Orange");
	pictureOptions->Append("SauceCaramel");
	pictureOptions->Append("SauceChocolate");
	pictureOptions->Append("SauceStrawberry");
	pictureOptions->Append("SprinklesChocolate");
	pictureOptions->Append("SprinklesRainbow");
	pictureOptions->Append("SprinklesVanilla");
	pictureOptions->Append("Strawberry");
	pictureOptions->Append("Vanilla");
	PictureComboBox->ItemsSource = pictureOptions;
	PictureComboBox->SelectedIndex = 0;
	Vector<Platform::String^>^ groupOptions = ref new Vector<Platform::String^>();
	for each (auto val in source)
	{
		groupOptions->Append(val->Key);
	}
	GroupComboBox->ItemsSource = groupOptions;
	GroupComboBox->SelectedIndex = 0;
}


void CppUWPAddToGroupedGridView::MainPage::linkFooter_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	HyperlinkButton^ hyperlinkButton = safe_cast<HyperlinkButton^>(sender);;
	if (hyperlinkButton != nullptr) {
		Launcher::LaunchUriAsync(ref new Uri(hyperlinkButton->Tag->ToString()));
	}
}

/// <summary>
/// The event handler for the click event of the AddItem button. 
/// The method creates a new object of type Item.
/// From the observable collection containing the groups, the collection 
/// for the selected group is selected. Then the new item is added to the collection. As both 
/// collections are observable and are connected to the controls using data
/// binding, the list will automatically update. 
/// </summary>
/// <param name="sender">
/// The sender.
/// </param>
/// <param name="e">
/// The event arguments.
/// </param>
void CppUWPAddToGroupedGridView::MainPage::AddButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	Platform::String^ path =Platform::String::Concat(Platform::String::Concat("60", PictureComboBox->SelectedItem), ".png");
	Item^ item = ref new Item(TitleTextBox->Text, GroupComboBox->SelectedItem->ToString());
	item->ImageUrl = path;
	int index = 0;
	while (index < source->Size)
	{
		if (source->GetAt(index)->Key == item->Category) {
			source->GetAt(index)->Items->Append(item);
			break;
		}
		index++;
	}
}

/// <summary>
/// If the selection in the combo-box for the group-selection changes, 
/// the grouped grid view scrolls the selected group into view. This is 
/// especially useful in narrow views. 
/// </summary>
/// <param name="sender">
/// The sender.
/// </param>
/// <param name="e">
/// The event arguments.
/// </param>
void CppUWPAddToGroupedGridView::MainPage::GroupComboBox_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	Platform::String^ key = GroupComboBox->SelectedItem->ToString();
	GroupInfoCollection^ groupInfoCollection;
	int index = 0;
	while (index < source->Size)
	{
		if (source->GetAt(index)->Key == key) {
			ItemsByCategory->ScrollIntoView(source->GetAt(index));
			return;
		}
		index++;
	}
}

void CppUWPAddToGroupedGridView::MainPage::ItemsByCategory_LayoutUpdated(Platform::Object^ sender, Platform::Object^ e)
{
		Platform::String^ key = GroupComboBox->SelectedItem->ToString();
		int index = 0;
		while (index < source->Size)
		{
			if (source->GetAt(index)->Key == key) {
				ItemsByCategory->ScrollIntoView(source->GetAt(index));				
				return;
			}
			index++;
		}
}
