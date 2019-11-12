/****************************** Module Header ******************************\
* Module Name:  IObjectWithSite.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* Provides simple objects with a lightweight siting mechanism (lighter than 
* IOleObject). A BHO must implement this interface.
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

namespace CSIEExplorerBar.NativeMethods
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
    public interface IObjectWithSite
    {
        void SetSite([In, MarshalAs(UnmanagedType.IUnknown)] Object pUnkSite);

        void GetSite(
            ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out Object ppvSite);
    }
}
