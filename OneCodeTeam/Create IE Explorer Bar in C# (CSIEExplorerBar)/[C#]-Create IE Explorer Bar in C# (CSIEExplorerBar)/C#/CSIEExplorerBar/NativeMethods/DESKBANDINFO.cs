/****************************** Module Header ******************************\
* Module Name:  DESKBANDINFO.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* Contains and receives information for a band object. This structure is used
* with the IDeskBand.GetBandInfo method.
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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DESKBANDINFO
    {
        public UInt32 dwMask;
        public POINT ptMinSize;
        public POINT ptMaxSize;
        public POINT ptIntegral;
        public POINT ptActual;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public String wszTitle;
        public DBIM dwModeFlags;
        public Int32 crBkgnd;
    }


}
