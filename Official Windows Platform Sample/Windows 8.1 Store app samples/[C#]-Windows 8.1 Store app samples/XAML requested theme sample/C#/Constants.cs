//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using RequestedTheme;
using SDKTemplate.Common;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage  
    {
        public const string FEATURE_NAME = "Requested Theme";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Basic RequestedTheme use", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Inheritance Properties", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Binding RequestedTheme Property", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Using code to change RequestedTheme Properties", ClassType = typeof(Scenario4) },
			new Scenario() { Title = "Specifying application theme in XAML", ClassType = typeof(Scenario5) },
            new Scenario() { Title = "Specifying application theme at startup", ClassType = typeof(Scenario6) }
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
