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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace std;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
{
    InitializeComponent();

	// Hook the LoadCompleted event for the WebView to know when the URL is fully loaded 
	WebView4->ScriptNotify += ref new NotifyEventHandler(this, &Scenario4::WebView4_ScriptNotify);
}


// Invoked when this page is about to be displayed in a Frame.
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


// Script notify for WebView4
void SDKSample::WebViewControl::Scenario4::WebView4_ScriptNotify(Platform::Object^ sender, Windows::UI::Xaml::Controls::NotifyEventArgs^ e)
{
	String^  str = "Response from script: " + e->Value;
	rootPage->NotifyUser(str, NotifyType::StatusMessage);
}


// Click handler for 'Fire Script' button
void SDKSample::WebViewControl::Scenario4::FireScript_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (NavToString->IsChecked->Value == true)
	{
	    // We can run script that uses window.external.notify() to send data back to the app 
	    // without having to set the AllowedScriptNotifyUris property because the app is
	    // trusted and it owns the content of the script.
	    WebView4->InvokeScript("SayGoodbye", nullptr);
	}
	else
	{
	    if (Nav->IsChecked->Value == true)
	    {
	        // Here we have to set the AllowedScriptNotifyUri property because we are navigating
	        // to some actual site where we don't own the content and we want to allow window.external.notify()
	        // to pass back data to our application.
			IVector<Uri^>^ allowedUris = ref new Platform::Collections::Vector<Uri^>();
	        allowedUris->Append(ref new Uri("http://www.bing.com"));
	        WebView4->AllowedScriptNotifyUris = allowedUris;
	
	        // Since this site does not have a function that we can call that actually uses window.external.notify() we have to inject that into
	        // the page using eval().  See the next scenario for more information on this technique.
			Array<String^,1>^ args = ref new Array<String^>(1);
			args[0] = "window.external.notify('GoodBye');";
	        WebView4->InvokeScript("eval", args);
			
	    }
	    else
	    {
	        rootPage->NotifyUser("Please choose a navigation method", NotifyType::ErrorMessage);
	    }
	}
}


// Click handler for 'NavToString' button
void SDKSample::WebViewControl::Scenario4::NavToString_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage->NotifyUser("", NotifyType::StatusMessage);
    // Let's create an HTML fragment that contains some javascript code that we will fire using
    // InvokeScript().
	String^ htmlFragment = ""
	+ "\n\t<html>"
    + "\n\t		<head>"
    + "\n\t			<script type='text/javascript'>"
    + "\n\t				function SayGoodbye() "
    + "\n\t				{"
    + "\n\t					window.external.notify('GoodBye'); "
    + "\n\t				}"
    + "\n\t			</script>"
    + "\n\t		</head>"
    + "\n\t		<body>"
    + "\n\t			Page with 'Goodbye' script loaded.  Click the 'Fire Script' button to run the script and send data back to the application."
    + "\n\t		/body>"
	+ "\n\t</html>";

	// Load the fragment into the HTML text box so it will be visible.
	HTML4->Text = htmlFragment;
	WebView4->NavigateToString(HTML4->Text);
}


// Click handlerfor 'Nav' button
void SDKSample::WebViewControl::Scenario4::Nav_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	 rootPage->NotifyUser("", NotifyType::StatusMessage);
     WebView4->Navigate(ref new Uri("http://www.bing.com"));
}
