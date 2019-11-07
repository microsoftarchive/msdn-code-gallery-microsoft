/****************************** Module Header ******************************\
* Module Name:  HtmlInputElementFactory.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This class HtmlInputElementFactory is used to get an HtmlInputElement from an 
* HtmlElement in the web page. 
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
using System.Windows.Forms;
using System.Security.Permissions;

namespace CSWebBrowserAutomation
{
    public static class HtmlInputElementFactory
    {

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static HtmlInputElement GetInputElement(HtmlElement element)
        {
            if (!element.TagName.Equals("input", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            HtmlInputElement input = null;

            string type = element.GetAttribute("type").ToLower();

            switch (type)
            {
                case "checkbox":
                    input = new HtmlCheckBox(element);
                    break;
                case "password":
                    input = new HtmlPassword(element);
                    break;
                case "submit":
                    input = new HtmlSubmit(element);
                    break;
                case "text":
                    input = new HtmlText(element);
                    break;
                default:
                    break;

            }
            return input;
        }
    }
}
