//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	// Let's create an HTML fragment that contains some javascript code that we will fire using
    // InvokeScript().
    String^ htmlFragment = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>"
	+ "\n\t<html>" 
	+ "\n\t		<head>" 
	+ "\n\t			<script type='text/javascript'>"
	+ "\n\t				function SayGoodbye() "
	+ "\n\t				{"
	+ "\n\t					document.getElementById('myDiv').innerText = 'GoodBye'; "
	+ "\n\t				}"
	+ "\n\t			</script>"
	+ "\n\t		</head>"
	+ "\n\t		<body>" 
	+ "\n\t			<div id='myDiv'>Hello</div>" 
	+ "\n\t		</body>" 
	+ "\n\t	</html>";

    // Load it into the HTML text box so it will be visible.
    HTML3->Text = htmlFragment;
}


// Click handler for the 'Load' button.
void SDKSample::WebViewControl::Scenario3::Load_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Grab the HTML from the text box and load it into the WebView
    WebView3->NavigateToString(HTML3->Text);
}


// Click handler for the 'Script' button.
void SDKSample::WebViewControl::Scenario3::Script_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Invoke the javascript function called 'SayGoodbye' that is loaded into the WebView.
    WebView3->InvokeScript("SayGoodbye", nullptr);
}
