'******************************** Module Header **********************************\
' Module Name:	Client.vb
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
'**********************************************************************************/


Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Linq
Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Namespace Client
    ''' <summary>
    ''' Client can connect to the server.
    ''' </summary>
    Partial Public Class Client
        Inherits Form
#Region "Constructor"

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region

#Region "Button Event"

        ''' <summary>
        ''' Connect the server with the given ip address and port.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConnect.Click
            Dim port As Integer
            Dim _ipAddress As IPAddress

            If String.IsNullOrEmpty(tbxAddress.Text) OrElse String.IsNullOrEmpty(tbxPort.Text) Then
                MessageBox.Show(Me, My.Resources.IsEmptyMsg)
                Return
            End If

            Try
                _ipAddress = IPAddress.Parse(tbxAddress.Text)
            Catch
                MessageBox.Show(Me, My.Resources.InvalidAddressMsg)
                Return
            End Try

            Try
                port = Convert.ToInt32(tbxPort.Text)
            Catch
                MessageBox.Show(Me, My.Resources.InvalidPortMsg)
                Return
            End Try

            If port < 0 OrElse port > 65535 Then
                MessageBox.Show(Me, My.Resources.InvalidPortMsg)
                Return
            End If

            If String.IsNullOrEmpty(tbxSavePath.Text) Then
                MessageBox.Show(Me, My.Resources.EmptyPath)
                Return
            End If

            AsynchronousClient.IpAddress = _ipAddress
            AsynchronousClient.Port = port
            AsynchronousClient.FileSavePath = tbxSavePath.Text
            AsynchronousClient.Client = Me

            Dim threadClient As New Thread(New ThreadStart(AddressOf AsynchronousClient.StartClient))
            threadClient.IsBackground = True
            threadClient.Start()
            Me.btnConnect.Enabled = False
        End Sub

        ''' <summary>
        ''' Set the path to store the file sent from the server.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub btnSavePath_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSavePath.Click
            Dim path As New FolderBrowserDialog()
            path.ShowDialog()
            Me.tbxSavePath.Text = path.SelectedPath
        End Sub

#End Region

#Region "Change the progressBar"

        ''' <summary>
        ''' Set the progress length of the progressBar
        ''' </summary>
        ''' <param name="len"></param>
        Public Sub SetProgressLength(ByVal len As Integer)
            progressBar.Minimum = 0
            progressBar.Maximum = len
            progressBar.Value = 0
            progressBar.[Step] = 1
        End Sub

        ''' <summary>
        ''' Change the position of the progressBar
        ''' </summary>
        Public Sub ProgressChanged()
            progressBar.PerformStep()
        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Notify the user when receive the file completely.
        ''' </summary>
        Public Sub FileReceiveDone()
            MessageBox.Show(Me, My.Resources.FileReceivedDoneMsg)
        End Sub

        ''' <summary>
        ''' Notify the user when connect to the server successfully.
        ''' </summary>
        Public Sub ConnectDone()
            MessageBox.Show(Me, My.Resources.ConnectionMsg)
        End Sub

        ''' <summary>
        ''' Enable the Connect button if failed to connect the sever. 
        ''' </summary>
        Public Sub EnableConnectButton()
            btnConnect.Enabled = True
        End Sub

#End Region
    End Class
End Namespace

