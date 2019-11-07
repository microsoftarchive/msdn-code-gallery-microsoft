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
// HomeGroupVideoStream.xaml.cpp
// Implementation of the HomeGroupVideoStream class
//

#include "pch.h"
#include "HomeGroupVideoStream.xaml.h"

using namespace SDKSample::HomeGroup;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage;
using namespace concurrency;
using namespace Windows::Storage::Streams;
using namespace Platform;

HomeGroupVideoStream::HomeGroupVideoStream()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void HomeGroupVideoStream::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Use file picker to select a video file from a HomeGroup machine and play it in 'Media Element'
void SDKSample::HomeGroup::HomeGroupVideoStream::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (rootPage->EnsureUnsnapped())
        {
            auto picker = ref new FileOpenPicker();
            picker->ViewMode = PickerViewMode::Thumbnail;
            picker->SuggestedStartLocation = PickerLocationId::HomeGroup;
            picker->FileTypeFilter->Append(".mp4");
            picker->FileTypeFilter->Append(".wmv");
            create_task(picker->PickSingleFileAsync()).then([this](StorageFile^ file)
            {
                if (file)
                {
                    // The video tag has built in capabilities to stream the video over the network.
                    create_task(file->OpenReadAsync()).then([this](IRandomAccessStreamWithContentType^ videoStream)
                    {
                        if (videoStream)
                        {
                            //Clear status that may have been previously set by a picker being cancelled.
                            rootPage->NotifyUser("", NotifyType::StatusMessage);
                            videoPlayer->SetSource(videoStream, "video/x-flv");
                            videoPlayer->Visibility = Windows::UI::Xaml::Visibility::Visible;
                            videoPlayer->Play();
                        }
                        else
                        {
                            rootPage->NotifyUser("Stream was not returned", NotifyType::ErrorMessage);
                        }
                    });
                }
                else
                {
                    rootPage->NotifyUser("File was not returned", NotifyType::ErrorMessage);
                }
            });
        }
    }
}
