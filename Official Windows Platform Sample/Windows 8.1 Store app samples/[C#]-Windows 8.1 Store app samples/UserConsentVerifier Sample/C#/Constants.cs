//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using UserConsentVerifierCS;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "User Consent Verifier";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Check Consent Availability", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Request Consent", ClassType = typeof(Scenario2) }
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
