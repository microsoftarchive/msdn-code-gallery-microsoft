/****************************** Module Header ******************************\
* Module Name:  IDockingWindow.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* Exposes methods that notify the docking window object of changes, including
* showing, hiding, and impending removal. This interface is implemented by 
* window objects that can be docked within the border space of a Windows 
* Explorer window.
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
    [Guid("012dd920-7b26-11d0-8ca9-00a0c92dbfe8")]
    public interface IDockingWindow
    {
        void GetWindow(out IntPtr phwnd);

        void ContextSensitiveHelp([In] bool fEnterMode);

        void ShowDW([In] bool fShow);

        void CloseDW([In] uint dwReserved);

        void ResizeBorderDW(
            IntPtr prcBorder, 
            [In, MarshalAs(UnmanagedType.IUnknown)] object punkToolbarSite,
            bool fReserved);


    }



}
