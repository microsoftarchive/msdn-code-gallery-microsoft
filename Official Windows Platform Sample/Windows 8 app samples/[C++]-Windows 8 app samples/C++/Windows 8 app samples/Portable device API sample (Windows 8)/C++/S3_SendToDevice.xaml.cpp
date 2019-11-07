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
// S3_SendToDevice.xaml.cpp
// Implementation of the S3_SendToDevice class
//

#include "pch.h"
#include "S3_SendToDevice.xaml.h"

using namespace SDKSample::PortableDeviceCPP;

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S3_SendToDevice::S3_SendToDevice() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S3_SendToDevice::OnNavigatedTo(NavigationEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// This is the click handler for the 'Run' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S3_SendToDevice::Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowDeviceSelectorAsync();
}

/// <summary>
/// Enumerates all portable devices and populates the device selection list.
/// </summary>
void S3_SendToDevice::ShowDeviceSelectorAsync()
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
void S3_SendToDevice::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    SendContentToDeviceAsync(deviceInfo);
}

/// <summary>
/// Launches the picker to select a file and then calls TransferFileToDevice to send
/// the file to the selected portable device.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device.</param>
void S3_SendToDevice::SendContentToDeviceAsync(DeviceInformation^ deviceInfoElement)
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

    // Verify that we are currently not snapped, or that we can unsnap to open the file picker
    auto currentState = Windows::UI::ViewManagement::ApplicationView::Value;
    if (currentState == Windows::UI::ViewManagement::ApplicationViewState::Snapped && !Windows::UI::ViewManagement::ApplicationView::TryUnsnap())
    {
        rootPage->NotifyUser("Could not unsnap (required to launch the file open picker). Please unsnap this app before proceeding", NotifyType::ErrorMessage);
        return;
    }

    auto picker = ref new FileOpenPicker();
    picker->FileTypeFilter->Append(".jpg");
    picker->FileTypeFilter->Append(".png");
    picker->FileTypeFilter->Append(".gif");
    picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;

    create_task(picker->PickSingleFileAsync()).then([this, device](StorageFile^ sourceFile)
    {
        if (sourceFile != nullptr)
        {
            rootPage->NotifyUser("Selected file to send to device: " + sourceFile->Name, NotifyType::StatusMessage);
            TransferFileToDeviceAsync(sourceFile, device.Get());
        }
        else
        {
            rootPage->NotifyUser("No file was selected", NotifyType::StatusMessage);
        }
    });
}

