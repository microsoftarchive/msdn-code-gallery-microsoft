/****************************** Module Header ******************************\
* Module Name:  HtmlInputElement.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This abstract class HtmlInputElement represents an HtmlElement with the tag "input".
* 
* The XmlIncludeAttribute allows the XmlSerializer to recognize the classes which 
* inherit HtmlInputElement when it serializes or deserializes an object.
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
using System.Xml.Serialization;

namespace CSWebBrowserAutomation
{

    [XmlInclude(typeof(HtmlCheckBox)),
    XmlInclude(typeof(HtmlPassword)),
    XmlInclude(typeof(HtmlSubmit)),
    XmlInclude(typeof(HtmlText))]
    public abstract class HtmlInputElement
    {
        public string ID { get; set; }

        /// <summary>
        /// This parameterless constructor is used in deserialization.
        /// </summary>
        protected HtmlInputElement() { }

        protected HtmlInputElement(string ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Set the value of the HtmlElement.
        /// </summary>
        public virtual void SetValue(HtmlElement element) { }
    }
}
