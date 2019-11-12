//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using ContactActions;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Handling Contact Actions";

        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Handling an activation to make a call", ClassType = typeof(CallScenario) },
            new Scenario() { Title = "Handling an activation to send a message", ClassType = typeof(SendMessageScenario) },
            new Scenario() { Title = "Handling an activation to map an address", ClassType = typeof(MapAddressScenario) }
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
