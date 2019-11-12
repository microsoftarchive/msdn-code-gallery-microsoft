// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Push and Periodic Notifications";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Registering a notification channel", ClassType=typeof(Scenario1)},
            new Scenario() { Title="Renewing channels", ClassType=typeof(Scenario2)},
            new Scenario() { Title="Listening for push notifications", ClassType=typeof(Scenario3)},
            new Scenario() { Title="Polling for tile updates", ClassType=typeof(Scenario4)},
            new Scenario() { Title="Polling for badge updates", ClassType=typeof(Scenario5)}
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}