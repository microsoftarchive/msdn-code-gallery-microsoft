'**************************** Module Header ******************************\
' Module Name:  AutoUpdate.vb
' Project:      VBEFAutoUpdate
' Copyright (c) Microsoft Corporation.
' 
' We can use the Sqldependency to get the notification when the data is changed 
' in database, but EF doesn’t have the same feature. In this sample, we will 
' demonstrate how to automatically update by Sqldependency in Entity Framework.
' In this sample, we will demonstrate two ways that use SqlDependency to get the 
' change notification to auto update data:
' 1. Get the notification of changes immediately.
' 2. Get the notification of changes regularly.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.ComponentModel
Imports System.Data.Entity.Infrastructure
Imports System.Data.Objects
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Security.Permissions
Imports System.Security

Partial Public Class AutoUpdate
    Inherits Form
    Private warehouse As New WarehouseContext()
    Private iquery As IQueryable = Nothing
    Private notification As ImmediateNotificationRegister(Of Product) = Nothing
    Private regularNotificaton As RegularlyNotificationRegister(Of Product) = Nothing
    Private formTimer As Timer = Nothing
    Private interval As Int32
    Private count As Int32

    Private Function CanRequestNotifications() As Boolean
        ' In order to use the callback feature of the
        ' SqlDependency, the application must have
        ' the SqlClientPermission permission.
        Try
            Dim perm As New SqlClientPermission(PermissionState.Unrestricted)

            perm.Demand()

            Return True
        Catch se As SecurityException
            MessageBox.Show(se.Message, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Catch e As Exception
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub AutoUpdate_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        btnGetData.Enabled = CanRequestNotifications()

        CreateDatabaseIfNotExist()

        ' start notification monitor
        ImmediateNotificationRegister(Of Product).StartMonitor(warehouse)
        RegularlyNotificationRegister(Of Product).StartMonitor(warehouse)
    End Sub


    ''' <summary>
    ''' Create the database and insert the data if there's no database
    ''' </summary>
    Private Sub CreateDatabaseIfNotExist()
        If warehouse IsNot Nothing AndAlso (Not warehouse.Database.Exists()) Then
            Dim products() As Product = {
                New Product With {.Name = "Red Bicycle", .Price = CType(805.5, Decimal), .Amount = 1050},
                New Product With {.Name = "White Bicycle", .Price = CType(1049.9, Decimal), .Amount = 312},
                New Product With {.Name = "Black Bicycle", .Price = CType(888.8, Decimal), .Amount = 965}}

            For Each product As Product In products
                warehouse.Products.Add(product)
            Next product

            warehouse.SaveChanges()
        End If
    End Sub

    Private Sub GetData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGetData.Click
        Dim lowPrice, highPrice As Decimal

        If Not Decimal.TryParse(txtLowPrice.Text, lowPrice) Then
            lowPrice = 0
        End If

        If Not Decimal.TryParse(txtHighPrice.Text, highPrice) Then
            highPrice = Decimal.MaxValue
        End If


        ' Create the query.
        iquery = From p In warehouse.Products
                 Where p.Price >= lowPrice AndAlso p.Price <= highPrice
                 Select p

        If Me.rabtnImUpdate.Checked Then
            ' If need to update immediately, use ImmediateNotificationRegister to register 
            ' SqlDependency.
            notification = New ImmediateNotificationRegister(Of Product)(warehouse, iquery)
            AddHandler notification.OnChanged, AddressOf NotificationOnChanged
        Else
            ' We can use RegularlyNotificationRegister to implement update regularly.
            If Int32.TryParse(Me.txtInterval.Text, interval) Then
                regularNotificaton = New RegularlyNotificationRegister(Of Product)(warehouse, iquery, interval * 1000)
                AddHandler regularNotificaton.OnChanged, AddressOf NotificationOnChanged

                ' Only for displaying the progress
                Me.proBar.Value = 100 \ interval
                count = 1
                formTimer = New Timer()
                formTimer.Interval = 1000
                AddHandler formTimer.Tick, AddressOf formTimer_Tick
                formTimer.Start()
            Else
                Return
            End If
        End If

        GetData()

        ChangeButtonState()
    End Sub

    ''' <summary>
    ''' Display progress
    ''' </summary>
    Private Sub formTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        count = count Mod interval + 1
        Me.proBar.Value = (100 * count) / interval
    End Sub

    ''' <summary>
    ''' When changed the data, the method weill be invokced.
    ''' </summary>
		Private Sub NotificationOnChanged(ByVal sender As Object, ByVal e As EventArgs)
        ' If InvokeRequired returns True, the code
        ' is executing on a worker thread.
        If Me.InvokeRequired Then
            ' Create a delegate to perform the thread switch.
            Me.BeginInvoke(New Action(AddressOf GetData), Nothing)

            Return
        End If

        GetData()
    End Sub

    ''' <summary>
    ''' Display the data in the DataGridView
    ''' </summary>
    Private Sub GetData()
        dgvWatch.DataSource = (TryCast(iquery, DbQuery(Of Product))).ToList()
    End Sub

    ''' <summary>
    ''' Update the new price.
    ''' </summary>
    Private Sub Update_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        Dim id As Int32
        Dim newPrice As Decimal

        If Int32.TryParse(txtId.Text, id) AndAlso Decimal.TryParse(txtNewPrice.Text, newPrice) Then
            Dim product As Product = (
                From p In warehouse.Products
                Where p.Id = id
                Select p).FirstOrDefault()

            If product IsNot Nothing Then
                product.Price = newPrice
                warehouse.SaveChanges()
            End If
        Else
            MessageBox.Show("Please input the valid values.")
        End If
    End Sub


    Private Sub dgvWatch_CellMouseClick(ByVal sender As Object,
                                        ByVal e As DataGridViewCellMouseEventArgs) Handles dgvWatch.CellMouseClick
        Dim datagrid As DataGridView = CType(sender, DataGridView)
        Dim products As List(Of Product) = CType(datagrid.DataSource, List(Of Product))

        If e.RowIndex >= 0 AndAlso e.RowIndex < products.Count Then
            Dim product As Product = products(e.RowIndex)

            txtId.Text = product.Id.ToString()
            txtNewPrice.Text = product.Price.ToString()
        End If
    End Sub

    ''' <summary>
    ''' Stop SqlDependency.
    ''' </summary>
    Private Sub StopSqlDependency(ByVal sender As Object, ByVal e As EventArgs) Handles btnStop.Click
        Try
            If notification IsNot Nothing Then
                notification.Dispose()
                notification = Nothing
            End If

            If regularNotificaton IsNot Nothing Then
                regularNotificaton.Dispose()
                regularNotificaton = Nothing
            End If

            If formTimer IsNot Nothing Then
                formTimer.Stop()
                formTimer.Dispose()
                formTimer = Nothing
                Me.proBar.Value = 0
            End If

            ChangeButtonState()
        Catch ex As ArgumentException
            MessageBox.Show(ex.Message, "Paramter Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            If ex.InnerException IsNot Nothing Then
                MessageBox.Show(ex.Message & "(" & ex.InnerException.Message & ")", "Failed to Stop SqlDependency", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show(ex.Message, "Failed to Stop SqlDependency", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    Private Sub ChangeButtonState()
        Dim state As Boolean = Not Me.btnGetData.Enabled

        Me.btnGetData.Enabled = state
        Me.txtLowPrice.Enabled = state
        Me.txtHighPrice.Enabled = state
        Me.rabtnImUpdate.Enabled = state
        Me.rabtnRegUpdate.Enabled = state
        ChangeStateByRadioButton()

        Me.txtId.Enabled = Not state
        Me.txtNewPrice.Enabled = Not state
        Me.btnUpdate.Enabled = Not state

        Me.btnStop.Enabled = Not state
    End Sub

    Private Sub ChangeStateByRadioButton()
        Dim state As Boolean = Me.rabtnRegUpdate.Enabled AndAlso Me.rabtnRegUpdate.Checked

        Me.txtInterval.Enabled = state
        Me.proBar.Enabled = state
    End Sub

    Private Sub rabtnRegUpdate_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rabtnRegUpdate.CheckedChanged
        ChangeStateByRadioButton()
    End Sub

    Private Sub AutoUpdate_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' stop notification monitor
        ImmediateNotificationRegister(Of Product).StopMonitor(warehouse)
        RegularlyNotificationRegister(Of Product).StopMonitor(warehouse)
    End Sub
End Class

