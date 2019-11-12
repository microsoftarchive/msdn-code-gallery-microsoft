//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using DataSourceAdapter;

using System;
using System.Collections.Generic;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "GetVirtualizedFilesVector C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Display the Pictures library in a GridView and ListView",                         ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Display the Pictures library in a GridView and ListView, organized by folder",    ClassType = typeof(Scenario2) },
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
