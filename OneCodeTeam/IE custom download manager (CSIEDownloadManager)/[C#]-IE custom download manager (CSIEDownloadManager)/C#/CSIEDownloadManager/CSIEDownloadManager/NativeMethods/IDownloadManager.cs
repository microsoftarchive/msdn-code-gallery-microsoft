/****************************** Module Header ******************************\
 * Module Name:  IDownloadManager.cs
 * Project:      CSIEDownloadManager
 * Copyright (c) Microsoft Corporation.
 * 
 * Provides access to the method of a custom download manager object that Windows
 * Internet Explorer and WebBrowser applications use to download a file. See
 * http://msdn.microsoft.com/en-us/library/aa753613(VS.85).aspx
 * 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace CSIEDownloadManager.NativeMethods
{
    [ComVisible(false), ComImport]
    [Guid("988934A4-064B-11D3-BB80-00104B35E7F9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDownloadManager
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Download(
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pmk,
            [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc,
            [In, MarshalAs(UnmanagedType.U4)] UInt32 dwBindVerb,
            [In] int grfBINDF,
            [In] IntPtr pBindInfo,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszHeaders,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszRedir,
            [In, MarshalAs(UnmanagedType.U4)] uint uiCP);
    }
}
