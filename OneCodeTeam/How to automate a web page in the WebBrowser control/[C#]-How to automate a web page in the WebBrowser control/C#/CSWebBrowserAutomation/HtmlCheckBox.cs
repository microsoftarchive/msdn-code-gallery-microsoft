/****************************** Module Header ******************************\
* Module Name:  HtmlCheckBox.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This class HtmlCheckBox represents an HtmlElement with the tag "input" and its 
* type is "checkbox".
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
    public class HtmlCheckBox : HtmlInputElement
    {
        public bool Checked { get; set; }

        /// <summary>
        /// This parameterless constructor is used in deserialization.
        /// </summary>
        public HtmlCheckBox() { }

        /// <summary>
        /// Initialize an instance of HtmlCheckBox. This constructor is used by 
        /// HtmlInputElementFactory.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HtmlCheckBox(HtmlElement element)
            : base(element.Id)
        {

            // The checkbox is checked if it has the attribute "checked".
            string chekced = element.GetAttribute("checked");
            Checked = !string.IsNullOrEmpty(chekced);
        }

        /// <summary>
        /// Set the value of the HtmlElement.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void SetValue(HtmlElement element)
        {
            // The checkbox is checked if it has the attribute "checked".
            if (Checked)
            {
                element.SetAttribute("checked", "checked");
            }
            else
            {
                element.SetAttribute("checked", null);
            }
        }
    }
}
