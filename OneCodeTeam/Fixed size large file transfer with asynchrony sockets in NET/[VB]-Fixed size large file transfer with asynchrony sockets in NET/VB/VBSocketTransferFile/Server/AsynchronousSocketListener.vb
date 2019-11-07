'******************************** Module Header **********************************\
' Module Name:	AsynchronousSocketListener.vb
' Project:		Server
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to implement fixed size large file transfer with asynchrony sockets in NET.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*********************************************************************************/


Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Namespace Server
    ''' <summary>
    ''' AsynchronousSocketListener is used to asynchronously listen the client and send file to the client.
    ''' </summary>
    Public NotInheritable Class AsynchronousSocketListener
        Private Sub New()
        End Sub
#Region "Constants"

        Private Const c_clientSockets As Integer = 100
        Private Const c_bufferSize As Integer = 5242880

#End Region

#Region "Members"

        Private Shared m_port As Integer
        Private Shared signal As Integer
        Private Shared allDone As New ManualResetEvent(False)
        Private Shared sendDone As New ManualResetEvent(False)
        Private Delegate Sub AddClientHandler(ByVal IpEndPoint As IPEndPoint)
        Private Delegate Sub CompleteSendHandler()
        Private Delegate Sub RemoveItemHandler(ByVal ipAddress As String)
        Private Delegate Sub EnableSendHandler()

#End Region

#Region "Properties"

        Public Shared Property Server() As Server
            Get
                Return m_Server
            End Get
            Set(ByVal value As Server)
                m_Server = Value
            End Set
        End Property
        Private Shared m_Server As Server
        Public Shared WriteOnly Property Port() As String
            Set(ByVal value As String)
                Try
                    m_port = Convert.ToInt32(value)
                Catch generatedExceptionName As FormatException
                    Throw New Exception(My.Resources.InvalidPortMsg)
                Catch ex As OverflowException
                    Throw New Exception(ex.Message)
                End Try

                If m_port < 0 OrElse m_port > 65535 Then
                    Throw New Exception(My.Resources.InvalidPortMsg)
                End If
            End Set
        End Property
        Public Shared Property FileToSend() As String
            Get
                Return m_FileToSend
            End Get
            Set(ByVal value As String)
                m_FileToSend = Value
            End Set
        End Property
        Private Shared m_FileToSend As String
        Public Shared Clients As IList(Of Socket) = New List(Of Socket)()
        Public Shared ClientsToSend As IDictionary(Of Socket, IPEndPoint) = New Dictionary(Of Socket, IPEndPoint)()

#End Region

#Region "Functions"

        ''' <summary>
        ''' Server start to listen the client connection.
        ''' </summary>
        Public Shared Sub StartListening()
            Dim localEndPoint As New IPEndPoint(IPAddress.Any, m_port)

            ' Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
            Dim listener As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            Try
                listener.Bind(localEndPoint)
            Catch ex As SocketException
                MessageBox.Show(ex.Message)
                Return
            End Try

            listener.Listen(c_clientSockets)

            'loop listening the client.
            While True
                allDone.Reset()
                listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), listener)
                allDone.WaitOne()
            End While
        End Sub

        ''' <summary>
        ''' Callback when one client successfully connected to the server.
        ''' </summary>
        ''' <param name="ar"></param>
        Private Shared Sub AcceptCallback(ByVal ar As IAsyncResult)
            Dim listener As Socket = DirectCast(ar.AsyncState, Socket)
            Dim handler As Socket = listener.EndAccept(ar)

            Dim ipEndPoint As IPEndPoint = TryCast(handler.RemoteEndPoint, IPEndPoint)

            If (ipEndPoint) IsNot Nothing Then
                Server.BeginInvoke(New AddClientHandler(AddressOf Server.AddClient), ipEndPoint)
            End If

            Clients.Add(handler)

            allDone.[Set]()
        End Sub

        ''' <summary>
        ''' Send file information to clients.
        ''' </summary>
        Public Shared Sub SendFileInfo()
            Dim fileName As String = FileToSend.Replace("\", "/")
            Dim closedSockets As IList(Of Socket) = New List(Of Socket)()
            Dim removedItems As IList(Of [String]) = New List(Of [String])()

            While fileName.IndexOf("/") > -1
                fileName = fileName.Substring(fileName.IndexOf("/") + 1)
            End While

            Dim fileInfo As New FileInfo(FileToSend)
            Dim fileLen As Long = fileInfo.Length

            Dim fileLenByte As Byte() = BitConverter.GetBytes(fileLen)

            Dim fileNameByte As Byte() = Encoding.ASCII.GetBytes(fileName)

            Dim clientData As Byte() = New Byte(4 + fileNameByte.Length + 7) {}

            Dim fileNameLen As Byte() = BitConverter.GetBytes(fileNameByte.Length)

            fileNameLen.CopyTo(clientData, 0)
            fileNameByte.CopyTo(clientData, 4)
            fileLenByte.CopyTo(clientData, 4 + fileNameByte.Length)

            ' Send the file name and length to the clients. 
            For Each kvp As KeyValuePair(Of Socket, IPEndPoint) In ClientsToSend
                Dim handler As Socket = kvp.Key
                Dim ipEndPoint As IPEndPoint = kvp.Value
                Try
                    handler.Send(clientData)
                Catch
                    If Not handler.Connected Then
                        closedSockets.Add(handler)

                        removedItems.Add(ipEndPoint.ToString())
                    End If
                End Try
            Next

            ' Remove the clients which are disconnected.
            RemoveClient(closedSockets)
            RemoveClientItem(removedItems)
            closedSockets.Clear()
            removedItems.Clear()
        End Sub

        ''' <summary>
        ''' Send file from server to clients.
        ''' </summary>
        Public Shared Sub Send()
            Dim readBytes As Integer = 0
            Dim buffer As Byte() = New Byte(c_bufferSize - 1) {}
            Dim closedSockets As IList(Of Socket) = New List(Of Socket)()
            Dim removedItems As IList(Of [String]) = New List(Of [String])()

            ' Send file information to the clients.
            SendFileInfo()

            ' Blocking read file and send to the clients asynchronously.
            Using stream As New FileStream(FileToSend, FileMode.Open)
                Do
                    sendDone.Reset()
                    signal = 0
                    stream.Flush()
                    readBytes = stream.Read(buffer, 0, c_bufferSize)

                    If ClientsToSend.Count = 0 Then
                        sendDone.[Set]()
                    End If

                    For Each kvp As KeyValuePair(Of Socket, IPEndPoint) In ClientsToSend
                        Dim handler As Socket = kvp.Key
                        Dim ipEndPoint As IPEndPoint = kvp.Value
                        Try
                            handler.BeginSend(buffer, 0, readBytes, SocketFlags.None, New AsyncCallback(AddressOf SendCallback), handler)
                        Catch
                            If Not handler.Connected Then
                                closedSockets.Add(handler)
                                signal += 1
                                removedItems.Add(ipEndPoint.ToString())

                                ' Signal if all the clients are disconnected.
                                If signal >= ClientsToSend.Count Then
                                    sendDone.[Set]()
                                End If
                            End If
                        End Try
                    Next
                    sendDone.WaitOne()

                    ' Remove the clients which are disconnected.
                    RemoveClient(closedSockets)
                    RemoveClientItem(removedItems)
                    closedSockets.Clear()
                    removedItems.Clear()
                Loop While readBytes > 0
            End Using

            ' Disconnect all the connection when the file has send to the clients completely.
            ClientDisconnect()
            CompleteSendFile()
        End Sub

        ''' <summary>
        ''' Callback when a part of the file has been sent to the clients successfully.
        ''' </summary>
        ''' <param name="ar"></param>
        Private Shared Sub SendCallback(ByVal ar As IAsyncResult)
            SyncLock Server
                Dim handler As Socket = Nothing
                Try
                    handler = DirectCast(ar.AsyncState, Socket)
                    signal += 1
                    Dim bytesSent As Integer = handler.EndSend(ar)

                    ' Close the socket when all the data has sent to the client.
                    If bytesSent = 0 Then
                        handler.Shutdown(SocketShutdown.Both)
                        handler.Close()
                    End If
                Catch argEx As ArgumentException
                    MessageBox.Show(argEx.Message)
                Catch generatedExceptionName As SocketException
                    ' Close the socket if the client disconnected.
                    handler.Shutdown(SocketShutdown.Both)
                    handler.Close()
                Finally
                    ' Signal when the file chunk has sent to all the clients successfully. 
                    If signal >= ClientsToSend.Count Then
                        sendDone.[Set]()
                    End If
                End Try
            End SyncLock
        End Sub

        ''' <summary>
        ''' Disconnect all the connection.
        ''' </summary>
        Private Shared Sub ClientDisconnect()
            Clients.Clear()
            ClientsToSend.Clear()
        End Sub

        ''' <summary>
        ''' Change the presentation of listbox when the file send to the clients finished.
        ''' </summary>
        Private Shared Sub CompleteSendFile()
            Server.BeginInvoke(New CompleteSendHandler(AddressOf Server.CompleteSend))
        End Sub

        ''' <summary>
        ''' Change the presentation of listbox when the some clients disconnect the connection.
        ''' </summary>
        ''' <param name="ipAddressList"></param>
        Private Shared Sub RemoveClientItem(ByVal ipAddressList As IList(Of [String]))
            For Each ipAddress As String In ipAddressList
                Server.BeginInvoke(New RemoveItemHandler(AddressOf Server.RemoveItem), ipAddress)
            Next

            If ClientsToSend.Count = 0 Then
                Server.BeginInvoke(New EnableSendHandler(AddressOf Server.EnableSendButton))
            End If
        End Sub

        ''' <summary>
        ''' Remove the sockets if some client disconnect the connection.
        ''' </summary>
        ''' <param name="listSocket"></param>
        Private Shared Sub RemoveClient(ByVal listSocket As IList(Of Socket))
            If listSocket.Count > 0 Then
                For Each socket As Socket In listSocket
                    Clients.Remove(socket)
                    ClientsToSend.Remove(socket)
                Next
            End If
        End Sub

#End Region
    End Class
End Namespace



