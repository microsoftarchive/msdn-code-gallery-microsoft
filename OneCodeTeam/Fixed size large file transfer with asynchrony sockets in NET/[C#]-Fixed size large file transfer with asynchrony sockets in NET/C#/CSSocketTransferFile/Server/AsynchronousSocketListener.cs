/********************************* Module Header **********************************\
* Module Name:	AsynchronousSocketListener.cs
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
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    /// <summary>
    /// AsynchronousSocketListener is used to asynchronously listen the client and send file to the client.
    /// </summary>
    public static class AsynchronousSocketListener
    {
        #region Constants

        private const int c_clientSockets = 100;
        private const int c_bufferSize = 5242880;

        #endregion

        #region Memebers

        private static int port;
        private static int signal;
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private delegate void AddClientHandler(IPEndPoint IpEndPoint);
        private delegate void CompleteSendHandler();
        private delegate void RemoveItemHandler(string ipAddress);
        private delegate void EnableSendHandler();

        #endregion

        #region Properties

        public static Server Server { get; set; }
        public static string Port 
        {
            set
            {
                try
                {
                    port = Convert.ToInt32(value);
                }
                catch (FormatException)
                {
                    throw new Exception(Properties.Resources.InvalidPortMsg);
                }
                catch (OverflowException ex)
                {
                    throw new Exception(ex.Message);
                }

                if (port < 0 || port > 65535)
                {
                    throw new Exception(Properties.Resources.InvalidPortMsg);
                }
            }
        }
        public static string FileToSend { get; set; }
        public static IList<Socket> Clients = new List<Socket>();
        public static IDictionary<Socket,IPEndPoint> ClientsToSend = new Dictionary<Socket,IPEndPoint>();

        #endregion 

        #region Functions

        /// <summary>
        /// Server start to listen the client connection.
        /// </summary>
        public static void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

            // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            listener.Listen(c_clientSockets);

            //loop listening the client.
            while (true)
            {
                allDone.Reset();
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                allDone.WaitOne();
            }
        }

        /// <summary>
        /// Callback when one client successfully connected to the server.
        /// </summary>
        /// <param name="ar"></param>
        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            IPEndPoint ipEndPoint = handler.RemoteEndPoint as IPEndPoint;

            if ((ipEndPoint) != null)
            {
                Server.BeginInvoke(new AddClientHandler(Server.AddClient),ipEndPoint);
            }

            Clients.Add(handler);
            
            allDone.Set();
        }

        /// <summary>
        /// Send file information to clients.
        /// </summary>
        public static void SendFileInfo()
        {
            string fileName = FileToSend.Replace("\\", "/");
            IList<Socket> closedSockets = new List<Socket>();
            IList<String> removedItems = new List<String>();

            while (fileName.IndexOf("/") > -1)
            {
                fileName = fileName.Substring(fileName.IndexOf("/") + 1);
            }

            FileInfo fileInfo = new FileInfo(FileToSend);
            long fileLen = fileInfo.Length;

            byte[] fileLenByte = BitConverter.GetBytes(fileLen);

            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);

            byte[] clientData = new byte[4 + fileNameByte.Length + 8];

            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

            fileNameLen.CopyTo(clientData, 0);
            fileNameByte.CopyTo(clientData, 4);
            fileLenByte.CopyTo(clientData, 4 + fileNameByte.Length);

            // Send the file name and length to the clients. 
            foreach (KeyValuePair<Socket, IPEndPoint> kvp in ClientsToSend)
            {
                Socket handler = kvp.Key;
                IPEndPoint ipEndPoint = kvp.Value;
                try
                {
                    handler.Send(clientData);
                }
                catch
                {
                    if (!handler.Connected)
                    {
                        closedSockets.Add(handler);

                        removedItems.Add(ipEndPoint.ToString());
                    }
                }
            }

            // Remove the clients which are disconnected.
            RemoveClient(closedSockets);
            RemoveClientItem(removedItems);
            closedSockets.Clear();
            removedItems.Clear();
        }

        /// <summary>
        /// Send file from server to clients.
        /// </summary>
        public static void Send()
        {
            int readBytes = 0;
            byte[] buffer = new byte[c_bufferSize];
            IList<Socket> closedSockets = new List<Socket>();
            IList<String> removedItems = new List<String>();

            // Send file information to the clients.
            SendFileInfo();

            // Blocking read file and send to the clients asynchronously.
            using (FileStream stream = new FileStream(FileToSend, FileMode.Open))
            {
                do
                {
                    sendDone.Reset();
                    signal = 0;
                    stream.Flush();
                    readBytes = stream.Read(buffer,0,c_bufferSize);

                    if (ClientsToSend.Count == 0)
                    {
                        sendDone.Set();
                    }

                    foreach (KeyValuePair<Socket,IPEndPoint> kvp in ClientsToSend)
                    {
                        Socket handler = kvp.Key;
                        IPEndPoint ipEndPoint = kvp.Value;
                        try
                        {
                            handler.BeginSend(buffer, 0, readBytes, SocketFlags.None, new AsyncCallback(SendCallback), handler);
                        }
                        catch
                        {
                            if (!handler.Connected)
                            {
                                closedSockets.Add(handler);
                                signal++;
                                removedItems.Add(ipEndPoint.ToString());

                                // Signal if all the clients are disconnected.
                                if (signal >= ClientsToSend.Count)
                                {
                                    sendDone.Set();
                                }
                            }
                        }
                    }
                    sendDone.WaitOne();

                    // Remove the clients which are disconnected.
                    RemoveClient(closedSockets);
                    RemoveClientItem(removedItems);
                    closedSockets.Clear();
                    removedItems.Clear();
                }
                while (readBytes > 0);
            }

            // Disconnect all the connection when the file has send to the clients completely.
            ClientDisconnect();
            CompleteSendFile();
        }

        /// <summary>
        /// Callback when a part of the file has been sent to the clients successfully.
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            lock (Server)
            {
                Socket handler = null;
                try
                {
                    handler = (Socket)ar.AsyncState;
                    signal++;
                    int bytesSent = handler.EndSend(ar);

                    // Close the socket when all the data has sent to the client.
                    if (bytesSent == 0)
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
                catch (ArgumentException argEx)
                {
                    MessageBox.Show(argEx.Message);
                }
                catch (SocketException)
                {
                    // Close the socket if the client disconnected.
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                finally
                {
                    // Signal when the file chunk has sent to all the clients successfully. 
                    if (signal >= ClientsToSend.Count)
                    {
                        sendDone.Set();
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect all the connection.
        /// </summary>
        private static void ClientDisconnect()
        {
            Clients.Clear();
            ClientsToSend.Clear();
        }

        /// <summary>
        /// Change the presentation of listbox when the file send to the clients finished.
        /// </summary>
        private static void CompleteSendFile()
        {
            Server.BeginInvoke(new CompleteSendHandler(Server.CompleteSend));
        }

        /// <summary>
        /// Change the presentation of listbox when the some clients disconnect the connection.
        /// </summary>
        /// <param name="ipAddressList"></param>
        private static void RemoveClientItem(IList<String> ipAddressList)
        {
            foreach (string ipAddress in ipAddressList)
            {
                Server.BeginInvoke(new RemoveItemHandler(Server.RemoveItem), ipAddress);
            }

            if (ClientsToSend.Count == 0)
            {
                Server.BeginInvoke(new EnableSendHandler(Server.EnableSendButton));
            }
        }

        /// <summary>
        /// Remove the sockets if some client disconnect the connection.
        /// </summary>
        /// <param name="listSocket"></param>
        private static void RemoveClient(IList<Socket> listSocket)
        {
            if (listSocket.Count > 0)
            {
                foreach (Socket socket in listSocket)
                {
                    Clients.Remove(socket);
                    ClientsToSend.Remove(socket);
                }
            }
        }

        #endregion
    }
}
