//
// SimplePageType1Cacheable.xaml.cpp
// Implementation of the SimplePageType1Cacheable class
//

#include "pch.h"
#include "SimplePageType1Cacheable.xaml.h"

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

SimplePageType1Cacheable::SimplePageType1Cacheable()
{
	InitializeComponent();
	_numberSimplePageType1Cacheable = _numberSimplePageType1Cacheable + 1;
	_newPage = true;
}

void SimplePageType1Cacheable::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	GuidText->Text = "You are navigated to Page Type 1 #" + _numberSimplePageType1Cacheable + ".";
	if (_newPage)
	{
		GuidText->Text += "\nThis is a new instance of the page.";
		GuidText->Text += "\nIt has been added to the cache.";
		_newPage = false;
	}
	else
	{
		GuidText->Text += "\nThis is cached instance.";
	}

	if (e->Parameter != nullptr)
	{
		auto _pageParameters = dynamic_cast<String^>(e->Parameter);
		MessageText->Text = _pageParameters;
	}
}

