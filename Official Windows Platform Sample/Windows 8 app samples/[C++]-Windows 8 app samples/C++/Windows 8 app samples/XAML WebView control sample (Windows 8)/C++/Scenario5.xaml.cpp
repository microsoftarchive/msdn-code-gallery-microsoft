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
#include "Scenario5.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

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

	// Hook the LoadCompleted event for the WebView to know when the URL is fully loaded
	WebView5->LoadCompleted += ref new LoadCompletedEventHandler(this, &Scenario5::WebView5_LoadCompleted);

	WebView5->Navigate(ref new Uri("http://kexp.org/Playlist"));
}

// Load completed handler for WebView5
void SDKSample::WebViewControl::Scenario5::WebView5_LoadCompleted(Platform::Object^ sender, Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	IVector<Uri^>^ allowedUris = ref new Platform::Collections::Vector<Uri^>();
	allowedUris->Append(ref new Uri("http://kexp.org"));
	WebView5->AllowedScriptNotifyUris = allowedUris;
	WebView5->ScriptNotify += ref new NotifyEventHandler(this, &Scenario5::WebView5_ScriptNotify);


	Array<String^,1>^ args = ref new Array<String^>(1);
	args[0] = "document.title;";
	String^ foo = WebView5->InvokeScript("eval", args);
	String^ str = "Title is: {0}" + foo;
	rootPage->NotifyUser(str, NotifyType::StatusMessage);
}

// Script notify for WebView5
void SDKSample::WebViewControl::Scenario5::WebView5_ScriptNotify(Platform::Object^ sender, Windows::UI::Xaml::Controls::NotifyEventArgs^ e)
{
	rootPage->NotifyUser(e->Value, NotifyType::StatusMessage);
}
