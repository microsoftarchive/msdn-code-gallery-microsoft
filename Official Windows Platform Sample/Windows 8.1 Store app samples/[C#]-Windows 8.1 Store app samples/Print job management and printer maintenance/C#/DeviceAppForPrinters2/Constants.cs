//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using DeviceAppForPrinters2;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "DeviceAppForPrinters2";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Printer device maintenance scenario", ClassType = typeof(DeviceMaintenanceScenario) },
            new Scenario() { Title = "Print job management scenario", ClassType = typeof(PrintJobManagementScenario) },
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

    internal class DisplayStrings
    {
        internal const string NoPrintersEnumerated = 
            "No printers were enumerated. \r\n" +
            "Please verify you have the appropriate device metadata staged in the system's" +
            "local metadata store.\r\n" +
            "Device metadata may be authored and staged using the device metadata authoring wizard\r\n" +
            "http://msdn.microsoft.com/en-us/library/windows/hardware/hh454213(v=vs.85).aspx";

        internal const string PrintersEnumerating = "Enumerating printers. Please wait";

        internal const string PrinterEnumerated = "Printers enumerated. Please select a printer to proceed";

        internal const string EnumeratePrintersToContinue = "Please enumerate printers to continue";

        // Private constructor to prevent default construction.
        private DisplayStrings()
        { }
    }
}
