// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Sensors.Internal;
using Windows7.Sensors;

namespace Windows7.Location.Internal
{
    /// <summary>
    /// Creates a lat long location report for the specified data.  Used to set the default location.
    /// </summary>
    internal class DefaultLatLongReport : ILatLongReport
    {
        /// <summary>
        /// Initializes the position report.
        /// </summary>
        public DefaultLatLongReport(double latitude, double longitude, double errorRadius, double altitude, double altitudeError)
        {
            _latitude = latitude;
            _longitude = longitude;
            _errorRadius = errorRadius;
            _altitude = altitude;
            _altitudeError = altitudeError;
            _timeStamp = new SYSTEMTIME(DateTime.UtcNow);
        }

        /// <summary>
        /// Return the sensor ID for the default location provider.
        /// </summary>
        public Guid GetSensorID()
        {
            return SensorIDs.DEFAULT_LOCATION_PROVIDER;
        }

        /// <summary>
        /// Gets the age of the data.
        /// </summary>
        public SYSTEMTIME GetTimestamp()
        {
            return _timeStamp;
        }

        /// <summary>
        /// Gets the specified property value.
        /// </summary>
        /// <param name="pKey">The key of the requested property.</param>
        public void GetValue(ref PropertyKey pKey, out PROPVARIANT pValue)
        {
            pValue = new PROPVARIANT();
            if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_LATITUDE_DEGREES))
            {
                pValue.SetValue(_latitude);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_LONGITUDE_DEGREES))
            {
                pValue.SetValue(_longitude);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_ERROR_RADIUS_METERS))
            {
                pValue.SetValue(_errorRadius);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS))
            {
                pValue.SetValue(_altitude);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS))
            {
                pValue.SetValue(_altitudeError);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_TIMESTAMP))
            {
                pValue.SetFileTime(_timeStamp);
            }
        }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        public double GetLatitude()
        {
            return _latitude;
        }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        public double GetLongitude()
        {
            return _longitude;
        }

        /// <summary>
        /// Gets the error radius.
        /// </summary>
        public double GetErrorRadius()
        {
            return _errorRadius;
        }

        /// <summary>
        /// Gets the altitude.
        /// </summary>
        public double GetAltitude()
        {
            return _altitude;
        }

        /// <summary>
        /// Gets the altitude error.
        /// </summary>
        public double GetAltitudeError()
        {
            return _altitudeError;
        }

        private double _latitude;
        private double _longitude;
        private double _errorRadius;
        private double _altitude;
        private double _altitudeError;
        private SYSTEMTIME _timeStamp;
    }
}