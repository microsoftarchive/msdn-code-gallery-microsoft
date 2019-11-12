//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using SearchBox;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Search Control C# Sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "App provided suggestions",                    ClassType = typeof(S1_SearchBoxWithSuggestions) },
            new Scenario() { Title = "Suggestions in East Asian Languages",         ClassType = typeof(S2_SuggestionsEastAsian) },
            new Scenario() { Title = "Windows provided suggestions",                ClassType = typeof(S3_SuggestionsWindows) },
            new Scenario() { Title = "Suggestions from Open Search",                ClassType = typeof(S4_SuggestionsOpenSearch) },
            new Scenario() { Title = "Search box and suggestions from Open Search", ClassType = typeof(S5_SuggestionsXML) },
            new Scenario() { Title = "Give the search box focus by typing",         ClassType = typeof(S6_KeyboardFocus) } 
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
