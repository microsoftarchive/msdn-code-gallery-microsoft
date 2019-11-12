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
// CaptureVideo.xaml.cpp
// Implementation of the CaptureVideo class
//

#include "pch.h"
#include "CaptureVideo.xaml.h"

using namespace SDKSample::CameraCapture;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Media::Capture;
using namespace Windows::Storage;
using namespace Windows::Foundation;


CaptureVideo::CaptureVideo()
{
    InitializeComponent();
    appSettings = ApplicationData::Current->LocalSettings->Values;
    ResetButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CaptureVideo::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    // Set the initial video

    // Check for any previously stored photo in application data
    if (appSettings->HasKey("capturedVideo"))
    {
        CaptureVideoButton->IsEnabled = false;

        // Load the image from specified path
        Platform::String^ filePath = safe_cast<IPropertyValue^>(appSettings->Lookup("capturedVideo"))->GetString();

        LoadVideo(filePath);
    }
    else
    {
        // Load the placeholder message
        rootPage->NotifyUser("The video will be shown here.", NotifyType::StatusMessage);
    }
}

/// <summary>
/// Invoked when CaptureVideo button is clicked.
/// Shows the camera capture UI in video mode.
/// When the user takes a video, shows the image in output section.
/// </summary>
void CaptureVideo::CaptureVideo_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        CameraCaptureUI^ dialog = ref new CameraCaptureUI();

        dialog->VideoSettings->Format = CameraCaptureUIVideoFormat::Mp4;

        concurrency::task<StorageFile^>(dialog->CaptureFileAsync(CameraCaptureUIMode::Video)).then([this] (StorageFile^ file)
        {
            if (file != nullptr)
            {
                concurrency::task<Streams::IRandomAccessStream^> (file->OpenAsync(FileAccessMode::Read)).then([this] (Streams::IRandomAccessStream^ stream)
                {
                    CapturedVideo->SetSource(stream, "video/mp4");
                    ResetButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
                    rootPage->NotifyUser("", NotifyType::StatusMessage);
                });

                // Store the path in Application Data
                appSettings->Insert("capturedVideo", PropertyValue::CreateString(file->Path));
            }
            else
            {
                rootPage->NotifyUser("No video captured", NotifyType::ErrorMessage);
            }

        });
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Invoked when Reset button is clicked.
/// Resets the scenario.
/// </summary>
void CaptureVideo::Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage->NotifyUser("The video will be shown here.", NotifyType::StatusMessage);
    ResetButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    CapturedVideo->Source = nullptr;

    // Remove file from Application Data
    appSettings->Remove("capturedVideo");
}

/// <summary>
/// Loads the video from the specified file.
/// </summary>
void CaptureVideo::LoadVideo(Platform::String^ filePath)
{
    concurrency::task<StorageFile^>(StorageFile::GetFileFromPathAsync(filePath)).then([this] (StorageFile^ file)
    {
        return file->OpenAsync(FileAccessMode::Read);
    })
        .then([this](concurrency::task<Streams::IRandomAccessStream^> openTask)
    {
        try
        {
            auto stream = openTask.get();
            CapturedVideo->SetSource(stream, "video/mp4");
            ResetButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
            CaptureVideoButton->IsEnabled = true;
            rootPage->NotifyUser("", NotifyType::StatusMessage);
        }
        catch (Platform::Exception^ ex)
        {
            CaptureVideoButton->IsEnabled = true;
            appSettings->Remove("capturedVideo");
            rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
        }
    });
}
