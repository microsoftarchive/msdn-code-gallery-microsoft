//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using PDFAPI;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        
        public const string FEATURE_NAME = "PDF API";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Render PDF page", ClassType = typeof(Scenario1) },          
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
