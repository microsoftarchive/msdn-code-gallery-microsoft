//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using ScanRuntimeAPI;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Scan Runtime API";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Scanner Enumeration", ClassType = typeof(Scenario1EnumerateScanners) },
            new Scenario() { Title = "Just Scan", ClassType = typeof(Scenario2JustScan) },
            new Scenario() { Title = "Preview From Flatbed", ClassType = typeof(Scenario3PreviewFromFlatbed) },
            new Scenario() { Title = "Device Auto-Configured Scanning", ClassType = typeof(Scenario4DeviceAutoConfiguredScan) },
            new Scenario() { Title = "Scan From Flatbed", ClassType = typeof(Scenario5ScanFromFlatbed) },
            new Scenario() { Title = "Scan From Feeder", ClassType = typeof(Scenario6ScanFromFeeder) },
            new Scenario() { Title = "Multiple Results With Progress", ClassType = typeof(Scenario7MultipleResultsWithProgress) }
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
