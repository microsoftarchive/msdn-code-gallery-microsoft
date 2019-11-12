//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "DeviceList.h"

using namespace SDKSample;
using namespace SDKSample::UsbCdcControl;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::ApplicationModel;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::Devices::Usb;

Platform::Collections::Vector<DeviceList^>^ DeviceList::instances = nullptr;

Windows::Foundation::Collections::IVectorView<DeviceList^>^ DeviceList::Instances::get()
{
    if (DeviceList::instances == nullptr) {
        DeviceList::instances = ref new Platform::Collections::Vector<DeviceList^>();
    }
    return DeviceList::instances->GetView();
}

DeviceList::DeviceList(Platform::String^ deviceSelector)
    : watcherStarted(false)
    , watcherSuspended(false)
    , deviceSelector(deviceSelector)
{
    this->watcher = nullptr;
    this->list = ref new Vector<DeviceListEntry^>();
    InitDeviceWatcher();

    // Register for app suspend/resume handlers.
    // Ideally, one should close the device on app suspend and reopen the device on app resume because the API will close the device
    // for you if you don't explicitly close the device. Please see CustomUsbDeviceAccess sample for an example of how that should 
    // be done.
    App::Current->Suspending += ref new SuspendingEventHandler(this, &DeviceList::SuspendDeviceWatcher);
    App::Current->Resuming += ref new EventHandler<Object^>(this, &DeviceList::ResumeDeviceWatcher);

    // Add this to the static list.
    this->instances->Append(this);
}    

void DeviceList::InitDeviceWatcher() 
{
    // Create a device watcher to look for instances of the  device interface
    this->watcher = DeviceInformation::CreateWatcher(const_cast<Platform::String^>(this->deviceSelector));

    this->watcher->Added += ref new TypedEventHandler<DeviceWatcher^, DeviceInformation^>(this, &DeviceList::OnAdded);
    this->watcher->Removed += ref new TypedEventHandler<DeviceWatcher^, DeviceInformationUpdate^>(this, &DeviceList::OnRemoved);
    this->watcher->EnumerationCompleted += ref new TypedEventHandler<DeviceWatcher^,Object^>(this, &DeviceList::OnEnumerationComplete);
}

void DeviceList::StartWatcher() {
    std::for_each(
        begin(this->list), 
        end(this->list), 
        [](DeviceListEntry^ Entry) {
            Entry->Matched = false;
        }
    );
    WatcherStarted = true;
    this->watcher->Start();
}

void DeviceList::StopWatcher() {
    this->watcher->Stop();
    WatcherStarted = false;
}

DeviceListEntry^ DeviceList::FindDevice(String^ Id)
{
    auto i = std::find_if(begin(this->list), 
                          end(this->list), 
                          [Id](DeviceListEntry^ e) {return e->Id == Id;});
    if (i == end(this->list)) {
        return nullptr;
    } else {
        return *i;
    }
}
        
void DeviceList::OnAdded(DeviceWatcher^ /* Sender */, DeviceInformation^ devInterface)
{
    // search the device list for a device with a matching interface ID
    auto match = FindDevice(devInterface->Id);

    // If we found a match then mark it as verified and return
    if (match != nullptr)
    {
        if (match->Matched == false)
        {
            // Notify
            DeviceAdded(this, ref new UsbDeviceInfo(match));
        }
        match->Matched = true;
        return;
    }

    // Create a new elemetn for this device interface, and queue up the query of its
    // device information
    match = ref new DeviceListEntry(devInterface);

    // Add the new element to the end of the list of devices
    this->list->Append(match);

    // Notify
    DeviceAdded(this, ref new UsbDeviceInfo(match));
}

void DeviceList::OnRemoved(DeviceWatcher^ /* Sender */, DeviceInformationUpdate^ DevInterface)
{
    auto deviceId = DevInterface->Id;

    // Search the list of devices for one with a matching ID.
    auto i = std::find_if(begin(this->list), 
                            end(this->list),
                            [deviceId](DeviceListEntry^ e) {return e->Id == deviceId;});

    // if there's no match return.
    if (i == end(this->list)) {
        return;
    }

    unsigned int index;
    this->list->IndexOf(*i, &index);

    // Notify
    DeviceRemoved(this, ref new UsbDeviceInfo(this->list->GetAt(index)));

    // Remove the item from the list.
    this->list->RemoveAt(index);
}

void DeviceList::OnEnumerationComplete(DeviceWatcher^ /* Sender */, Object^ /* o */) 
{
    // Move all the unmatched elements to the end of the list
    auto i = std::remove_if(begin(this->list), 
                            end(this->list),
                            [](DeviceListEntry^ e) { return e->Matched == false;});

    // Determine the number of unmatched entries
    auto unmatchedCount = end(this->list) - i;
                
    while (unmatchedCount > 0) {
        this->list->RemoveAtEnd();
        unmatchedCount -= 1;
    }
}

void DeviceList::SuspendDeviceWatcher(Object^, SuspendingEventArgs^)
{
    if (WatcherStarted) {
        this->watcherSuspended = true;
        StopWatcher();
    } else {
        this->watcherSuspended = false;
    }
}

void DeviceList::ResumeDeviceWatcher(Object^,Object^) 
{
    if (this->watcherSuspended) {
        this->watcherSuspended = false;
        StartWatcher();
    }

    // Notify the user that they have to go back to scenario 1 to reconnect to the device.
    SDKSample::MainPage::Current->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([]()
    {
		SDKSample::MainPage::Current->NotifyUser("If device was connected on app suspension, then it was closed. Please go to scenario 1 to reconnect to the device.", SDKSample::NotifyType::StatusMessage);
	}));
}
