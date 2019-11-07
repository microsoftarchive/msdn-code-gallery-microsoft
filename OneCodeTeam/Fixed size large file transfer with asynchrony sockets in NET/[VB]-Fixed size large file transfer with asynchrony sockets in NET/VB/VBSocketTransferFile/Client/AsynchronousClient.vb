'******************************** Module Header **********************************\
' Module Name:	AsynchronousClient.vb
' Project:		Client
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
Imports System.Linq
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Namespace Client
    ''' <summary>
    ''' AsynchronousClient is used to asynchronously receive the file from server.
    ''' </summary>
    Public NotInheritable Class AsynchronousClient
        Private Sub New()
        End Sub
#Region "Members"

        Private Shared fileName As String
        Private Shared m_fileSavePath As String = "C:/"
        Private Shared fileLen As Long

        Private Shared connectDone As New AutoResetEvent(False)
        Private Shared receiveDone As New ManualResetEvent(False)
        Private Shared connected As Boolean = False

        Private Delegate Sub ProgressChangeHandler()
        Private Delegate Sub FileReceiveDoneHandler()
        Private Delegate Sub ConnectDoneHandler()
        Private Delegate Sub EnableConnectButtonHandler()
        Private Delegate Sub SetProgressLengthHandler(ByVal len As Integer)

#End Region

#Region "Properties"

        Public Shared Property Client() As Client
            Get
                Return m_Client
            End Get
            Set(ByVal value As Client)
                m_Client = Value
            End Set
        End Property
        Private Shared m_Client As Client
        Public Shared Property IpAddress() As IPAddress
            Get
                Return m_IpAddress
            End Get
            Set(ByVal value As IPAddress)
                m_IpAddress = Value
            End Set
        End Property
        Private Shared m_IpAddress As IPAddress
        Public Shared Property Port() As Integer
            Get
                Return m_Port
            End Get
            Set(ByVal value As Integer)
                m_Port = Value
            End Set
        End Property
        Private Shared m_Port As Integer
        Public Shared Property FileSavePath() As String
            Get
                Return m_fileSavePath
            End Get
            Set(ByVal value As String)
                m_fileSavePath = value.Replace("\", "/")
            End Set
        End Property

#End Region

#Region "Functions"

        ''' <summary>
        ''' Start connect to the server.
        ''' </summary>
        Public Shared Sub StartClient()
            connected = False
            If IpAddress Is Nothing Then
                MessageBox.Show(My.Resources.InvalidAddressMsg)
                Return
            End If

            Dim remoteEP As New IPEndPoint(IpAddress, Port)

            ' Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
            Dim clientSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            ' Begin to connect the server.
            clientSocket.BeginConnect(remoteEP, New AsyncCallback(AddressOf ConnectCallback), clientSocket)
            connectDone.WaitOne()

            If connected Then
                ' Begin to receive the file after connecting to server successfully.
                Receive(clientSocket)
                receiveDone.WaitOne()

                ' Notify the user whether receive the file completely.
                Client.BeginInvoke(New FileReceiveDoneHandler(AddressOf Client.FileReceiveDone))

                ' Close the socket.
                clientSocket.Shutdown(SocketShutdown.Both)
                clientSocket.Close()
            Else
                Thread.CurrentThread.Abort()
            End If
        End Sub

        ''' <summary>
        ''' Callback when the client connect to the server successfully.
        ''' </summary>
        ''' <param name="ar"></param>
        Private Shared Sub ConnectCallback(ByVal ar As IAsyncResult)
            Try
                Dim clientSocket As Socket = DirectCast(ar.AsyncState, Socket)

                clientSocket.EndConnect(ar)
            Catch
                MessageBox.Show(My.Resources.InvalidConnectionMsg)
                Client.BeginInvoke(New EnableConnectButtonHandler(AddressOf Client.EnableConnectButton))
                connectDone.[Set]()
                Return
            End Try

            Client.BeginInvoke(New ConnectDoneHandler(AddressOf Client.ConnectDone))
            connected = True
            connectDone.[Set]()
        End Sub

        ''' <summary>
        ''' Receive the file information send by the server.
        ''' </summary>
        ''' <param name="clientSocket"></param>
        Private Shared Sub ReceiveFileInfo(ByVal clientSocket As Socket)
            ' Get the filename length from the server.
            Dim fileNameLenByte As Byte() = New Byte(3) {}
            Try
                clientSocket.Receive(fileNameLenByte)
            Catch
                If Not clientSocket.Connected Then
                    HandleDisconnectException()
                End If
            End Try
            Dim fileNameLen As Integer = BitConverter.ToInt32(fileNameLenByte, 0)

            ' Get the filename from the server.
            Dim fileNameByte As Byte() = New Byte(fileNameLen - 1) {}

            Try
                clientSocket.Receive(fileNameByte)
            Catch
                If Not clientSocket.Connected Then
                    HandleDisconnectException()
                End If
            End Try

            fileName = Encoding.ASCII.GetString(fileNameByte, 0, fileNameLen)

            m_fileSavePath = m_fileSavePath & "/" & fileName

            ' Get the file length from the server.
            Dim fileLenByte As Byte() = New Byte(7) {}
            clientSocket.Receive(fileLenByte)
            fileLen = BitConverter.ToInt64(fileLenByte, 0)
        End Sub

        ''' <summary>
        ''' Receive the file send by the server.
        ''' </summary>
        ''' <param name="clientSocket"></param>
        Private Shared Sub Receive(ByVal clientSocket As Socket)
            Dim state As New StateObject()
            state.workSocket = clientSocket

            ReceiveFileInfo(clientSocket)

            Dim progressLen As Integer = CInt(fileLen / StateObject.BufferSize + 1)
            Dim length As Object() = New Object(0) {}
            length(0) = progressLen
            Client.BeginInvoke(New SetProgressLengthHandler(AddressOf Client.SetProgressLength), length)

            ' Begin to receive the file from the server.
            Try
                clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReceiveCallback), state)
            Catch
                If Not clientSocket.Connected Then
                    HandleDisconnectException()
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Callback when receive a file chunk from the server successfully.
        ''' </summary>
        ''' <param name="ar"></param>
        Private Shared Sub ReceiveCallback(ByVal ar As IAsyncResult)
            Dim state As StateObject = DirectCast(ar.AsyncState, StateObject)
            Dim clientSocket As Socket = state.workSocket
            Dim writer As BinaryWriter

            Dim bytesRead As Integer = clientSocket.EndReceive(ar)
            If bytesRead > 0 Then
                'If the file doesn't exist, create a file with the filename got from server. If the file exists, append to the file.
                If Not File.Exists(m_fileSavePath) Then
                    writer = New BinaryWriter(File.Open(m_fileSavePath, FileMode.Create))
                Else
                    writer = New BinaryWriter(File.Open(m_fileSavePath, FileMode.Append))
                End If

                writer.Write(state.buffer, 0, bytesRead)
                writer.Flush()
                writer.Close()

                ' Notify the progressBar to change the position.
                Client.BeginInvoke(New ProgressChangeHandler(AddressOf Client.ProgressChanged))

                ' Recursively receive the rest file.
                Try
                    clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReceiveCallback), state)
                Catch
                    If Not clientSocket.Connected Then
                        MessageBox.Show(My.Resources.DisconnectMsg)
                    End If
                End Try
            Else
                ' Signal if all the file received.
                receiveDone.[Set]()
            End If
        End Sub

#End Region

#Region "Private Functions"

        ''' <summary>
        ''' Handle the exception when disconnect from the server.
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Sub HandleDisconnectException()
            MessageBox.Show(My.Resources.DisconnectMsg)
            Client.BeginInvoke(New EnableConnectButtonHandler(AddressOf Client.EnableConnectButton))
            Thread.CurrentThread.Abort()
        End Sub

#End Region
    End Class
End Namespace


