'******************************** Module Header **********************************\
' Module Name:	Server.vb
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
Imports System.ComponentModel
Imports System.Drawing
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Namespace Server
    ''' <summary>
    ''' The server used to listen the client connection and send file to them. 
    ''' </summary>
    Partial Public Class Server
        Inherits Form

        Private Shared HasStartup As Boolean = False


#Region "Constructor"

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region

#Region "Button Event"

        Private Sub btnStartup_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStartup.Click
            If Not HasStartup Then
                Try
                    AsynchronousSocketListener.Port = tbxPort.Text
                Catch ex As Exception
                    MessageBox.Show(Me, ex.Message)
                    Return
                End Try
                AsynchronousSocketListener.Server = Me
                Dim listener As New Thread(New ThreadStart(AddressOf AsynchronousSocketListener.StartListening))
                listener.IsBackground = True
                listener.Start()
                HasStartup = True
            End If

            MessageBox.Show(Me, My.Resources.StartupMsg)
        End Sub

        Private Sub btnSelectFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectFile.Click
            OpenFileDialog.Multiselect = False

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then
                tbxFile.Text = OpenFileDialog.FileName
                AsynchronousSocketListener.FileToSend = tbxFile.Text
            End If

        End Sub

        Private Sub btnSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
            If lbxServer.SelectedItems.Count = 0 Then
                MessageBox.Show(Me, My.Resources.SelectClientMsg)
                Return
            End If

            If String.IsNullOrEmpty(tbxFile.Text) Then
                MessageBox.Show(Me, My.Resources.EmptyFilePathMsg)
                Return
            End If

            If AsynchronousSocketListener.Clients.Count = 0 Then
                MessageBox.Show(Me, My.Resources.ConnectionMsg)
                Return
            End If

            For Each item As Object In lbxServer.SelectedItems
                For Each handler As Socket In AsynchronousSocketListener.Clients
                    Dim ipEndPoint As IPEndPoint = DirectCast(handler.RemoteEndPoint, IPEndPoint)
                    Dim address As String = ipEndPoint.ToString()
                    If String.Equals(item.ToString(), address, StringComparison.OrdinalIgnoreCase) Then
                        AsynchronousSocketListener.ClientsToSend.Add(handler, ipEndPoint)
                        Exit For
                    End If
                Next
            Next

            Dim sendThread As New Thread(New ThreadStart(AddressOf AsynchronousSocketListener.Send))
            sendThread.IsBackground = True
            sendThread.Start()
            btnSend.Enabled = False

        End Sub

#End Region

#Region "ListBox item change functions"

        ''' <summary>
        ''' Add the client address to lbxClient.
        ''' </summary>
        ''' <param name="IpEndPoint"></param>
        Public Sub AddClient(ByVal IpEndPoint As IPEndPoint)
            lbxServer.BeginUpdate()
            lbxServer.Items.Add(IpEndPoint.ToString())
            lbxServer.EndUpdate()
        End Sub

        ''' <summary>
        ''' Clear the content of lbxClient.
        ''' </summary>
        Public Sub CompleteSend()
            While lbxServer.SelectedIndices.Count > 0
                lbxServer.Items.RemoveAt(lbxServer.SelectedIndices(0))
            End While
            btnSend.Enabled = True
        End Sub

        ''' <summary>
        ''' Remove the client address which disconnect from the server.
        ''' </summary>
        ''' <param name="ipAddress"></param>
        Public Sub RemoveItem(ByVal ipAddress As String)
            Dim index As Integer = 0
            Dim flag As Boolean = False

            For Each item As Object In lbxServer.SelectedItems
                If Not String.Equals(item.ToString(), ipAddress, StringComparison.OrdinalIgnoreCase) Then
                    index += 1
                Else
                    flag = True
                    Exit For
                End If
            Next

            If flag Then
                lbxServer.Items.RemoveAt(index)
            End If
        End Sub

        ''' <summary>
        ''' Enable the Send button.
        ''' </summary>
        Public Sub EnableSendButton()
            btnSend.Enabled = True
        End Sub

#End Region
    End Class
End Namespace

