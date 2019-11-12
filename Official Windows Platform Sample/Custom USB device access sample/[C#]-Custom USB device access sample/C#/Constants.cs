//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using CustomUsbDeviceAccess;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Custom USB Device Access";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Connecting To Device", ClassType = typeof(DeviceConnect) },
            new Scenario() { Title = "Control Transfer", ClassType = typeof(ControlTransfer) },
            new Scenario() { Title = "Interrupt Pipes", ClassType = typeof(InterruptPipes) },
            new Scenario() { Title = "Bulk Pipes", ClassType = typeof(BulkPipes) },
            new Scenario() { Title = "Usb Descriptors", ClassType = typeof(UsbDescriptors) },
            new Scenario() { Title = "Interface Settings", ClassType = typeof(InterfaceSettings) },
            new Scenario() { Title = "Sync with Device", ClassType = typeof(SyncDevice) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}

namespace CustomUsbDeviceAccess
{
    public enum DeviceType
    {
        OsrFx2,
        SuperMutt,
        All,    // Can be any device
        None
    };

    public enum Descriptor
    {
        Device,
        Configuration,
        Interface,
        Endpoint,
        String,
        Custom
    };

    public class LocalSettingKeys
    {
        public const String SyncBackgroundTaskStatus = "SyncBackgroundTaskStatus";
        public const String SyncBackgroundTaskResult = "SyncBackgroundTaskResult";
    }

    public class SyncBackgroundTaskInformation
    {
        public const String Name = "SyncBackgroundTask";
        public const String TaskEntryPoint = "BackgroundTasks.IoSyncBackgroundTask";
        public const String TaskCanceled = "Canceled";
        public const String TaskCompleted = "Completed";
    }

    public class DeviceProperties
    {
        public const String DeviceInstanceId = "System.Devices.DeviceInstanceId";
    }

    public class OsrFx2
    {
        public static class VendorCommand
        {
            public const Byte GetSevenSegment = 0xD4;
            public const Byte GetSwitchState = 0xD6;
            public const Byte SetSevenSegment = 0xDB;
        }

        public class Pipe
        {
            public const UInt32 InterruptInPipeIndex = 0;
            public const UInt32 BulkInPipeIndex = 0;
            public const UInt32 BulkOutPipeIndex = 0;
        }


        // Seven Segment Masks and their associated numeric values
        public static Byte[] SevenLedSegmentMask =
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

        public const UInt16 DeviceVid = 0x0547;
        public const UInt16 DevicePid = 0x1002;
    }

    public class SuperMutt
    {
        public class VendorCommand
        {
            public const Byte GetLedBlinkPattern = 0x03;
            public const Byte SetLedBlinkPattern = 0x03;
        }

        public class Pipe
        {
            public const UInt32 InterruptInPipeIndex = 0;
            public const UInt32 InterruptOutPipeIndex = 0;
            public const UInt32 BulkInPipeIndex = 0;
            public const UInt32 BulkOutPipeIndex = 0;
        }

        public const UInt16 DeviceVid = 0x045E;
        public const UInt16 DevicePid = 0x0611;
        public static Guid DeviceInterfaceClass = new Guid("{875d47fc-d331-4663-b339-624001a2dc5e}");
    }
}
