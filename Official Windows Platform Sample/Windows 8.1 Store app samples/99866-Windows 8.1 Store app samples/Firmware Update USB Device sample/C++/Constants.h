//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    internal:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return "Firmware Update Usb Device"; 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }
    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    namespace FirmwareUpdateUsbDevice
    {
        namespace SuperMutt
        {
            namespace Device
            {
                const uint16 Vid = 0x045E;
                const uint16 Pid = 0x0611;
            }
        }

        namespace FirmwareUpdateTaskInformation
        {
            static Platform::String^ Name = "FirmwareUpdateBackgroundTask";
            static Platform::String^ TaskEntryPoint = "BackgroundTask.UpdateFirmwareTask";
            static Platform::String^ TaskCanceled = "Canceled";
            static Platform::String^ TaskCompleted = "Completed";
            
            // Wait for 2 minutes
            // Convert minutes into units of 100 nanoseconds
            // (2 minutes * 60 seconds/minute * 1000 milliseconds/second * 1000 nanoseconds/millisecond) / 100 nanosecond resolution
            static const Windows::Foundation::TimeSpan ApproximateFirmwareUpdateTime = { 2 * 60 * 1000 * 10 };
        }

        namespace LocalSettingKeys
        {
            namespace FirmwareUpdateBackgroundTask
            {
                static Platform::String^ TaskStatus = "FirmwareUpdateBackgroundTaskStatus";
                static Platform::String^ NewFirmwareVersion = "FirmwareUpdateBackgroundTaskNewFirmwareVersion";
            }
        }
    }

    // All possible hex digits where the index is the decimal value of the char
    static const Platform::String^ HexDigits = "0123456789ABCDEF";

    class Utilities
    {
    public:
        /// <summary>
        /// Returns a hex string that represents without the 0x prefix
        /// </summary>
        /// <param name="value">Value to convert to hex</param>
        /// <returns>Hex representation of number</returns>
        static Platform::String^ ConvertToHex(uint32 value)
        {
            Platform::String^ hexString = "";

            // Convert number to hex
            for (uint32 quotient = value; quotient > 0; quotient = quotient / 16)
            {
                uint8 digit = quotient % 16;
        
                // Converts digit to a hex character
                hexString = (*(const_cast<Platform::String^>(HexDigits)->Begin() + digit)).ToString() + hexString;
            }

            if (value == 0)
            {
                hexString = "0";
            }

            return hexString;
        };

        /// <summary>
        /// Returns a hex string that represents a 16 value
        /// </summary>
        /// <param name="value">Value to convert to hex</param>
        /// <returns>Hex representation of 16 bits</returns>
        static Platform::String^ ConvertToHex16Bit(uint16 value)
        {
            Platform::String^ hexString = ConvertToHex(value);

            // Pad with 0s if there arn't enough digits inserted
            // 2 hex digits represent 1 byte
            while (hexString->Length() < (sizeof(value) * 2))   
            {
                hexString = "0" + hexString;
            }

            return "0x" + hexString;
        };
    };
}
