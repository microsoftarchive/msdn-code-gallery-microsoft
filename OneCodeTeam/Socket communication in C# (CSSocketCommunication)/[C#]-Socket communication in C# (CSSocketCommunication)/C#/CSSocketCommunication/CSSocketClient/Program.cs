/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSSocketClient
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

#region Using directives
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

#endregion


public class Program
{
    static void Main(string[] args)
    {
        // Receiving byte array 
        byte[] bytes = new byte[1024];
        try
        {
            // Create one SocketPermission for socket access restrictions
            SocketPermission permission = new SocketPermission(
                NetworkAccess.Connect,    // Connection permission
                TransportType.Tcp,        // Defines transport types
                "",                       // Gets the IP addresses
                SocketPermission.AllPorts // All ports
                );

            // Ensures the code to have permission to access a Socket
            permission.Demand();

            // Resolves a host name to an IPHostEntry instance           
            IPHostEntry ipHost = Dns.GetHostEntry("");

            // Gets first IP address associated with a localhost
            IPAddress ipAddr = ipHost.AddressList[0];

            // Creates a network endpoint
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 4510);

            // Create one Socket object to setup Tcp connection
            Socket sender = new Socket(
                ipAddr.AddressFamily,// Specifies the addressing scheme
                SocketType.Stream,   // The type of socket 
                ProtocolType.Tcp     // Specifies the protocols 
                );

            sender.NoDelay = false;   // Using the Nagle algorithm

            // Establishes a connection to a remote host
            sender.Connect(ipEndPoint);
            Console.WriteLine("Socket connected to {0}",
                sender.RemoteEndPoint.ToString());

            // Sending message
            //<Client Quit> is the sign for end of data
            string theMessage = "Hello World!";
            byte[] msg = Encoding.Unicode.GetBytes(theMessage +"<Client Quit>");

            // Sends data to a connected Socket.
            int bytesSend = sender.Send(msg);

            // Receives data from a bound Socket.
            int bytesRec = sender.Receive(bytes);

            // Converts byte array to string
            theMessage = Encoding.Unicode.GetString(bytes, 0, bytesRec);

            // Continues to read the data till data isn't available
            while (sender.Available > 0)
            {
                bytesRec = sender.Receive(bytes);
                theMessage += Encoding.Unicode.GetString(bytes, 0, bytesRec);
            }
            Console.WriteLine("The server reply: {0}", theMessage);

            // Disables sends and receives on a Socket.
            sender.Shutdown(SocketShutdown.Both);

            //Closes the Socket connection and releases all resources
            sender.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.ToString());
        }

        Console.Read();
    }
}

