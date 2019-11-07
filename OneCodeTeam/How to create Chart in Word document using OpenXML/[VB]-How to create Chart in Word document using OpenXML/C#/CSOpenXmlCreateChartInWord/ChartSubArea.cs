/****************************** Module Header ******************************\
* Module Name:  ChartSubArea.cs
* Project:      CSOpenXmlCreateChartInWord
* Copyright(c)  Microsoft Corporation.
* 
* The Class is an Entity class.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using DocumentFormat.OpenXml.Drawing;
using System;

namespace CSOpenXmlCreateChartInWord
{
    public class ChartSubArea
    {
        public SchemeColorValues Color { get; set; }
        public String Label { get; set; }
        public string Value { get; set; }
    }
}
