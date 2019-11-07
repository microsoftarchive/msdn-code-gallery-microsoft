/****************************** Module Header ******************************\
* Module Name:  IDeskBand.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* Gets information about a band object.  
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

namespace CSIEExplorerBar.NativeMethods
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EB0FE172-1A3A-11D0-89B3-00A0C90A90AC")]
    public interface IDeskBand
    {
        void GetWindow(out IntPtr phwnd);

        void ContextSensitiveHelp([In] bool fEnterMode);

        void ShowDW([In] bool fShow);

        void CloseDW([In] uint dwReserved);

        void ResizeBorderDW(IntPtr prcBorder, 
            [In, MarshalAs(UnmanagedType.IUnknown)] object punkToolbarSite,
            bool fReserved);

        void GetBandInfo(uint dwBandID, uint dwViewMode, ref DESKBANDINFO pdbi);
    }
}
