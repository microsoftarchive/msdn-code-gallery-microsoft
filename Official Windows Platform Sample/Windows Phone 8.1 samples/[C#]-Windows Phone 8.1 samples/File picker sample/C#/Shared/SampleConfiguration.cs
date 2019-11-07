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
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "File picker C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Pick a single photo",   ClassType = typeof(FilePicker.Scenario1) },
            new Scenario() { Title = "Pick multiple files",   ClassType = typeof(FilePicker.Scenario2) },
            new Scenario() { Title = "Pick a folder",         ClassType = typeof(FilePicker.Scenario3) },
            new Scenario() { Title = "Save a file",           ClassType = typeof(FilePicker.Scenario4) },
            new Scenario() { Title = "Open a cached file",    ClassType = typeof(FilePicker.Scenario5) },
            new Scenario() { Title = "Update a cached file",  ClassType = typeof(FilePicker.Scenario6) },
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
