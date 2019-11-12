//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6_InterfaceSettings.xaml.cpp
// Implementation of the Scenario6_InterfaceSettings class
//

#include "pch.h"
#include "Scenario6_InterfaceSettings.xaml.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Concurrency;
using namespace Windows::Devices::Usb;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::CustomUsbDeviceAccess;

InterfaceSettings::InterfaceSettings(void)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
///
/// We will enable/disable parts of the UI if the device doesn't support it.
///
/// The list of interfaces settings will be hardcoded to be 0 or 1 because those settings on the
/// defaultInterface of the SuperMutt are identical. Changing the setting will not effect the funcitonality
/// of other scenarios.
/// </summary>
/// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void InterfaceSettings::OnNavigatedTo(NavigationEventArgs^ /* eventArgs */)
{
    // We will disable the scenario that is not supported by the device.
    // If no devices are connected, none of the scenarios will be shown and an error will be displayed
    Map<DeviceType, UIElement^>^ deviceScenarios = ref new Map<DeviceType, UIElement^>();
    deviceScenarios->Insert(DeviceType::All, GenericScenario);

    Utilities::SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);

    // Enumerate all the interface settings of the default interface and add them to list for user to choose
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto defaultInterface = EventHandlerForDevice::Current->Device->DefaultInterface;
        for (uint8 settingNumber = 0; settingNumber < defaultInterface->InterfaceSettings->Size; settingNumber++)
        {
            InterfaceSettingsToChoose->Items->Append(settingNumber.ToString());
        }

        // Default select the first interface setting because it's always going to be available
        InterfaceSettingsToChoose->SelectedIndex = 0;

        // Only allow setting of the interface settings for the SuperMutt device because it will not break the rest of the scenarios
        if (!Utilities::IsSuperMuttDevice(EventHandlerForDevice::Current->Device))
        {
            ButtonSetSetting->IsEnabled = false;
        }
    }
}

/// <summary>
/// Determines which item is clicked and what the interfaceSettingNumber that item corresponds to.
/// Will not allow changing of the interface setting if not a SuperMutt. If it is a SuperMutt,
/// only the first two interface settings can be used because the first two interface settings
/// are identical with respect to the API.
/// </summary>
void InterfaceSettings::SetSuperMuttInterfaceSetting_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        // Since we added the the settings in order, the index of the item clicked will give us the index of the
        // setting in UsbInterface->InterfaceSettings
        SetInterfaceSetting(InterfaceSettingsToChoose->SelectedIndex);
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void InterfaceSettings::GetInterfaceSetting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        GetInterfaceSetting();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

/// <summary>
/// Selects the interface setting on the default interface.
/// 
/// The interfaceSetting is 0 based, where setting 0 is the default interfaceSetting.
/// </summary>
/// <param name="settingNumber"></param>
void InterfaceSettings::SetInterfaceSetting(uint8 settingNumber)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto interfaceSetting = EventHandlerForDevice::Current->Device->DefaultInterface->InterfaceSettings->GetAt(settingNumber);

        create_task(interfaceSetting->SelectSettingAsync())
            .then([settingNumber](task<void> selectTask)
            {
                // May throw an exception if selection failed
                selectTask.get();   

                MainPage::Current->NotifyUser("Interface Setting is set to " + settingNumber, NotifyType::StatusMessage);
            });
    }
}

/// <summary>
/// Figures out which interface setting is currently selected by checking each interface setting even if the
/// setting cannot be selected by this sample app.
/// It will print out the selected interface setting number.
/// <summary>
void InterfaceSettings::GetInterfaceSetting(void)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto interfaceSettings = EventHandlerForDevice::Current->Device->DefaultInterface->InterfaceSettings;

        for each(UsbInterfaceSetting^ interfaceSetting in interfaceSettings)
        {
            if (interfaceSetting->Selected)
            {
                uint8 interfaceSettingNumber = interfaceSetting->InterfaceDescriptor->AlternateSettingNumber;

                MainPage::Current->NotifyUser("Current interface setting : " + interfaceSettingNumber.ToString(), NotifyType::StatusMessage);

                break;
            }
        }
    }
}