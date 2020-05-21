// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Windows7.Location
{
    /// <summary>
    /// An attribute which is applied on <see cref="LocationProvider"/>-derived types. Provides essential metadata
    /// such as the report type GUID for which the location provider was written.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LocationProviderDescriptionAttribute : Attribute
    {
        private string _reportTypeGuid;

        /// <summary>
        /// Constructs the attribue with a string represening the report type GUID and the type of the location report class.
        /// </summary>
        /// <param name="reportTypeGuid">String representing the report GUID.</param>
        public LocationProviderDescriptionAttribute(string reportTypeGuid)
        {
            _reportTypeGuid = reportTypeGuid;
        }

        /// <summary>
        /// Gets or sets a string representing the location report type GUID.
        /// </summary>
        public string SensorType
        {
            get { return _reportTypeGuid; }
            set { _reportTypeGuid = value; }
        }

        /// <summary>
        /// Gets the GUID of the location report.
        /// </summary>
        public Guid ReportTypeGuid
        {
            get { return new Guid(_reportTypeGuid); }
        }
    }
}