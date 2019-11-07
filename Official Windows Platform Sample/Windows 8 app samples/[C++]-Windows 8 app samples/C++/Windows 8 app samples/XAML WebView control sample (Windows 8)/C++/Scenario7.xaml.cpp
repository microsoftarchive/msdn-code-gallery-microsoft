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
// Scenario7.xaml.cpp
// Implementation of the Scenario7 class
//

#include "pch.h"
#include "Scenario7.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::DataTransfer;

Scenario7::Scenario7()
{
    InitializeComponent();
	DataTransferManager^ dataTransferManager = nullptr;
}


// Invoked when this page is about to be displayed in a Frame.
void Scenario7::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	WebView7->LoadCompleted += ref new LoadCompletedEventHandler(this, &Scenario7::WebView7_LoadCompleted);
    WebView7->Navigate(ref new Uri("http://www.wsj.com"));
}


// Load completed handler for WebView7
void SDKSample::WebViewControl::Scenario7::WebView7_LoadCompleted(Platform::Object^ sender, Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	WebView7->Visibility = Windows::UI::Xaml::Visibility::Visible;
	BlockingRect->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	ProgressRing1->IsActive = false;
}

// Click handler for 'Share' button
void SDKSample::WebViewControl::Scenario7::Share_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	dataTransferManager = DataTransferManager::GetForCurrentView();
	dataRequestedToken = dataTransferManager->DataRequested += ref new TypedEventHandler<DataTransferManager^, DataRequestedEventArgs^>(this, &Scenario7::dataTransferManager_DataRequested);
	DataTransferManager::ShowShareUI();
}

// Data requested handler
void SDKSample::WebViewControl::Scenario7::dataTransferManager_DataRequested(DataTransferManager^ sender, DataRequestedEventArgs^ args)
{
	DataRequest^ request = args->Request;
	DataPackage^ p = WebView7->DataTransferPackage;
	
	if (p->GetView()->Contains(StandardDataFormats::Text))
	{
	    p->Properties->Title = "WebView Sharing Excerpt";
	    p->Properties->Description = "This is a snippet from the content hosted in the WebView control";
	    request->Data = p;
	}
	else
	{
	    request->FailWithDisplayText("Nothing to share");
	}
    dataTransferManager->DataRequested -= dataRequestedToken;
}