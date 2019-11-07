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
// S4_Autoplay.xaml.cpp
// Implementation of the S4_Autoplay class
//

#include "pch.h"
#include "S4_Autoplay.xaml.h"

using namespace SDKSample::RemovableStorageCPP;

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Portable;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Search;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

S4_Autoplay::S4_Autoplay() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S4_Autoplay::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Enable the button only when launched from Autoplay or File Activation
    ScenarioInput->IsEnabled = (rootPage->AutoplayFileSystemDeviceFolder != nullptr ||
                                rootPage->AutoplayNonFileSystemDeviceId  != nullptr ||
                                rootPage->FileActivationFiles            != nullptr);

}

/// <summary>
/// This is the click handler for the 'Get Image' button.
/// If launched by Autoplay, this will find and display the first image file on the storage.
/// If launched by file activation, this will display the first image file from the activation file list.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S4_Autoplay::GetImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (rootPage->FileActivationFiles != nullptr)
    {
        if (rootPage->FileActivationFiles->Size > 0)
        {
            // Because this sample only supports image file types in its manifest,
            // we can know that all files in the array of files will be image files.
            rootPage->NotifyUser("[File Activation] Displaying first image file ...", NotifyType::StatusMessage);
            auto imageFile = safe_cast<StorageFile^>(rootPage->FileActivationFiles->GetAt(0));
            DisplayImageAsync(imageFile);
        }
        else
        {
            rootPage->NotifyUser("[File Activation] File activation occurred but 0 files were received", NotifyType::ErrorMessage);
        }
    }
    else
    {
        if (rootPage->AutoplayFileSystemDeviceFolder != nullptr)
        {
            GetFirstImageFromStorageAsync(rootPage->AutoplayFileSystemDeviceFolder);
        }
        else
        {
            auto storage = StorageDevice::FromId(rootPage->AutoplayNonFileSystemDeviceId);
            GetFirstImageFromStorageAsync(storage);
        }
    }
}

/// <summary>
/// Finds and displays the first image file on the storage.
/// </summary>
/// <param name="storage"></param>
void S4_Autoplay::GetFirstImageFromStorageAsync(StorageFolder^ storage)
{
    auto storageName = storage->Name;

    // Construct the query for image files
    auto filter = ref new Vector<String^>();
    filter->Append(".jpg");
    filter->Append(".png");
    filter->Append(".gif");
    auto queryOptions = ref new QueryOptions(CommonFileQuery::OrderByName, filter);
    auto imageFileQuery = storage->CreateFileQueryWithOptions(queryOptions);

    // Run the query for image files
    rootPage->NotifyUser("[Launched by Autoplay] Looking for images on " + storageName + " ...", NotifyType::StatusMessage);
    create_task(imageFileQuery->GetFilesAsync()).then([this, storageName] (IVectorView<StorageFile^>^ imageFiles)
    {
        if (imageFiles->Size > 0)
        {
            auto imageFile = imageFiles->GetAt(0);
            rootPage->NotifyUser("[Launched by Autoplay] Found " + imageFile->Name + " on " + storageName, NotifyType::StatusMessage);
            DisplayImageAsync(imageFile);
        }
        else
        {
            rootPage->NotifyUser("[Launched by Autoplay] No images were found on " + storageName + ". You can use scenario 2 to transfer an image to it", NotifyType::StatusMessage);
        }
    });
}

/// <summary>
/// Displays an image file in the 'ScenarioOutputImage' element.
/// </summary>
/// <param name="imageFile">The image file to display.</param>
void S4_Autoplay::DisplayImageAsync(Windows::Storage::StorageFile^ imageFile)
{
    create_task(imageFile->GetBasicPropertiesAsync()).then([this, imageFile] (BasicProperties^ imageProperties)
    {
        if (imageProperties->Size > 0)
        {
            auto dateFormatter = ref new DateTimeFormatter("longdate");
            auto timeFormatter = ref new DateTimeFormatter("longtime");
            rootPage->NotifyUser("Displaying: " + imageFile->Name + ", date modified: " + 
                dateFormatter->Format(imageProperties->DateModified) + " " +
                timeFormatter->Format(imageProperties->DateModified) + ", size: " +
                imageProperties->Size + " bytes", NotifyType::StatusMessage);

            // Note: The following task is nested instead of chained from the previous task because it runs only when the image size is > 0.
            create_task(imageFile->OpenAsync(FileAccessMode::Read)).then([this] (IRandomAccessStream^ stream)
            {
                // BitmapImage.SetSource needs to be called in the UI thread
                Dispatcher->RunAsync(
                    CoreDispatcherPriority::Normal,
                    ref new DispatchedHandler(
                        [this, stream]()
                        {
                            auto bitmap = ref new BitmapImage();
                            bitmap->SetSource(stream);
                            ScenarioOutputImage->SetValue(Image::SourceProperty, bitmap);
                        },
                        CallbackContext::Any
                    )
                );
            });
        }
        else
        {
            rootPage->NotifyUser("Cannot display " + imageFile->Name + " because its size is 0", NotifyType::ErrorMessage);
        }
    });
}
