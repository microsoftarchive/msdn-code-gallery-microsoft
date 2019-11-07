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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1_NavToUrl.xaml.h"
#include "helper.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Documents;

Scenario1::Scenario1()
{
	InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// A pointer back to the main page.  This is needed if you want to call methods in MainPage such
	// as NotifyUser()
	rootPage = MainPage::Current;
	address->KeyUp += ref new KeyEventHandler(this, &SDKSample::WebViewControl::Scenario1::Address_KeyUp);
	webView1->NavigationStarting += ref new TypedEventHandler<WebView^, WebViewNavigationStartingEventArgs^>(this, &Scenario1::WebView1_NavigationStarting);
	webView1->ContentLoading += ref new TypedEventHandler<WebView^, WebViewContentLoadingEventArgs^>(this, &Scenario1::WebView1_ContentLoading);
	webView1->DOMContentLoaded += ref new TypedEventHandler<WebView^, WebViewDOMContentLoadedEventArgs^>(this, &Scenario1::WebView1_DOMContentLoaded);
	webView1->UnviewableContentIdentified += ref new TypedEventHandler<WebView^, WebViewUnviewableContentIdentifiedEventArgs^>(this, &Scenario1::WebView1_UnviewableContentIdentified);
	webView1->NavigationCompleted += ref new TypedEventHandler<WebView^, WebViewNavigationCompletedEventArgs^>(this, &Scenario1::WebView1_NavigationCompleted);
	address->Text = "http://www.microsoft.com";
}

// Click handler for Navigate button
void Scenario1::GoButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (!_pageIsLoading)
	{
		NavigateWebView(address->Text);
	}
	else
	{
		webView1->Stop();
		SetPageLoading(false);
	}
}

// Key up handler for Address text box
void Scenario1::Address_KeyUp(Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e)
{
	if (e->Key == Windows::System::VirtualKey::Enter)
	{
		NavigateWebView(address->Text);
	}
}

void Scenario1::NavigateWebView(String^ url)
{
	try
	{
		Uri^ targetUri = ref new Uri(address->Text);
		webView1->Navigate(targetUri);
	}
	catch (FailureException^ myE)
	{
		// Bad address
		String^ str =FormatStr("<h1>Address is invalid, try again.  Details --> %s</h1>", myE->Message);
		webView1->NavigateToString(str);
	}
}

void Scenario1::SetPageLoading(bool isLoading)
{
	_pageIsLoading = isLoading;
	goButton->Content = (isLoading ? "Stop" : "Go");
	progressRing1->Visibility = (isLoading ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Visible);

	if (!isLoading)
	{
		navigateBack->IsEnabled = webView1->CanGoBack;
		navigateForward->IsEnabled = webView1->CanGoForward;
	}
}

void Scenario1::WebView1_NavigationStarting(WebView^ sender, WebViewNavigationStartingEventArgs^ args)
{
	String^ url = "";
	try {
		url = args->Uri->DisplayUri;
	}
	catch (Exception^) {}

	address->Text = url;
	rootPage->NotifyUser(FormatStr("Starting navigation to: \"%s\".\n", url), NotifyType::StatusMessage);
	SetPageLoading(true);

}

void Scenario1::WebView1_UnviewableContentIdentified(WebView^ sender, WebViewUnviewableContentIdentifiedEventArgs^ args)
{
	rootPage->NotifyUser(FormatStr("Content for \"%s\" cannot be loaded into webview. Invoking the default launcher instead.\n", args->Uri->DisplayUri), NotifyType::StatusMessage);
	// We turn around and hand the Uri to the system launcher to launch the default handler for it
	Windows::Foundation::IAsyncOperation<bool>^ b = Windows::System::Launcher::LaunchUriAsync(args->Uri);
	SetPageLoading(false);
}

void Scenario1::WebView1_ContentLoading(WebView^ sender, WebViewContentLoadingEventArgs^ args)
{
	String^ url = (args->Uri != nullptr) ? args->Uri->DisplayUri : "<null>";
	rootPage->NotifyUser(FormatStr("Loading content for \"%s\".\n", url), NotifyType::StatusMessage);
}

void Scenario1::WebView1_DOMContentLoaded(WebView^ sender, WebViewDOMContentLoadedEventArgs^ args)
{
	String^ url = (args->Uri != nullptr) ? args->Uri->DisplayUri : "<null>";
	rootPage->NotifyUser(FormatStr("Content for \"%s\" has finished loading.\n", url), NotifyType::StatusMessage);
}

void Scenario1::WebView1_NavigationCompleted(WebView^ sender, WebViewNavigationCompletedEventArgs^ args)
{
	SetPageLoading(false);
	if (args->IsSuccess)
	{
		String^ url = (args->Uri != nullptr) ? args->Uri->DisplayUri : "<null>";
		rootPage->NotifyUser(FormatStr("Navigation to \"%s\"completed successfully.\n" , url), NotifyType::StatusMessage);
	}
	else
	{
		String^ url = "";
		try {
			url = args->Uri->DisplayUri;
		}
		catch (Exception^){}

		address->Text = url;
		rootPage->NotifyUser(FormatStr("Navigation to: \"%s\" failed with error code %s.\n", url, args->WebErrorStatus.ToString()), NotifyType::StatusMessage); 
	}
}

void Scenario1::NavigateBackButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	webView1->GoBack();
}

void Scenario1::NavigateForwardButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	webView1->GoForward();
}


