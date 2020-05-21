// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows7.Sensors;
using Windows7.Sensors.Internal;


namespace Windows7.Location.Internal
{
    /// <summary>
    /// COM interface for the base report.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C8B7F7EE-75D0-4DB9-B62D-7A0F369CA456")]
    internal interface ILocationReport
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Guid GetSensorID();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        SYSTEMTIME GetTimestamp();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetValue([In] ref PropertyKey pKey, out PROPVARIANT pValue);
    }

    /// <summary>
    /// COM interface for the latitude/longitude/altitude report.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7FED806D-0EF8-4F07-80AC-36A0BEAE3134")]
    internal interface ILatLongReport : ILocationReport
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new Guid GetSensorID();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new SYSTEMTIME GetTimestamp();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetValue([In] ref PropertyKey pKey, out PROPVARIANT pValue);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        double GetLatitude();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        double GetLongitude();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        double GetErrorRadius();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        double GetAltitude();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        double GetAltitudeError();
    }

    /// <summary>
    /// COM interface for the civic address report.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C0B19F70-4ADF-445D-87F2-CAD8FD711792")]
    internal interface ICivicAddressReport : ILocationReport
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new Guid GetSensorID();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new SYSTEMTIME GetTimestamp();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetValue([In] ref PropertyKey pKey, out PROPVARIANT pValue);

        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetAddressLine1();

        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetAddressLine2();

        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetCity();

        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetStateProvince();

        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetPostalCode();

        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetCountryRegion();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetDetailLevel();
    }
}