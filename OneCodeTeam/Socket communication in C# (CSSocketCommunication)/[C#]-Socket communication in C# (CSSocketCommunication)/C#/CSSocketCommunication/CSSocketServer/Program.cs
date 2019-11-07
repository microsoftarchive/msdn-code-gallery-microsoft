/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSSocketServer
* Copyright (c) Microsoft Corporation.
* 
* Sockets are an application programming interface(API) in an operating system
* used for in inter-process communication. Sockets constitute a mechanism for
* delivering incoming data packets to the appropriate application process or
* thread, based on a combination of local and remote IP addresses and port
* numbers. Each socket is mapped by the operational system to a communicating
* application process or thread.
*
*
* .NET supplies a Socket class which implements the Berkeley sockets interface.
* It provides a rich set of methods and properties for network communications. 
* Socket class allows you to perform both synchronous and asynchronous data
* transfer using any of the communication protocols listed in the ProtocolType
* enumeration. It supplies the following types of socket:
*
* Stream: Supports reliable, two-way, connection-based byte streams without 
* the duplication of data and without preservation of boundaries.
*
* Dgram:Supports datagrams, which are connectionless, unreliable messages of
* a fixed (typically small) maximum length. 
*
* Raw: Supports access to the underlying transport protocol.Using the 
* SocketTypeRaw, you can communicate using protocols like Internet Control 
* Message Protocol (Icmp) and Internet Group Management Protocol (Igmp). 
*
* Rdm: Supports connectionless, message-oriented, reliably delivered messages, 
* and preserves message boundaries in data. 
*
* Seqpacket:Provides connection-oriented and reliable two-way transfer of 
* ordered byte streams across a network.
*
* Unknown:Specifies an unknown Socket type.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using Directive
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

#endregion


public class Program
{
    static void Main(string[] args)
    {
        // Creates one SocketPermission object for access restrictions
        SocketPermission permission = new SocketPermission(
            NetworkAccess.Accept,     // Allowed to accept connections
            TransportType.Tcp,        // Defines transport types
            "",                       // The IP addresses of local host
            SocketPermission.AllPorts // Specifies all ports
            );

        // Listening Socket object
        Socket sListener = null;

        try
        {
            // Ensures the code to have permission to access a Socket
            permission.Demand();

            // Resolves a host name to an IPHostEntry instance
            IPHostEntry ipHost = Dns.GetHostEntry("");

            // Gets first IP address associated with a localhost
            IPAddress ipAddr = ipHost.AddressList[0];

            // Creates a network endpoint
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 4510);

            // Create one Socket object to listen the incoming connection
            sListener = new Socket(
                ipAddr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
                );

            // Associates a Socket with a local endpoint
            sListener.Bind(ipEndPoint);

            // Places a Socket in a listening state and specifies the maximum
            // Length of the pending connections queue
            sListener.Listen(10);

            Console.WriteLine("Waiting for a connection on port {0}",
                ipEndPoint);

            // Begins an asynchronous operation to accept an attempt
            AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
            sListener.BeginAccept(aCallback, sListener);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.ToString());
            return;
        }

        Console.WriteLine("Press the Enter key to exit ...");
        Console.ReadLine();
        
        if (sListener.Connected)
        {
            sListener.Shutdown(SocketShutdown.Receive);
            sListener.Close();
        }
    }


    /// <summary>
    /// Asynchronously accepts an incoming connection attempt and creates
    /// a new Socket to handle remote host communication.
    /// </summary>     
    /// <param name="ar">the status of an asynchronous operation
    /// </param> 
    public static void AcceptCallback(IAsyncResult ar)
    {
        Socket listener = null;

        // A new Socket to handle remote host communication
        Socket handler = null;
        try
        {
            // Receiving byte array
            byte[] buffer = new byte[1024];
            // Get Listening Socket object
            listener = (Socket)ar.AsyncState;
            // Create a new socket
            handler = listener.EndAccept(ar);

            // Using the Nagle algorithm
            handler.NoDelay = false;

            // Creates one object array for passing data
            object[] obj = new object[2];
            obj[0] = buffer;
            obj[1] = handler;

            // Begins to asynchronously receive data
            handler.BeginReceive(
                buffer,        // An array of type Byt for received data
                0,             // The zero-based position in the buffer 
                buffer.Length, // The number of bytes to receive
                SocketFlags.None,// Specifies send and receive behaviors
                new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate
                obj            // Specifies infomation for receive operation
                );

            // Begins an asynchronous operation to accept an attempt
            AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
            listener.BeginAccept(aCallback, listener);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.ToString());
        }
    }

    /// <summary>
    /// Asynchronously receive data from a connected Socket.
    /// </summary>
    /// <param name="ar">
    /// the status of an asynchronous operation
    /// </param> 
    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Fetch a user-defined object that contains information
            object[] obj = new object[2];
            obj = (object[])ar.AsyncState;

            // Received byte array
            byte[] buffer = (byte[])obj[0];

            // A Socket to handle remote host communication.
            Socket handler = (Socket)obj[1];

            // Received message
            string content = string.Empty;

            // The number of bytes received.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                content += Encoding.Unicode.GetString(buffer, 0,
                    bytesRead);
                // If message contains "<Client Quit>", finish receiving
                if (content.IndexOf("<Client Quit>") > -1)
                {
                    // Convert byte array to string
                    string str =
                        content.Substring(0, content.LastIndexOf("<Client Quit>"));
                    Console.WriteLine(
                        "Read {0} bytes from client.\n Data: {1}",
                        str.Length * 2, str);

                    // Prepare the reply message
                    byte[] byteData =
                        Encoding.Unicode.GetBytes(str);

                    // Sends data asynchronously to a connected Socket
                    handler.BeginSend(byteData, 0, byteData.Length, 0,
                        new AsyncCallback(SendCallback), handler);
                }
                else
                {
                    // Continues to asynchronously receive data
                    byte[] buffernew = new byte[1024];
                    obj[0] = buffernew;
                    obj[1] = handler;
                    handler.BeginReceive(buffernew, 0, buffernew.Length,
                        SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), obj);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.ToString());
        }
    }

    /// <summary>
    /// Sends data asynchronously to a connected Socket.
    /// </summary>
    /// <param name="ar">
    /// The status of an asynchronous operation
    /// </param> 
    public static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // A Socket which has sent the data to remote host
            Socket handler = (Socket)ar.AsyncState;

            // The number of bytes sent to the Socket
            int bytesSend = handler.EndSend(ar);
            Console.WriteLine(
                "Sent {0} bytes to Client",bytesSend);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.ToString());
        }
    }
}
  



