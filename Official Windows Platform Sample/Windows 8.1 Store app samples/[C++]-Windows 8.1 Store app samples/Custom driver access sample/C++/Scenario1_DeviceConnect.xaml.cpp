//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1_DeviceConnect.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::CustomDeviceAccess;

using namespace Platform;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Custom;

using namespace Concurrency;

DeviceConnect::DeviceConnect()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceConnect::OnNavigatedTo(NavigationEventArgs ^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;

    listSource->Source = DeviceList::Current->Fx2Devices;
    UpdateStartStopButtons();
}

void DeviceConnect::deviceConnectStart_Click_1(Object ^ /* sender */, RoutedEventArgs ^ /* e */)
{
    DeviceList::Current->StartFx2Watcher();
    UpdateStartStopButtons();
}


void DeviceConnect::deviceConnectStop_Click_1(Object ^ /* sender */, RoutedEventArgs ^ /* e */)
{
    DeviceList::Current->StopFx2Watcher();
    UpdateStartStopButtons();
}

void DeviceConnect::deviceConnectDevices_SelectionChanged_1(Object ^ /* sender */, SelectionChangedEventArgs ^ /* e */)
{
    auto selection = deviceConnectDevices->SelectedItems;
    DeviceListEntry^ entry = nullptr;

    if (selection->Size > 0) {
        auto o = selection->GetAt(0);
        entry = safe_cast<DeviceListEntry^>(o) ;
    }

    auto currentlySelectedId = DeviceList::Current->GetSelectedDeviceId();
    auto newlySelectedId = entry ? entry->Id : nullptr;

    if (currentlySelectedId != nullptr) 
    {
        CloseFx2Device();
    }

    if (newlySelectedId != nullptr) {
        OpenFx2DeviceAsync(newlySelectedId);
    }
}

void DeviceConnect::UpdateStartStopButtons(void)
{
    this->deviceConnectStart->IsEnabled = !DeviceList::Current->WatcherStarted;
    this->deviceConnectStop->IsEnabled = DeviceList::Current->WatcherStarted;
}

task<void> DeviceConnect::OpenFx2DeviceAsync(String^ Id)
{
    return
    create_task(CustomDevice::FromIdAsync(Id, DeviceAccessMode::ReadWrite, DeviceSharingMode::Exclusive)).
    then(
        [=](task<CustomDevice^> result)
        {
            try 
            {
                auto device = result.get();

                DeviceList::Current->SetSelectedDevice(Id, device);

                rootPage->NotifyUser("Fx2 " + Id + " opened", NotifyType::StatusMessage);
            }
            catch (Exception^ ex)
            {
                rootPage->NotifyUser("Error opening Fx2 device @" + Id + ": " + ex->Message, NotifyType::ErrorMessage);
            }
        }
    );
}

void DeviceConnect::CloseFx2Device()
{
    if (DeviceList::Current->GetSelectedDevice() != nullptr) 
    {
        rootPage->NotifyUser("Fx2 " + DeviceList::Current->GetSelectedDeviceId() + " closed", NotifyType::StatusMessage);
        DeviceList::Current->ClearSelectedDevice();
    }
}
