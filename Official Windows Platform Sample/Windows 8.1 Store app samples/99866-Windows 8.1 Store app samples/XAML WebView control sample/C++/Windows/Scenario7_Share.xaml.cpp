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
#include "Scenario7_Share.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Platform;
using namespace concurrency;

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
    webView7->Navigate(ref new Uri("http://msdn.microsoft.com/en-US/windows/"));
}

// Click handler for 'Share' button
void SDKSample::WebViewControl::Scenario7::Share_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	dataTransferManager = DataTransferManager::GetForCurrentView();
	dataRequestedEventToken = dataTransferManager->DataRequested += ref new TypedEventHandler<DataTransferManager^, DataRequestedEventArgs^>(this, &Scenario7::dataTransferManager_DataRequested);
	DataTransferManager::ShowShareUI();
}

// Data requested handler
void SDKSample::WebViewControl::Scenario7::dataTransferManager_DataRequested(DataTransferManager^ sender, DataRequestedEventArgs^ args)
{
	dataTransferManager->DataRequested -= dataRequestedEventToken;
	DataRequest^ request = args->Request;
	DataRequestDeferral^ deferral = args->Request->GetDeferral();
	Uri^ source = webView7->Source;
	create_task(webView7->CaptureSelectedContentToDataPackageAsync()).then([this,request,source, deferral](DataPackage^ dp)
	{
		if (dp != nullptr)
		{
			dp->Properties->Title = "WebView Sharing Excerpt";
			request->Data = dp;
		}
		else
		{
			DataPackage^ myData = ref new DataPackage();
			myData->SetWebLink(source);
			myData->Properties->Title="This is the URI from the webview control";
			request->Data = myData;
		}
		deferral->Complete();
	});
	
}