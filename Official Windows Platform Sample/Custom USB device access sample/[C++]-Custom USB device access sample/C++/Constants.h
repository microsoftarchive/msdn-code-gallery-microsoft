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
                return "Custom USB Device Access"; 
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

    namespace CustomUsbDeviceAccess
    {
        // All possible hex digits where the index is the decimal value of the char
        static const Platform::String^ HexDigits = "0123456789ABCDEF";

        public enum class DeviceType
        {
            OsrFx2,
            SuperMutt,
            All,    // Can be any device
            None
        };

        public enum class Descriptor
        {
            Device,
            Configuration,
            Interface,
            Endpoint,
            String,
            Custom
        };

        namespace DeviceProperties
        {
            static const Platform::String^ const DeviceInstanceId = "System.Devices.DeviceInstanceId";
        }
        
        namespace LocalSettingKeys
        {
            static Platform::String^ SyncBackgroundTaskStatus = "SyncBackgroundTaskStatus";
            static Platform::String^ SyncBackgroundTaskResult = "SyncBackgroundTaskResult";
        }

        namespace SyncBackgroundTaskInformation
        {
            static Platform::String^ Name = "SyncBackgroundTask";
            static Platform::String^ TaskEntryPoint = "BackgroundTasks.IoSyncBackgroundTask";
            static Platform::String^ TaskCanceled = "Canceled";
            static Platform::String^ TaskCompleted = "Completed";
        }

        namespace OsrFx2
        {
            namespace VendorCommand
            {
                static const uint8 GetSevenSegment = 0xD4;
                static const uint8 GetSwitchState = 0xD6;
                static const uint8 SetSevenSegment = 0xDB;
            }

            namespace Pipe
            {
                static const uint32 InterruptInPipeIndex = 0;
                static const uint32 BulkInPipeIndex = 0;
                static const uint32 BulkOutPipeIndex = 0;
            }

            // Seven Segment Masks and their associated numeric values
            static const uint8 SevenLedSegmentMask[] = 
                {
                    0xD7, // 0
                    0x06, // 1
                    0xB3, // 2
                    0xA7, // 3
                    0x66, // 4
                    0xE5, // 5
                    0xF4, // 6
                    0x07, // 7
                    0xF7, // 8
                    0x67, // 9
                };

            static const uint16 DeviceVid = 0x0547;
            static const uint16 DevicePid = 0x1002;
        }

        namespace SuperMutt
        {
            namespace VendorCommand
            {
                static const uint8 GetLedBlinkPattern = 0x03;
                static const uint8 SetLedBlinkPattern = 0x03;
            }

            namespace Pipe
            {
                static const uint32 InterruptInPipeIndex = 0;
                static const uint32 InterruptOutPipeIndex = 0;
                static const uint32 BulkInPipeIndex = 0;
                static const uint32 BulkOutPipeIndex = 0;
            }

            static const uint16 DeviceVid = 0x045E;
            static const uint16 DevicePid = 0x0611;

            // SuperMutt's Interface class {875D47FC-D331-4663-B339-624001A2DC5E}
            static const Platform::Guid DeviceInterfaceClass = Platform::Guid(0x875d47fc, 0xd331, 0x4663, 0xb3, 0x39, 0x62, 0x40, 0x01, 0xa2, 0xdc, 0x5e);
        }
    }
}
