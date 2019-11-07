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
// S2_SendToStorage.xaml.cpp
// Implementation of the S2_SendToStorage class
//

#include "pch.h"
#include "S2_SendToStorage.xaml.h"

using namespace SDKSample::RemovableStorageCPP;

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Portable;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S2_SendToStorage::S2_SendToStorage() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S2_SendToStorage::OnNavigatedTo(NavigationEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// This is the click handler for the 'Send Image' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S2_SendToStorage::SendImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowDeviceSelectorAsync();
}

/// <summary>
/// Enumerates all storages and populates the device selection list.
/// </summary>
void S2_SendToStorage::ShowDeviceSelectorAsync()
{
    _deviceInfoCollection = nullptr;

    // Find all storage devices using Windows.Devices.Enumeration
    create_task(DeviceInformation::FindAllAsync(StorageDevice::GetDeviceSelector())).then([this] (DeviceInformationCollection^ deviceInfoCollection)
    {
        _deviceInfoCollection = deviceInfoCollection;
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
void S2_SendToStorage::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    SendImageFileToStorageAsync(deviceInfo);
}

/// <summary>
/// Sends a user-selected image file to the storage referenced by the device information element.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device.</param>
void S2_SendToStorage::SendImageFileToStorageAsync(DeviceInformation^ deviceInfoElement)
{
    // Verify that we are currently not snapped, or that we can unsnap to open the file picker
    auto currentState = Windows::UI::ViewManagement::ApplicationView::Value;
    if (currentState == Windows::UI::ViewManagement::ApplicationViewState::Snapped && !Windows::UI::ViewManagement::ApplicationView::TryUnsnap())
    {
        rootPage->NotifyUser("Could not unsnap (required to launch the file open picker). Please unsnap this app before proceeding", NotifyType::ErrorMessage);
        return;
    }

    // Launch the picker to select an image file
    auto picker = ref new FileOpenPicker();
    picker->FileTypeFilter->Append(".jpg");
    picker->FileTypeFilter->Append(".png");
    picker->FileTypeFilter->Append(".gif");
    picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;

    create_task(picker->PickSingleFileAsync()).then([this, deviceInfoElement] (StorageFile^ sourceFile)
    {
        if (sourceFile != nullptr)
        {
            // Convert the selected device information element to a StorageFolder
            auto storage = StorageDevice::FromId(deviceInfoElement->Id);
            auto storageName = deviceInfoElement->Name;

            rootPage->NotifyUser("Copying image: " + sourceFile->Name + " to " + storageName + " ...", NotifyType::StatusMessage);
            CopyFileToFolderOnStorageAsync(sourceFile, storage);
        }
        else
        {
            rootPage->NotifyUser("No file was selected", NotifyType::StatusMessage);
        }
    });
}

/// <summary>
/// Copies a file to the first folder on a storage.
/// </summary>
/// <param name="sourceFile"></param>
/// <param name="storage"></param>
void S2_SendToStorage::CopyFileToFolderOnStorageAsync(StorageFile^ sourceFile, StorageFolder^ storage)
{
    auto storageName = storage->Name;

    // Construct a folder search to find sub-folders under the current storage.
    // The default (shallow) query should be sufficient in finding the first level of sub-folders.
    // If the first level of sub-folders are not writable, a deep query + recursive copy may be needed.
    create_task(storage->GetFoldersAsync()).then([this, sourceFile, storageName] (IVectorView<StorageFolder^>^ folders)
    {
        if (folders->Size > 0)
        {
            auto destinationFolder = folders->GetAt(0);
            auto destinationFolderName = destinationFolder->Name;
            rootPage->NotifyUser("Trying the first folder on storage: " + destinationFolderName + "...", NotifyType::StatusMessage);

            // The following CopyAsync task is nested (instead of chained) with the containing GetFoldersAsync task because we only call it when a sub-folder is present.
            create_task(sourceFile->CopyAsync(destinationFolder, sourceFile->Name, NameCollisionOption::GenerateUniqueName)).then([this, storageName, destinationFolderName] (task<StorageFile^> thisTask)
            {
                try
                {
                    auto newFile = thisTask.get();
                    rootPage->NotifyUser("Image " + newFile->Name + " created on " + storageName, NotifyType::StatusMessage);
                }
                catch (Exception^ e) // Catching all exceptions because different removable storages can exhibit different error behavior on CopyAsync
                {
                    rootPage->NotifyUser("Caught exception while copying the image to the first sub-folder: " + destinationFolderName + ", " + storageName + " may not allow sending files to its top level folders. Error: " + e->Message, NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            rootPage->NotifyUser("No sub-folders found on " + storageName + " to copy to", NotifyType::StatusMessage);
        }
    });
}
