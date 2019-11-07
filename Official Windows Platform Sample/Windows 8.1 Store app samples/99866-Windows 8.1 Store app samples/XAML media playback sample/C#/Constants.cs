//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using BasicMediaPlayback;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public bool SystemMediaTransportControlsInitialized = false;

        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "XAML media playback sample";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Playing a media file", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Play To devices", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Using markers", ClassType = typeof(Scenario3) },            
            new Scenario() { Title = "Custom transport controls", ClassType = typeof(Scenario4) },
            new Scenario() { Title = "Playing multiple files", ClassType = typeof(Scenario5) },
            new Scenario() { Title = "Playing background media", ClassType = typeof(Scenario6) }
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
