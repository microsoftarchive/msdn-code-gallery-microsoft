/****************************** Module Header ******************************\
* Module Name:  IInputObjectSite.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* Exposes a method that is used to communicate focus changes for a user input 
* object contained in the Shell.
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
    [Guid("f1db8392-7331-11d0-8c99-00a0c92dbfe8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInputObjectSite
    {
        [PreserveSig]
        int OnFocusChangeIS([MarshalAs(UnmanagedType.IUnknown)] object punkObj, 
            int fSetFocus);
    }

 

 

}
