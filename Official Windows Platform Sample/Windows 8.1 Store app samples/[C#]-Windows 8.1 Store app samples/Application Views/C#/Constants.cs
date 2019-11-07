//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using ApplicationViews;

namespace SDKTemplate
{
    public partial class MainPage
    {
        public const string FEATURE_NAME = "Application Views C# Sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Use window orientation to change stacking direction of UI", ClassType = typeof(S1_Orientation) },
            new Scenario() { Title = "Use edge information to position controls", ClassType = typeof(S2_Edges) },
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
