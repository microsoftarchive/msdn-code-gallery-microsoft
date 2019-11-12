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
// S4_GetFromDevice.xaml.cpp
// Implementation of the S4_GetFromDevice class
//

#include "pch.h"
#include "S4_GetFromDevice.xaml.h"

using namespace SDKSample::PortableDeviceCPP;

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S4_GetFromDevice::S4_GetFromDevice() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S4_GetFromDevice::OnNavigatedTo(NavigationEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// This is the click handler for the 'Run' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S4_GetFromDevice::Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowDeviceSelectorAsync();
}

/// <summary>
/// Enumerates all portable devices and populates the device selection list.
/// </summary>
void S4_GetFromDevice::ShowDeviceSelectorAsync()
{
    _deviceInfoCollection = nullptr;

    // Find all portable devices
    String^ aqsFilter = "System.Devices.InterfaceClassGuid:=\"{6AC27878-A6FA-4155-BA85-F98F491D4F33}\" AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True";
    create_task(DeviceInformation::FindAllAsync(aqsFilter)).then([this] (DeviceInformationCollection^ deviceInfoCollection)
    {
        _deviceInfoCollection = deviceInfoCollection;
        if (_deviceInfoCollection->Size > 0)
        {
            auto items = ref new Vector<String^>();
            std::for_each(begin(_deviceInfoCollection), end(_deviceInfoCollection), [items](IDeviceInformation^ deviceInfoElement)
            {
                items->Append(ref new String(deviceInfoElement->Name->Begin()));
            });
            cvs->Source = items;
            DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Visible;
        }
        else
        {
            rootPage->NotifyUser("No portable devices were found. Please attach a portable device to the system (e.g. a camera, music player, or cellular phone)", NotifyType::ErrorMessage);
        }
    });
}

/// <summary>
/// This is the tapped handler for the device selection list. It runs the scenario
/// for the selected portable device.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S4_GetFromDevice::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    GetContentFromDeviceAsync(deviceInfo);
}

/// <summary>
/// Transfers content from the selected portable device.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device.</param>
void S4_GetFromDevice::GetContentFromDeviceAsync(DeviceInformation^ deviceInfoElement)
{
    ComPtr<IPortableDevice> device;
    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceFTM, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&device)));

    auto clientInfo = GetClientInfo();
    HRESULT hr = device->Open(deviceInfoElement->Id->Begin(), clientInfo.Get());
    if (hr == E_ACCESSDENIED)
    {
        rootPage->NotifyUser("Access to " + deviceInfoElement->Name + " is denied. Only a Privileged Application may access this device.", NotifyType::ErrorMessage);
        return;
    }
    ThrowIfFailed(hr);

    // Perform the find and transfer asynchronously
    create_task([this, device, deviceInfoElement]() -> void
    {
        FindAndTransferFirstImageFileFromDevice(device.Get());
    }).then([this, deviceInfoElement] (task<void> thisTask)
    {
        // Handle errors. We're catching all exceptions here because the device may return any failure HRESULT.
        try
        {
            thisTask.get();
        }
        catch (Exception^ e)
        {
            // Call NotifyUser from the UI thread
            rootPage->DispatcherNotifyUser("Transfer from " + deviceInfoElement->Name + " failed with error: " + e->Message, NotifyType::ErrorMessage);
        }
    });
}

