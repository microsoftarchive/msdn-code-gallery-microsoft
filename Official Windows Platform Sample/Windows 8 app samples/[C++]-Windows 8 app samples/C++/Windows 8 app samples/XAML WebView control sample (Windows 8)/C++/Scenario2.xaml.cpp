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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::Data::Html;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	
	// Let's create a very simple HTML fragment
	String^ htmlFragment = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>"
		+ "\n\t<html>" 
		+ "\n\t  <head>" 
		+ "\n\t    <title>Map with valid credentials</title>"
		+ "\n\t    <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>"
		+ "\n\t    <script type='text/javascript' src='http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0'></script>"		
        + "\n\t    <script type='text/javascript'>"    
		+ "\n\t    var map = null;"
		+ "\n\t    function getMap()"
		+ "\n\t    {"
		+ "\n\t			map = new Microsoft.Maps.Map(document.getElementById('myMap'), {credentials: 'Your Bing Maps Key'});"
		+ "\n\t    }"
		+ "\n\t   </script>"
		+ "\n\t  </head>"
		+ "\n\t  <body onload='getMap();'>"
        + "\n\t    <div id='myMap' style='position:relative; width:400px; height:400px;'></div>"
        + "\n\t  </body>"
		+ "\n\t<html>;";
	
	// Load HTML fragment into the HTML text box so it will be visible
	HTML2->Text = htmlFragment;
	
}


// Click handler for the 'Load' button.
void SDKSample::WebViewControl::Scenario2::Load_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	WebView2->NavigateToString(HTML2->Text);
}
