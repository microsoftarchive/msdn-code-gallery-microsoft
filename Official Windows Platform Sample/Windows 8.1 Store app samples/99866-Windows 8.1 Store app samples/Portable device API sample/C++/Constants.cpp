//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Get device properties", "SDKSample.PortableDeviceCPP.S1_DeviceProperties" }, 
    { "List objects", "SDKSample.PortableDeviceCPP.S2_Enumeration" },
    { "Send file to device", "SDKSample.PortableDeviceCPP.S3_SendToDevice" },
    { "Get file from device", "SDKSample.PortableDeviceCPP.S4_GetFromDevice" },
    { "Event registration", "SDKSample.PortableDeviceCPP.S5_Events" },
    { "Get device status", "SDKSample.PortableDeviceCPP.S6_DeviceStatus" }
}; 

/// <summary>
/// Calls MainPage::NotifyUser from the UI thread.
/// </summary>
/// <param name="message"></param>
/// <param name="type"></param>
void MainPage::DispatcherNotifyUser(String^ message, NotifyType type)
{
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, message, type]()
            {
                NotifyUser(message, type);
            },
            CallbackContext::Any
        )
    );
}

/// <summary>
/// Returns an IPortableDeviceValues containing information about this client app.
/// </summary>
/// <returns>The client information.</returns>
ComPtr<IPortableDeviceValues> SDKSample::GetClientInfo()
{
    ComPtr<IPortableDeviceValues> result;

    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceValues, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&result)));

    result->SetStringValue(WPD_CLIENT_NAME, MainPage::FEATURE_NAME->Begin());
    result->SetUnsignedIntegerValue(WPD_CLIENT_MAJOR_VERSION, 1);
    result->SetUnsignedIntegerValue(WPD_CLIENT_MINOR_VERSION, 0);
    result->SetUnsignedIntegerValue(WPD_CLIENT_REVISION, 0);
    return result;
}

/// <summary>
/// Returns the first storage object identifier for a portable device.
/// </summary>
/// <param name="device">Pointer to the IPortableDevice for a portable device.</param>
/// <returns>Object identifier string for the first storage on the device.</returns>
String^ SDKSample::GetFirstStorageId(_In_ IPortableDevice* device)
{
    PROPVARIANT value = {};
    PropVariantPtr valuePtr(&value);
    DWORD storageCount = 0;
    String^ storageId = nullptr;
    ComPtr<IPortableDeviceCapabilities> capabilities;
    ComPtr<IPortableDevicePropVariantCollection> storageIds;

    ThrowIfFailed(device->Capabilities(&capabilities));
    if (SUCCEEDED(capabilities->GetFunctionalObjects(WPD_FUNCTIONAL_CATEGORY_STORAGE, &storageIds)))
    {
        ThrowIfFailed(storageIds->GetCount(&storageCount));
        if (storageCount > 0)
        {
            ThrowIfFailed(storageIds->GetAt(0, &value));
            storageId = ref new String(value.pwszVal);
        }
    }
    return storageId;
}

/// <summary>
/// Copies data from a source stream to a destination stream using the
/// specified transferSizeBytes as the temporary buffer size.
/// </summary>
/// <param name="destStream">Pointer to the destination IStream.</param>
/// <param name="sourceStream">Pointer to the source IStream.</param>
/// <param name="transferSizeBytes">The temporary buffer size to use during the copy.</param>
/// <returns>The total number of bytes copied to the destination stream.</returns>
ULONGLONG SDKSample::StreamCopy(_In_ IStream* destStream, _In_ IStream* sourceStream, DWORD transferSizeBytes)
{
    HRESULT hr = S_OK;
    DWORD bytesRead = 0;
    ULONGLONG totalBytesWritten = 0;

    // Allocate a temporary buffer (of Optimal transfer size) for the read results to be written to.
    std::unique_ptr<BYTE[]> objectData(new BYTE[transferSizeBytes]);

    // Read until the number of bytes returned from the source stream is 0, or
    // an error occured during transfer.
    do
    {
        // Read object data from the source stream
        hr = sourceStream->Read(objectData.get(), transferSizeBytes, &bytesRead);
        if (SUCCEEDED(hr))
        {
            // Write object data to the destination stream
            DWORD bytesWritten = 0;
            hr = destStream->Write(objectData.get(), bytesRead, &bytesWritten);
            if (SUCCEEDED(hr))
            {
                totalBytesWritten += bytesWritten;
            }
        }

    } while (SUCCEEDED(hr) && (bytesRead > 0));

    return totalBytesWritten;
}
