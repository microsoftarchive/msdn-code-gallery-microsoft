//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using MultipleViews;
using System;
using System.Collections.Generic;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Multiple Views";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Creating and showing multiple views", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Responding to activation",            ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Using animations when switching",     ClassType = typeof(Scenario3) }
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