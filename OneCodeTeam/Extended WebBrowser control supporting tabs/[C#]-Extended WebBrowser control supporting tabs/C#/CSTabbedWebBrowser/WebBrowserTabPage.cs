/****************************** Module Header ******************************\
* Module Name:  WebBrowserTabPage.cs
* Project:	    CSTabbedWebBrowser
* Copyright (c) Microsoft Corporation.
* 
* This class inherits the the System.Windows.Forms.TabPage class and contains
* a WebBrowserEx property. An instance of this class could be add to a tab control
* directly.
* 
* It exposes the NewWindow3 event of WebBrowserEx, and handle the DocumentTitleChanged
* event.
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

namespace CSTabbedWebBrowser
{
    public class WebBrowserTabPage : TabPage
    {
        public WebBrowserEx WebBrowser { get; private set; }

        // Expose the NewWindow3 event of WebBrowserEx.
        public event EventHandler<WebBrowserNewWindowEventArgs> NewWindow
        {
            add
            {
                WebBrowser.NewWindow3 += value;
            }
            remove
            {
                WebBrowser.NewWindow3 -= value;
            }
        }

        /// <summary>
        /// Initialize the WebBrowserEx instance.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserTabPage()
            : base()
        {
            WebBrowser = new WebBrowserEx();
            WebBrowser.Dock = DockStyle.Fill;
            WebBrowser.DocumentTitleChanged += new EventHandler(WebBrowser_DocumentTitleChanged);

            this.Controls.Add(WebBrowser);
        }

        /// <summary>
        /// Change the title of the tab page.
        /// </summary>
        void WebBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Text = WebBrowser.DocumentTitle;
        }

    }
}
