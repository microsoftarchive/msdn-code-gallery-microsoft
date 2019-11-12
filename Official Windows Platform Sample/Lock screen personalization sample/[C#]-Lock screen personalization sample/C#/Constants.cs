//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Personalization;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Personalization C#";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Pick and set lock screen image", ClassType = typeof(SetLockScreen) },
            new Scenario() { Title = "Register lock screen slide show image feed", ClassType = typeof(AddImageFeedScenario) }
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
