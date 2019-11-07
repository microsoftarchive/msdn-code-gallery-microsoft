//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using LibraryManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Library management sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Adding a folder to the Pictures library",     ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Listing folders in the Pictures library",     ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Removing a folder from the Pictures library", ClassType = typeof(Scenario3) }
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
