'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Net

' State object for reading client data asynchronously
Public Class StateObject
    ' Client  socket.
    Public workSocket As Socket = Nothing
    ' Size of receive buffer.
    Public Const BufferSize As Integer = 1024
    ' Receive buffer.
    Public buffer(BufferSize - 1) As Byte
    ' Received data string.
    Public sb As New StringBuilder()
End Class

Public Class AsynchronousSocketListener
    ' Thread signal.
    Public Shared allDone As New ManualResetEvent(False)

    Public Sub New()
    End Sub

    Public Shared Sub StartListening()
        ' Data buffer for incoming data.
        Dim bytes(StateObject.BufferSize - 1) As Byte

        ' Establish the local endpoint for the socket.
        ' Note: remember to keep the portnumber updated if you change
        ' it on here, or on the client
        Dim localEndPoint As New IPEndPoint(IPAddress.Any, 13001)

        ' Create a TCP/IP socket.
        Dim listener As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

        ' Bind the socket to the local endpoint and listen for incoming connections.
        Try
            listener.Bind(localEndPoint)
            listener.Listen(10)

            Do
                ' Set the event to nonsignaled state.
                allDone.Reset()

                ' Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...")
                listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), listener)

                ' Wait until a connection is made before continuing.
                allDone.WaitOne()
            Loop

        Catch e As Exception
            Console.WriteLine(e.ToString())
        End Try

        Console.WriteLine(vbLf & "Press ENTER to continue...")
        Console.Read()

    End Sub

    Public Shared Sub AcceptCallback(ByVal ar As IAsyncResult)
        ' Signal the main thread to continue.
        allDone.Set()

        ' Get the socket that handles the client request.
        Dim listener As Socket = CType(ar.AsyncState, Socket)
        Dim handler As Socket = listener.EndAccept(ar)

        ' Create the state object.
        Dim state As New StateObject()
        state.workSocket = handler
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReadCallback), state)
    End Sub

    Public Shared Sub ReadCallback(ByVal ar As IAsyncResult)
        Dim content As String = String.Empty

        ' Retrieve the state object and the handler socket
        ' from the asynchronous state object.
        Dim state As StateObject = CType(ar.AsyncState, StateObject)
        Dim handler As Socket = state.workSocket

        ' Read data from the client socket. 
        Dim bytesRead As Integer = handler.EndReceive(ar)

        If bytesRead > 0 Then
            ' There  might be more data, so store the data received so far.
            state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead))

            ' Check for end-of-file tag. If it is not there, read 
            ' more data.
            content = state.sb.ToString()
            If content.IndexOf("<EOF>") > -1 Then
                ' All the data has been read from the 
                ' client. Display it on the console.
                Console.WriteLine("Read {0} bytes from socket. " & vbLf & " Data : {1}", content.Length, content)

                ' Respond to the client
                Send(handler, content)
            Else
                ' Not all data received. Get more.
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReadCallback), state)
            End If
        End If
    End Sub

    Private Shared Sub Send(ByVal handler As Socket, ByVal data As String)
        data = data.Replace("<EOF>", "")

        ' What we want to send back in this application is a game move based on what
        ' has been received. So we call Play on the GameLogic to give us a move to send back
        data = GameLogic.Play(data)

        ' Convert the string data to byte data using ASCII encoding.
        Dim byteData() As Byte = Encoding.UTF8.GetBytes(data)

        ' Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0, New AsyncCallback(AddressOf SendCallback), handler)
    End Sub

    Private Shared Sub SendCallback(ByVal ar As IAsyncResult)
        Try
            ' Retrieve the socket from the state object.
            Dim handler As Socket = CType(ar.AsyncState, Socket)

            ' Complete sending the data to the remote device.
            Dim bytesSent As Integer = handler.EndSend(ar)
            Console.WriteLine("Sent {0} bytes to client.", bytesSent)

            handler.Shutdown(SocketShutdown.Both)
            handler.Close()

        Catch e As Exception
            Console.WriteLine(e.ToString())
        End Try
    End Sub


    Public Shared Function Main(ByVal args() As String) As Integer
        StartListening()
        Return 0
    End Function
End Class
