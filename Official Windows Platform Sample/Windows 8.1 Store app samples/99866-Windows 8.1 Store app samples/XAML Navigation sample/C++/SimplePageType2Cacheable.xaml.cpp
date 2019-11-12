//
// SimplePageType2Cacheable.xaml.cpp
// Implementation of the SimplePageType2Cacheable class
//

#include "pch.h"
#include "SimplePageType2Cacheable.xaml.h"

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

SimplePageType2Cacheable::SimplePageType2Cacheable()
{
	InitializeComponent();
	_numberSimplePageType2Cacheable = _numberSimplePageType2Cacheable + 1;
	_newPage = true;
}

void SimplePageType2Cacheable::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	GuidText->Text = "You are navigated to Page Type 2 #" + _numberSimplePageType2Cacheable + ".";
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
