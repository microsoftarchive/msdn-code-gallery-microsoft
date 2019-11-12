//
// PageWithParameters.xaml.cpp
// Implementation of the PageWithParameters class
//

#include "pch.h"
#include "PageWithParameters.xaml.h"
#include "PageWithParametersConfiguration.h"

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

PageWithParameters::PageWithParameters()
{
	InitializeComponent();
}

void PageWithParameters::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	//We are going to cast the property Parameter of NavigationEventArgs object
	//into PageWithParametersConfiguration.
	//PageWithParametersConfiguration contains a set of parameters to pass to the page 			
	_pageParameters = dynamic_cast<SDKSample::Navigation::PageWithParametersConfiguration^>(e->Parameter);
	if (_pageParameters != nullptr)
	{
		MessageText->Text = _pageParameters->Message;
		MessageText->Text += "\nPage ID: " + _pageParameters->ID.ToString();
	}
}


