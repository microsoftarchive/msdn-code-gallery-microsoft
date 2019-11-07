//
// SimplePageType3Cacheable.xaml.cpp
// Implementation of the SimplePageType3Cacheable class
//

#include "pch.h"
#include "SimplePageType3Cacheable.xaml.h"

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

SimplePageType3Cacheable::SimplePageType3Cacheable()
{
	InitializeComponent();
	_numberSimplePageType3Cacheable = _numberSimplePageType3Cacheable + 1;
	_newPage = true;
}

void SimplePageType3Cacheable::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	GuidText->Text = "You are navigated to Page Type 3 #" + _numberSimplePageType3Cacheable + ".";
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

