//
// SimplePage.xaml.cpp
// Implementation of the SimplePage class
//

#include "pch.h"
#include "SimplePage.xaml.h"

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

SimplePage::SimplePage()
{
	InitializeComponent();
	this->ID = _numberSimplePages;
	_numberSimplePages = _numberSimplePages + 1;
	GuidText->Text = "You are navigated to Page ID #" + this->ID.ToString()+ ".";
}
