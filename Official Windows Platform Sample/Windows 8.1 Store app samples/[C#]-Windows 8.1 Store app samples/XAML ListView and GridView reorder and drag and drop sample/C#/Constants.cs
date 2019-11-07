//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using ListViewDnD;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Drag and Drop and Reorder with the XAML ListView and GridView";

        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Enabling Reorder Within the GridView", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Dropping From a GridView to a Generic Element", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Dropping a List Item Between Items in Another List", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Dropping a List Item Into Another List (app defined location)", ClassType = typeof(Scenario4) },
            new Scenario() { Title = "Dropping a List Item on an Item in Another List", ClassType = typeof(Scenario5) }
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
