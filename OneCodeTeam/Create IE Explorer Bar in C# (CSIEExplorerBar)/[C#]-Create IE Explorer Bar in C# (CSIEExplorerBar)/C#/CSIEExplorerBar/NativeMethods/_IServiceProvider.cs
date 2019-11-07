/****************************** Module Header ******************************\
* Module Name:  _IServiceProvider.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* Provides a generic access mechanism to locate a GUID-identified service. 
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
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    internal interface _IServiceProvider
    {
        /// <summary>
        /// Acts as the factory method for any services exposed through an 
        /// implementation of IServiceProvider.
        /// </summary>
        void QueryService(
            ref Guid guid,
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out object Obj);
    }

 

 

}
