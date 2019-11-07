'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBSocketClient
' Copyright (c) Microsoft Corporation.
' 
' Sockets are an application programming interface(API) in an operating system
' used for in inter-process communication. Sockets constitute a mechanism for
' delivering incoming data packets to the appropriate application process or
' thread, based on a combination of local and remote IP addresses and port
' numbers. Each socket is mapped by the operational system to a communicating
' application process or thread.
'
'
' .NET supplies a Socket class which implements the Berkeley sockets interface.
' It provides a rich set of methods and properties for network communications. 
' Socket class allows you to perform both synchronous and asynchronous data
' transfer using any of the communication protocols listed in the ProtocolType
' enumeration. It supplies the following types of socket:
'
' Stream: Supports reliable, two-way, connection-based byte streams without 
' the duplication of data and without preservation of boundaries.
'
' Dgram:Supports datagrams, which are connectionless, unreliable messages of
' a fixed (typically small) maximum length. 
'
' Raw: Supports access to the underlying transport protocol.Using the 
' SocketTypeRaw, you can communicate using protocols like Internet Control 
' Message Protocol (Icmp) and Internet Group Management Protocol (Igmp). 
'
' Rdm: Supports connectionless, message-oriented, reliably delivered messages, 
' and preserves message boundaries in data. 
'
' Seqpacket:Provides connection-oriented and reliable two-way transfer of 
' ordered byte streams across a network.
'
' Unknown:Specifies an unknown Socket type.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

#Region "Using directives"
Imports System.Text
Imports System.Net
Imports System.Net.Sockets

#End Region


Module MainModule
    Sub Main()
        ' Receiving byte array 
        Dim bytes(1023) As Byte
        Try
            ' Create one SocketPermission for socket access restrictions
            Dim permission As New SocketPermission(NetworkAccess.Connect, TransportType.Tcp, "", SocketPermission.AllPorts) ' All ports -  Gets the IP addresses -  Defines transport types -  Connection permission

            ' Ensures the code to have permission to access a Socket
            permission.Demand()

            ' Resolves a host name to an IPHostEntry instance           
            Dim ipHost As IPHostEntry = Dns.GetHostEntry("")

            ' Gets first IP address associated with a localhost
            Dim ipAddr As IPAddress = ipHost.AddressList(0)

            ' Creates a network endpoint
            Dim ipEndPoint As New IPEndPoint(ipAddr, 4510)

            ' Create one Socket object to setup Tcp connection
            Dim sender As New Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp) ' Specifies the protocols -  The type of socket -  Specifies the addressing scheme

            sender.NoDelay = False ' Using the Nagle algorithm

            ' Establishes a connection to a remote host
            sender.Connect(ipEndPoint)
            Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString())

            ' Sending message
            '<Client Quit> is the sign for end of data
            Dim theMessage As String = "Hello World!"
            Dim msg() As Byte = Encoding.Unicode.GetBytes(theMessage & "<Client Quit>")

            ' Sends data to a connected Socket.
            Dim bytesSend As Integer = sender.Send(msg)

            ' Receives data from a bound Socket.
            Dim bytesRec As Integer = sender.Receive(bytes)

            ' Converts byte array to string
            theMessage = Encoding.Unicode.GetString(bytes, 0, bytesRec)

            ' Continues to read the data till data isn't available
            Do While sender.Available > 0
                bytesRec = sender.Receive(bytes)
                theMessage &= Encoding.Unicode.GetString(bytes, 0, bytesRec)
            Loop
            Console.WriteLine("The server reply: {0}", theMessage)

            ' Disables sends and receives on a Socket.
            sender.Shutdown(SocketShutdown.Both)

            'Closes the Socket connection and releases all resources
            sender.Close()
        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.ToString())
        End Try

        Console.Read()
    End Sub
End Module

