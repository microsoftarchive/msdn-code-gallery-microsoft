'********************************* Module Header **********************************\
' Module Name:  DataGridViewPaging.vb
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to page data in the  DataGridView control;
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.

' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Imports System.Data.SqlClient
Imports System.Configuration


Namespace VBWinFormDataGridView.DataGridViewPaging

    Public Class MainForm
        Private PageSize As Integer = 30
        Private CurrentPageIndex As Integer = 1
        Private TotalPage As Integer
        Private connstr As String = ConfigurationManager.AppSettings("connectionString").ToString()
        Private conn As SqlConnection
        Private adapter As SqlDataAdapter
        Private command As SqlCommand

        Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.conn = New SqlConnection(connstr)
            Me.adapter = New SqlDataAdapter()
            Me.command = conn.CreateCommand()

            ' Get total count of the pages
            Me.GetTotalPageCount()
            Me.dataGridView1.ReadOnly = True

            ' Load the first page of data
            Me.dataGridView1.DataSource = GetPageData(1)
        End Sub

        Private Sub GetTotalPageCount()
            command.CommandText = "Select Count(OrderID) From Orders"
            Try
                conn.Open()
                Dim rowCount As Integer = CType(command.ExecuteScalar(), Integer)
                Me.TotalPage = rowCount / PageSize
                If rowCount Mod PageSize > 0 Then
                    Me.TotalPage += 1
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                conn.Close()
            End Try
        End Sub

        Private Function GetPageData(ByVal page As Integer) As DataTable
            Dim dt As DataTable = New DataTable()
            If page = 1 Then
                command.CommandText = "Select Top " & PageSize & " * From Orders Order By OrderID"
            Else
                Dim lowerPageBoundary = (page - 1) * PageSize
                command.CommandText = "Select Top " & PageSize _
                & " * From Orders " _
                & " WHERE OrderID NOT IN " _
                & " (SELECT TOP " & lowerPageBoundary _
                & " OrderID From Orders Order By OrderID) " _
                & " Order By OrderID"

            End If
            Try
                Me.conn.Open()
                Me.adapter.SelectCommand = command
                Me.adapter.Fill(dt)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                conn.Close()
            End Try
            Return dt
        End Function

        Private Sub toolStripButtonFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles toolStripButtonFirst.Click
            Me.CurrentPageIndex = 1
            Me.dataGridView1.DataSource = GetPageData(Me.CurrentPageIndex)
        End Sub

        Private Sub toolStripButtonPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles toolStripButtonPrev.Click
            If Me.CurrentPageIndex > 1 Then
                Me.CurrentPageIndex -= 1
                Me.dataGridView1.DataSource = GetPageData(Me.CurrentPageIndex)
            End If
        End Sub

        Private Sub toolStripButtonNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles toolStripButtonNext.Click
            If Me.CurrentPageIndex < Me.TotalPage Then
                Me.CurrentPageIndex += 1
                Me.dataGridView1.DataSource = GetPageData(Me.CurrentPageIndex)
            End If
        End Sub

        Private Sub toolStripButtonLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles toolStripButtonLast.Click
            Me.CurrentPageIndex = TotalPage
            Me.dataGridView1.DataSource = GetPageData(Me.CurrentPageIndex)
        End Sub
    End Class

End Namespace