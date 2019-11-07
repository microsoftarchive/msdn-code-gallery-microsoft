/****************************** Module Header ******************************\
* Module Name:  HtmlSubmit.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This class HtmlSubmit represents an HtmlElement with the tag "input" and its 
* type is "submit".
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Windows.Forms;
using System.Security.Permissions;

namespace CSWebBrowserAutomation
{
    public class HtmlSubmit : HtmlInputElement
    {

        /// <summary>
        /// This parameterless constructor is used in deserialization.
        /// </summary>
        public HtmlSubmit() { }

        /// <summary>
        /// Initialize an instance of HtmlSubmit. This constructor is used by 
        /// HtmlInputElementFactory.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HtmlSubmit(HtmlElement element)
            : base(element.Id)
        {
        }

    }
}
