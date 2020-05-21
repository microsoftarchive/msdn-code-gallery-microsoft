// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HRESULT = System.Int32;

namespace Windows7.Location.Internal
{
    /// <summary>
    /// Specifies the default location.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("A65AF77E-969A-4A2E-8ACA-33BB7CBB1235")]
    internal interface IDefaultLocation
    {
        HRESULT SetReport([In] ref Guid reportType, [In, MarshalAs(UnmanagedType.Interface)] ILocationReport pLocationReport);

        ILocationReport GetReport([In] ref Guid reportType);
    }

    /// <summary>
    /// COM wrapper for getting and setting the default location.
    /// </summary>
    [ComImport, GuidAttribute("8B7FBFE0-5CD7-494A-AF8C-283A65707506"), ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class COMDefaultLocation : IDefaultLocation
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern ILocationReport GetReport([In] ref Guid reportType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT SetReport([In] ref Guid reportType, [In, MarshalAs(UnmanagedType.Interface)] ILocationReport pLocationReport);
    }
}