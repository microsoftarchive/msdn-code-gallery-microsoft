//
// Page2.xaml.cpp
// Implementation of the Page2 class
//

#include "pch.h"
#include "App.xaml.h"
#include "Page2.xaml.h"

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
// NOTE: the 'superstar' button does not currently load, so this part of the scenario does not work

Page2::Page2()
{
	InitializeComponent();
	rootPage = nullptr;
	starButton = nullptr;
	rightPanel = nullptr;
}

// Invoked when this page is about to be displayed in a Frame.
void Page2::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = (GlobalPage^) e->Parameter;

	// While our AppBar has global commands for navigation, we can demonstrate
	// how we can add contextual AppBar command buttons that only pertain to 
	// particular pages.  
	
	// In this case, whenever we navigate to this page, we want to add a new command button
	// to our AppBar
	
	// Add command buttons to the right side StackPanel within the AppBar.
	rightPanel = (StackPanel^) rootPage->FindName("RightCommands");
	if (rightPanel != nullptr)
	{
	    // Create the button, add to AppBar and setup a handler to receive Click notification from it
	    starButton = ref new Button();
        starButton->Style = safe_cast<Windows::UI::Xaml::Style^>(App::Current->Resources->Lookup("StarAppBarButtonStyle"));
	    starButton->Click += ref new RoutedEventHandler(this, &Page2::starButton_Click);
	
	    rightPanel->Children->Append(starButton);
	}
}
void Page2::OnNavigatingFrom(NavigatingCancelEventArgs^ e) 
{
}

void Page2::starButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::UI::Popups::MessageDialog^ dialog = ref new Windows::UI::Popups::MessageDialog("You're a Superstar!");
	dialog->ShowAsync();
}
