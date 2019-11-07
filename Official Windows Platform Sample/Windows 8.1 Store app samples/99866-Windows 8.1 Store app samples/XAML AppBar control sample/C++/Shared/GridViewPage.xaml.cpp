//
// GridViewPage.xaml.cpp
// Implementation of the GridViewPage class
//

#include "pch.h"
#include "GridViewPage.xaml.h"
#include "MainPage.xaml.h"
#include <time.h>

using namespace SDKSample;
using namespace SDKSample::AppBarControl;

using namespace Platform;
using namespace Windows::UI::Popups;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

//MainPage^ MainPage::Current = nullptr;

GridViewPage::GridViewPage()
{
	InitializeComponent();
	rootPage = MainPage::Current;

	// see our random number gnerator for the data creation
	srand((unsigned int) time(NULL));

	Flavors = ref new Platform::Collections::Vector<IceCream^>();
	IceCreamList->SelectionChanged += ref new Windows::UI::Xaml::Controls::SelectionChangedEventHandler(this, &SDKSample::AppBarControl::GridViewPage::OnSelectionChanged);
}

void GridViewPage::OnSelectionChanged(Platform::Object ^sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs ^e)
{
	GridView^ gv = safe_cast<GridView^>(sender) ;
	if (gv != nullptr)
	{
		if (gv->SelectedItem != nullptr)
		{
			// We have selected items so show the AppBar and make it sticky
			BottomAppBar->IsSticky = true;
			BottomAppBar->IsOpen = true;
		}
		else
		{
			// No selections so hide the AppBar and don't make it sticky any longer
			BottomAppBar->IsSticky = false;
			BottomAppBar->IsOpen = false;
		}
	}
}
void GridViewPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Populate our collection of ice cream flavors
	for (int i = 0; i < 50; i++)
	{
		Flavors->Append(GenerateItem());
	}

	IceCreamList->ItemsSource = Flavors;
}

IceCream^ GridViewPage::GenerateItem()
{
	int type = (std::rand() % 6);
	IceCream^ flavor = ref new IceCream();

	// Randomnly create data items
	switch (type)
	{
	case 1:
		flavor->Name = "Banana Blast";
		flavor->Type = "Low-fat Frozen Yogurt";
		flavor->Image = "60Banana.png";
		break;
	case 2:
		flavor->Name = "Lavish Lemon Ice";
		flavor->Type = "Sorbet";
		flavor->Image = "60Lemon.png";
		break;
	case 3:
		flavor->Name = "Marvelous Mint";
		flavor->Type = "Gelato";
		flavor->Image = "60Mint.png";
		break;
	case 4:
		flavor->Name = "Creamy Orange";
		flavor->Type = "Sorbet";
		flavor->Image = "60Orange.png";
		break;
	case 5:
		flavor->Name = "Very Vanilla";
		flavor->Type = "Ice Cream";
		flavor->Image = "60Vanilla.png";
		break;
	default:
		flavor->Name = "Succulent Strawberry";
		flavor->Type = "Sorbet";
		flavor->Image = "60Strawberry.png";
	}
	return flavor;
}

void SDKSample::AppBarControl::GridViewPage::SelectAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	IceCreamList->SelectAll();
}


void SDKSample::AppBarControl::GridViewPage::Clear_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	IceCreamList->SelectedIndex = -1;
}


void SDKSample::AppBarControl::GridViewPage::Delete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	items = ref new Platform::Collections::Vector<IceCream^>();
	for each (Object^ item in IceCreamList->SelectedItems)
	{
		items->Append(safe_cast<IceCream^>(item));
	}

	for each (IceCream^ item in items)
	{
		IceCream^ flavor = safe_cast<IceCream^>(item);
		unsigned int index = 0;
		Flavors->IndexOf(flavor, &index);
		Flavors->RemoveAt(index);
	}
}


void SDKSample::AppBarControl::GridViewPage::Add_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	MessageDialog^ d = ref new MessageDialog("XAML AppBar Control Sample");
	d->Content = "Add button pressed";
	d->ShowAsync();
}


void SDKSample::AppBarControl::GridViewPage::Back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage->Frame->GoBack();
}
