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
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Display orientation C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Adjust for Rotation", ClassType = typeof(DisplayOrientation.Scenario1) },
            new Scenario() { Title = "Set a Rotation Preference", ClassType = typeof(DisplayOrientation.Scenario2) },
            new Scenario() { Title = "Adjust Layout for Screen Orientation", ClassType = typeof(DisplayOrientation.Scenario3) }
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
