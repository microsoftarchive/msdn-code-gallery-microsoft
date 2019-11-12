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
#include "Scenario2_NavToString.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace Windows::Foundation;
using namespace concurrency;

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

	//Using the storage classes to read the content from a file
	create_task(StorageFile::GetFileFromApplicationUriAsync(ref new Uri("ms-appx:///html/html_example.html"))).then([this](StorageFile^ file)
	{
		create_task(FileIO::ReadTextAsync(file)).then([this](String ^htmlFragment)
		{
			HTML2->Text = htmlFragment;
		});
	});
}


// Click handler for the 'Load' button.
void SDKSample::WebViewControl::Scenario2::Load_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	WebView2->NavigateToString(HTML2->Text);
}
