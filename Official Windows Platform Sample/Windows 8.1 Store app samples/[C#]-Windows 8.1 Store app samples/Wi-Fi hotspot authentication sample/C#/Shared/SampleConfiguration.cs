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
using Windows.UI.Xaml.Controls;using System;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Hotspot Authentication";

        // This is used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Initialization", ClassType = typeof(HotspotAuthenticationApp.Initialization) },
            new Scenario() { Title = "Authentication by background task", ClassType = typeof(HotspotAuthenticationApp.AuthByBackgroundTask) },
            new Scenario() { Title = "Authentication by foreground app", ClassType = typeof(HotspotAuthenticationApp.AuthByForegroundApp) }
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
