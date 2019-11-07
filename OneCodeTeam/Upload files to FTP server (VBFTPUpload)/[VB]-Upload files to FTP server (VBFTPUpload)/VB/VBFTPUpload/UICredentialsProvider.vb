'*************************** Module Header ******************************'
' Module Name:  UICredentialsProvider.vb
' Project:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' The Form UICredentialsProvider contains 3 textboxes that accept UserName, 
' Password and Domain to initialize a NetworkCredential instance.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Net

Partial Public Class UICredentialsProvider
    Inherits Form
    Public Property Credentials() As NetworkCredential

    Private _useOriginalCredentials As Boolean = True

    Public Sub New()
        Me.New(Nothing)
    End Sub


    Public Sub New(ByVal credentials As NetworkCredential)

        InitializeComponent()

        Me.Credentials = credentials

        If Me.Credentials IsNot Nothing Then
            Me.tbUserName.Text = Me.Credentials.UserName
            Me.tbDomain.Text = Me.Credentials.Domain
            Me.tbPassword.Text = Me.Credentials.Password
            _useOriginalCredentials = True
        Else
            _useOriginalCredentials = False
        End If

    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnOK.Click

        If Not _useOriginalCredentials Then
            If chkAnonymous.Checked Then

                ' Use Anonymous Credentials by default.
                Credentials = New NetworkCredential("Anonymous", "")

            ElseIf String.IsNullOrWhiteSpace(tbUserName.Text) _
                OrElse String.IsNullOrWhiteSpace(tbPassword.Text) Then
                MessageBox.Show("Please type the user name and password!")
                Return
            Else
                Credentials = New NetworkCredential(
                    tbUserName.Text.Trim(),
                    tbPassword.Text,
                    tbDomain.Text.Trim())
            End If
        End If
        Me.DialogResult = System.Windows.Forms.DialogResult.OK

    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub chkAnonymous_CheckedChanged(ByVal sender As System.Object,
                                            ByVal e As System.EventArgs) _
                                        Handles chkAnonymous.CheckedChanged
        tbDomain.Enabled = Not chkAnonymous.Checked
        tbPassword.Enabled = Not chkAnonymous.Checked
        tbUserName.Enabled = Not chkAnonymous.Checked

        _useOriginalCredentials = False

    End Sub

   
    Private Sub tb_TextChanged(ByVal sender As System.Object,
                                       ByVal e As System.EventArgs) _
        Handles tbUserName.TextChanged, tbPassword.TextChanged, tbDomain.TextChanged

        _useOriginalCredentials = False

    End Sub

End Class
