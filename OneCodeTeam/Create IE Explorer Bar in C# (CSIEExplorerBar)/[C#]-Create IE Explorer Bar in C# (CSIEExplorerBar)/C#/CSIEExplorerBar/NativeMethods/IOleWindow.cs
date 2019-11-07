/****************************** Module Header ******************************\
* Module Name:  IOleWindow.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* The IOleWindow interface provides methods that allow an application to 
* obtain the handle to the various windows that participate in in-place 
* activation, and also to enter and exit context-sensitive help mode. 
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
    [Guid("00000114-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleWindow
    {
        void GetWindow(out IntPtr phwnd);

        void ContextSensitiveHelp([In] bool fEnterMode);

    }
}
