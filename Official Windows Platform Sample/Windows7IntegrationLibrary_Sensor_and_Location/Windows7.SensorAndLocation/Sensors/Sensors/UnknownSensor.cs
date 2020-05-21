// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.Sensors.Sensors
{
    /// <summary>
    /// Sensor type used for sensors for which there is no specific implementation.
    /// </summary>
    public class UnknownSensor : Sensor
    {
        protected override void Initialize()
        {
        }

        protected override SensorDataReport CreateReport()
        {
            return new UnknownSensorDataReport();
        }
    }
}