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
using SDKTemplate.Common;
using ShareSource;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Share Source C#";
        public const string MissingTitleError = "Enter a title for what you are sharing and try again.";

        public static List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Share text",                  ClassType = typeof(ShareText) },
            new Scenario() { Title = "Share web link",              ClassType = typeof(ShareWebLink) },
            new Scenario() { Title = "Share application link",      ClassType = typeof(ShareApplicationLink) },
            new Scenario() { Title = "Share image",                 ClassType = typeof(ShareImage) },
            new Scenario() { Title = "Share files",                 ClassType = typeof(ShareFiles) },
            new Scenario() { Title = "Share delay rendered files",  ClassType = typeof(ShareDelayRenderedFiles) },
            new Scenario() { Title = "Share HTML content",          ClassType = typeof(ShareHtml) },
            new Scenario() { Title = "Share custom data",           ClassType = typeof(ShareCustomData) },
            new Scenario() { Title = "Fail with display text",      ClassType = typeof(SetErrorMessage) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public Uri ApplicationLink
        {
            get
            {
                return SharePage.GetApplicationLink(ClassType.Name);
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
