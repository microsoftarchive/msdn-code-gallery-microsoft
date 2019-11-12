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
// S5_Events.xaml.cpp
// Implementation of the S5_Events class
//

#include "pch.h"
#include "Constants.h"
#include "EventCallback.h"
#include "S5_Events.xaml.h"

using namespace SDKSample::PortableDeviceCPP;

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S5_Events::S5_Events() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void S5_Events::OnNavigatedTo(NavigationEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    ScenarioOutput->Text = "";
    if (IsRegisteredForDeviceEvents())
    {
        ScenarioInput->Content = "Unregister for Events";
        ScenarioEventTrigger->IsEnabled = true;
    }
    else
    {
        ScenarioInput->Content = "Register for Events";
        ScenarioEventTrigger->IsEnabled = false;
    }
}

/// <summary>
/// Invoked when this page is no longer displayed.
/// </summary>
/// <param name="e></param>
void S5_Events::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Clean up the existing device event registration, if any.
    if (IsRegisteredForDeviceEvents())
    {
        UnregisterForDeviceEvents();
    }
}


/// <summary>
/// This is the click handler for the 'Run' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S5_Events::Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ScenarioOutput->Text = "";
    if (IsRegisteredForDeviceEvents())
    {
        UnregisterForDeviceEvents();
    }
    else
    {
        ShowDeviceSelectorAsync();
    }
}

/// <summary>
/// This is the click handler for the 'Trigger Events' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void S5_Events::Trigger_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (IsRegisteredForDeviceEvents())
    {
        CreateAndThenDeleteFolderOnDeviceAsync();
    }
}

/// <summary>
/// Enumerates all portable devices and populates the device selection list.
/// </summary>
void S5_Events::ShowDeviceSelectorAsync()
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
void S5_Events::DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    DeviceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    if (_deviceInfoCollection == nullptr)
    {
        return; // not yet populated
    }

    auto deviceInfo = _deviceInfoCollection->GetAt(DeviceList->SelectedIndex);
    RegisterForDeviceEvents(deviceInfo);
}

/// <summary>
/// Registers to receive events from a portable device.
/// </summary>
/// <param name="deviceInfoElement">Contains information about a selected device.</param>
void S5_Events::RegisterForDeviceEvents(DeviceInformation^ deviceInfoElement)
{
    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceFTM, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&_deviceEventSource)));

    auto clientInfo = GetClientInfo();
    HRESULT hr = _deviceEventSource->Open(deviceInfoElement->Id->Begin(), clientInfo.Get());
    if (hr == E_ACCESSDENIED)
    {
        rootPage->NotifyUser("Access to " + deviceInfoElement->Name + " is denied. Only a Privileged Application may access this device.", NotifyType::ErrorMessage);
        return;
    }
    ThrowIfFailed(hr);

    // Assign our delegate to the event callback.  The event callback is a COM object that implements IPortableDeviceEventCallback.
    PWSTR cookie = nullptr;
    auto callback = Make<EventCallback>(ref new EventReceived(this, &S5_Events::OnDeviceEventReceived));

    // Register the callback object with the PortableDevice COM API and receive a cookie that can be used for unregistration.
    hr = _deviceEventSource->Advise(0, callback.Get(), nullptr, &cookie);
    CoTaskMemString cookiePtr(cookie);
    ThrowIfFailed(hr);
    _deviceEventCookie = ref new String(cookie);

    rootPage->NotifyUser("Successfully registered for events (cookie '" + _deviceEventCookie + "'). Now click 'Trigger Events' to perform operations on '"
        + deviceInfoElement->Name + "' that can trigger the events.", NotifyType::StatusMessage);

    ScenarioInput->Content = "Unregister for Events";
    ScenarioEventTrigger->IsEnabled = true;
}

/// <summary>
/// Unregisters the existing portable device event registration.
/// </summary>
void S5_Events::UnregisterForDeviceEvents()
{
    if (IsRegisteredForDeviceEvents())
    {
        ThrowIfFailed(_deviceEventSource->Unadvise(_deviceEventCookie->Begin()));
        rootPage->NotifyUser("Successfully unregistered for events (cookie '" + _deviceEventCookie + "')", NotifyType::StatusMessage);

        // Cleanup
        _deviceEventCookie = nullptr;
        _deviceEventSource = nullptr;

        ScenarioInput->Content = "Register for Events";
        ScenarioEventTrigger->IsEnabled = false;
    }
}

/// <summary>
/// Event delegate that is invoked with an event is received from the device.
/// </summary>
/// <param name="deviceId">Identifies the device sending the event.</param>
/// <param name="eventData">Contains information about the event being received.</param>
void S5_Events::OnDeviceEventReceived(String^ senderDeviceId, String^ eventData)
{
    ScenarioOutput->Text += "[Device Event] " + eventData + ", Device Id: " + senderDeviceId + "\n";
}

