//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using AzureMobileAuthentication;

namespace AzureMobileAuthentication
{
    public partial class MainPage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Windows Azure Mobile Service - Authentication";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Connect to Microsoft Account Services", ClassType = typeof(MicrosoftAuth) },
            new Scenario() { Title = "Connect to Facebook Services", ClassType = typeof(FacebookAuth) },
            new Scenario() { Title = "Connect to Twitter Services", ClassType = typeof(TwitterAuth) },
            new Scenario() { Title = "Connect to Google Services", ClassType = typeof(GoogleAuth) }
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
