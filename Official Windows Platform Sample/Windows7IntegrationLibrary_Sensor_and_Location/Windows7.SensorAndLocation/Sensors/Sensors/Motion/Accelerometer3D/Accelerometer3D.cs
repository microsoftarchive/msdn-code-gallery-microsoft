// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.Sensors.Sensors.Motion
{
    /// <summary>
    /// Represents a 3D accelerometer.
    /// </summary>
    [SensorDescription("C2FB0F5F-E2D2-4C78-BCD0-352A9582819D")]
    public class Accelerometer3D : Sensor
    {
        protected override void Initialize()
        {
        }

        protected override SensorDataReport CreateReport()
        {
            return new Accelerometer3DReport();
        }
    }
}