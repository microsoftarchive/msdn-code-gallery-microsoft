//
// Page3.xaml.cpp
// Implementation of the Page3 class
//

#include "pch.h"
#include "Page3.xaml.h"
#include "App.xaml.h"

using namespace AppBarControl;

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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
MainPage^ Page3::rootPage = nullptr;

Page3::Page3()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Page3::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = (MainPage^)e->Parameter;
	BottomAppBar->IsOpen = true;
}

void Page3::Back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage->Frame->GoBack();
}

void Page3::RemoveSaveButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (Save != nullptr)
	{
		unsigned int index;
		LeftPanel->Children->IndexOf(Save, &index);
		LeftPanel->Children->RemoveAt(index);
	}
}

void Page3::AddFavoriteButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ favButton = ref new Button();
	favButton->Style = safe_cast<Windows::UI::Xaml::Style^>(App::Current->Resources->Lookup("FavoriteAppBarButtonStyle"));
	LeftPanel->Children->InsertAt(2, favButton);
}
