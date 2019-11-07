//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;

namespace AzureMobileStorage
{
    public partial class MainPage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Windows Azure Mobile Services - simple doto";
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
