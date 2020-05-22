// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;


namespace Windows7.Location
{
    /// <summary>
    /// Represents the state of the report.
    /// </summary>
    public enum ReportStatus
    {
        NotSupported = 0,
        Error = 1,
        AccessDenied = 2,
        Initializing = 3,
        Running = 4
    }
}