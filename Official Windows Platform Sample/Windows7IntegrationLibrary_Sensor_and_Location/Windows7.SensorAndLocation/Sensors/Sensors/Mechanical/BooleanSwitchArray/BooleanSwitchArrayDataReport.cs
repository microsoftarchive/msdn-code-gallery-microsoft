// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Windows7.Sensors.Sensors.Mechanical
{
    /// <summary>
    /// Represents a data report coming from <see cref="BooleanSwitchArray"/> sensor. Provides strongly-typed accessors for data properties.
    /// </summary>
    public class BooleanSwitchArrayDataReport : SensorDataReport
    {
        /// <summary>
        /// State of switch array. Each bit represents the pressed state of the switch.
        /// </summary>
        public uint SwitchArrayState
        {
            get 
            {
                return (uint) this.GetDataField(SensorPropertyKeys.SENSOR_DATA_TYPE_BOOLEAN_SWITCH_ARRAY_STATE);
                //return BitConverter.ToUInt32(BitConverter.GetBytes(fv), 0);
            }
        }

        protected override void Initialize()
        {
        }
    }
}