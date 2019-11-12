/****************************** Module Header ******************************\
* Module Name:  UICredentialsProvider.cs
* Project:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* The Form UICredentialsProvider contains 3 textboxes that accept UserName, 
* Password and Domain to initialize a NetworkCredential instance.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Net;
using System.Windows.Forms;

namespace CSFTPUpload
{
    public partial class UICredentialsProvider : Form
    {
        public NetworkCredential Credentials { get; set; }

        bool useOriginalCredentials = true;

        public UICredentialsProvider()
            : this(null)
        { }

        public UICredentialsProvider(NetworkCredential credentials)
        {

            InitializeComponent();

            this.Credentials = credentials;

            if (this.Credentials != null)
            {
                this.tbUserName.Text = this.Credentials.UserName;
                this.tbDomain.Text = this.Credentials.Domain;
                this.tbPassword.Text = this.Credentials.Password;
                useOriginalCredentials = true;
            }
            else
            {
                useOriginalCredentials = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!useOriginalCredentials)
            {
                if (chkAnonymous.Checked)
                {

                    // Use Anonymous Credentials by default.
                    Credentials = new NetworkCredential("Anonymous", "");
                }
                else if (String.IsNullOrWhiteSpace(tbUserName.Text)
                        || String.IsNullOrWhiteSpace(tbPassword.Text))
                {
                    MessageBox.Show("Please type the user name and password!");
                    return;
                }
                else
                {
                    Credentials = new NetworkCredential(
                        tbUserName.Text.Trim(),
                        tbPassword.Text,
                        tbDomain.Text.Trim());
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void chkAnonymous_CheckedChanged(object sender, EventArgs e)
        {
            tbDomain.Enabled = !chkAnonymous.Checked;
            tbPassword.Enabled = !chkAnonymous.Checked;
            tbUserName.Enabled = !chkAnonymous.Checked;

            useOriginalCredentials = false;
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            useOriginalCredentials = false;
        }
    }
}
