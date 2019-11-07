// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Action_Center_Quickstart
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Action Center Quickstart";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="sending a toast", ClassType=typeof(Scenario1)},
            new Scenario() { Title="sending a toast, no popup", ClassType=typeof(Scenario2)},
            new Scenario() { Title="action center queue", ClassType=typeof(Scenario3)},
            new Scenario() { Title="removing notifications", ClassType=typeof(Scenario4)},
            new Scenario() { Title="replacing a notification", ClassType=typeof(Scenario5)}
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
