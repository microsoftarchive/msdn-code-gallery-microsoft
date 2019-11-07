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
#include "LocateVideo.xaml.h"

using namespace SDKSample::MediaServerClient;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Foundation;
using namespace Windows::Storage::Search;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Platform;
using namespace Windows::UI::Xaml::Media;

Scenario1::Scenario1()
{
    InitializeComponent();
    InitializeMediaServers();
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
}

void Scenario1::dmsRefreshButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        rootPage->NotifyUser("Media Servers being refreshed... ", NotifyType::StatusMessage);
        InitializeMediaServers();
    }
}

void Scenario1::dmsSelect_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    localVideo->Stop();
    mediaSelect->Items->Clear();

    if (dmsSelect->SelectedIndex != -1)
    {
        rootPage->NotifyUser("Retrieving media files...", NotifyType::StatusMessage);
        LoadMediaFiles();
    }
}

void Scenario1::mediaSelect_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    if (mediaSelect->SelectedIndex != -1 && mediaFiles != nullptr)
    {
        mediaFile = mediaFiles->GetAt(mediaSelect->SelectedIndex);
        concurrency::create_task(mediaFile->OpenAsync(FileAccessMode::Read))
            .then([this](concurrency::task<IRandomAccessStream^> result)
        {
            localVideo->Stop();
            try
            {
                mediaStream = result.get();
                localVideo->SetSource(mediaStream, mediaFile->ContentType);
                localVideo->Play();
            }
            catch (Exception ^ex)
            {
                rootPage->NotifyUser("Error during file selection :" + ex->Message, NotifyType::ErrorMessage);
            }
        });
    }
}

void Scenario1::LoadMediaFiles()
{
    mediaSelect->Items->Clear();
    StorageFolder^ mediaServerFolder = mediaServers->GetAt(dmsSelect->SelectedIndex);
    auto queryOptions = ref new QueryOptions();
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->UserSearchFilter = "System.Kind:=video";
    auto queryFolder = mediaServerFolder->CreateFileQueryWithOptions(queryOptions);
    concurrency::create_task(queryFolder->GetFilesAsync(0, 25))
        .then([this](concurrency::task<IVectorView<StorageFile^>^> result)
    {
        try
        {
            mediaFiles = result.get();
            if (mediaFiles->Size != 0)
            {
                std::for_each(begin(mediaFiles), end(mediaFiles), [this](StorageFile^ file)
                {
                    mediaSelect->Items->Append(file->DisplayName);
                });
                rootPage->NotifyUser("Media files retrieved", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No Media Files found", NotifyType::StatusMessage);
            }
        }
        catch (Platform::Exception^ e)
        {
            rootPage->NotifyUser("Error locating media files :" + e->Message, NotifyType::ErrorMessage);
        }
    });
}

void Scenario1::localVideo_MediaFailed(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e)
{
    rootPage->NotifyUser("File :"+ mediaFile->DisplayName+ " - Playback error  :"+ e->ErrorMessage, NotifyType::ErrorMessage);
}

void Scenario1::InitializeMediaServers()
{
    dmsSelect->Items->Clear();
    concurrency::create_task(KnownFolders::MediaServerDevices->GetFoldersAsync())
        .then([this](concurrency::task<IVectorView<StorageFolder^>^> result)
    {
        try
        {
            mediaServers = result.get();
            if (mediaServers->Size != 0)
            {
                std::for_each(begin(mediaServers), end(mediaServers), [this](StorageFolder^ folder)
                {
                    dmsSelect->Items->Append(folder->DisplayName);
                });
                rootPage->NotifyUser("Media Servers refreshed", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No MediaServers found", NotifyType::StatusMessage);
            }
        }
        catch (Exception ^ex)
        {
            rootPage->NotifyUser("Unable to retrieve the media servers.", NotifyType::StatusMessage);
        }
    });
}

void Scenario1::localVideo_CurrentStateChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (mediaFile != nullptr)
    {
        String^ filename = mediaFile->DisplayName;
        switch (localVideo->CurrentState)
        {
        case MediaElementState::Playing:
            rootPage->NotifyUser("File :"+ filename + " - Playing", NotifyType::StatusMessage);
            break;
        case MediaElementState::Paused:
            rootPage->NotifyUser("File :"+ filename + " - Paused", NotifyType::StatusMessage);
            break;
        case MediaElementState::Stopped:
            rootPage->NotifyUser("File :"+ filename + " - Stopped", NotifyType::StatusMessage);
            break;
        case MediaElementState::Opening:
            rootPage->NotifyUser("File :"+ filename + " - Opening", NotifyType::StatusMessage);
            break;
        case MediaElementState::Closed:
            break;
        case MediaElementState::Buffering:
            rootPage->NotifyUser("File :"+ filename + " - Buffering", NotifyType::StatusMessage);
            break;
        default:
            break;
        }
    }
}
