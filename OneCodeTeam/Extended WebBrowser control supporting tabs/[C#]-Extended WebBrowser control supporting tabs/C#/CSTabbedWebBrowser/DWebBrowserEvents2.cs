/****************************** Module Header ******************************\
* Module Name:  DWebBrowserEvents2.cs
* Project:	    CSTabbedWebBrowser
* Copyright (c) Microsoft Corporation.
* 
* The interface DWebBrowserEvents2 designates an event sink interface that an
* application must implement to receive event notifications from a WebBrowser 
* control or from the Windows Internet Explorer application. The event 
* notifications include NewWindow3 event that will be used in this 
* application.
* 
* To get the full event list of DWebBrowserEvents2, see
* http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace CSTabbedWebBrowser
{
    [ComImport, TypeLibType(TypeLibTypeFlags.FHidden),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    public interface DWebBrowserEvents2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ppDisp">
        /// An interface pointer that, optionally, receives the IDispatch interface
        /// pointer of a new WebBrowser object or an InternetExplorer object.
        /// </param>
        /// <param name="Cancel">
        /// value that determines whether the current navigation should be canceled
        /// </param>
        /// <param name="dwFlags">
        /// The flags from the NWMF enumeration that pertain to the new window
        /// See http://msdn.microsoft.com/en-us/library/bb762518(VS.85).aspx.
        /// </param>
        /// <param name="bstrUrlContext">
        /// The URL of the page that is opening the new window.
        /// </param>
        /// <param name="bstrUrl">The URL that is opened in the new window.</param>
        [DispId(0x111)]
        void NewWindow3(
            [In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppDisp,
            [In, Out] ref bool Cancel,
            [In] uint dwFlags,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrUrlContext,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrUrl);
    }
}
