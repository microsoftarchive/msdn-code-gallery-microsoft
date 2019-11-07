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
// DeviceEvents.xaml.cpp
// Implementation of the DeviceEvents class
//

#include "pch.h"
#include "Scenario3_DeviceEvents.xaml.h"

using namespace SDKSample::CustomDeviceAccess;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

DeviceEvents::DeviceEvents() : switchChangedEventsRegistered(false)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceEvents::OnNavigatedTo(NavigationEventArgs^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
    ClearSwitchStateTable();
    UpdateRegisterButton();
    deviceClosingHandler = DeviceList::Current->DeviceClosing::add(ref new DeviceClosingHandler(this, &DeviceEvents::Current_DeviceClosing));
}

void DeviceEvents::OnNavigatedFrom(NavigationEventArgs^ /* e */) 
{
    if (switchChangedEventsRegistered == true)
    {
        RegisterForSwitchStateChangedEvent(false);
    }
    DeviceList::Current->DeviceClosing::remove(deviceClosingHandler);
}

void DeviceEvents::deviceEventsGet_Click_1(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (DeviceList::Current->Fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    Platform::Array<bool>^ switchStateArray;

    try 
    {
        switchStateArray = DeviceList::Current->Fx2Device->SwitchState;
    }
    catch (Platform::Exception^ exception)
    {
        rootPage->NotifyUser(exception->ToString(), NotifyType::ErrorMessage);
        return;
    }

    UpdateSwitchStateTable(switchStateArray);
}


void DeviceEvents::deviceEventsRegister_Click_1(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (DeviceList::Current->Fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    RegisterForSwitchStateChangedEvent(switchChangedEventsRegistered == false);
}

void DeviceEvents::Current_DeviceClosing(Platform::Object^ /* sender */, Samples::Devices::Fx2::Fx2Device^ /* device */) 
{
    if (switchChangedEventsRegistered) {
        RegisterForSwitchStateChangedEvent(false);
    }
}

void DeviceEvents::RegisterForSwitchStateChangedEvent(bool r)
{
    if (r) 
    {
        switchChangedEventsHandler = 
            DeviceList::Current->Fx2Device->SwitchStateChanged::add(
                ref new Samples::Devices::Fx2::SwitchStateChangedHandler(
                    this, 
                    &DeviceEvents::OnSwitchStateChangedEvent
                    )
                );

    } else {
        DeviceList::Current->Fx2Device->SwitchStateChanged::remove(switchChangedEventsHandler);
    }
    switchChangedEventsRegistered = r;
    UpdateRegisterButton();
    ClearSwitchStateTable();
}

void DeviceEvents::UpdateRegisterButton(void)
{
    if (switchChangedEventsRegistered)
    {
        deviceEventsRegister->Content = "Unregister from Switch State Change Event";
    }
    else 
    {
        deviceEventsRegister->Content = "Register for Switch State Change Event";
    }
}

void DeviceEvents::OnSwitchStateChangedEvent(Samples::Devices::Fx2::Fx2Device^ /* sender */, Samples::Devices::Fx2::SwitchStateChangedEventArgs^ e) 
{
    Platform::Array<bool>^ switchState;

    try 
    {
        switchState = e->SwitchState;
    }
    catch (Platform::Exception^ x) 
    {
        rootPage->NotifyUser(
            ref new Platform::String(L"Error accessing Fx2 device:\n") + x->Message,
            NotifyType::ErrorMessage
            );
        ClearSwitchStateTable();
        return;
    }

    UpdateSwitchStateTable(e->SwitchState);
}

void DeviceEvents::ClearSwitchStateTable()
{
    deviceEventsSwitches->Inlines->Clear();
    previousSwitchValues = nullptr;
}

void DeviceEvents::UpdateSwitchStateTable(Platform::Array<bool>^ switchStateArray)
{
    auto output = deviceEventsSwitches;

    DeviceList::CreateBooleanTable(
        output->Inlines,
        switchStateArray,
        previousSwitchValues,
        "Switch Number",
        "Switch State",
        "off",
        "on"
        );
    previousSwitchValues = switchStateArray;
}
