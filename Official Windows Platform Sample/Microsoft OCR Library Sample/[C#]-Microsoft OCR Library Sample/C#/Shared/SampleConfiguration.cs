// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace OCR
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Windows Runtime OCR";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Extract text from image", ClassType=typeof(ExtractText)},
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}