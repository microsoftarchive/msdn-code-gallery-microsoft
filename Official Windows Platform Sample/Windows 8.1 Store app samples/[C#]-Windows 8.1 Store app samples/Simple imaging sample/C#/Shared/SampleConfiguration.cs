
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
using Windows.UI.Xaml.Controls;
using SimpleImaging;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Simple imaging C# sample";

        // This list defines the scenarios covered in this sample and their titles.
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() {
                Title = "Image properties (FileProperties)",
                ClassType = typeof(ImagingProperties)
            },
            new Scenario() {
                Title = "Image transforms/encode (BitmapDecoder)",
                ClassType = typeof(ImagingTransforms)
            },
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
