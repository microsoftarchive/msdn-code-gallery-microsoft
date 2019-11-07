/********************************* Module Header **********************************\
* Module Name:	Server.cs
* Project:		Server
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
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    /// <summary>
    /// The server used to listen the client connection and send file to them. 
    /// </summary>
    public partial class Server : Form
    {
        private static bool HasStartup = false;

        #region Constructor

        public Server()
        {
            InitializeComponent();
        }

        #endregion

        #region Button Event

        private void btnStartup_Click(object sender, EventArgs e)
        {
            if (!HasStartup)
            {
                try
                {
                    AsynchronousSocketListener.Port = tbxPort.Text;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this,ex.Message);
                    return;
                }
                AsynchronousSocketListener.Server = this;
                Thread listener = new Thread(new ThreadStart(AsynchronousSocketListener.StartListening));
                listener.IsBackground = true;
                listener.Start();
                HasStartup = true;
            }

            MessageBox.Show(this,Properties.Resources.StartupMsg);
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbxFile.Text = openFileDialog.FileName;
                AsynchronousSocketListener.FileToSend = tbxFile.Text;
            }
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(lbxServer.SelectedItems.Count == 0)
            {
                MessageBox.Show(this,Properties.Resources.SelectClientMsg);
                return;
            }

            if (string.IsNullOrEmpty(tbxFile.Text))
            {
                MessageBox.Show(this,Properties.Resources.EmptyFilePathMsg);
                return;
            }

            if(AsynchronousSocketListener.Clients.Count == 0)
            {
                MessageBox.Show(this,Properties.Resources.ConnectionMsg);
                return;
            }

            foreach (object item in lbxServer.SelectedItems)
            {
                foreach (Socket handler in AsynchronousSocketListener.Clients)
                {
                    IPEndPoint ipEndPoint = (IPEndPoint)handler.RemoteEndPoint;
                    string address = ipEndPoint.ToString();
                    if (string.Equals(item.ToString(), address, StringComparison.OrdinalIgnoreCase))
                    {
                        AsynchronousSocketListener.ClientsToSend.Add(handler,ipEndPoint);
                        break;
                    }
                }
            }

            Thread sendThread = new Thread(new ThreadStart(AsynchronousSocketListener.Send));
            sendThread.IsBackground = true;
            sendThread.Start();
            btnSend.Enabled = false;

        }

        #endregion

        #region ListBox item change functions

        /// <summary>
        /// Add the client address to lbxClient.
        /// </summary>
        /// <param name="IpEndPoint"></param>
        public void AddClient(IPEndPoint IpEndPoint)
        {
            lbxServer.BeginUpdate();
            lbxServer.Items.Add(IpEndPoint.ToString());
            lbxServer.EndUpdate();
        }

        /// <summary>
        /// Clear the content of lbxClient.
        /// </summary>
        public void CompleteSend()
        {
            while (lbxServer.SelectedIndices.Count > 0)
            {
                lbxServer.Items.RemoveAt(lbxServer.SelectedIndices[0]);
            }
            btnSend.Enabled = true;
        }

        /// <summary>
        /// Remove the client address which disconnect from the server.
        /// </summary>
        /// <param name="ipAddress"></param>
        public void RemoveItem(string ipAddress)
        {
            int index = 0;
            bool flag = false;

            foreach (object item in lbxServer.SelectedItems)
            {
                if (!string.Equals(item.ToString(), ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
                else
                {
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                lbxServer.Items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Enable the Send button.
        /// </summary>
        public void EnableSendButton()
        {
            btnSend.Enabled = true;
        }

        #endregion
    }
}
