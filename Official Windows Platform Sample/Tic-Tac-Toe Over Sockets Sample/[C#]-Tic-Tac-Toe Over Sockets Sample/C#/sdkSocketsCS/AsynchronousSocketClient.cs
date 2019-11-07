/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace sdkSocketsCS
{
    public class AsynchronousClient
    {
        // The port number for the remote device.
        private int _port = 13001;

        // A timeout for all socket communication
        private const int TIMEOUT_MILLISECONDS = 3000;

        // Notify the callee or user of this class through a custom event
        internal event ResponseReceivedEventHandler ResponseReceived;

        // Store data from the client which is to be sent to the server once the 
        // socket is established
        static string dataIn = String.Empty;

        private string _serverName = string.Empty;

        public AsynchronousClient(string serverName, int portNumber)
        {
            if (String.IsNullOrWhiteSpace(serverName))
            {
                throw new ArgumentNullException("serverName");
            }

            if (portNumber < 0 || portNumber > 65535)
            {
                throw new ArgumentNullException("portNumber");
            }

            _serverName = serverName;
            _port = portNumber;
        }

        /// <summary>
        /// Send data to the server
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <remarks> This is an asynchronous call, with the result being passed to the callee
        /// through the ResponseReceived event</remarks>
        public void SendData(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data");
            }

            dataIn = data;

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

            DnsEndPoint hostEntry = new DnsEndPoint(_serverName, _port);
            
            // Create a socket and connect to the server 

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventArg_Completed);
            socketEventArg.RemoteEndPoint = hostEntry;

            socketEventArg.UserToken = sock;

            try
            {
                sock.ConnectAsync(socketEventArg);
            }
            catch (SocketException ex)
            {
                throw new SocketException((int)ex.ErrorCode);
            }

        }
        #region

        // A single callback is used for all socket operations. 
        // This method forwards execution on to the correct handler 
        // based on the type of completed operation 
        void SocketEventArg_Completed(object sender, SocketAsyncEventArgs e) 
        { 
            switch (e.LastOperation) 
            { 
                case SocketAsyncOperation.Connect: 
                    ProcessConnect(e); 
                    break; 
                case SocketAsyncOperation.Receive: 
                    ProcessReceive(e); 
                    break; 
                case SocketAsyncOperation.Send: 
                    ProcessSend(e); 
                    break; 
                default: 
                    throw new Exception("Invalid operation completed"); 
            } 
        }

        // Called when a ReceiveAsync operation completes  
        private void ProcessReceive(SocketAsyncEventArgs e) 
        { 
            if (e.SocketError == SocketError.Success) 
            {
                // Received data from server 
                string dataFromServer = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                
                Socket sock = e.UserToken as Socket; 
                sock.Shutdown(SocketShutdown.Send); 
                sock.Close(); 

                // Respond to the client in the UI thread to tell him that data was received
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ResponseReceivedEventArgs args = new ResponseReceivedEventArgs();
                    args.response = dataFromServer;
                    OnResponseReceived(args);
                });

            } 
            else 
            {
                throw new SocketException((int)e.SocketError); 
            } 
        }

        // Invoke the ResponseReceived event
        protected void OnResponseReceived(ResponseReceivedEventArgs e)
        {
            if (ResponseReceived != null)
                ResponseReceived(this,e);
        }

        // Called when a SendAsync operation completes 
        private void ProcessSend(SocketAsyncEventArgs e) 
        { 
            if (e.SocketError == SocketError.Success) 
            { 
                //Read data sent from the server 
                Socket sock = e.UserToken as Socket; 

                sock.ReceiveAsync(e); 
            } 
            else 
            {
                ResponseReceivedEventArgs args = new ResponseReceivedEventArgs();
                args.response = e.SocketError.ToString();
                args.isError = true;
                OnResponseReceived(args);
            } 
        }

        // Called when a ConnectAsync operation completes 
        private void ProcessConnect(SocketAsyncEventArgs e) 
        { 
            if (e.SocketError == SocketError.Success) 
            { 
                // Successfully connected to the server 
                // Send data to the server 
                byte[] buffer = Encoding.UTF8.GetBytes(dataIn + "<EOF>"); 
                e.SetBuffer(buffer, 0, buffer.Length); 
                Socket sock = e.UserToken as Socket; 
                sock.SendAsync(e); 
            } 
            else 
            {
                ResponseReceivedEventArgs args = new ResponseReceivedEventArgs();
                args.response = e.SocketError.ToString();
                args.isError = true;
                OnResponseReceived(args);

            } 
        } 
        #endregion
    }

    // A delegate type for hooking up change notifications.
    public delegate void ResponseReceivedEventHandler(object sender, ResponseReceivedEventArgs e);

    public class ResponseReceivedEventArgs : EventArgs
    {
        // True if an error occured, False otherwise
        public bool isError { get; set; }

        // If there was an erro, this will contain the error message, data otherwise
        public string response { get; set; }
    }

}
