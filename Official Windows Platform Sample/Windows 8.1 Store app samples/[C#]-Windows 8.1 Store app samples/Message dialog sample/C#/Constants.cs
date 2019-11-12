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

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Message Dialog C# Sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Use custom commands", ClassType = typeof(MessageDialogSample.CustomCommand) },
            new Scenario() { Title = "Use default close command", ClassType = typeof(MessageDialogSample.DefaultCloseCommand) },
            new Scenario() { Title = "Use completed callback", ClassType = typeof(MessageDialogSample.CompletedCallback) },
            new Scenario() { Title = "Use cancel command", ClassType = typeof(MessageDialogSample.CancelCommand) }
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