/// <summary>
/// Sends a file to the device using the Windows.Storage and Portable Device COM APIs.
/// </summary>
/// <param name="file">The file to send to the device.</param>
/// <param name="device">Pointer to the IPortableDevice for a portable device.</param>
void S3_SendToDevice::TransferFileToDeviceAsync(StorageFile^ file, _In_ IPortableDevice* device)
{
    if (file == nullptr) throw ref new Platform::NullReferenceException;

    // Allows the random access stream pointer to be shared between tasks and task continuations
    auto randomAccessStream = std::make_shared<IRandomAccessStream^>(nullptr);
    
    auto storageId = GetFirstStorageId(device);
    if (storageId != nullptr)
    {
        ComPtr<IPortableDevice> deviceRef = device; // for capturing in the completion lambda

        create_task(file->OpenAsync(FileAccessMode::Read)).then([this, file, randomAccessStream] (IRandomAccessStream^ rastream)
        {
            if (rastream == nullptr) throw ref new Platform::NullReferenceException;
            *randomAccessStream = rastream;
            return file->GetBasicPropertiesAsync();

        }).then([this, file, deviceRef, storageId, randomAccessStream] (BasicProperties^ basicProperties)
        {
            ComPtr<IStream> sourceStream, destStream;
            DWORD optimalTransferSizeBytes = 0;

            ComPtr<IPortableDeviceContent> content;
            ThrowIfFailed(deviceRef->Content(&content));

            // Create a new object with properties and data
            auto imageProperties = FillInPropertiesForImage(file, storageId, basicProperties->Size);
            HRESULT hr = content->CreateObjectWithPropertiesAndData(imageProperties.Get(), // Properties describing the object data
                                                            &destStream,                    // Returned object data stream (to transfer the data to)
                                                            &optimalTransferSizeBytes,      // Returned optimal buffer size to use during transfer
                                                            nullptr);
            if (hr == HRESULT_FROM_WIN32(ERROR_FILE_EXISTS))
            {
                rootPage->NotifyUser("The file '" + file->Name + "' already exists on the device, please try a different file or remove the existing one from the device.",
                    NotifyType::ErrorMessage);
                return;
            }
            ThrowIfFailed(hr);

            // Transfer the object data
            ThrowIfFailed(CreateStreamOverRandomAccessStream(*randomAccessStream, IID_PPV_ARGS(&sourceStream)));
            ULONGLONG totalTransferredBytes = StreamCopy(destStream.Get(), sourceStream.Get(), optimalTransferSizeBytes);
            if (totalTransferredBytes >= basicProperties->Size)
            {
                // Commit the object data to save it
                ComPtr<IPortableDeviceDataStream> dataStream;
                ThrowIfFailed(destStream.As(&dataStream));
                ThrowIfFailed(dataStream->Commit(STGC_DEFAULT));

                // Get the object Id of the newly created object
                PWSTR newlyCreatedObjectId = nullptr;
                HRESULT hr = dataStream->GetObjectID(&newlyCreatedObjectId);
                CoTaskMemString objectIdPtr(newlyCreatedObjectId); // Ensure that the object Id is properly freed upon an exception
                ThrowIfFailed(hr);

                rootPage->NotifyUser("The file '" + file->Path + "' was transferred to the device (" + totalTransferredBytes.ToString()
                    + " bytes).\nThe newly created object's ID is '" + ref new String(newlyCreatedObjectId), NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("The file '" + file->Path + "' was not completely transferred to the device (only "
                    + totalTransferredBytes.ToString() + " bytes were transferred).", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("No storages were found on the device. Please attach a device with at least 1 storage", NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Returns an IPortableDeviceValues containing properties used for creating an image object on the device.
/// </summary>
/// <param name="file">The image file that will be transferred to the new image object.</param>
/// <param name="parentId">The parent object identifier for the new image object.</param>
/// <param name="size">The size of the new image object.</param>
/// <returns>IPortableDeviceValues ComPtr containing the image object property keys and values.</returns>
#define COMPILETIMESTRLEN(A) ARRAYSIZE(A) - 1
ComPtr<IPortableDeviceValues> S3_SendToDevice::FillInPropertiesForImage(StorageFile^ file, String^ parentId, ULONGLONG size)
{
    ComPtr<IPortableDeviceValues> result;
    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceValues, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&result)));

    ThrowIfFailed(result->SetStringValue(WPD_OBJECT_PARENT_ID, parentId->Begin()));
    ThrowIfFailed(result->SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, size));
    ThrowIfFailed(result->SetStringValue(WPD_OBJECT_NAME, file->DisplayName->Begin()));
    ThrowIfFailed(result->SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, file->Name->Begin()));
    ThrowIfFailed(result->SetGuidValue(WPD_OBJECT_CONTENT_TYPE, WPD_CONTENT_TYPE_IMAGE));

    // using culture-insensitive, case-insensitive comparison for the file extensions
    if (0 == CompareStringOrdinal(file->FileType->Data(), file->FileType->Length(), L".gif", COMPILETIMESTRLEN(L".gif"), TRUE))
    {
        ThrowIfFailed(result->SetGuidValue(WPD_OBJECT_FORMAT, WPD_OBJECT_FORMAT_GIF));
    }
    else if (0 == CompareStringOrdinal(file->FileType->Data(), file->FileType->Length(), L".png", COMPILETIMESTRLEN(L".png"), TRUE))
    {
        ThrowIfFailed(result->SetGuidValue(WPD_OBJECT_FORMAT, WPD_OBJECT_FORMAT_PNG));
    }
    else
    {
        // since we already filtered the file types in the file picker, we can fall back to jpg
        ThrowIfFailed(result->SetGuidValue(WPD_OBJECT_FORMAT, WPD_OBJECT_FORMAT_EXIF));
    }
    return result;
}
