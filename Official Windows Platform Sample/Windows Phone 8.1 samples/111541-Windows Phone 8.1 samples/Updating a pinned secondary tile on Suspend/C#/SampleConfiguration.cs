// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace TileUpdateAfterSuspension
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "How to update a tile after it has been pinned.";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="not recommended: Pin tile and update immediately", ClassType=typeof(Scenario1)},
            new Scenario() { Title="recommended: Pin tile and update on Suspend", ClassType=typeof(Scenario2)},
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
