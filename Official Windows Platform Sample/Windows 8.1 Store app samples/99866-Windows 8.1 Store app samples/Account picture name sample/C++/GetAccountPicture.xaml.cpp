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
// GetAccountPicture.xaml.cpp
// Implementation of the GetAccountPicture class
//

#include "pch.h"
#include "GetAccountPicture.xaml.h"

using namespace AccountPictureName;
using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::System::UserProfile;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;

GetAccountPicture::GetAccountPicture()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GetAccountPicture::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void GetAccountPicture::GetSmallImageButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    HideImageAndVideoControls();

    if (!UserInformation::NameAccessAllowed)
    {
        rootPage->NotifyUser("Access to account picture disabled by Privacy Setting or Group Policy", NotifyType::ErrorMessage);
    }
    else
    {
        auto smallImageFile = UserInformation::GetAccountPicture(AccountPictureKind::SmallImage);
        if (smallImageFile != nullptr)
        {
            rootPage->NotifyUser("Path: " + smallImageFile->Path, NotifyType::StatusMessage);
            create_task(smallImageFile->OpenReadAsync()).then(
                [this](task<IRandomAccessStreamWithContentType^> imageStreamTask)
                {
                    try
                    {
                        auto imageStream = imageStreamTask.get();
                        auto bitmapImage = ref new BitmapImage();
                        bitmapImage->SetSource(imageStream);
                        AccountPictureImage->Source = bitmapImage;
                        AccountPictureImage->Visibility = Windows::UI::Xaml::Visibility::Visible;
                    }
                    catch (Exception^ Ex)
                    {
                        rootPage->NotifyUser("No large account picture image returned for current user.", NotifyType::ErrorMessage);
                    }
                });
        }
        else
        {
            rootPage->NotifyUser("No small account picture image returned for current user.", NotifyType::ErrorMessage);
        }
    }
}

void GetAccountPicture::GetLargeImageButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    HideImageAndVideoControls();

    if (!UserInformation::NameAccessAllowed)
    {
        rootPage->NotifyUser("Access to account picture disabled by Privacy Setting or Group Policy", NotifyType::ErrorMessage);
    }
    else
    {
        auto largeImageFile = UserInformation::GetAccountPicture(AccountPictureKind::LargeImage);
        if (largeImageFile != nullptr)
        {
            rootPage->NotifyUser("Path: " + largeImageFile->Path, NotifyType::StatusMessage);
            create_task(largeImageFile->OpenReadAsync()).then(
                [this](task<IRandomAccessStreamWithContentType^> imageStreamTask)
                {
                    try
                    {
                        auto imageStream = imageStreamTask.get();
                        auto bitmapImage = ref new BitmapImage();
                        bitmapImage->SetSource(imageStream);
                        AccountPictureImage->Source = bitmapImage;
                        AccountPictureImage->Visibility = Windows::UI::Xaml::Visibility::Visible;
                    }
                    catch (Exception^ ex)
                    {
                        rootPage->NotifyUser("Failed to read from stream. " + ex->Message, NotifyType::ErrorMessage);
                    }
                });
        }
        else
        {
            rootPage->NotifyUser("No large account picture image returned for current user.", NotifyType::ErrorMessage);
        }
    }
}

void GetAccountPicture::GetVideoButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    HideImageAndVideoControls();

    if (!UserInformation::NameAccessAllowed)
    {
        rootPage->NotifyUser("Access to account picture disabled by Privacy Setting or Group Policy", NotifyType::ErrorMessage);
    }
    else
    {
        auto videoFile = UserInformation::GetAccountPicture(AccountPictureKind::Video);
        if (videoFile != nullptr)
        {
            rootPage->NotifyUser("Path: " + videoFile->Path, NotifyType::StatusMessage);
            create_task(videoFile->OpenReadAsync()).then(
                [this](task<IRandomAccessStreamWithContentType^> videoStreamTask)
                {
                    try
                    {
                        auto videoStream = videoStreamTask.get();
                        AccountPictureVideo->SetSource(videoStream, "video/mp4");
                        AccountPictureVideo->Visibility = Windows::UI::Xaml::Visibility::Visible;
                    }
                    catch (Exception^ Ex)
                    {
                        rootPage->NotifyUser("No large account picture image returned for current user.", NotifyType::ErrorMessage);
                    }
                });
        }
        else
        {
            rootPage->NotifyUser("No video account picture returned for current user.", NotifyType::ErrorMessage);
        }
    }
}

void GetAccountPicture::HideImageAndVideoControls()
{
    AccountPictureImage->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    AccountPictureVideo->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}
