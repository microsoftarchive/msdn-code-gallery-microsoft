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
                return "Usb Cdc Control";
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

    namespace UsbCdcControl
    {
        value struct DeviceHardwareId
        {
            uint32 Vid;
            uint32 Pid;
        };

        namespace Constants
        {
            static const int InfiniteTimeout = -1;
            static const unsigned int DteRateTestValue = 4800;
            static const byte DataBitsTestValue = 8;
            static const unsigned int ExpectedResultSetLineCoding = 7;
            static const unsigned int ExpectedResultGetLineCoding = 7;
        }

        namespace InterfaceClass
        {
            static const byte CdcControl = 2;
            static const byte CdcData = 10;
            static const byte VendorSpecific = 255;
        }

        namespace RequestType
        {
            static const byte Set = 0x21;
            static const byte Get = 0xA1;
        }

        namespace RequestCode
        {
            static const byte SetLineCoding = 0x20;
            static const byte GetLineCoding = 0x21;
            static const byte SetControlLineState = 0x22;
            static const byte SendBreak = 0x23;
        }

        public enum class Parity
        {
            None = 0,
            Odd,
            Even,
            Mark,
            Space
        };

        public enum class StopBits
        {
            None = -1,
            One = 0,
            OnePointFive = 1,
            Two = 2
        };

        class SampleDevices
        {
        public:
            static Platform::Array<DeviceHardwareId>^ SupportedDevices;
        };
    }
}