'********************************* Module Header **********************************\
' Module Name:  JustInTimeDataLoading
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to use virtual mode in the DataGridView control 
' with a data cache that loads data from a server only when it is needed. 
' This kind of data loading is called "Just-in-time data loading". 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Imports System.Data.SqlClient
Imports System.Configuration

Namespace VBWinFormDataGridView.JustInTimeDataLoading
    Public Class MainForm
        Private memoryCache As Cache

        ' Specify a connection string. Replace the given value with a
        ' valid connection string for a Northwind SQL Server sample
        ' database accessible to your system.
        Private connectionString As String = ConfigurationManager.AppSettings("connectionString")

        Private table As String = "Orders"

        Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' Enable VirtualMode on the DataGridView
            Me.dataGridView1.VirtualMode = True
            ' Handle the CellValueNeeded event to retrieve the requested cell value
            ' from the data store or the Customer object currently in edit.
            ' This event occurs whenever the DataGridView control needs to paint a cell.

            ' Create a DataRetriever and use it to create a Cache object
            ' and to initialize the DataGridView columns and rows.
            Try
                Dim retriever As DataRetriever = New DataRetriever(connectionString, table)
                memoryCache = New Cache(retriever, 16)
                For Each column As DataColumn In retriever.Columns
                    dataGridView1.Columns.Add(column.ColumnName, column.ColumnName)
                Next
                Me.dataGridView1.RowCount = retriever.RowCount
            Catch ex As Exception
                MessageBox.Show("Connection could not be established. " & _
                    "Verify that the connection string is valid.")
                Application.Exit()
            End Try
        End Sub

        Private Sub dataGridView1_CellValueNeeded(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellValueEventArgs) Handles dataGridView1.CellValueNeeded

            e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex)


        End Sub
    End Class
End Namespace