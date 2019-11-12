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
#include "Scenario1.xaml.h"

using namespace SDKSample::PlayTo;

using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::Pickers;
using namespace Windows::Media::PlayTo;
using namespace concurrency;


Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	dispatcher = Window::Current->CoreWindow->Dispatcher;
	playToManager = PlayToManager::GetForCurrentView();
	sourceRequestedToken = playToManager->SourceRequested += ref new TypedEventHandler<PlayToManager^, PlayToSourceRequestedEventArgs^>(this, &Scenario1::playToManager_SourceRequested);
}

void Scenario1::playToManager_SourceRequested(Windows::Media::PlayTo::PlayToManager^ sender, Windows::Media::PlayTo::PlayToSourceRequestedEventArgs^ e)
{
	auto deferral = e->SourceRequest->GetDeferral();

	Windows::UI::Core::DispatchedHandler^ handler = ref new Windows::UI::Core::DispatchedHandler([=] (){

		e->SourceRequest->SetSource(VideoSource->PlayToSource);
		deferral->Complete();

	}, Platform::CallbackContext::Any);

	auto action = dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, handler);
}

void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
	playToManager->SourceRequested -= sourceRequestedToken;
}

void Scenario1::webContent_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		VideoSource->Source = ref new Uri("http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4");
		rootPage->NotifyUser("You are playing a web content", NotifyType::StatusMessage);
    }
}


void Scenario1::videoFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        // Verify the snapped state and unsnap if needed
		Windows::UI::ViewManagement::ApplicationViewState currentSnappedState = Windows::UI::ViewManagement::ApplicationView::Value;
        if (currentSnappedState == Windows::UI::ViewManagement::ApplicationViewState::Snapped && !Windows::UI::ViewManagement::ApplicationView::TryUnsnap()) {
            rootPage->NotifyUser("Cannot unsnap the app to launch the file picker", NotifyType::StatusMessage);
            return;
        }
		
		FileOpenPicker^ filePicker = ref new FileOpenPicker();
		filePicker->SuggestedStartLocation = PickerLocationId::VideosLibrary;
		filePicker->FileTypeFilter->Append(".mp4");
		filePicker->FileTypeFilter->Append(".wmv");
		filePicker->ViewMode = PickerViewMode::Thumbnail;
		
		task<StorageFile^>(filePicker->PickSingleFileAsync()).then(
			[this](StorageFile^ videoFile)
		{
			if (videoFile)
			{
				localFile = videoFile;
				task<IRandomAccessStream^>(videoFile->OpenAsync(FileAccessMode::Read)).then(
					[this](IRandomAccessStream^ stream)
				{
					if (stream)
					{
						VideoSource->SetSource(stream, localFile->ContentType);
						rootPage->NotifyUser("You are playing a local video file", NotifyType::StatusMessage);
					}
				});
			}
		});
    }
}


void Scenario1::playButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	VideoSource->Play();
}


void Scenario1::pauseButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	VideoSource->Pause();
}
