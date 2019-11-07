/****************************** Module Header ******************************\
* Module Name:  WebBrowserEx.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This WebBrowserEx class inherits WebBrowser class. It supplies following features:
* 1. Block the specified web sites.
* 2. Complete the html input elements automatically.
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
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.ComponentModel;

namespace CSWebBrowserAutomation
{
    public partial class WebBrowserEx : WebBrowser
    {

        /// <summary>
        /// Specify whether the current page could be completed automatically.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool CanAutoComplete { get; private set; }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserEx()
        {
        }

        /// <summary>
        /// After the docunment is loaded, check whether the page could be completed
        /// automatically.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {

            // Check whether the current page has been stored.
            StoredSite form = StoredSite.GetStoredSite(this.Url.Host);
            CanAutoComplete = form != null
                && form.Urls.Contains(this.Url.AbsolutePath.ToLower());

            base.OnDocumentCompleted(e);
        }

        /// <summary>
        /// Complete the page automatically
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void AutoComplete()
        {
            if (CanAutoComplete)
            {
                StoredSite form = StoredSite.GetStoredSite(this.Url.Host);
                form.FillWebPage(this.Document, true);
            }
        }

        /// <summary>
        /// Check whether the url is included in the block list.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (BlockSites.Instance.Hosts.Contains(e.Url.Host.ToLower()))
            {
                e.Cancel = true;
                this.Navigate(string.Format(@"{0}\Resources\Block.htm",
                    Environment.CurrentDirectory));
            }
            else
            {
                base.OnNavigating(e);
            }
        }

    }
}
