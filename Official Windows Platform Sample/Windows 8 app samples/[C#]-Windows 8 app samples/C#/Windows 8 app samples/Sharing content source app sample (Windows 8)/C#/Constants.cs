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

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Share Source C#";
        public const string MissingTitleError = "Enter a title for what you are sharing and try again.";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Share text",                  ClassType = typeof(ShareSource.ShareText) },
            new Scenario() { Title = "Share link",                  ClassType = typeof(ShareSource.ShareLink) },
            new Scenario() { Title = "Share image",                 ClassType = typeof(ShareSource.ShareImage) },
            new Scenario() { Title = "Share files",                 ClassType = typeof(ShareSource.ShareFiles) },
            new Scenario() { Title = "Share delay rendered files",  ClassType = typeof(ShareSource.ShareDelayRenderedFiles) },
            new Scenario() { Title = "Share HTML content",          ClassType = typeof(ShareSource.ShareHtml) },
            new Scenario() { Title = "Share custom data",           ClassType = typeof(ShareSource.ShareCustomData) },
            new Scenario() { Title = "Fail with display text",      ClassType = typeof(ShareSource.SetErrorMessage) }
        };

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state. If an app wants to show a FilePicker while snapped,
            // it must attempt to unsnap first.
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                NotifyUser("Cannot unsnap the sample application.", NotifyType.StatusMessage);
            }
            return unsnapped;
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
