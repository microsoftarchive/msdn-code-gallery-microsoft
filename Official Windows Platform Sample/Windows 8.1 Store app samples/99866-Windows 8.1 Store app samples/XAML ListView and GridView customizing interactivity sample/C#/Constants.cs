//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using ListViewInteraction;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "XAML ListView and GridView customizing interactivity";

        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Multi-select storefront", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Creating a master-detail ListView", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Creating a static ListView", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Creating a 'picker' GridView", ClassType = typeof(Scenario4) }
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
