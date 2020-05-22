// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Sensors.Internal;



namespace Windows7.Sensors.Sensors.Motion
{
    /// <summary>
    /// Represents a data report coming from <see cref="Accelerometer3D"/> sensor. Provides strongly-typed accessors for data properties.
    /// </summary>
    public class Accelerometer3DReport : SensorDataReport
    {
        /// <summary>
        /// Acceleration in g's in the X axis.
        /// </summary>
        public float AxisX_G
        {
            get { return (float)this.GetDataField(PropertyKey.Create(new Guid(0X3F8A69A2, 0X7C5, 0X4E48, 0XA9, 0X65, 0XCD, 0X79, 0X7A, 0XAB, 0X56, 0XD5), 2)); }
        }

        /// <summary>
        /// Acceleration in g's in the Y axis.
        /// </summary>
        public float AxisY_G
        {
            get { return (float)this.GetDataField(PropertyKey.Create(new Guid(0X3F8A69A2, 0X7C5, 0X4E48, 0XA9, 0X65, 0XCD, 0X79, 0X7A, 0XAB, 0X56, 0XD5), 3)); }
        }

        /// <summary>
        /// Acceleration in g's in the Z axis.
        /// </summary>
        public float AxisZ_G
        {
            get { return (float)this.GetDataField(PropertyKey.Create(new Guid(0X3F8A69A2, 0X7C5, 0X4E48, 0XA9, 0X65, 0XCD, 0X79, 0X7A, 0XAB, 0X56, 0XD5), 4)); }
        }

        protected override void Initialize()
        {
        }
    }
}