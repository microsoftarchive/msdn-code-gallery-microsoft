// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Sensors.Internal;
using Windows7.Sensors;

namespace Windows7.Location.Internal
{
    /// <summary>
    /// Creates a civic address location report for the specified data.  Used to set the default location.
    /// </summary>
    internal class DefaultCivicAddressReport : ICivicAddressReport
    {
        /// <summary>
        /// Initializes the address report.
        /// </summary>
        public DefaultCivicAddressReport(string address1, string address2, string city, string stateProvince, string postalCode, string countryRegion)
        {
            _timestamp = DateTime.UtcNow;
            InitializeString(ref _CountryRegion, countryRegion);
            InitializeString(ref _address1, address1);
            InitializeString(ref _address2, address2);
            InitializeString(ref _city, city);
            InitializeString(ref _StateProvince, stateProvince);
            InitializeString(ref _PostalCode, postalCode);
        }

        /// <summary>
        /// Sets a string to the specified value.  Sets the string to an empty string when the value is null.
        /// </summary>
        private void InitializeString(ref string text, string value)
        {
            if (value != null)
            {
                text = value;
            }
            else
            {
                text = string.Empty;
            }
        }

        /// <summary>
        /// Return the sensor ID for the default location provider.
        /// </summary>
        public Guid GetSensorID()
        {
            return SensorIDs.DEFAULT_LOCATION_PROVIDER;
        }

        /// <summary>
        /// Returns the age of the data.
        /// </summary>
        public SYSTEMTIME GetTimestamp()
        {
            return new SYSTEMTIME(_timestamp);
        }

        /// <summary>
        /// Gets the specified property value.
        /// </summary>
        /// <param name="pKey">The key identifying requested property.</param>
        public void GetValue(ref PropertyKey pKey, out PROPVARIANT pValue)
        {
            pValue = new PROPVARIANT();
            if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_ADDRESS1))
            {
                pValue.SetValue(_address1);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_ADDRESS2))
            {
                pValue.SetValue(_address2);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_CITY))
            {
                pValue.SetValue(_city);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_STATE_PROVINCE))
            {
                pValue.SetValue(_StateProvince);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_POSTALCODE))
            {
                pValue.SetValue(_PostalCode);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_TIMESTAMP))
            {
                pValue.SetFileTime(_timestamp);
            }
            else if (pKey.Equals(SensorPropertyKeys.SENSOR_DATA_TYPE_COUNTRY_REGION))
            {
                pValue.SetValue(_CountryRegion);
            }
        }

        /// <summary>
        /// Gets the first line of the address.
        /// </summary>
        public string GetAddressLine1()
        {
            return _address1;
        }

        /// <summary>
        /// Gets the second line of the address.
        /// </summary>
        public string GetAddressLine2()
        {
            return _address2;
        }

        /// <summary>
        /// Gets the city.
        /// </summary>
        public string GetCity()
        {
            return _city;
        }

        /// <summary>
        /// Gets the StateProvince.
        /// </summary>
        public string GetStateProvince()
        {
            return _StateProvince;
        }

        /// <summary>
        /// Gets the postal code.
        /// </summary>
        public string GetPostalCode()
        {
            return _PostalCode;
        }

        /// <summary>
        /// Gets the CountryRegion.
        /// </summary>
        public string GetCountryRegion()
        {
            return _CountryRegion;
        }

        /// <summary>
        /// Reserved.  Not implemented.
        /// </summary>
        public uint GetDetailLevel()
        {
            const uint DEFAULT_DETAIL_LEVEL = 1;
            return DEFAULT_DETAIL_LEVEL;
        }

        private string _address1;
        private string _address2;
        private string _city;
        private string _StateProvince;
        private string _PostalCode;
        private string _CountryRegion;
        private DateTime _timestamp;
    }
}