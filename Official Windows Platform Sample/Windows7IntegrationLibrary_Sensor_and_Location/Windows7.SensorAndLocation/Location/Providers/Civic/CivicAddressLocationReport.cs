// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Location.Internal;

namespace Windows7.Location
{
    /// <summary>
    /// Detail level of the civic address report.
    /// </summary>
    public enum DetailLevel
    {
        Address = 1,
        PostalCode = 2,
        City = 3,
        StateOrProvince = 4,
        CountryOrRegion = 5
    }
    
    /// <summary>
    /// Represents a civic address location report.
    /// </summary>
    public class CivicAddressLocationReport : LocationReport
    {
        private ICivicAddressReport _civicAddressReport;

        public CivicAddressLocationReport()
        {

        }

        public CivicAddressLocationReport(string address1, string address2, string city, string stateProvince, string postalCode, string countryRegion)
        {
            DefaultCivicAddressReport report = new DefaultCivicAddressReport(address1, address2, city, stateProvince, postalCode, countryRegion);
            InitializeReport(report);
        }

        /// <summary>
        /// Initialized the instance.
        /// </summary>
        protected override void Initialize()
        {
            _civicAddressReport = (ICivicAddressReport)InnerObject;
        }
        
        /// <summary>
        /// Retrieves the first street address line.
        /// </summary>
        public string AddressLine1
        {
            get { return _civicAddressReport.GetAddressLine1(); }
        }

        /// <summary>
        /// Retrieves the second street address line.
        /// </summary>
        public string AddressLine2
        {
            get { return _civicAddressReport.GetAddressLine2(); }
        }

        /// <summary>
        /// Retrieves the city name.
        /// </summary>
        public string City
        {
            get { return _civicAddressReport.GetCity(); }
        }

        /// <summary>
        /// Retrieves the country or region name.
        /// </summary>
        public string CountryOrRegion
        {
            get { return _civicAddressReport.GetCountryRegion(); }
        }

        /// <summary>
        /// Retrieves the postal code.
        /// </summary>
        public string PostalCode
        {
            get { return _civicAddressReport.GetPostalCode(); }
        }
        
        /// <summary>
        /// Retrieves the state or province name.
        /// </summary>
        public string StateOrProvince
        {
            get { return _civicAddressReport.GetStateProvince(); }
        }

        /// <summary>
        /// Retrieves a value that indicates the level of detail provided by the civic address report.
        /// </summary>
        public DetailLevel DetailLevel
        {
            get { return (DetailLevel) _civicAddressReport.GetDetailLevel(); }
        }
    }
}