// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DWORD = System.UInt32;
using HRESULT = System.Int32;

namespace Windows7.Location.Internal
{
    /// <summary>
    /// COM interface for getting/registering/querying status of reports.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("AB2ECE69-56D9-4F28-B525-DE1B0EE44237")]
    internal interface ILocation
    {
        HRESULT RegisterForReport([In, MarshalAs(UnmanagedType.Interface)] ILocationEvents pEvents, [In] ref Guid reportType, [In] uint dwMinReportInterval);

        HRESULT UnregisterForReport([In] ref Guid reportType);

        HRESULT GetReport([In] ref Guid reportType, [MarshalAs(UnmanagedType.Interface)] out ILocationReport locationReport);

        [PreserveSig]
        HRESULT GetReportStatus([In] ref Guid reportType, out ReportStatus pStatus);

        HRESULT GetReportInterval([In] ref Guid reportType, out DWORD reportInterval);

        HRESULT SetReportInterval([In] ref Guid reportType, [In] DWORD milliseconds);

        HRESULT GetDesiredAccuracy([In] ref Guid reportType, out DesiredAccuracy desiredAccuracy);

        HRESULT SetDesiredAccuracy([In] ref Guid reportType, [In] DesiredAccuracy desiredAccuracy);
        [PreserveSig]
        HRESULT RequestPermissions([In] IntPtr hParent, [In] Guid[] pReportTypes, [In] uint count, [In] int fModal);
    }

    /// <summary>
    /// COM wrapper for getting/registering/querying status of reports.
    /// </summary>
    [ComImport, GuidAttribute("E5B8E079-EE6D-4E33-A438-C87F2E959254"), ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class COMLocation : ILocation
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT RegisterForReport([In, MarshalAs(UnmanagedType.Interface)] ILocationEvents pEvents, [In] ref Guid reportType, [In] uint dwMinReportInterval);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT UnregisterForReport([In] ref Guid reportType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT GetReport([In] ref Guid reportType, [MarshalAs(UnmanagedType.Interface)] out ILocationReport locationReport);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT GetReportStatus([In] ref Guid reportType, out ReportStatus pStatus);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT GetReportInterval([In] ref Guid reportType, out DWORD reportInterval);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT SetReportInterval([In] ref Guid reportType, [In] DWORD milliseconds);
		
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT GetDesiredAccuracy([In] ref Guid reportType, out DesiredAccuracy pStatus);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT SetDesiredAccuracy([In] ref Guid reportType, [In] DesiredAccuracy pStatus);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT RequestPermissions([In] IntPtr hParent, [In] Guid[] pReportTypes, [In] uint count, [In] int fModal);
    }
}