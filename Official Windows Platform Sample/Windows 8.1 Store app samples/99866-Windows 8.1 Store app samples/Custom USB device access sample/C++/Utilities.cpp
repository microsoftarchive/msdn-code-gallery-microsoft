//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "Utilities.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace SDKSample;
using namespace SDKSample::CustomUsbDeviceAccess;

/// <summary>
/// Displays the compatible scenarios and hides the non-compatible ones.
/// If there are no supported devices, the scenarioContainer will be hidden and an error message
/// will be displayed.
/// </summary>
/// <param name="scenarios">The key is the device type that the value, scenario, supports.</param>
/// <param name="scenarioContainer">The container that encompasses all the scenarios that are specific to devices</param>
void Utilities::SetUpDeviceScenarios(IMap<CustomUsbDeviceAccess::DeviceType, UIElement^>^ scenarios, UIElement^ scenarioContainer)
{
    UIElement^ supportedScenario = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        for each (IKeyValuePair<DeviceType, UIElement^>^ deviceScenario in scenarios)
        {
            // Enable the scenario if it's generic or the device type matches
            if ((deviceScenario->Key == DeviceType::All)
                || (deviceScenario->Key == Utilities::GetDeviceType(EventHandlerForDevice::Current->Device)))
            {
                // Make the scenario visible in case other devices use the same scenario and collapsed it.
                deviceScenario->Value->Visibility = Visibility::Visible;

                supportedScenario = deviceScenario->Value;
            }
            else if (deviceScenario->Value != supportedScenario)    
            {
                // Don't hide the scenario if it is supported by the current device and is shared by other devices
                deviceScenario->Value->Visibility = Visibility::Collapsed;
            }
        }
    }

    if (supportedScenario == nullptr)
    {
        // Hide the container so that common elements shared across scenarios are also hidden
        scenarioContainer->Visibility = Visibility::Collapsed;

        NotifyDeviceNotConnected();
    }
}

/// <summary>
/// Will check if a device is currently connected (can be used). If there is no such device, this method will
/// notify the user with an error message.
/// </summary>
/// <returns>True if a notification was sent to the UI (No device is connected); False otherwise</returns>
void Utilities::NotifyDeviceNotConnected(void)
{
    MainPage::Current->NotifyUser("Device is not connected, please select a plugged in device to try the scenario again", NotifyType::ErrorMessage);
}

/// <summary>
/// Attempts to match the provided device with a well known device and returns the well known type
/// </summary>
/// <param name="device"></param>
/// <returns>Device type of the current connected device or DeviceType.None if there are no devices connected or is not recognized</returns>
DeviceType Utilities::GetDeviceType(Windows::Devices::Usb::UsbDevice^ device)
{
    if (device != nullptr)
    {
        if (device->DeviceDescriptor->VendorId == OsrFx2::DeviceVid
            && device->DeviceDescriptor->ProductId == OsrFx2::DevicePid)
        {
            return DeviceType::OsrFx2;
        }
        else if (device->DeviceDescriptor->VendorId == SuperMutt::DeviceVid
            && device->DeviceDescriptor->ProductId == SuperMutt::DevicePid)
        {
            return DeviceType::SuperMutt;
        }
    }
   
    return DeviceType::None;
}

bool Utilities::IsSuperMuttDevice(Windows::Devices::Usb::UsbDevice^ device)
{
    return (GetDeviceType(device) == DeviceType::SuperMutt);
}

/// <summary>
/// Returns a hex string that represents a 16 value
/// </summary>
/// <param name="value">Value to convert to hex</param>
/// <returns>Hex representation of 16 bits</returns>
String^ Utilities::ConvertToHex16Bit(uint16 value)
{
    String^ hexString = ConvertToHex(value);

    // Pad with 0s if there arn't enough digits inserted
    // 2 hex digits represent 1 byte
    while (hexString->Length() < (sizeof(value) * 2))   
    {
        hexString = "0" + hexString;
    }

    return "0x" + hexString;
}

/// <summary>
/// Returns a hex string that represents an 8 bit value
/// </summary>
/// <param name="value">Value to convert to hex</param>
/// <returns>Hex representation of 8 bits</returns>
String^ Utilities::ConvertToHex8Bit(uint8 value)
{
    String^ hexString = ConvertToHex(value);

    // Pad with 0s if there arn't enough digits inserted
    // 2 hex digits represent 1 byte
    while (hexString->Length() < (sizeof(value) * 2))   
    {
        hexString = "0" + hexString;
    }

    return "0x" + hexString;
}

/// <summary>
/// Returns a hex string that represents without the 0x prefix
/// </summary>
/// <param name="value">Value to convert to hex</param>
/// <returns>Hex representation of number</returns>
String^ Utilities::ConvertToHex(uint32 value)
{
        String^ hexString = "";

    // Convert number to hex
    for (uint32 quotient = value; quotient > 0; quotient = quotient / 16)
    {
        uint8 digit = quotient % 16;
        
        // Converts digit to a hex character
        hexString = (*(const_cast<String^>(HexDigits)->Begin() + digit)).ToString() + hexString;
    }

    if (value == 0)
    {
        hexString = "0";
    }

    return hexString;
}
