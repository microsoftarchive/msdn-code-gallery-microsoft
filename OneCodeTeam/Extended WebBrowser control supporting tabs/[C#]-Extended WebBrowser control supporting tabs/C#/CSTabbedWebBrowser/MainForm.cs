/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSTabbedWebBrowser
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

namespace CSTabbedWebBrowser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Initialize the checkbox.
            chkEnableTab.Checked = TabbedWebBrowserContainer.IsTabEnabled;

            chkEnableTab.CheckedChanged += new EventHandler(chkEnableTab_CheckedChanged);

        }

        /// <summary>
        /// Handle the KeyDown event of tbUrl. When the key is Enter, then navigate
        /// to the url in the tbUrl.
        /// </summary>
        private void tbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                webBrowserContainer.Navigate(tbUrl.Text);
            }
        }

        /// <summary>
        /// Handle the event when Back button is clicked.
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            webBrowserContainer.GoBack();
        }

        /// <summary>
        /// Handle the event when Forward button is clicked.
        /// </summary>
        private void btnForward_Click(object sender, EventArgs e)
        {
            webBrowserContainer.GoForward();
        }

        /// <summary>
        /// Handle the event when Refresh button is clicked.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            webBrowserContainer.RefreshWebBrowser();
        }

        /// <summary>
        /// Handle the event when New Tab button is clicked.
        /// </summary>
        private void btnNewTab_Click(object sender, EventArgs e)
        {
            webBrowserContainer.NewTab("about:blank");
        }

        /// <summary>
        /// Handle the event when Close Tab button is clicked.
        /// </summary>
        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            webBrowserContainer.CloseActiveTab();
        }

        /// <summary>
        /// Handle the CheckedChanged event of chkEnableTab.
        /// </summary>
        private void chkEnableTab_CheckedChanged(object sender, EventArgs e)
        {
            TabbedWebBrowserContainer.IsTabEnabled = chkEnableTab.Checked;
            MessageBox.Show("The context menu \"Open in new tab\" will take effect"
                + " after the application is restarted.");
            Application.Restart();
        }

    }
}
