//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Search;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Search contract C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Using the Search contract",                ClassType = typeof(SearchContract.Scenario1) },
            new Scenario() { Title = "Suggestions from an app-defined list",     ClassType = typeof(SearchContract.Scenario2) },
            new Scenario() { Title = "Suggestions in East Asian languages",      ClassType = typeof(SearchContract.Scenario3) },
            new Scenario() { Title = "Suggestions provided by Windows",          ClassType = typeof(SearchContract.Scenario4) },
            new Scenario() { Title = "Suggestions from Open Search",             ClassType = typeof(SearchContract.Scenario5) },
            new Scenario() { Title = "Suggestions from a service returning XML", ClassType = typeof(SearchContract.Scenario6) },
            new Scenario() { Title = "Open Search charm by typing",              ClassType = typeof(SearchContract.Scenario7) },
        };

        internal const int SearchPaneMaxSuggestions = 5;

        internal void ProcessQueryText(string queryText)
        {
            NotifyUser("Query submitted: " + queryText, NotifyType.StatusMessage);
        }
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
