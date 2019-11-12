/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This is the main form of this application. It is used to initialize the UI and 
* handle the events.
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
    public partial class MainForm : Form
    {

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public MainForm()
        {
            InitializeComponent();

            // Register the events.
            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_Navigating);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
        }

        /// <summary>
        /// Disable the btnAutoComplete when webBrowser is navigating.
        /// </summary>
        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            btnAutoComplete.Enabled = false;
        }

        /// <summary>
        /// Refresh the UI after the web page is loaded.
        /// </summary>
        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            btnAutoComplete.Enabled = webBrowser.CanAutoComplete;
            tbUrl.Text = e.Url.ToString();
        }

        /// <summary>
        /// Handle the Click event of btnAutoComplete
        /// </summary>
        private void btnAutoComplete_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser.AutoComplete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Handle the Click event of btnGo
        /// </summary>
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser.Navigate(tbUrl.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
