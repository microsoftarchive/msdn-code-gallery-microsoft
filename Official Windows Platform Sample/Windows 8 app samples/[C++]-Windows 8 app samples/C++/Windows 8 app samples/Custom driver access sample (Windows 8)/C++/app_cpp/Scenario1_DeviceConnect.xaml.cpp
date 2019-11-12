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
// DeviceConnect.xaml.cpp
// Implementation of the DeviceConnect class
//

#include "pch.h"
#include "Scenario1_DeviceConnect.xaml.h"

#include <assert.h>

using namespace SDKSample::CustomDeviceAccess;

using namespace Platform;
using namespace Platform::Collections;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace SDKSample::CustomDeviceAccess;

DeviceConnect::DeviceConnect()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceConnect::OnNavigatedTo(NavigationEventArgs^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;

    listSource->Source = DeviceList::Current->Fx2Devices;
    UpdateStartStopButtons();
}


void DeviceConnect::deviceConnectStart_Click_1(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    DeviceList::Current->StartFx2Watcher();
    UpdateStartStopButtons();
}


void DeviceConnect::deviceConnectStop_Click_1(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    DeviceList::Current->StopFx2Watcher();
    UpdateStartStopButtons();
}

void DeviceConnect::deviceConnectDevices_SelectionChanged_1(Object^ /* sender */, SelectionChangedEventArgs^ /* e */) 
{
    auto selection = deviceConnectDevices->SelectedItems;
    DeviceListEntry^ entry = nullptr;

    if (selection->Size > 0) {
        auto o = selection->GetAt(0);
        entry = safe_cast<DeviceListEntry^>(o);
    }

    auto currentlySelectedId = (DeviceList::Current->Fx2Device != nullptr ? DeviceList::Current->Fx2Device->Id : nullptr);
    auto newlySelectedId = entry ? entry->Id : nullptr;

    if (currentlySelectedId != nullptr) 
    {
        CloseFx2Device();
    }

    if (newlySelectedId != nullptr) {
        OpenFx2Device(newlySelectedId);
    }
}

void DeviceConnect::UpdateStartStopButtons(void)
{
    this->deviceConnectStart->IsEnabled = !DeviceList::Current->WatcherStarted;
    this->deviceConnectStop->IsEnabled = DeviceList::Current->WatcherStarted;
}

void DeviceConnect::OpenFx2Device(String^ Id)
{
    assert(DeviceList::Current->Fx2Device == nullptr);

    try 
    {
        DeviceList::Current->Fx2Device = Samples::Devices::Fx2::Fx2Device::FromId(Id);
    }
    catch (Exception^ ex) {
        rootPage->NotifyUser("Error opening Fx2 device @" + Id + ": " + ex->Message, NotifyType::ErrorMessage);
        return;
    }
    rootPage->NotifyUser("Fx2 " + Id + " opened", NotifyType::StatusMessage);
}

void DeviceConnect::CloseFx2Device()
{
    if (DeviceList::Current->Fx2Device != nullptr) {
        rootPage->NotifyUser("Fx2 " + DeviceList::Current->Fx2Device->Id + " closed", NotifyType::StatusMessage);
        DeviceList::Current->Fx2Device = nullptr;
    }
}
