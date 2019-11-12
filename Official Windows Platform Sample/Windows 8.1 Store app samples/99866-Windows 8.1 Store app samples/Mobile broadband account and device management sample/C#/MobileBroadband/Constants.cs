//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "MobileBroadband";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Mobile Broadband Device", ClassType = typeof(MobileBroadband.BroadbandDevice) },
            new Scenario() { Title = "Network Account Watcher", ClassType = typeof(MobileBroadband.AccountWatcher) },
            new Scenario() { Title = "Operator Notification", ClassType = typeof(MobileBroadband.OperatorMessage) },
            new Scenario() { Title = "Connection UI", ClassType = typeof(MobileBroadband.ShowConnection) },
            new Scenario() { Title = "Get Connection Profiles", ClassType = typeof(MobileBroadband.GetConnectionProfiles) },
            new Scenario() { Title = "Tethering", ClassType = typeof(MobileBroadband.StartStopTethering) }
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
