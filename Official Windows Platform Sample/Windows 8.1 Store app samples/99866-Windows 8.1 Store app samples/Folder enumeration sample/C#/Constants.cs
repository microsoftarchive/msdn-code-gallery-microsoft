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

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Folder enumeration C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Enumerate files and folders in the Pictures library",        ClassType = typeof(FolderEnumeration.Scenario1) },
            new Scenario() { Title = "Enumerate files in the Pictures library, by groups",         ClassType = typeof(FolderEnumeration.Scenario2) },
            new Scenario() { Title = "Enumerate files in the Pictures library with prefetch APIs", ClassType = typeof(FolderEnumeration.Scenario3) },
            new Scenario() { Title = "Enumerate files in a folder and display availability",       ClassType = typeof(FolderEnumeration.Scenario4) },
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
