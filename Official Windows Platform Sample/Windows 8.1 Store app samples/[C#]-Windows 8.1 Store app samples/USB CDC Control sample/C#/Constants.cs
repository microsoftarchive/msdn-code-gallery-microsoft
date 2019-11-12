//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using UsbCdcControl;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "USB CDC ACM";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Select & Initialize", ClassType = typeof(CdcAcmInitialize) },
            new Scenario() { Title = "Read Data", ClassType = typeof(CdcAcmRead) },
            new Scenario() { Title = "Write Data", ClassType = typeof(CdcAcmWrite) },
            new Scenario() { Title = "Loopback Test", ClassType = typeof(CdcAcmLoopback) },
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

namespace UsbCdcControl
{
    public class DeviceHardwareId
    {
        public UInt32 Vid { get; set; }
        public UInt32 Pid { get; set; }
    }

    public class Constants
    {
        public const int InfiniteTimeout = -1;
        public const uint DteRateTestValue = 4800;
        public const byte DataBitsTestValue = 8;
        public const uint ExpectedResultSetLineCoding = 7;
        public const uint ExpectedResultGetLineCoding = 7;

        public static List<DeviceHardwareId> SupportedDevices = new List<DeviceHardwareId>
        {
            new DeviceHardwareId() { Vid = 0x056e, Pid = 0x5003 },
            new DeviceHardwareId() { Vid = 0x056e, Pid = 0x5004 }
        };
    }
    
    public class InterfaceClass
    {
        public const byte CdcControl = 2;
        public const byte CdcData = 10;
        public const byte VendorSpecific = 255;
    }

    public class RequestType
    {
        public const byte Set = 0x21;
        public const byte Get = 0xA1;
    }

    public class RequestCode
    {
        public const byte SetLineCoding = 0x20;
        public const byte GetLineCoding = 0x21;
        public const byte SetControlLineState = 0x22;
        public const byte SendBreak = 0x23;
    }

    public enum Parity
    {
        None = 0,
        Odd,
        Even,
        Mark,
        Space
    };

    public enum StopBits
    {
        None = -1,
        One = 0,
        OnePointFive = 1,
        Two = 2
    };
}
