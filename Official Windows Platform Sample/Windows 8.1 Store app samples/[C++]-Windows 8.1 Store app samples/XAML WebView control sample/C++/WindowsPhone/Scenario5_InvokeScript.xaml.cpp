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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5_InvokeScript.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace concurrency;

Scenario5::Scenario5()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	String ^ htmlFragment = L"<!DOCTYPE html>\n" +
		"<html>\n" +
		"  <head>\n" +
		"    <script type = 'text/javascript'>\n" +
		"      function doSomething() {\n" +
		"        document.getElementById('myDiv').innerText = 'GoodBye';\n" +
		"        return 'Hello World!';\n" +
		"      }\n" +
		"    </script>\n" +
		"  </head>\n" +
		"  <body>\n" +
		"    <div id = 'myDiv'>Hello</div>\n" +
		"  </body>\n" +
		"</html>\n";
	HTML3->Text = htmlFragment;
}

// This is the click handler for the 'Load' button.
void Scenario5::load_Click(Object^ sender, RoutedEventArgs^ e)
{
	// Grab the HTML from the text box and load it into the WebView
	webView5->NavigateToString(HTML3->Text);
}

//This is the click handler for the 'Script' button.
void Scenario5::script_Click(Object^ sender, RoutedEventArgs^ e)
{
	// Invoke the javascript function called 'doSomething' that is loaded into the WebView.
	create_task(webView5->InvokeScriptAsync(L"doSomething", nullptr)).then([this](String ^result)
	{
		Platform::Details::Console::WriteLine(L"Result from script " + result);
	})
	.then([](task<void> t)
	{
		try
		{
			t.get();
		}
		catch (Platform::Exception^)
		{
			// An exception can be thrown if a webpage has not been loaded into the WebView or no javascript function named "doSomething" is found in the webpage.
		}
	});
}