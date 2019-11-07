// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Scheduled Notifications";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Schedule Notifications", ClassType=typeof(Scenario1)},
            new Scenario() { Title="Managed Scheduled Notifications", ClassType=typeof(Scenario2)},
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}