//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using Windows.Devices.Usb;
using FirmwareUpdateUsbDevice;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Firmware Update Usb Device";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Update Firmware", ClassType = typeof(FirmwareUpdate) },
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

namespace FirmwareUpdateUsbDevice
{
    public class SuperMutt
    {
        public class Device
        {
            public const UInt16 Vid = 0x045E;
            public const UInt16 Pid = 0x0611;
        }
    }

    public class FirmwareUpdateTaskInformation
    {
        public const String Name = "FirmwareUpdateBackgroundTask";
        public const String TaskEntryPoint = "BackgroundTask.UpdateFirmwareTask";
        public const String TaskCanceled = "Canceled";
        public const String TaskCompleted = "Completed";
        public static TimeSpan ApproximateFirmwareUpdateTime = TimeSpan.FromMinutes(2);
    }

    public class LocalSettingKeys
    {
        public class FirmwareUpdateBackgroundTask
        {
            public const String TaskStatus = "FirmwareUpdateBackgroundTaskStatus";
            public const String NewFirmwareVersion = "FirmwareUpdateBackgroundTaskNewFirmwareVersion";
        }
    }
}
