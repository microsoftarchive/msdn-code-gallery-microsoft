// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.Sensors.Sensors.Light
{
    /// <summary>
    /// Represents an ambient light sensor.
    /// </summary>
    [SensorDescription("97F115C8-599A-4153-8894-D2D12899918A")]
    public class AmbientLightSensor : Sensor
    {
        protected override void Initialize()
        {
        }

        protected override SensorDataReport CreateReport()
        {
            return new AmbientLightSensorDataReport();
        }
    }
}