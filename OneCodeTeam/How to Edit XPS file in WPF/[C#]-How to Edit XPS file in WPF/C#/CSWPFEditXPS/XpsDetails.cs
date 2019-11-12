/****************************** Module Header ******************************\
Module Name:  XpsDetails.cs
Project:      EditXPS
Copyright (c) Microsoft Corporation.

XpsDetails is a calss structure to maintain the XPS resoruces in list. 
This helps us to copy the XPS resource and content from the source in 
a structured way.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Collections.Generic;
using System.Windows.Xps.Packaging;

namespace EditXPS
{
    /// <summary>
    /// Class to represent the basic properties we use from XPS files.
    /// </summary>
    public class XpsDetails
    {
        public XpsResource resource { get; set; }
        public Uri sourceURI { get; set; }
        public Uri destURI { get; set; }
    }

}
