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
// S6_DeviceStatus.xaml.cpp
// Implementation of the S6_DeviceStatus class
//

#include "pch.h"

// Status device service declarations
#include <initguid.h>
#include <guiddef.h>
#include <propkeydef.h>
#define DEFINE_DEVSVCGUID DEFINE_GUID
#define DEFINE_DEVSVCPROPKEY DEFINE_PROPERTYKEY
#include <StatusDeviceService.h>

#include "S6_DeviceStatus.xaml.h"

using namespace SDKSample::PortableDeviceCPP;

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Portable;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S6_DeviceStatus::S6_DeviceStatus() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S6_DeviceStatus::OnNavigatedTo(NavigationEventArgs^ e)
{
    ScenarioOutput->Text = "";
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// This is the click handler for the 'Run' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S6_DeviceStatus::Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowDeviceSelectorAsync();
}

/// <summary>
/// Enumerates all MTP device services and populates the device selection list.
/// </summary>
void S6_DeviceStatus::ShowDeviceSelectorAsync()
{
    ScenarioOutput->Text = "";
    _deviceInfoCollection = nullptr;

    // Find all portable devices
    String^ aqsFilter = ServiceDevice::GetDeviceSelector(ServiceDeviceType::DeviceStatusService);
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
            rootPage->NotifyUser("No portable devices were found. Please attach a portable device to the system that supports the MTP Device Status service (e.g. the MTP Device Simulator)", NotifyType::ErrorMessage);
        }
    });
}

/// <summary>
/// This is the tapped handler for the device selection list. It runs the scenario
/// for the selected portable device.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S6_DeviceStatus::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    DisplayDeviceStatusAsync(deviceInfo);
}

/// <summary>
/// Gets device status from the selected MTP device status service.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device status service.</param>
void S6_DeviceStatus::DisplayDeviceStatusAsync(DeviceInformation^ deviceInfoElement)
{
    ComPtr<IPortableDeviceService> service;
    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceServiceFTM, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&service)));

    ComPtr<IPortableDeviceServiceActivation> activator;
    ThrowIfFailed(service.As(&activator));

    OnServiceOpenComplete^ openCallback = ref new OnServiceOpenComplete([this, deviceInfoElement, service] (HRESULT hr)
    {
        if (SUCCEEDED(hr))
        {
            rootPage->DispatcherNotifyUser("Successfully opened MTP device status service for " + deviceInfoElement->Name, NotifyType::StatusMessage);
            DisplayDeviceStatusServiceProperties(service.Get());
        }
        else
        {
            if (hr == E_ACCESSDENIED)
            {
                rootPage->DispatcherNotifyUser("Access to the MTP device status service for " + deviceInfoElement->Name + " is denied. Only a Privileged Application may access this device.", NotifyType::ErrorMessage);
            }
            else
            {
                rootPage->DispatcherNotifyUser("Accessing the MTP device status service for " + deviceInfoElement->Name + " failed with error code: " + hr.ToString(), NotifyType::ErrorMessage);
            }
        }
    });

    auto clientInfo = GetClientInfo();
    ThrowIfFailed(activator->OpenAsync(deviceInfoElement->Id->Begin(), clientInfo.Get(), reinterpret_cast<IPortableDeviceServiceOpenCallback*>(openCallback)));
}

/// <summary>
/// Retrieves and displays the device status, which is available as properties of the MTP device status service.
/// </summary>
/// <param name="device">Pointer to the IPortableDeviceService for the MTP status service.</param>
void S6_DeviceStatus::DisplayDeviceStatusServiceProperties(_In_ IPortableDeviceService* service)
{
    PWSTR serviceObjectId = nullptr;
    HRESULT hr = service->GetServiceObjectID(&serviceObjectId);
    CoTaskMemString serviceObjectIdPtr(serviceObjectId);
    ThrowIfFailed(hr);

    ComPtr<IPortableDeviceContent2> content;
    ThrowIfFailed(service->Content(&content));

    ComPtr<IPortableDeviceProperties> properties;
    ThrowIfFailed(content->Properties(&properties));

    ComPtr<IPortableDeviceKeyCollection> propertiesToRead;
    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceKeyCollection, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&propertiesToRead)));

    // device status values are properties on the device's Status service
    ThrowIfFailed(propertiesToRead->Add(PKEY_StatusSvc_SignalStrength));
    ThrowIfFailed(propertiesToRead->Add(PKEY_StatusSvc_VoiceMail));
    ThrowIfFailed(propertiesToRead->Add(PKEY_StatusSvc_NetworkName));
    ThrowIfFailed(propertiesToRead->Add(PKEY_StatusSvc_NetworkType));

    ComPtr<IPortableDeviceValues> deviceStatusProperties;
    ThrowIfFailed(properties->GetValues(serviceObjectId, propertiesToRead.Get(), &deviceStatusProperties));

    DWORD statusNumericValue = 0;
    PWSTR statusStringValue = nullptr;
    String^ statusResult = "Device Status\n";
    if (SUCCEEDED(deviceStatusProperties->GetUnsignedIntegerValue(PKEY_StatusSvc_SignalStrength, &statusNumericValue)))
    {
        statusResult += "Signal Strength: " + statusNumericValue.ToString() + "\n";
    }

    if (SUCCEEDED(deviceStatusProperties->GetUnsignedIntegerValue(PKEY_StatusSvc_VoiceMail, &statusNumericValue)))
    {
        statusResult += "New Voice Mails: " + statusNumericValue.ToString() + "\n";
    }

    if (SUCCEEDED(deviceStatusProperties->GetStringValue(PKEY_StatusSvc_NetworkName, &statusStringValue)))
    {
        statusResult += "Network Name: " + ref new String(statusStringValue) + "\n";
    }
    CoTaskMemFree(statusStringValue);
    statusStringValue = nullptr;

    if (SUCCEEDED(deviceStatusProperties->GetStringValue(PKEY_StatusSvc_NetworkType, &statusStringValue)))
    {
        statusResult += "Network Type: " + ref new String(statusStringValue) + "\n";
    }
    CoTaskMemFree(statusStringValue);
    statusStringValue = nullptr;

    Dispatcher->RunAsync(CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, statusResult]()
            {
                ScenarioOutput->Text = statusResult;
            },
            CallbackContext::Any
        )
    );
}
