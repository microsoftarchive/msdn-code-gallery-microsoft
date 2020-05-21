// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.Location
{
    /// <summary>
    /// Represents a default civic address location provider.
    /// </summary>
    [LocationProviderDescription("C0B19F70-4ADF-445d-87F2-CAD8FD711792")]
    public class DefaultCivicAddressLocationProvider : DefaultLocationProvider
    {
        protected override LocationReport CreateReport()
        {
            return new CivicAddressLocationReport();
        }
    }
}