/// <summary>
/// Creates a folder on the first storage of a portable device and then deletes it.
/// On success, this triggers the DeviceEventReceived delegate for two events:
////- 'Object Added' for the folder creation and
/// - 'Object Removed' for the subsequent delete.
/// </summary>
/// <see cref="DeviceEventReceived"/>
void S5_Events::CreateAndThenDeleteFolderOnDeviceAsync()
{
    ScenarioOutput->Text = "";
    if (_deviceEventSource != nullptr)
    {
        // The event triggerring is not required to be asynchronous, but the recommendation is to move
        // long running operations involving devices in tasks to avoid blocking UI.
        create_task([this]() -> void { 

            // For demonstration purposes, open a separate IPortableDevice connection to perform the folder operations.
            PWSTR deviceId = nullptr;
            HRESULT hr = _deviceEventSource->GetPnPDeviceID(&deviceId);
            CoTaskMemString deviceIdPtr(deviceId);
            ThrowIfFailed(hr);

            ComPtr<IPortableDevice> device;
            ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceFTM, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&device)));

            auto clientInfo = GetClientInfo();
            ThrowIfFailed(device->Open(deviceId, clientInfo.Get()));

            ComPtr<IPortableDeviceContent> content;
            ThrowIfFailed(device->Content(&content));

            // Create a folder on the storage and then delete it
            auto storageId = GetFirstStorageId(device.Get());
            if (storageId != nullptr)
            {
                ComPtr<IPortableDeviceContent> content;
                ThrowIfFailed(device->Content(&content));

                // Create a new object with properties and data
                String^ statusMessage = "Creating a folder on the device to trigger the 'Object Added' event ... \n";
                rootPage->DispatcherNotifyUser(statusMessage, NotifyType::StatusMessage);

                auto folderProperties = FillInPropertiesForFolder(storageId);
                PWSTR folderId = nullptr;
                hr = content->CreateObjectWithPropertiesOnly(folderProperties.Get(), // Properties describing the folder data
                                                             &folderId);             // Object id of the newly created folder
                CoTaskMemString folderIdPtr(folderId);
                if (hr == E_ACCESSDENIED)
                {
                    rootPage->DispatcherNotifyUser("Device does not allow folder creation in storage '" + storageId + "', no events will be triggered.", NotifyType::ErrorMessage);
                    return;
                }
                ThrowIfFailed(hr);
                String^ strFolderId = ref new String(folderId);
                statusMessage += "New folder '" + strFolderId + "' was created. Now deleting the folder to trigger the 'Object Removed' event ...\n";
                rootPage->DispatcherNotifyUser(statusMessage, NotifyType::StatusMessage);

                // Delete the folder object we just created
                ComPtr<IPortableDevicePropVariantCollection> objectsToDelete;
                ComPtr<IPortableDevicePropVariantCollection> objectsToDeleteResults;
                ThrowIfFailed(CoCreateInstance(CLSID_PortableDevicePropVariantCollection, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&objectsToDelete)));

                PROPVARIANT folderIdPropVariant = {};
                folderIdPropVariant.vt = VT_LPWSTR;
                folderIdPropVariant.pwszVal = folderId; // Assignment, don't PropVariantClear

                ThrowIfFailed(objectsToDelete->Add(&folderIdPropVariant));
                hr = content->Delete(PORTABLE_DEVICE_DELETE_NO_RECURSION, objectsToDelete.Get(), &objectsToDeleteResults);
                if (hr == S_OK)
                {
                    statusMessage += "New folder '" + strFolderId + "' was deleted.\n";
                    rootPage->DispatcherNotifyUser(statusMessage, NotifyType::StatusMessage);
                }
                else if (hr == S_FALSE)
                {
                    PROPVARIANT deleteResult = {};
                    ThrowIfFailed(objectsToDeleteResults->GetAt(0, &deleteResult));
                    rootPage->DispatcherNotifyUser("New folder '" + strFolderId + "' failed to be deleted, delete result = " + deleteResult.scode.ToString(), NotifyType::ErrorMessage);
                }
                ThrowIfFailed(hr);
            }
            else
            {
                rootPage->DispatcherNotifyUser("No storages were found on the device. Please attach a device with at least 1 storage.", NotifyType::ErrorMessage);
            }

        }).then([this] (task<void> thisTask)
        {
            // Handle errors from the task. We're catching all exceptions here because the device may return any failure HRESULT.
            try
            {
                thisTask.get();
            }
            catch (Exception^ e)
            {
                rootPage->DispatcherNotifyUser("Creating and deleting a new folder failed with error: " + e->Message, NotifyType::ErrorMessage);
            }
        });
    }
}

/// <summary>
/// Returns an IPortableDeviceValues containing properties used for creating a folder object on the device.
/// </summary>
/// <param name="parentId">The parent object identifier for the new image object.</param>
/// <returns>IPortableDeviceValues ComPtr containing the folder object property keys and values.</returns>
ComPtr<IPortableDeviceValues> S5_Events::FillInPropertiesForFolder(String^ parentId)
{
    ComPtr<IPortableDeviceValues> result;
    ThrowIfFailed(CoCreateInstance(CLSID_PortableDeviceValues, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&result)));

    ThrowIfFailed(result->SetStringValue(WPD_OBJECT_PARENT_ID, parentId->Begin()));
    ThrowIfFailed(result->SetStringValue(WPD_OBJECT_NAME, L"My new folder"));
    ThrowIfFailed(result->SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, L"New folder"));
    ThrowIfFailed(result->SetGuidValue(WPD_OBJECT_FORMAT, WPD_OBJECT_FORMAT_PROPERTIES_ONLY));
    ThrowIfFailed(result->SetGuidValue(WPD_OBJECT_CONTENT_TYPE, WPD_CONTENT_TYPE_FOLDER));

    return result;
}
