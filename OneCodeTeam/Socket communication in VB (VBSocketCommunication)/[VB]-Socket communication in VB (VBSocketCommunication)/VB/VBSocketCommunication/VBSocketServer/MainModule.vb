'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBSocketServer
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

#Region "Using Directive"
Imports System.Text
Imports System.Net
Imports System.Net.Sockets

#End Region


Module MainModule
    Sub Main()
        ' Creates one SocketPermission object for access restrictions
        Dim permission As New SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts) ' Specifies all ports -  The IP addresses of local host -  Defines transport types -  Allowed to accept connections

        ' Listening Socket object
        Dim sListener As Socket = Nothing

        Try
            ' Ensures the code to have permission to access a Socket
            permission.Demand()

            ' Resolves a host name to an IPHostEntry instance
            Dim ipHost As IPHostEntry = Dns.GetHostEntry("")

            ' Gets first IP address associated with a localhost
            Dim ipAddr As IPAddress = ipHost.AddressList(0)

            ' Creates a network endpoint
            Dim ipEndPoint As New IPEndPoint(ipAddr, 4510)

            ' Create one Socket object to listen the incoming connection
            sListener = New Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp)

            ' Associates a Socket with a local endpoint
            sListener.Bind(ipEndPoint)

            ' Places a Socket in a listening state and specifies the maximum
            ' Length of the pending connections queue
            sListener.Listen(10)

            Console.WriteLine("Waiting for a connection on port {0}", ipEndPoint)

            ' Begins an asynchronous operation to accept an attempt
            Dim aCallback As New AsyncCallback(AddressOf AcceptCallback)
            sListener.BeginAccept(aCallback, sListener)
        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.ToString())
            Return
        End Try

        Console.WriteLine("Press the Enter key to exit ...")
        Console.ReadLine()

        If sListener.Connected Then
            sListener.Shutdown(SocketShutdown.Receive)
            sListener.Close()
        End If
    End Sub


    ''' <summary>
    ''' Asynchronously accepts an incoming connection attempt and creates
    ''' a new Socket to handle remote host communication.
    ''' </summary>     
    ''' <param name="ar">the status of an asynchronous operation
    ''' </param> 
    Public Sub AcceptCallback(ByVal ar As IAsyncResult)
        Dim listener As Socket = Nothing

        ' A new Socket to handle remote host communication
        Dim handler As Socket = Nothing
        Try
            ' Receiving byte array
            Dim buffer(1023) As Byte
            ' Get Listening Socket object
            listener = CType(ar.AsyncState, Socket)
            ' Create a new socket
            handler = listener.EndAccept(ar)

            ' Using the Nagle algorithm
            handler.NoDelay = False

            ' Creates one object array for passing data
            Dim obj(1) As Object
            obj(0) = buffer
            obj(1) = handler

            ' Begins to asynchronously receive data
            handler.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf ReceiveCallback), obj) ' Specifies infomation for receive operation - An AsyncCallback delegate -  Specifies send and receive behaviors -  The number of bytes to receive -  The zero-based position in the buffer -  An array of type Byt for received data

            ' Begins an asynchronous operation to accept an attempt
            Dim aCallback As New AsyncCallback(AddressOf AcceptCallback)
            listener.BeginAccept(aCallback, listener)
        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Asynchronously receive data from a connected Socket.
    ''' </summary>
    ''' <param name="ar">
    ''' the status of an asynchronous operation
    ''' </param> 
    Public Sub ReceiveCallback(ByVal ar As IAsyncResult)
        Try
            ' Fetch a user-defined object that contains information
            Dim obj(1) As Object
            obj = CType(ar.AsyncState, Object())

            ' Received byte array
            Dim buffer() As Byte = CType(obj(0), Byte())

            ' A Socket to handle remote host communication.
            Dim handler As Socket = CType(obj(1), Socket)

            ' Received message
            Dim content As String = String.Empty

            ' The number of bytes received.
            Dim bytesRead As Integer = handler.EndReceive(ar)

            If bytesRead > 0 Then
                content &= Encoding.Unicode.GetString(buffer, 0, bytesRead)
                ' If message contains "<Client Quit>", finish receiving
                If content.IndexOf("<Client Quit>") > -1 Then
                    ' Convert byte array to string
                    Dim str As String = content.Substring(0, content.LastIndexOf("<Client Quit>"))
                    Console.WriteLine("Read {0} bytes from client." & vbLf & " Data: {1}", str.Length * 2, str)

                    ' Prepare the reply message
                    Dim byteData() As Byte = Encoding.Unicode.GetBytes(str)

                    ' Sends data asynchronously to a connected Socket
                    handler.BeginSend(byteData, 0, byteData.Length, 0, New AsyncCallback(AddressOf SendCallback), handler)
                Else
                    ' Continues to asynchronously receive data
                    Dim buffernew(1023) As Byte
                    obj(0) = buffernew
                    obj(1) = handler
                    handler.BeginReceive(buffernew, 0, buffernew.Length, SocketFlags.None, New AsyncCallback(AddressOf ReceiveCallback), obj)
                End If
            End If
        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Sends data asynchronously to a connected Socket.
    ''' </summary>
    ''' <param name="ar">
    ''' The status of an asynchronous operation
    ''' </param> 
    Public Sub SendCallback(ByVal ar As IAsyncResult)
        Try
            ' A Socket which has sent the data to remote host
            Dim handler As Socket = CType(ar.AsyncState, Socket)

            ' The number of bytes sent to the Socket
            Dim bytesSend As Integer = handler.EndSend(ar)
            Console.WriteLine("Sent {0} bytes to Client", bytesSend)
        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.ToString())
        End Try
    End Sub
End Module




