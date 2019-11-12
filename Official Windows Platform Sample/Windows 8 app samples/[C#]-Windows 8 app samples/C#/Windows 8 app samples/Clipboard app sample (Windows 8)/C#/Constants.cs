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
using System;
using Windows.UI.ViewManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Clipboard C#";

        public bool isClipboardContentChangeChecked = false;
        public bool needToPrintClipboardFormat = false;
        public EventHandler<object> clipboardContentChanged = null;

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Copy and paste text",        ClassType = typeof(Clipboard.CopyText) },
            new Scenario() { Title = "Copy and paste an image",    ClassType = typeof(Clipboard.CopyImage) },
            new Scenario() { Title = "Copy and paste files",       ClassType = typeof(Clipboard.CopyFile) },
            new Scenario() { Title = "Other Clipboard operations", ClassType = typeof(Clipboard.OtherScenarios) }
        };

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state. If an app wants to show a FilePicker while snapped,
            // it must attempt to unsnap first.
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                NotifyUser("Cannot unsnap the sample app.", NotifyType.StatusMessage);
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
