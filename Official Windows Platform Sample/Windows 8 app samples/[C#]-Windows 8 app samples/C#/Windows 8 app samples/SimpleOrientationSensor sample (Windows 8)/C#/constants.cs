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
        public const string FEATURE_NAME = "Simple Orientation Sensor Raw Data";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Simple orientation sensor data events", ClassType = typeof(Microsoft.Samples.Devices.Sensors.SimpleOrientationSensorSample.Scenario1) },
            new Scenario() { Title = "Current simple orientation sensor reading", ClassType = typeof(Microsoft.Samples.Devices.Sensors.SimpleOrientationSensorSample.Scenario2) }
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
