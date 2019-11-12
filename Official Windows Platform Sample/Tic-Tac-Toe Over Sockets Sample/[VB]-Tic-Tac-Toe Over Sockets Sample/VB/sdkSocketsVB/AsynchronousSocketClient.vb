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


Public Class AsynchronousClient
    ' The port number for the remote device.
    Private _port As Integer = 13001

    ' A timeout for all socket communication
    Private Const TIMEOUT_MILLISECONDS As Integer = 3000

    ' Notify the callee or user of this class through a custom event
    Friend Event ResponseReceived As ResponseReceivedEventHandler

    ' Store data from the client which is to be sent to the server once the 
    ' socket is established
    Private Shared dataIn As String = String.Empty

    Private _serverName As String = String.Empty

    Public Sub New(ByVal serverName As String, ByVal portNumber As Integer)
        If String.IsNullOrWhiteSpace(serverName) Then
            Throw New ArgumentNullException("serverName")
        End If

        If portNumber < 0 OrElse portNumber > 65535 Then
            Throw New ArgumentNullException("portNumber")
        End If

        _serverName = serverName
        _port = portNumber
    End Sub

    ''' <summary>
    ''' Send data to the server
    ''' </summary>
    ''' <param name="data">The data to send</param>
    ''' <remarks> This is an asynchronous call, with the result being passed to the callee
    ''' through the ResponseReceived event</remarks>
    Public Sub SendData(ByVal data As String)
        If String.IsNullOrWhiteSpace(data) Then
            Throw New ArgumentNullException("data")
        End If

        dataIn = data

        Dim socketEventArg As New SocketAsyncEventArgs()

        Dim hostEntry As New DnsEndPoint(_serverName, _port)

        ' Create a socket and connect to the server 

        Dim sock As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        AddHandler socketEventArg.Completed, AddressOf SocketEventArg_Completed
        socketEventArg.RemoteEndPoint = hostEntry

        socketEventArg.UserToken = sock

        Try
            sock.ConnectAsync(socketEventArg)
        Catch ex As SocketException
            Throw New SocketException(CInt(Fix(ex.ErrorCode)))
        End Try

    End Sub
#Region ""

    ' A single callback is used for all socket operations. 
    ' This method forwards execution on to the correct handler 
    ' based on the type of completed operation 
    Private Sub SocketEventArg_Completed(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
        Select Case e.LastOperation
            Case SocketAsyncOperation.Connect
                ProcessConnect(e)
            Case SocketAsyncOperation.Receive
                ProcessReceive(e)
            Case SocketAsyncOperation.Send
                ProcessSend(e)
            Case Else
                Throw New Exception("Invalid operation completed")
        End Select
    End Sub

    ' Called when a ReceiveAsync operation completes  
    Private Sub ProcessReceive(ByVal e As SocketAsyncEventArgs)
        If e.SocketError = SocketError.Success Then
            ' Received data from server 
            Dim dataFromServer As String = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred)

            Dim sock As Socket = TryCast(e.UserToken, Socket)
            sock.Shutdown(SocketShutdown.Send)
            sock.Close()

            ' Respond to the client in the UI thread to tell him that data was received
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Sub()
                                                                         Dim args As New ResponseReceivedEventArgs()
                                                                         args.response = dataFromServer
                                                                         OnResponseReceived(args)
                                                                     End Sub)

        Else
            Throw New SocketException(CInt(Fix(e.SocketError)))
        End If
    End Sub

    ' Invoke the ResponseReceived event
    Protected Sub OnResponseReceived(ByVal e As ResponseReceivedEventArgs)
        RaiseEvent ResponseReceived(Me, e)
    End Sub

    ' Called when a SendAsync operation completes 
    Private Sub ProcessSend(ByVal e As SocketAsyncEventArgs)
        If e.SocketError = SocketError.Success Then
            'Read data sent from the server 
            Dim sock As Socket = TryCast(e.UserToken, Socket)

            sock.ReceiveAsync(e)
        Else
            Dim args As New ResponseReceivedEventArgs()
            args.response = e.SocketError.ToString()
            args.isError = True
            OnResponseReceived(args)
        End If
    End Sub

    ' Called when a ConnectAsync operation completes 
    Private Sub ProcessConnect(ByVal e As SocketAsyncEventArgs)
        If e.SocketError = SocketError.Success Then
            ' Successfully connected to the server 
            ' Send data to the server 
            Dim buffer() As Byte = Encoding.UTF8.GetBytes(dataIn & "<EOF>")
            e.SetBuffer(buffer, 0, buffer.Length)
            Dim sock As Socket = TryCast(e.UserToken, Socket)
            sock.SendAsync(e)
        Else
            Dim args As New ResponseReceivedEventArgs()
            args.response = e.SocketError.ToString()
            args.isError = True
            OnResponseReceived(args)

        End If
    End Sub
#End Region
End Class

' A delegate type for hooking up change notifications.
Public Delegate Sub ResponseReceivedEventHandler(ByVal sender As Object, ByVal e As ResponseReceivedEventArgs)

Public Class ResponseReceivedEventArgs
    Inherits EventArgs
    ' True if an error occured, False otherwise
    Public Property isError() As Boolean

    ' If there was an erro, this will contain the error message, data otherwise
    Public Property response() As String
End Class

