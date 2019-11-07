//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;using System;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "XAML WebView control sample";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
#if WINDOWS_PHONE_APP
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Navigate to a URL", ClassType = typeof(Controls_WebView.Scenario1) },
            new Scenario() { Title = "Navigate to a String", ClassType = typeof(Controls_WebView.Scenario2) },
            new Scenario() { Title = "Navigate to package and local state", ClassType = typeof(Controls_WebView.Scenario3) },
            new Scenario() { Title = "Navigate with custom stream", ClassType = typeof(Controls_WebView.Scenario4) },
            new Scenario() { Title = "Invoke script", ClassType = typeof(Controls_WebView.Scenario5) },
            new Scenario() { Title = "Using ScriptNotify", ClassType = typeof(Controls_WebView.Scenario6) },
            new Scenario() { Title = "Using CaptureBitmap", ClassType = typeof(Controls_WebView.Scenario8) }
        };
#else
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Navigate to a URL", ClassType = typeof(Controls_WebView.Scenario1) },
            new Scenario() { Title = "Navigate to a String", ClassType = typeof(Controls_WebView.Scenario2) },
            new Scenario() { Title = "Navigate to package and local state", ClassType = typeof(Controls_WebView.Scenario3) },
            new Scenario() { Title = "Navigate with custom stream", ClassType = typeof(Controls_WebView.Scenario4) },
            new Scenario() { Title = "Invoke script", ClassType = typeof(Controls_WebView.Scenario5) },
            new Scenario() { Title = "Using ScriptNotify", ClassType = typeof(Controls_WebView.Scenario6) },
            new Scenario() { Title = "Supporting the Share contract", ClassType = typeof(Controls_WebView.Scenario7) },
            new Scenario() { Title = "Using CaptureBitmap", ClassType = typeof(Controls_WebView.Scenario8) }
        };
#endif
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
