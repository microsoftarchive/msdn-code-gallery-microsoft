// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Windows7.Location
{
    /// <summary>
    /// Represents a civic address location provider.
    /// </summary>
    [LocationProviderDescription("C0B19F70-4ADF-445d-87F2-CAD8FD711792")]
    public class CivicAddressLocationProvider : LocationProvider
    {
        /// <summary>
        /// Constructs the location provider with the specified minimal report interval.
        /// </summary>
        /// <param name="reportInterval">Report interval in milliseconds.</param>
        public CivicAddressLocationProvider(uint minReportInterval)
            : base(minReportInterval)
        {

        }

        protected override LocationReport CreateReport()
        {
            return new CivicAddressLocationReport();
        }
    }
}