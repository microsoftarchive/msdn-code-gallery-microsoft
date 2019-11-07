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
// Msappdata.xaml.cpp
// Implementation of the Msappdata class
//

#include "pch.h"
#include "Msappdata.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace concurrency;

Msappdata::Msappdata()
{
    InitializeComponent();
}

task<void> Msappdata::CopyFile(Uri^ sourceUri, StorageFolder^ destFolder, String^ destFilename)
{
    return create_task(StorageFile::GetFileFromApplicationUriAsync(sourceUri)).then([=](StorageFile^ sourceFile) 
    {
        return sourceFile->CopyAsync(destFolder, destFilename, NameCollisionOption::FailIfExists);
    }).then([=](task<StorageFile^> copyTask)
    {
        try
        {
            auto destFile = copyTask.get();
        }
        catch (Platform::Exception ^e) 
        {
            if (e->HResult == HRESULT_FROM_WIN32(ERROR_ALREADY_EXISTS))
            {
                // Ignore this error.  It's expected that the file may already have been copied into place.
            }
            else
            {
                rootPage->NotifyUser(e->Message, NotifyType::ErrorMessage);
            }
        }
    });
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Msappdata::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    ApplicationData^ appData = ApplicationData::Current;

    // Copy the files into their respective ApplicationData folders, and then set the image source.

    CopyFile(ref new Uri("ms-appx:///assets/appDataLocal.png"), appData->LocalFolder, "appDataLocal.png").then([=]()
    {
        LocalImage->Source = ref new BitmapImage(ref new Uri("ms-appdata:///local/appDataLocal.png"));
    });

    CopyFile(ref new Uri("ms-appx:///assets/appDataRoaming.png"), appData->RoamingFolder, "appDataRoaming.png").then([=]()
    {
        RoamingImage->Source = ref new BitmapImage(ref new Uri("ms-appdata:///roaming/appDataRoaming.png"));
    });

    CopyFile(ref new Uri("ms-appx:///assets/appDataTemp.png"), appData->TemporaryFolder, "appDataTemp.png").then([=]()
    {
        TempImage->Source = ref new BitmapImage(ref new Uri("ms-appdata:///temp/appDataTemp.png"));
    });
}

/// <summary>
/// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
/// </summary>
/// <param name="e">
/// Event data that can be examined by overriding code. The event data is representative
/// of the navigation that will unload the current Page unless canceled. The
/// navigation can potentially be canceled by setting Cancel.
/// </param>
void Msappdata::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    LocalImage->Source = nullptr;
    RoamingImage->Source = nullptr;
    TempImage->Source = nullptr;
}