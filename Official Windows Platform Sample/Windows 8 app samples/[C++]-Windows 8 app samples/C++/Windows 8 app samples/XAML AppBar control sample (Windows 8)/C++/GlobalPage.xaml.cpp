//
// GlobalPage.xaml.cpp
// Implementation of the GlobalPage class
//

#include "pch.h"
#include "Page1.xaml.h"
#include "GlobalPage.xaml.h"

using namespace AppBarControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
MainPage^ GlobalPage::rootPage = nullptr;

GlobalPage::GlobalPage()
{

	InitializeComponent();
	//Add Click handler to "Back" button
	Back->Click += ref new RoutedEventHandler(this, &GlobalPage::Back_Click);
}


// Invoked when this page is about to be displayed in a Frame.
void GlobalPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = (MainPage^) e->Parameter;
	TypeName page1 = {Page1::typeid->FullName, TypeKind::Custom};
	Frame1->Navigate(page1, this);
}

void GlobalPage::Back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (Frame1->CanGoBack)
            {
                Frame1->GoBack();
            }
            else
            {
               rootPage->Frame->GoBack();
            }
}
