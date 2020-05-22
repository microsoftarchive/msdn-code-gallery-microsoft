// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.Sensors.Sensors.Mechanical
{
    /// <summary>
    /// Represents an array of on/off switches/buttons.
    /// </summary>
    [SensorDescription("545C8BA5-B143-4545-868F-CA7FD986B4F6")]
    public class BooleanSwitchArray : Sensor
    {

        protected override void Initialize()
        {
        }

        protected override SensorDataReport CreateReport()
        {
            return new BooleanSwitchArrayDataReport();
        }
    }
}