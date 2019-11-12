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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6_ScriptNotify.xaml.h"
#include "helper.h"

using namespace SDKSample::WebViewControl;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI;
using namespace Platform;

Scenario6::Scenario6()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	String^ src = L"ms-appx-web:///html/scriptNotify_example.html";
	webView6->Navigate(ref new Uri(src));
	webView6->ScriptNotify += ref new NotifyEventHandler(this, &Scenario6::webView6_ScriptNotify);
}

void Scenario6::webView6_ScriptNotify(Object^ sender, NotifyEventArgs^ e)
{
	// Be sure to verify the source of the message when performing actions with the data.
	// As webview can be navigated, you need to check that the message is coming from a page/code
	// that you trust.
	Color c = Colors::Red;

	if (e->CallingUri->SchemeName == "ms-appx-web")
	{
		if (e->Value == "blue") c = Colors::Blue;
		else if (e->Value == "green") c = Colors::Green;
	}
	rootPage->NotifyUser(FormatStr("Response from script at '%s': '%s'", e->CallingUri->DisplayUri,  e->Value), NotifyType::StatusMessage);
}
