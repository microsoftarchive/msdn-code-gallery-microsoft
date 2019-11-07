/****************************** Module Header ******************************\
* Module Name:  StoredSite.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This class represents a site whose html elements are stored. A site is stored 
* as an XML file under StoredSites folder, and can be deserialized.
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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Permissions;

namespace CSWebBrowserAutomation
{
    public class StoredSite
    {
        /// <summary>
        /// The host of the site.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The urls that can be completed automatically in the site .
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// The html input elements that can be completed automatically.
        /// </summary>
        public List<HtmlInputElement> InputElements { get; set; }

        public StoredSite()
        {
            InputElements = new List<HtmlInputElement>();
        }

        /// <summary>
        /// Save the instance as an XML file.
        /// </summary>
        public void Save()
        {
            string folderPath = string.Format(@"{0}\StoredSites\",
                Environment.CurrentDirectory);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filepath = string.Format(@"{0}\{1}.xml", folderPath, this.Host);

            XMLSerialization<StoredSite>.SerializeFromObjectToXML(this, filepath);
        }

        /// <summary>
        /// Complete the web page automatically.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void FillWebPage(HtmlDocument document, bool submit)
        {
            // The submit button in the page.
            HtmlElement inputSubmit = null;

            // Set the value of html elements that are stored. If the element is
            // a submit button, then assign to inputSubmit.
            foreach (HtmlInputElement input in this.InputElements)
            {
                HtmlElement element = document.GetElementById(input.ID);
                if (element == null)
                {
                    continue;
                }
                if (input is HtmlSubmit)
                {
                    inputSubmit = element;
                }
                else
                {
                    input.SetValue(element);
                }
            }

            // Click the submit button automatically.
            if (submit && inputSubmit != null)
            {
                inputSubmit.InvokeMember("click");
            }
        }

        /// <summary>
        /// Get the stored site by the host name.
        /// </summary>
        public static StoredSite GetStoredSite(string host)
        {
            StoredSite storedForm = null;

            string folderPath = string.Format(@"{0}\StoredSites\",
                Environment.CurrentDirectory);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filepath = string.Format(@"{0}\{1}.xml", folderPath, host);
            if (File.Exists(filepath))
            {
                storedForm =
                    XMLSerialization<StoredSite>.DeserializeFromXMLToObject(filepath);
            }
            return storedForm;
        }
    }
}
