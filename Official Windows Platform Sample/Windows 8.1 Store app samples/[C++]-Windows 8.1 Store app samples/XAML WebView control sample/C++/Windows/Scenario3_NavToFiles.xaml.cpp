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
#include "Scenario3_NavToFiles.xaml.h"
#include "helper.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace Windows::Foundation;
using namespace concurrency;

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

	// Copies the file "html\html_example2.html" from this package's installed location to
	// a new file "NavigateToState\test.html" in the local state folder.  When this is
	// done, enables the 'Load HTML' button.
	create_task(ApplicationData::Current->LocalFolder->CreateFolderAsync("NavigateToState", CreationCollisionOption::OpenIfExists)).then([this](StorageFolder^ stateFolder)
	{
		create_task(Windows::ApplicationModel::Package::Current->InstalledLocation->GetFileAsync(L"html\\html_example2.html")).then([this, stateFolder](StorageFile^ htmlFile)
		{
			create_task(htmlFile->CopyAsync(stateFolder, "test.html", NameCollisionOption::ReplaceExisting)).then([this](StorageFile^ newFile)
			{
				loadFromLocalState->IsEnabled = true;
			});
		});
	});
}


// Navigates the webview to the application package
void SDKSample::WebViewControl::Scenario3::loadFromPackage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	String^ url = "ms-appx-web:///html/html_example2.html";
	webView3->Navigate(ref new Uri(url));
	webViewLabel->Text = FormatStr("Webview: %s" , url);
}

// Navigates the webview to the local state directory
void SDKSample::WebViewControl::Scenario3::loadFromLocalState_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	String^ url = "ms-appdata:///local/NavigateToState/test.html";
	webView3->Navigate(ref new Uri(url) );
	webViewLabel->Text = FormatStr("Webview: %s" , url);
}

