/****************************** Module Header ******************************\
* Module Name:  HtmlPassword.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This class HtmlPassword represents an HtmlElement with the tag "input" and its 
* type is "password".
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
    public class HtmlPassword : HtmlInputElement
    {
        public string Value { get; set; }

        /// <summary>
        /// This parameterless constructor is used in deserialization.
        /// </summary>
        public HtmlPassword() { }

        /// <summary>
        /// Initialize an instance of HtmlPassword. This constructor is used by 
        /// HtmlInputElementFactory.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HtmlPassword(HtmlElement element)
            : base(element.Id)
        {
            Value = element.GetAttribute("value");
        }

        /// <summary>
        /// Set the value of the HtmlElement.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void SetValue(HtmlElement element)
        {
            element.SetAttribute("value", Value);
        }
    }
}
