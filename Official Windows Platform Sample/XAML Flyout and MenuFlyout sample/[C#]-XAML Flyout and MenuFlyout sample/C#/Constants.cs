//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using Flyouts;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "XAML Flyout and MenuFlyout Controls";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Basic Flyout and MenuFlyout Usage", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Sharing a Flyout", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Non-Button Flyouts", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Dynamic Content", ClassType = typeof(Scenario4) }
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
