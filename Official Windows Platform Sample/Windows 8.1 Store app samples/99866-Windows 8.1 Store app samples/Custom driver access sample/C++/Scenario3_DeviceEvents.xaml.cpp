//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3_DeviceEvents.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::CustomDeviceAccess;

using namespace Platform;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Custom;
using namespace Windows::Storage::Streams;

using namespace Concurrency;

DeviceEvents::DeviceEvents() : running(false)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceEvents::OnNavigatedTo(NavigationEventArgs ^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
    ClearSwitchStateTable();
    UpdateRegisterButton();
}

void DeviceEvents::OnNavigatedFrom(NavigationEventArgs ^ /* e */)
{
    if (running == true)
    {
        cancelSource.cancel();
        running = false;
    }
}

void DeviceEvents::deviceEventsGet_Click_1(Platform::Object ^ sender, Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();

    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    auto button = safe_cast<Button^>(sender);
    button->IsEnabled = false;

    auto outputBuffer = ref new Buffer(1);

    create_task(
        fx2Device->SendIOControlAsync(Fx2Driver::ReadSwitches,
                                      nullptr,
                                      outputBuffer)
        ).
    then(
        [=](task<unsigned int> result)
        {
            try
            {
                auto bytesRead = result.get();

                if (bytesRead == 0)
                {
                    rootPage->NotifyUser("Fx2 device returned 0 byte interrupt message.  Stopping",
                                         NotifyType::ErrorMessage);
                }
                else
                {
                    auto data = GetArrayFromBuffer(outputBuffer);
                    auto switchStateArray = CreateSwitchStateArray(data);
                    UpdateSwitchStateTable(switchStateArray);
                }
            }
            catch (Exception^ exception)
            {
                rootPage->NotifyUser(exception->ToString(), NotifyType::ErrorMessage);
            }

            button->IsEnabled = true;
        }
        );
}

void DeviceEvents::deviceEventsBegin_Click_1(Platform::Object ^ /* sender */, Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();

    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    if (running == false)
    {
        StartInterruptMessageWorker(fx2Device);
    }

    UpdateRegisterButton();
}

void DeviceEvents::deviceEventsCancel_Click_1(Platform::Object ^ /* sender */, Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();

    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    if (running == true)
    {
        cancelSource.cancel();
    }

    UpdateRegisterButton();
}

void DeviceEvents::StartInterruptMessageWorker(CustomDevice^ fx2Device)
{
    auto switchMessageBuffer = ref new Buffer(1);

    cancelSource = cancellation_token_source();
    running = true;
    
    InterruptMessageWorker(fx2Device, switchMessageBuffer);
}

void DeviceEvents::InterruptMessageWorker(CustomDevice^ fx2Device, IBuffer^ switchMessageBuffer)
{
    create_task(
        fx2Device->SendIOControlAsync(
                    Fx2Driver::GetInterruptMessage,
                    nullptr,
                    switchMessageBuffer
                    ),
        cancelSource.get_token()
        ).
    then(
        [=](task<unsigned int> result) 
        {
            bool failure;

            try 
            {
                auto bytesRead = result.get();

                if (bytesRead == 0)
                {
                    rootPage->NotifyUser("Fx2 device returned 0 byte interrupt message.  Stopping",
                                         NotifyType::ErrorMessage);
                    failure = true;
                }
                else
                {
                    auto switchState = CreateSwitchStateArray(GetArrayFromBuffer(switchMessageBuffer));
                    UpdateSwitchStateTable(switchState);
                    failure = false;
                }

            }
            catch (task_canceled)
            {
                rootPage->NotifyUser("Pending GetInterruptMessage IO Control cancelled", NotifyType::StatusMessage);
                failure = true;
            }
            catch (Exception^ exception)
            {
                rootPage->NotifyUser("Error accessing Fx2 device:\n" + exception->Message, NotifyType::ErrorMessage);
                failure = true;
            }

            if (failure) 
            {
                ClearSwitchStateTable();
                running = false;
                UpdateRegisterButton();
            }
            else 
            {
                InterruptMessageWorker(fx2Device, switchMessageBuffer);
            }
        }
        );
}

void DeviceEvents::UpdateRegisterButton(void)
{
    deviceEventsBegin->IsEnabled = (running == false);
    deviceEventsCancel->IsEnabled = (running == true);
}


void DeviceEvents::ClearSwitchStateTable()
{
    deviceEventsSwitches->Inlines->Clear();
    previousSwitchValues = nullptr;
}

void DeviceEvents::UpdateSwitchStateTable(Array<bool>^ switchStateArray)
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
