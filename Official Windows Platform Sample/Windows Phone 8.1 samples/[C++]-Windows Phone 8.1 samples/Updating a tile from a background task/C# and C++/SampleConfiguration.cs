// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace CustomTileFromBackground
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "App Tile Updater ";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Register background task", ClassType=typeof(Scenario1)},
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
