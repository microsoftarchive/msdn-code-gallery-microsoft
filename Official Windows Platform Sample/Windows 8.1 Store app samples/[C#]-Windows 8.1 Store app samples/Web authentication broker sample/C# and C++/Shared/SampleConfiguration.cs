//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;using System;
using WebAuthentication;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Web Authentication";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Connect to Facebook Services", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Connect to Twitter Services", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Connect to Flickr Services", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Connect to Google Services", ClassType = typeof(Scenario4) },
            new Scenario() { Title = "Account Management", ClassType = typeof(Scenario5) },
            new Scenario() { Title = "OAuth2 using Http Filters", ClassType = typeof(Scenario6) }
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
