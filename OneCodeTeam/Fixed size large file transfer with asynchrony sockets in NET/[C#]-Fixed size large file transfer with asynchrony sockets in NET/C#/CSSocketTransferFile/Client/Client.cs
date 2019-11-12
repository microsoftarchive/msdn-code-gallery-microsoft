/********************************* Module Header **********************************\
* Module Name:	Client.cs
* Project:		Client
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to implement fixed size large file transfer with asynchrony sockets in NET.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* **********************************************************************************/

using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    /// <summary>
    /// Client can connect to the server.
    /// </summary>
    public partial class Client : Form
    {
        #region Constructor

        public Client()
        {
            InitializeComponent();
        }

        #endregion

        #region Button Event

        /// <summary>
        /// Connect the server with the given ip address and port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            int port;
            IPAddress ipAddress;

            if (string.IsNullOrEmpty(tbxAddress.Text) || string.IsNullOrEmpty(tbxPort.Text))
            {
                MessageBox.Show(this,Properties.Resources.IsEmptyMsg);
                return;
            }

            try
            {
                ipAddress = IPAddress.Parse(tbxAddress.Text);
            }
            catch
            {
                MessageBox.Show(this,Properties.Resources.InvalidAddressMsg);
                return;
            }

            try
            {
                port = Convert.ToInt32(tbxPort.Text);
            }
            catch
            {
                MessageBox.Show(this,Properties.Resources.InvalidPortMsg);
                return;
            }

            if (port < 0 || port > 65535)
            {
                MessageBox.Show(this, Properties.Resources.InvalidPortMsg);
                return;
            }

            if (string.IsNullOrEmpty(tbxSavePath.Text))
            {
                MessageBox.Show(this, Properties.Resources.EmptyPath);
                return;
            }

            AsynchronousClient.IpAddress = ipAddress;
            AsynchronousClient.Port = port;
            AsynchronousClient.FileSavePath = tbxSavePath.Text;
            AsynchronousClient.Client = this;

            Thread threadClient= new Thread(new ThreadStart(AsynchronousClient.StartClient));
            threadClient.IsBackground = true;
            threadClient.Start();
            btnConnect.Enabled = false;
        }

        /// <summary>
        /// Set the path to store the file sent from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSavePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.tbxSavePath.Text = path.SelectedPath;
        }

        #endregion

        #region Change the progressBar

        /// <summary>
        /// Set the progress length of the progressBar
        /// </summary>
        /// <param name="len"></param>
        public void SetProgressLength(int len)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = len;
            progressBar.Value = 0;
            progressBar.Step = 1;
        }

        /// <summary>
        /// Change the position of the progressBar
        /// </summary>
        public void ProgressChanged()
        {
            progressBar.PerformStep();
        }

        #endregion

        #region Functions

        /// <summary>
        /// Notify the user when receive the file completely.
        /// </summary>
        public void FileReceiveDone()
        {
            MessageBox.Show(this, Properties.Resources.FileReceivedDoneMsg);
        }

        /// <summary>
        /// Notify the user when connect to the server successfully.
        /// </summary>
        public void ConnectDone()
        {
            MessageBox.Show(this,Properties.Resources.ConnectionMsg);
        }

        /// <summary>
        /// Enable the Connect button if failed to connect the sever. 
        /// </summary>
        public void EnableConnectButton()
        {
            btnConnect.Enabled = true;
        }

        #endregion
    }
}
