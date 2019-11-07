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
// S3_GetFromStorage.xaml.cpp
// Implementation of the S3_GetFromStorage class
//

#include "pch.h"
#include "S3_GetFromStorage.xaml.h"

using namespace SDKSample::RemovableStorageCPP;

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
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

S3_GetFromStorage::S3_GetFromStorage() : rootPage(MainPage::Current) 
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S3_GetFromStorage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    ScenarioOutputImage->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// This is the click handler for the 'Get Image' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S3_GetFromStorage::GetImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowDeviceSelectorAsync();
}

/// <summary>
/// Enumerates all storages and populates the device selection list.
/// </summary>
void S3_GetFromStorage::ShowDeviceSelectorAsync()
{
    _deviceInfoCollection = nullptr;

    // Find all storage devices using Windows.Devices.Enumeration
    task<DeviceInformationCollection^>(DeviceInformation::FindAllAsync(StorageDevice::GetDeviceSelector())).then([this] (task<DeviceInformationCollection^> findTask)
    {
        _deviceInfoCollection = findTask.get();
        if (_deviceInfoCollection->Size > 0)
        {
            auto items = ref new Vector<String^>();
            std::for_each(begin(_deviceInfoCollection), end(_deviceInfoCollection), [items](IDeviceInformation^ deviceInfo)
            {
                items->Append(ref new String(deviceInfo->Name->Begin()));
            });
            cvs->Source = items;
            DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Visible;
        }
        else
        {
            rootPage->NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType::StatusMessage);
        }
    });
}

/// <summary>
/// This is the tapped handler for the device selection list. It runs the scenario
/// for the selected storage.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S3_GetFromStorage::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    GetFirstImageFromStorageAsync(deviceInfo);
}

/// <summary>
/// Finds and displays the first image file on the storage referenced by the device information element.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device.</param>
void S3_GetFromStorage::GetFirstImageFromStorageAsync(DeviceInformation^ deviceInfoElement)
{
    // Convert the selected device information element to a StorageFolder
    auto storage = StorageDevice::FromId(deviceInfoElement->Id);
    auto storageName = deviceInfoElement->Name;

    // Construct the query for image files
    auto filter = ref new Vector<String^>();
    filter->Append(".jpg");
    filter->Append(".png");
    filter->Append(".gif");
    auto queryOptions = ref new QueryOptions(CommonFileQuery::OrderByName, filter);
    auto imageFileQuery = storage->CreateFileQueryWithOptions(queryOptions);

    // Run the query for image files
    rootPage->NotifyUser("Looking for images on " + storageName + " ...", NotifyType::StatusMessage);
    create_task(imageFileQuery->GetFilesAsync()).then([this, storageName] (IVectorView<StorageFile^>^ imageFiles)
    {
        if (imageFiles->Size > 0)
        {
            auto imageFile = imageFiles->GetAt(0);
            rootPage->NotifyUser("Found " + imageFile->Name + " on " + storageName, NotifyType::StatusMessage);
            DisplayImageAsync(imageFile);
        }
        else
        {
            rootPage->NotifyUser("No images were found on " + storageName + ". You can use scenario 2 to transfer an image to it", NotifyType::StatusMessage);
        }
    });
}

/// <summary>
/// Displays an image file in the 'ScenarioOutputImage' element.
/// </summary>
/// <param name="imageFile">The image file to display.</param>
void S3_GetFromStorage::DisplayImageAsync(Windows::Storage::StorageFile^ imageFile)
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
                            ScenarioOutputImage->Visibility = Windows::UI::Xaml::Visibility::Visible;
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
