//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Projection;
using SecondaryViewsHelpers;
using System.Collections.Generic;
using System;

namespace SDKTemplate
{
    public partial class MainPage
    {
        public const string FEATURE_NAME = "Projection";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Creating and projecting a view", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Second screen availability", ClassType = typeof(Scenario2) }
        };

        // Keep track of the view that's being projected
        public ViewLifetimeControl ProjectionViewPageControl;
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
