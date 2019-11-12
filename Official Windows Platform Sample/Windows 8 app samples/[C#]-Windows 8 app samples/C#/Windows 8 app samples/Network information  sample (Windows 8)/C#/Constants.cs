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
        public const string FEATURE_NAME = "Network Information";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Get Internet Connection Profile", ClassType = typeof(NetworkInformationApi.InternetConnectionProfile) },
            new Scenario() { Title = "Get Local Data Usage for Internet Connection Profile", ClassType = typeof(NetworkInformationApi.ProfileLocalUsageData) },
            new Scenario() { Title = "Get Connection Profile List", ClassType = typeof(NetworkInformationApi.ConnectionProfileList) },
            new Scenario() { Title = "Get Lan Identifiers", ClassType = typeof(NetworkInformationApi.LanId) },
            new Scenario() { Title = "Register for Network Status Change Notifications", ClassType = typeof(NetworkInformationApi.NetworkStatusChange) }
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
