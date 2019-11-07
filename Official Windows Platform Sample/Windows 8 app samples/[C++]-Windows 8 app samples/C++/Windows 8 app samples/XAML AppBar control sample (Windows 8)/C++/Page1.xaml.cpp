//
// Page1.xaml.cpp
// Implementation of the Page1 class
//

#include "pch.h"
#include "Page1.xaml.h"
#include "Page2.xaml.h"
#include "GlobalPage.xaml.h"

using namespace AppBarControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

GlobalPage^ Page1::rootPage = nullptr;

Page1::Page1()
{
	InitializeComponent();
}


// Invoked when this page is about to be displayed in a Frame.
void Page1::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = (GlobalPage^) e->Parameter;
}


void AppBarControl::Page1::PageTwo_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	TypeName page2 = {Page2::typeid->FullName, TypeKind::Custom};
	Frame->Navigate(page2, rootPage);
}
