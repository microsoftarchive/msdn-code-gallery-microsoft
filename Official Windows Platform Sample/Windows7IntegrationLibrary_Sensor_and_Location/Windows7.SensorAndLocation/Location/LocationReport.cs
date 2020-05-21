// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Location.Internal;
using System.Runtime.InteropServices;
using Windows7.Sensors;
using Windows7.Sensors.Internal;


namespace Windows7.Location
{
    /// <summary>
    /// Base class for location reports.
    /// </summary>
    public abstract class LocationReport
    {
        private ILocationReport _locationReport;
        
        /// <summary>
        /// Gets or sets the inner COM object. Used internally.
        /// </summary>
        internal ILocationReport InnerObject
        {
            get { return _locationReport; }
        }
                
        /// <summary>
        /// Allows <see cref="LocationReport.Initialize"/> to be called internally without making in public.
        /// </summary>
        internal void InitializeReport(ILocationReport iLocReport)
        {
            _locationReport = iLocReport;
             Initialize();
        }

        /// <summary>
        /// Allows derived types to perform initializations which require accessing the base type's properties/methods.
        /// </summary>
        protected abstract void Initialize();
        
        /// <summary>
        /// Returns the time at which this report was created.
        /// </summary>
        public DateTime Timestamp
        {
            get { return _locationReport.GetTimestamp().DateTime; }
        }

        /// <summary>
        /// Gets the ID of the sensor which generated this report.
        /// </summary>
        public Guid SensorID
        {
            get { return _locationReport.GetSensorID(); }
        }

        /// <summary>
        /// Returns the value of the specified property in the report.
        /// </summary>
        /// <param name="key">Property ID.</param>
        /// <returns>Property value.</returns>
        public object GetValue(PropertyKey key)
        {
            PROPVARIANT variant;
            
            _locationReport.GetValue(ref key, out variant);
            try
            {
                return variant.Value;
            }
            finally
            {
                variant.Clear();
            }
        }
    }
}