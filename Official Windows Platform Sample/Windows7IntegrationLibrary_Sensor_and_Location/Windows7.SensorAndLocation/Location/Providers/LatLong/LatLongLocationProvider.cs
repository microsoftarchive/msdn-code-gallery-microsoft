// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Location;


namespace Windows7.Location
{
    /// <summary>
    /// Represents a latitude/longitude/altitude location provider.
    /// </summary>
    [LocationProviderDescription("7FED806D-0EF8-4F07-80AC-36A0BEAE3134")]
    public class LatLongLocationProvider : LocationProvider
    {
        /// <summary>
        /// Constructs the location provider with the specified minimal report interval.
        /// </summary>
        /// <param name="reportInterval">Report interval in milliseconds.</param>
        public LatLongLocationProvider(uint minReportInterval)
            : base(minReportInterval)
        {

        }
        
        protected override LocationReport CreateReport()
        {
            return new LatLongLocationReport();
        }
    }
}