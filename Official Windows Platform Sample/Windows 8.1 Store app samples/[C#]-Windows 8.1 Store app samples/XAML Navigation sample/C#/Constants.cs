//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using Navigation;

namespace SDKTemplate
{
    public partial class MainPage
    {
        public const string FEATURE_NAME = "XAML Navigation C#";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Basic Usage", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Passing information between pages", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Cancel Navigation", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Caching a Page", ClassType = typeof(Scenario4) }
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
