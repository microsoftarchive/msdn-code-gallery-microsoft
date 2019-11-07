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
using Windows.UI.Xaml.Controls;using System;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Inclinometer";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Data events", ClassType = typeof(Microsoft.Samples.Devices.Sensors.InclinometerSample.Scenario1) },
            new Scenario() { Title = "Polling", ClassType = typeof(Microsoft.Samples.Devices.Sensors.InclinometerSample.Scenario2) },
            new Scenario() { Title = "Calibration", ClassType = typeof(Microsoft.Samples.Devices.Sensors.InclinometerSample.Scenario3) }
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
