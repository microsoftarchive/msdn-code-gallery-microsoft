//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        class Utilities
        {
        public:
            static void SetUpDeviceScenarios(
                Windows::Foundation::Collections::IMap<CustomUsbDeviceAccess::DeviceType, Windows::UI::Xaml::UIElement^>^ scenarios, 
                Windows::UI::Xaml::UIElement^ scenarioContainer);

            static void NotifyDeviceNotConnected(void);

            static DeviceType GetDeviceType(Windows::Devices::Usb::UsbDevice^ device);

            static bool IsSuperMuttDevice(Windows::Devices::Usb::UsbDevice^ device);

            static Platform::String^ ConvertToHex16Bit(uint16 value);

            static Platform::String^ ConvertToHex8Bit(uint8 value);

            static Platform::String^ ConvertToHex(uint32 value);
        };
    }
}