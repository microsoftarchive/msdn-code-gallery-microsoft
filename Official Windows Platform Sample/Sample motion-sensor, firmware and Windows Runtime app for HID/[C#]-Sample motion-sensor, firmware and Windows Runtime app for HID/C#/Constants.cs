//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using HidInfraredSensor;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
 
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "HID PIR Sensor/Video Capture";

        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Connect to the Motion Sensor", ClassType = typeof(DeviceConnect) },
            new Scenario() { Title = "Enable sensor-triggered video captures", ClassType = typeof(SensorTriggeredVideoCapture) },
            new Scenario() { Title = "Set report interval", ClassType = typeof(SetReportInterval) }
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

namespace HidInfraredSensor
{
    public enum DeviceModel
    {
        IR_Sensor,
        Any,    // Can be any device
        None
    }

    public class DeviceProperties
    {
        public const String DeviceInstanceId = "System.Devices.DeviceInstanceId";
    }

    /// <summary>
    ///    Report descriptor for the HID presence sensor:
    ///    0x06,0x55,0xFF,     //HID_USAGE_PAGE_VENDOR_DEFINED
    ///    0x09,0xA5,          //HID_USAGE (vendor_defined)
    ///    0xA1,0x01,          //HID_COLLECTION(Application),
    ///    Input report (device-transmits)
    ///    0x09,0xA7,          //HID_USAGE (vendor_defined)
    ///    0x15,0x00,          //HID_LOGICAL_MIN_8(0), // False = not present
    ///    0x25,0x01,          //HID_LOGICAL_MAX_8(1), // True = present
    ///    0x75,0x08,          //HID_REPORT_SIZE(8),
    ///    0x95,0x01,          //HID_REPORT_COUNT(1),
    ///    0x81,0x02,          //HID_INPUT(Data_Var_Abs),
    ///    0x09,0xA8,          //HID_USAGE (vendor_defined)
    ///    0x15,0x01,          //HID_LOGICAL_MIN_8(1), // minimum 1-second
    ///    0x25,0x3C,          //HID_LOGICAL_MAX_8(60), // maximum 60-seconds
    ///    0x75,0x08,          //HID_REPORT_SIZE(8), 
    ///    0x95,0x01,          //HID_REPORT_COUNT(1), 
    ///    0x81,0x02,          //HID_INPUT(Data_Var_Abs),
    ///    Output report (device-receives)
    ///    0x09,0xA9,          //HID_USAGE (vendor_defined)
    ///    0x15,0x01,          //HID_LOGICAL_MIN_8(1), // minimum 1-second
    ///    0x25,0x3C,          //HID_LOGICAL_MAX_8(60), // maximum 60-seconds
    ///    0x75,0x08,          //HID_REPORT_SIZE(8), 
    ///    0x95,0x01,          //HID_REPORT_COUNT(1), 
    ///    0x91,0x02,          //HID_OUTPUT(Data_Var_Abs),
    /// </summary>
    public class IR_Sensor
    {
          public class Device
        {
            public const UInt16 Vid = 0x16C0;
            public const UInt16 Pid = 0x0012;
            public const UInt16 UsagePage = 0xFF55;
            public const UInt16 UsageId = 0xA5;

        }
    }
}
