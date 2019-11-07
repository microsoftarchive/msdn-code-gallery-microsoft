//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "PageWithParametersConfiguration.h"
#include "PageWithParameters.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Navigation;

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
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

// This Scenario shows how it is possible to Navigate to other page passing parameters
Scenario2::Scenario2()
{
	InitializeComponent();
}

void Scenario2::NavigateButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//We have created a class to serialize the set of parameters that we need to pass
	//the page.
	PageWithParametersConfiguration^  pageParameters = ref new PageWithParametersConfiguration();

	if (MessageTextBox->Text->IsEmpty())
	{
		Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
		Windows::Foundation::DateTime today = calendar->GetDateTime();
		DateTimeFormatter^ dateTimeFormatter = ref new DateTimeFormatter("longtime");
		
		pageParameters->Message = "This Page was created on: " + dateTimeFormatter->Format(today);
	}
	else
	{
		pageParameters->Message = MessageTextBox->Text;
	}
	//The second parameter of navigate method contains the parameters that 
	//will be passed to the page.
	this->MyFrame->Navigate(TypeName(PageWithParameters::typeid),pageParameters);
}

