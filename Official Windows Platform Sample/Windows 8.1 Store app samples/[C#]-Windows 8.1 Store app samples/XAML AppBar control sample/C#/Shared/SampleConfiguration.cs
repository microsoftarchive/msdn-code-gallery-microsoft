//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using AppBarControl;

namespace SDKTemplate
{
    public partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "XAML AppBar control sample";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
#if WINDOWS_PHONE_APP
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Create a CommandBar", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Customize CommandBar color", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Customize icons", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Control the CommandBar and commands", ClassType = typeof(Scenario6) },
            new Scenario() { Title = "Show contextual commands for a GridView", ClassType = typeof(Scenario7) },
            new Scenario() { Title = "Localizing CommandBar commands", ClassType = typeof(Scenario8) }
        };
#else
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Create an AppBar", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Customize AppBar color", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Customize icons", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Using CommandBar", ClassType = typeof(Scenario4) },
            new Scenario() { Title = "Custom content", ClassType = typeof(Scenario5) },
            new Scenario() { Title = "Control the AppBar and commands", ClassType = typeof(Scenario6) },
            new Scenario() { Title = "Show contextual commands for a GridView", ClassType = typeof(Scenario7) },
            new Scenario() { Title = "Localizing AppBar commands", ClassType = typeof(Scenario8) }
        };
#endif

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
