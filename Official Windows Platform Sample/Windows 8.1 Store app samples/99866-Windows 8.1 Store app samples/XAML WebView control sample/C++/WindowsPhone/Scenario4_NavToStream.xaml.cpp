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
#include "Scenario4_NavToStream.xaml.h"
#include "helper.h"

using namespace SDKSample::WebViewControl;

using namespace std;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Navigation;
using namespace concurrency;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

Scenario4::Scenario4()
{
    InitializeComponent();
}


// Invoked when this page is about to be displayed in a Frame.
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	myResolver = ref new StreamUriWinRTResolver();

	// The 'Host' part of the URI for the ms-local-stream protocol needs to be a combination of the package name
	// and an application defined key, which identifies the specfic resolver, in this case 'Mytag'.

	Uri^ url = webView4->BuildLocalStreamUri("MyTag", "/Minesweeper/default.html");

	// The resolver object needs to be passed in to the navigate call.
	webView4->NavigateToLocalStreamUri(url, myResolver);
	webViewLabel->Text = FormatStr("Webview: %s" , url->DisplayUri);
}


StreamUriWinRTResolver::StreamUriWinRTResolver()
{

}

IAsyncOperation<Windows::Storage::Streams::IInputStream^>^ StreamUriWinRTResolver::UriToStreamAsync(Uri^ uri)
{
	if (uri == nullptr)
	{
		throw Exception::CreateException(0, "Uri should not be null");
	}
	String^ relativePath = ref new String(uri->Path->Data());
	Uri^ appDataUri = ref new Uri(L"ms-appx:///html" + relativePath);
	return GetFileStreamFromApplicationUriAsync(appDataUri);
}

IAsyncOperation<IInputStream^>^ StreamUriWinRTResolver::GetFileStreamFromApplicationUriAsync(Uri^ uri)
{
	return create_async([this, uri]()->IInputStream^
	{
		task<StorageFile^> getFileTask(StorageFile::GetFileFromApplicationUriAsync(uri));

		task<IInputStream^> getInputStreamTask = getFileTask.then([](StorageFile^ storageFile)
		{
			return storageFile->OpenAsync(FileAccessMode::Read);
		}).then([](IRandomAccessStream^ stream)
		{
            // IRandomAccessStream requires IInputStream, so cast the stream and return it.
			return static_cast<IInputStream^>(stream);
		});

		return getInputStreamTask.get();
	});
}

