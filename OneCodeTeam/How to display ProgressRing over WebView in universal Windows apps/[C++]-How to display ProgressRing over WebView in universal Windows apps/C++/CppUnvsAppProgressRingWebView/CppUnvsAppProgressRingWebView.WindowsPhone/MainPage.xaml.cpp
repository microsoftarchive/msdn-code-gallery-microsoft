/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUnvsAppProgressRingWebView.WindowsPhone
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to display ProgressRing over WebView.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "pch.h"
#include "MainPage.xaml.h"

using namespace CppUnvsAppProgressRingWebView;

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

MainPage::MainPage()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter

	// TODO: Prepare page for display here.

	// TODO: If your application contains multiple pages, ensure that you are
	// handling the hardware Back button by registering for the
	// Windows::Phone::UI::Input::HardwareButtons.BackPressed event.
	// If you are using the NavigationHelper provided by some templates,
	// this event is handled for you.
}

// Click event handler for the link in the footer.
void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri((safe_cast<HyperlinkButton^>(sender))->Tag->ToString()));
}

// Load button click event handler.
void MainPage::LoadButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	Uri^ uri = ValidateAndGetUri(UrlTextBox->Text);
	if (uri != nullptr)
	{
		try
		{
			LoadingProcessProgressRing->IsActive = true;
			LoadingProcessProgressRing->Visibility = Windows::UI::Xaml::Visibility::Visible;
			LoadButton->IsEnabled = false;
			DisplayWebView->Navigate(uri);
		}
		catch (Platform::Exception^ ex)
		{
			NotifyUser(ex->ToString());
		}
	}
}

void MainPage::NotifyUser(Platform::String^ message)
{
	statusText->Text = message;
}

// Validate if the uri is available.
Uri^ MainPage::ValidateAndGetUri(Platform::String^ uriString)
{
	Uri^ uri = nullptr;
	try
	{
		uri = ref new Uri(uriString);
		HintTextBlock->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
	catch (InvalidArgumentException ^e)
	{
		HintTextBlock->Text = e->Message;
		HintTextBlock->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}

	return uri;
}

// WebView navigation completed event hanlder.
void MainPage::DisplayWebView_NavigationCompleted(WebView^ sender, WebViewNavigationCompletedEventArgs^ args)
{
	LoadingProcessProgressRing->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	LoadingProcessProgressRing->IsActive = false;
}

// When the user presses "Enter" key, call LoadButton_Click method to load the content.
void MainPage::UrlTextBox_KeyDown(Object^ sender, KeyRoutedEventArgs^ e)
{
	if (e->Key == Windows::System::VirtualKey::Enter)
	{
		LoadButton_Click(sender, e);
	}
}


void MainPage::UrlTextBox_TextChanged(Object^ sender, TextChangedEventArgs^ e)
{
	if (!UrlTextBox->Text->IsEmpty())
	{
		LoadButton->IsEnabled = true;
	}
	else
	{
		LoadButton->IsEnabled = false;
	}
}

