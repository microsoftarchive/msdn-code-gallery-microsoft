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
// CapturePhoto.xaml.cpp
// Implementation of the CapturePhoto class
//

#include "pch.h"
#include "CapturePhoto.xaml.h"

using namespace SDKSample::CameraCapture;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Media::Capture;
using namespace Windows::Storage;
using namespace Windows::Foundation;

CapturePhoto::CapturePhoto()
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
void CapturePhoto::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    // Set the initial photo

    // Check for any previously stored photo in application data
    if (appSettings->HasKey("capturedPhoto"))
    {
        CapturePhotoButton->IsEnabled = false;

        // Load the image from specified path
        Platform::String^ filePath = safe_cast<IPropertyValue^>(appSettings->Lookup("capturedPhoto"))->GetString();
        LoadPhoto(filePath);
    }
    else
    {
        // Load the placeholder image
        rootPage->NotifyUser("The photo will be shown here.", NotifyType::StatusMessage);
        CapturedPhoto->Source = ref new BitmapImage(ref new Windows::Foundation::Uri(BaseUri->AbsoluteUri, "Assets/placeholder-sdk.png"));
    }
}

/// <summary>
/// Invoked when CapturePhoto button is clicked.
/// Shows the camera capture UI in photo mode.
/// When the user takes a photo, shows the image in output section.
/// </summary>
void CapturePhoto::CapturePhoto_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        CameraCaptureUI^ dialog = ref new CameraCaptureUI();

        dialog->PhotoSettings->CroppedAspectRatio = Size(16, 9);

        concurrency::task<StorageFile^> (dialog->CaptureFileAsync(CameraCaptureUIMode::Photo)).then([this] (StorageFile^ file)
        {
            if (nullptr != file)
            {
                concurrency::task<Streams::IRandomAccessStream^> (file->OpenAsync(FileAccessMode::Read)).then([this] (Streams::IRandomAccessStream^ stream)
                {
                    BitmapImage^ bitmapImage = ref new BitmapImage();
                    bitmapImage->SetSource(stream);		
                    CapturedPhoto->Source = bitmapImage;	
                    ResetButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
                    rootPage->NotifyUser("", NotifyType::StatusMessage);
                });

                // Store the path in Application Data
                appSettings->Insert("capturedPhoto", PropertyValue::CreateString(file->Path));
            }
            else
            {
                rootPage->NotifyUser("No photo captured", NotifyType::ErrorMessage);
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
void CapturePhoto::Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage->NotifyUser("The photo will be shown here.", NotifyType::StatusMessage);
    ResetButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    CapturedPhoto->Source = ref new BitmapImage(ref new Windows::Foundation::Uri(BaseUri->AbsoluteUri, "Assets/placeholder-sdk.png"));

    // Remove file from Application Data
    appSettings->Remove("capturedPhoto");
}

/// <summary>
/// Loads the image from the specified file.
/// </summary>
void CapturePhoto::LoadPhoto(Platform::String^ filePath)
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
            BitmapImage^ bitmapImage = ref new BitmapImage();
            bitmapImage->SetSource(stream);		
            CapturedPhoto->Source = bitmapImage;	
            ResetButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
            CapturePhotoButton->IsEnabled = true;
            rootPage->NotifyUser("", NotifyType::StatusMessage);
        }
        catch (Platform::Exception^ ex)
        {
            CapturePhotoButton->IsEnabled = true;
            appSettings->Remove("capturedPhoto");
            rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
        }
    });
}
