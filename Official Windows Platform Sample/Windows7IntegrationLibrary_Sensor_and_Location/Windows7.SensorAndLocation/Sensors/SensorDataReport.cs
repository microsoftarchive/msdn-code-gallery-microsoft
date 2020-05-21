// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows7.Sensors.Internal;


namespace Windows7.Sensors.Internal
{
    /// <summary>
    /// COM interop wrapper for the ISensorDataReport interface.
    /// </summary>
    [ComImport, Guid("0AB9DF9B-C4B5-4796-8898-0470706A2E1D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISensorDataReport
    {
        /// <summary>
        /// Get the timestamp for the data report
        /// </summary>
        /// <param name="timeStamp">The timestamp returned for the data report</param>
        void GetTimestamp(out SYSTEMTIME pTimeStamp);

        /// <summary>
        /// Get a single value reported by the sensor
        /// </summary>
        /// <param name="propKey">The data field ID of interest</param>
        /// <param name="propValue">The data returned</param>
        void GetSensorValue(
            [In] ref PropertyKey pKey,
            out PROPVARIANT pValue); // value must be newed before call

        /// <summary>
        /// Get multiple values reported by a sensor
        /// </summary>
        /// <param name="keys">The collection of keys for values to obtain data for</param>
        /// <param name="values">The values returned by the query</param>
        void GetSensorValues(
            [In, MarshalAs(UnmanagedType.Interface)] IPortableDeviceKeyCollection pKeys,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPortableDeviceValues pValues);
    }
}
 
namespace Windows7.Sensors
{
    /// <summary>
    /// A base-class for sensor data reports. Contains methods which derived types can use to expose values in a friendly an type-safe manner.
    /// </summary>
    public abstract class SensorDataReport
    {
        private ISensorDataReport _iSensorReport;
        private Sensor _sensor;

        /// <summary>
        /// Constructs a data report.
        /// </summary>
        protected SensorDataReport()
        {
        }

        internal void InitializeReport(ISensorDataReport report, Sensor sensor)
        {
            _iSensorReport = report;
            _sensor = sensor;
            Initialize();
        }

        protected abstract void Initialize();

        /// <summary>
        /// Retrieves the sensor object which generated this report.
        /// </summary>
        protected Sensor Sensor
        {
            get { return _sensor; }
        }

        /// <summary>
        /// Retrieves the time at which the data report was created.
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                SYSTEMTIME sTime;
                _iSensorReport.GetTimestamp(out sTime);
                return sTime.DateTime;
            }
        }

        /// <summary>
        /// Returns the property IDs and values for all data contained in this data report.
        /// </summary>
        /// <returns>A dictionary mapping property IDs to values.</returns>
        public IDictionary<PropertyKey, object> GetDataFields()
        {
            Dictionary<PropertyKey, object> reportData = new Dictionary<PropertyKey, object>();
            IPortableDeviceValues valuesCollection;
            _iSensorReport.GetSensorValues(null, out valuesCollection);

            uint nItems = 0;
            valuesCollection.GetCount(ref nItems);
            for (uint i = 0; i < nItems; i++)
            {
                PropertyKey propKey = new PropertyKey();
                PROPVARIANT propValue = new PROPVARIANT();
                valuesCollection.GetAt(i, ref propKey, out propValue);

                try
                {
                    reportData.Add(propKey, propValue.Value);
                }
                finally
                {
                    propValue.Clear();
                }
            }

            return reportData;
        }

        /// <summary>
        /// Returns the property value for the given property ID.
        /// </summary>
        /// <param name="propKey">Property ID.</param>
        /// <returns>A value (may be of various types).</returns>
        public object GetDataField(PropertyKey propKey)
        {
            PROPVARIANT propVal = new PROPVARIANT();

            try
            {
                _iSensorReport.GetSensorValue(ref propKey, out propVal);
                return propVal.Value;
            }
            finally
            {
                propVal.Clear();
            }
        }
    }
}