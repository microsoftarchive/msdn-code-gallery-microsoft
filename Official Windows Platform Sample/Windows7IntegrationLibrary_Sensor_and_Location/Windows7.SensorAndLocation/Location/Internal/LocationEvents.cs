// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using HRESULT = System.Int32;

namespace Windows7.Location.Internal
{
    /// <summary>
    /// COM interface for location events.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("CAE02BBF-798B-4508-A207-35A7906DC73D")]
    internal interface ILocationEvents
    {
        [PreserveSig]
        HRESULT OnLocationChanged([In] ref Guid reportType, [In] ILocationReport pLocationReport);

        [PreserveSig]
        HRESULT OnStatusChanged([In] ref Guid reportType, [In] ReportStatus newStatus);
    }

}