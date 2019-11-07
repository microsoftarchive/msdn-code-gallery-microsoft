/********************************* Module Header **********************************\
* Module Name:	AsynchronousClient.cs
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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    /// <summary>
    /// AsynchronousClient is used to asynchronously receive the file from server.
    /// </summary>
    public static class AsynchronousClient
    {
        #region Members

        private static string fileName;
        private static string fileSavePath = "C:/";
        private static long fileLen;

        private static AutoResetEvent connectDone = new AutoResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static bool connected = false;

        private delegate void ProgressChangeHandler();
        private delegate void FileReceiveDoneHandler();
        private delegate void ConnectDoneHandler();
        private delegate void EnableConnectButtonHandler();
        private delegate void SetProgressLengthHandler(int len);

        #endregion

        #region Properties

        public static Client Client { get; set; }
        public static IPAddress IpAddress { get; set; }
        public static int Port { get; set; }
        public static string FileSavePath
        {
            get
            {
                return fileSavePath;
            }
            set
            {
                fileSavePath = value.Replace("\\", "/");
            }
        }

        #endregion

        #region Functions 

        /// <summary>
        /// Start connect to the server.
        /// </summary>
        public static void StartClient()
        {
            connected = false;
            if (IpAddress == null)
            {
                MessageBox.Show(Properties.Resources.InvalidAddressMsg);
                return;
            }

            IPEndPoint remoteEP = new IPEndPoint(IpAddress, Port);

            // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Begin to connect the server.
            clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
            connectDone.WaitOne();

            if (connected)
            {
                // Begin to receive the file after connecting to server successfully.
                Receive(clientSocket);
                receiveDone.WaitOne();

                // Notify the user whether receive the file completely.
                Client.BeginInvoke(new FileReceiveDoneHandler(Client.FileReceiveDone));

                // Close the socket.
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            else
            {
                Thread.CurrentThread.Abort();
            }
        }

        /// <summary>
        /// Callback when the client connect to the server successfully.
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;

                clientSocket.EndConnect(ar);
            }
            catch
            {
                MessageBox.Show(Properties.Resources.InvalidConnectionMsg);
                Client.BeginInvoke(new EnableConnectButtonHandler(Client.EnableConnectButton));
                connectDone.Set();
                return;
            }

            Client.BeginInvoke(new ConnectDoneHandler(Client.ConnectDone));
            connected = true;
            connectDone.Set();
        }

        /// <summary>
        /// Receive the file information send by the server.
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void ReceiveFileInfo(Socket clientSocket)
        {
            // Get the filename length from the server.
            byte[] fileNameLenByte = new byte[4];
            try
            {
                clientSocket.Receive(fileNameLenByte);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }
            int fileNameLen = BitConverter.ToInt32(fileNameLenByte, 0);

            // Get the filename from the server.
            byte[] fileNameByte = new byte[fileNameLen];

            try
            {
                clientSocket.Receive(fileNameByte);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }

            fileName = Encoding.ASCII.GetString(fileNameByte, 0, fileNameLen);

            fileSavePath = fileSavePath + "/" + fileName;

            // Get the file length from the server.
            byte[] fileLenByte = new byte[8];
            clientSocket.Receive(fileLenByte);
            fileLen = BitConverter.ToInt64(fileLenByte, 0);
        }

        /// <summary>
        /// Receive the file send by the server.
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void Receive(Socket clientSocket)
        {
            StateObject state = new StateObject();
            state.WorkSocket = clientSocket;

            ReceiveFileInfo(clientSocket);

            int progressLen = checked((int)(fileLen / StateObject.BufferSize + 1));
            object[] length = new object[1];
            length[0] = progressLen;
            Client.BeginInvoke(new SetProgressLengthHandler(Client.SetProgressLength), length);

            // Begin to receive the file from the server.
            try
            {
                clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }
        }

        /// <summary>
        /// Callback when receive a file chunk from the server successfully.
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket clientSocket = state.WorkSocket;
            BinaryWriter writer;

            int bytesRead = clientSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                //If the file doesn't exist, create a file with the filename got from server. If the file exists, append to the file.
                if (!File.Exists(fileSavePath))
                {
                    writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Create));
                }
                else
                {
                    writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Append));
                }

                writer.Write(state.Buffer, 0, bytesRead);
                writer.Flush();
                writer.Close();

                // Notify the progressBar to change the position.
                Client.BeginInvoke(new ProgressChangeHandler(Client.ProgressChanged));

                // Recursively receive the rest file.
                try
                {
                    clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                catch
                {
                    if (!clientSocket.Connected)
                    {
                        MessageBox.Show(Properties.Resources.DisconnectMsg);
                    }
                }
            }
            else
            {
                // Signal if all the file received.
                receiveDone.Set();
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Handle the exception when disconnect from the server.
        /// </summary>
        private static void HandleDisconnectException()
        {
            MessageBox.Show(Properties.Resources.DisconnectMsg);
            Client.BeginInvoke(new EnableConnectButtonHandler(Client.EnableConnectButton));
            Thread.CurrentThread.Abort();
        }

        #endregion
    }
}
