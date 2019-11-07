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
// S2_Enumeration.xaml.cpp
// Implementation of the S2_Enumeration class
//

#include "pch.h"
#include "S2_Enumeration.xaml.h"

using namespace SDKSample::PortableDeviceCPP;

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S2_Enumeration::S2_Enumeration() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S2_Enumeration::OnNavigatedTo(NavigationEventArgs^ e)
{
    ScenarioOutput->Text = "";
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

/// <summary>
/// This is the click handler for the 'Run' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S2_Enumeration::Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowDeviceSelectorAsync();
}

/// <summary>
/// Enumerates all portable devices and populates the device selection list.
/// </summary>
void S2_Enumeration::ShowDeviceSelectorAsync()
{
    ScenarioOutput->Text = "";
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
void S2_Enumeration::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    EnumerateObjects(deviceInfo);
}

/// <summary>
/// Enumerates objects using the Portable Device COM APIs for the selected portable device
/// and displays them.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device.</param>
void S2_Enumeration::EnumerateObjects(DeviceInformation^ deviceInfoElement)
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

    ComPtr<IPortableDeviceContent> content;
    ThrowIfFailed(device->Content(&content));

    // Get the children of the device object
    ComPtr<IEnumPortableDeviceObjectIDs> enumObjectIDs;
    ThrowIfFailed(content->EnumObjects(0, WPD_DEVICE_OBJECT_ID, nullptr, &enumObjectIDs));

    rootPage->NotifyUser("Top level objects for " + deviceInfoElement->Name, NotifyType::StatusMessage);
    ScenarioOutput->Text = "";
    while(hr == S_OK)
    {
        DWORD numFetched = 0;
        PWSTR objectIDArray[NUM_OBJECTS_TO_REQUEST] = {};
        CoTaskMemFreeArray arrayPtr(reinterpret_cast<void **>(objectIDArray), ARRAYSIZE(objectIDArray));

        hr = enumObjectIDs->Next(NUM_OBJECTS_TO_REQUEST,    // Number of objects to request on each NEXT call
                                 objectIDArray,             // Array of PWSTR array which will be populated on each NEXT call
                                 &numFetched);              // Number of objects written to the PWSTR array

        for (DWORD index = 0; index < numFetched; index++)
        {
            // print the object identifiers
            ScenarioOutput->Text += ref new String(objectIDArray[index]) + "\n";
        }
    }
}
