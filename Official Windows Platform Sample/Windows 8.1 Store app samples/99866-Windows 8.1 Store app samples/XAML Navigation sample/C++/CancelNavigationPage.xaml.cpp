//
// CancelNavigationPage.xaml.cpp
// Implementation of the CancelNavigationPage class
//

#include "pch.h"
#include "CancelNavigationPage.xaml.h"

using namespace SDKSample;

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

CancelNavigationPage::CancelNavigationPage()
{
	InitializeComponent();
	this->ID = _numberCancelNavigationPages;
	_numberCancelNavigationPages = _numberCancelNavigationPages + 1;
	GuidText->Text = "You are navigated to Page ID #" + this->ID.ToString()+ ".";
}

void CancelNavigationPage::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
	e->Cancel = CancelNavigationSwitch->IsOn;
}