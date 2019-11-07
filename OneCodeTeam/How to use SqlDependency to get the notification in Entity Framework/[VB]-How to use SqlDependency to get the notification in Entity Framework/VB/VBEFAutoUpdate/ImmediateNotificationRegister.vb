'**************************** Module Header ******************************\
' Module Name:  ChangeNotificationRegister.vb
' Project:      VBEFAutoUpdate
' Copyright (c) Microsoft Corporation.
' 
' We can use the Sqldependency to get the notification when the data is changed 
' in database, but EF doesn’t have the same feature. In this sample, we will 
' demonstrate how to automatically update by Sqldependency in Entity Framework.
' In this sample, we will demonstrate two ways that use SqlDependency to get the 
' change notification to auto update data.
' We can get the notification immediately by this class when the data changed.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Imports System.Data.Entity
Imports System.Data.Objects
Imports System.Data.SqlClient
Imports System.Linq

Public Class ImmediateNotificationRegister(Of TEntity As Class)
    Implements IDisposable

    Private conn As SqlConnection = Nothing
    Private cmd As SqlCommand = Nothing
    Private iquery As IQueryable = Nothing
    Private objectQuery As ObjectQuery = Nothing


    ' Summary:
    '     Occurs when a notification is received for any of the commands associated
    '     with this ImmediateNotificationRegister object.
    Public Event OnChanged As EventHandler
    Private dependency As SqlDependency = Nothing

    ''' <summary>
    ''' Initializes a new instance of ImmediateNotificationRegister class.
    ''' </summary>
    ''' <param name="query">an instance of ObjectQuery is used to get connection string and command 
    ''' string to register SqlDependency nitification.</param>
    Public Sub New(ByVal query As ObjectQuery)
        Try
            Me.objectQuery = query

            QueryExtension.GetSqlCommand(objectQuery, conn, cmd)

            RegisterSqlDependency()
        Catch ex As ArgumentException
            Throw New ArgumentException("Paramter cannot be null", "query", ex)
        Catch ex As Exception
            Throw New Exception("Fails to initialize a new instance of ImmediateNotificationRegister class.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes a new instance of ImmediateNotificationRegister class.
    ''' </summary>
    ''' <param name="context">an instance of DbContext is used to get an ObjectQuery object</param>
    ''' <param name="query">an instance of IQueryable is used to get ObjectQuery object, and then get 
    ''' connection string and command string to register SqlDependency nitification.</param>
    Public Sub New(ByVal context As DbContext, ByVal query As IQueryable)
        Try
            Me.iquery = query

            ' Get the ObjectQuery directly or convert the DbQuery to ObjectQuery.
            objectQuery = QueryExtension.GetObjectQuery(Of TEntity)(context, iquery)

            QueryExtension.GetSqlCommand(objectQuery, conn, cmd)

            RegisterSqlDependency()
        Catch ex As ArgumentException
            If ex.ParamName = "context" Then
                Throw New ArgumentException("Paramter cannot be null", "context", ex)
            Else
                Throw New ArgumentException("Paramter cannot be null", "query", ex)
            End If
        Catch ex As Exception
            Throw New Exception("Fails to initialize a new instance of ImmediateNotificationRegister class.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Starts the notification of SqlDependency
    ''' </summary>
    ''' <param name="context">An instance of dbcontext</param>
    Public Shared Sub StartMonitor(ByVal context As DbContext)
        Try
            SqlDependency.Start(context.Database.Connection.ConnectionString)
        Catch ex As Exception
            Throw New Exception("Fails to Start the SqlDependency in the ImmediateNotificationRegister class.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Stops the notification of SqlDependency
    ''' </summary>
    ''' <param name="context">An instance of dbcontext</param>
    Public Shared Sub StopMonitor(ByVal context As DbContext)
        Try
            SqlDependency.Stop(context.Database.Connection.ConnectionString)
        Catch ex As Exception
            Throw New Exception("Fails to Stop the SqlDependency in the ImmediateNotificationRegister class.", ex)
        End Try
    End Sub

    Private Sub RegisterSqlDependency()
        If Command Is Nothing OrElse Connection Is Nothing Then
            Throw New ArgumentException("command and connection cannot be null")
        End If

        ' Make sure the command object does not already have
        ' a notification object associated with it.
        cmd.Notification = Nothing

        ' Create and bind the SqlDependency object to the command object.
        dependency = New SqlDependency(cmd)
        AddHandler dependency.OnChange, AddressOf DependencyOnChange

        ' After register SqlDependency, the SqlCommand must be executed, or we can't 
        ' get the notification.
        RegisterSqlCommand()
    End Sub

    Private Sub DependencyOnChange(ByVal sender As Object, ByVal e As SqlNotificationEventArgs)
        ' Move the original SqlDependency event handler.
        Dim dependency As SqlDependency = CType(sender, SqlDependency)
        RemoveHandler dependency.OnChange, AddressOf DependencyOnChange

        RaiseEvent OnChanged(Me, Nothing)

        ' We re-register the SqlDependency.
        RegisterSqlDependency()
    End Sub

    Private Sub RegisterSqlCommand()
        If conn IsNot Nothing AndAlso cmd IsNot Nothing Then
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
        End If
    End Sub

    ''' <summary>
    ''' Releases all the resources by the ImmediateNotificationRegister.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Sub Dispose(ByVal disposed As Boolean)
        If disposed Then
            If cmd IsNot Nothing Then
                cmd.Dispose()
                cmd = Nothing
            End If

            If conn IsNot Nothing Then
                conn.Dispose()
                conn = Nothing
            End If

            iquery = Nothing
            RemoveHandler dependency.OnChange, AddressOf DependencyOnChange
            dependency = Nothing

        End If
    End Sub

    ''' <summary>
    ''' The SqlConnection is got from the Query.
    ''' </summary>
    Public ReadOnly Property Connection() As SqlConnection
        Get
            Return conn
        End Get
    End Property

    ''' <summary>
    ''' The SqlCommand is got from the Query.
    ''' </summary>
    Public ReadOnly Property Command() As SqlCommand
        Get
            Return cmd
        End Get
    End Property

    ''' <summary>
    ''' The ObjectQuery is got from the Query.
    ''' </summary>
    Public ReadOnly Property Oquery() As ObjectQuery
        Get
            Return objectQuery
        End Get
    End Property
End Class

