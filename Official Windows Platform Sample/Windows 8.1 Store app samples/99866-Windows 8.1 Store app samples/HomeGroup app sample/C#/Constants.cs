//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using HomeGroup;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "HomeGroup C#";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Open the file picker at HomeGroup", ClassType = typeof(OpenPicker) },
            new Scenario() { Title = "Search HomeGroup", ClassType = typeof(SearchHomeGroup) },
            new Scenario() { Title = "Stream video from Homegroup", ClassType = typeof(StreamVideo) },
            new Scenario() { Title = "Advanced search", ClassType = typeof(SearchByUser) }
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