/// <summary>
/// Finds and transfers the first image file from the selected portable device.
/// This can process many objects and is strongly recommended to be run asynchronously
/// to avoid blocking the UI thread. GetContentFromDeviceAsync demonstrates how to wrap
/// this function in a task and handle errors.
/// </summary>
/// <param name="device">Pointer to the IPortableDevice for a portable device.</param>
/// <see cref="GetContentFromDeviceAsync"/>
void S4_GetFromDevice::FindAndTransferFirstImageFileFromDevice(_In_ IPortableDevice* device)
{
    ComPtr<IPortableDeviceContent> content;
    ComPtr<IPortableDeviceProperties> properties;

    ThrowIfFailed(device->Content(&content));
    ThrowIfFailed(content->Properties(&properties));

    // Traverse the first level under storage and look for images
    auto storageId = GetFirstStorageId(device);
    if (storageId != nullptr)
    {
        ComPtr<IEnumPortableDeviceObjectIDs> enumObjectIDs;
        ThrowIfFailed(content->EnumObjects(0, storageId->Begin(), nullptr, &enumObjectIDs));

        bool found = false;
        HRESULT hrEnum = S_OK;
        while(hrEnum == S_OK && !found)
        {
            DWORD numFetched = 0;
            PWSTR objectIDArray[NUM_OBJECTS_TO_REQUEST] = {};
            CoTaskMemFreeArray arrayPtr(reinterpret_cast<void **>(objectIDArray), ARRAYSIZE(objectIDArray));

            hrEnum = enumObjectIDs->Next(NUM_OBJECTS_TO_REQUEST, // Number of objects to request on each NEXT call
                                         objectIDArray,          // Array of PWSTR array which will be populated on each NEXT call
                                         &numFetched);           // Number of objects written to the PWSTR array
            ThrowIfFailed(hrEnum);

            for (DWORD index = 0; !found && (index < numFetched) && (objectIDArray[index] != nullptr); index++)
            {
                GUID contentType;
                ComPtr<IPortableDeviceValues> objectProperties;
                rootPage->DispatcherNotifyUser(ref new String(objectIDArray[index]), NotifyType::StatusMessage);

                ThrowIfFailed(properties->GetValues(objectIDArray[index], nullptr, &objectProperties));

                if (SUCCEEDED(objectProperties->GetGuidValue(WPD_OBJECT_CONTENT_TYPE, &contentType)) && (contentType == WPD_CONTENT_TYPE_IMAGE))
                {
                    PWSTR sourceFilename = nullptr;
                    HRESULT hr = objectProperties->GetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, &sourceFilename);
                    CoTaskMemString sourceFilenamePtr(sourceFilename);
                    ThrowIfFailed(hr);

                    String^ objectToTransfer = ref new String(objectIDArray[index]);
                    String^ desiredFilename = ref new String(sourceFilename);
                    TransferFileFromDeviceToAppLocalFolderAsync(device, objectToTransfer, desiredFilename);
                    found = true;
                }
            }
        }

        if (!found)
        {
            rootPage->DispatcherNotifyUser("No images were found. Please run scenario 3 first to transfer an image to the device.", NotifyType::ErrorMessage);
        }
    }
    else
    {
        rootPage->DispatcherNotifyUser("No storages were found on the device. Please attach a device with at least 1 storage.", NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Transfers a file from a portable device to the application local folder.
/// </summary>
/// <param name="device">Pointer to the IPortableDevice for a portable device.</param>
/// <param name="sourceObjectId">Pointer to the IPortableDevice for a portable device.</param>
/// <param name="desiredFilename">Pointer to the IPortableDevice for a portable device.</param>
void S4_GetFromDevice::TransferFileFromDeviceToAppLocalFolderAsync(_In_ IPortableDevice* device, String^ sourceObjectId, String^ desiredFilename)
{
    // Ensures the lifetime of the device extends through the lambdas
    ComPtr<IPortableDevice> deviceRef = device;

    // Allows the StorageFile to be shared between task continuations
    auto newFileRef = std::make_shared<StorageFile^>(nullptr);

    task<StorageFile^> createFileTask(ApplicationData::Current->LocalFolder->CreateFileAsync(desiredFilename, CreationCollisionOption::GenerateUniqueName));
    createFileTask.then([this, newFileRef] (StorageFile^ newFile)
    {
        *newFileRef = newFile; // assign for use later in the continuation
        rootPage->DispatcherNotifyUser("Created new file '" + newFile->Path + "', transfer data ....", NotifyType::StatusMessage);
        return newFile->OpenAsync(FileAccessMode::ReadWrite);

    }).then([this, deviceRef, sourceObjectId, newFileRef] (IRandomAccessStream^ randomAccessStream)
    {
        ComPtr<IPortableDeviceContent> content;
        ComPtr<IPortableDeviceResources> resources;

        ThrowIfFailed(deviceRef->Content(&content));
        ThrowIfFailed(content->Transfer(&resources));

        // Get the object's size
        ComPtr<IPortableDeviceValues> resourceAttributes;
        ULONGLONG resourceSizeBytes = 0;
        ThrowIfFailed(resources->GetResourceAttributes(sourceObjectId->Begin(), WPD_RESOURCE_DEFAULT, &resourceAttributes));
        ThrowIfFailed(resourceAttributes->GetUnsignedLargeIntegerValue(WPD_RESOURCE_ATTRIBUTE_TOTAL_SIZE, &resourceSizeBytes));

        // Get the object's data
        ComPtr<IStream> sourceStream, destStream;
        DWORD optimalTransferSizeBytes = 0;
        ThrowIfFailed(resources->GetStream(sourceObjectId->Begin(), // Identifier of the object we want to transfer
                                WPD_RESOURCE_DEFAULT,               // We are transferring the default resource (which is the entire object's data)
                                STGM_READ,                          // Opening a stream in READ mode, because we are reading data from the device.
                                &optimalTransferSizeBytes,          // Optimal transfer size
                                &sourceStream));                    // IStream containing the object data

        // Transfer the object's data to to the file stream
        ThrowIfFailed(CreateStreamOverRandomAccessStream(randomAccessStream, IID_PPV_ARGS(&destStream)));

        ULONGLONG totalTransferredBytes = StreamCopy(destStream.Get(), sourceStream.Get(), optimalTransferSizeBytes);
        if (totalTransferredBytes == resourceSizeBytes)
        {
            // Flush the destination stream to commit the data
            task<bool> flushTask(randomAccessStream->FlushAsync());
            flushTask.then([this, sourceObjectId, newFileRef, totalTransferredBytes] (bool result)
            {
                rootPage->DispatcherNotifyUser("Transfer of '" + sourceObjectId + "' (" 
                    + totalTransferredBytes.ToString() + " bytes) from device to '" + (*newFileRef)->Path
                    + "' " + (result ? "is successful" : "failed"), NotifyType::StatusMessage);
            });
        }
        else
        {
            rootPage->DispatcherNotifyUser("The object '" + sourceObjectId + "' was not completely transferred from the device (only "
                + totalTransferredBytes.ToString() + " bytes were transferred).", NotifyType::ErrorMessage);
        }
    });
}
