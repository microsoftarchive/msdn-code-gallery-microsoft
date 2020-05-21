// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.Location
{
    /// <summary>
    /// Represents a default latitude/longitude/altitude location provider.
    /// </summary>
    [LocationProviderDescription("7FED806D-0EF8-4F07-80AC-36A0BEAE3134")]
    public class DefaultLatLongLocationProvider : DefaultLocationProvider
    {        
        protected override LocationReport CreateReport()
        {
            return new LatLongLocationReport();
        }
    }
}