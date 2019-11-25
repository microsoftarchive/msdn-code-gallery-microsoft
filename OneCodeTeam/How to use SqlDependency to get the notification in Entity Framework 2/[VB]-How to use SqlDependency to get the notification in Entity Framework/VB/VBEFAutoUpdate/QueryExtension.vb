'**************************** Module Header ******************************\
' Module Name:  QueryExtension.vb
' Project:      VBEFAutoUpdate
' Copyright (c) Microsoft Corporation.
' 
' We can use the Sqldependency to get the notification when the data is changed 
' in database, but EF doesn’t have the same feature. In this sample, we will 
' demonstrate how to automatically update by Sqldependency in Entity Framework.
' In this sample, we will demonstrate two ways that use SqlDependency to get the 
' change notification to auto update data.
' This class contains some methods to extend the EF.
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
Imports System.Data.Entity.Infrastructure
Imports System.Data.EntityClient
Imports System.Data.Objects
Imports System.Data.SqlClient
Imports System.Linq

Public NotInheritable Class QueryExtension
    ''' <summary>
    ''' Return the ObjectQuery directly or convert the DbQuery to ObjectQuery.
    ''' </summary>
    Private Sub New()
    End Sub
    Public Shared Function GetObjectQuery(Of TEntity As Class)(ByVal Context As DbContext, ByVal query As IQueryable) As ObjectQuery
        If TypeOf query Is ObjectQuery Then
            Return TryCast(query, ObjectQuery)
        End If

        If Context Is Nothing Then
            Throw New ArgumentException("Paramter cannot be null", "context")
        End If

        ' Use the DbContext to create the ObjectContext
        Dim objectContext As ObjectContext = (CType(Context, IObjectContextAdapter)).ObjectContext
        ' Use the DbSet to create the ObjectSet and get the appropriate provider.
        Dim iqueryable As IQueryable = TryCast(objectContext.CreateObjectSet(Of TEntity)(), IQueryable)
        Dim provider As IQueryProvider = iqueryable.Provider

        ' Use the provider and expression to create the ObjectQuery.
        Return TryCast(provider.CreateQuery(query.Expression), ObjectQuery)
    End Function

    ''' <summary>
    ''' Use ObjectQuery to get SqlConnection and SqlCommand.
    ''' </summary>
    Public Shared Sub GetSqlCommand(ByVal query As ObjectQuery, ByRef connection As SqlConnection, ByRef command As SqlCommand)
        If query Is Nothing Then
            Throw New System.ArgumentException("Paramter cannot be null", "query")
        End If

        If connection Is Nothing Then
            connection = New SqlConnection(QueryExtension.GetConnectionString(query))
        End If

        If command Is Nothing Then
            command = New SqlCommand(QueryExtension.GetSqlString(query), connection)

            ' Add all the parameters used in query.
            For Each parameter As ObjectParameter In query.Parameters
                command.Parameters.AddWithValue(parameter.Name, parameter.Value)
            Next parameter
        End If
    End Sub

    ''' <summary>
    ''' Use ObjectQuery to get the connection string.
    ''' </summary>
    Public Shared Function GetConnectionString(ByVal query As ObjectQuery) As String
        If query Is Nothing Then
            Throw New ArgumentException("Paramter cannot be null", "query")
        End If

        Dim connection As EntityConnection = TryCast(query.Context.Connection, EntityConnection)
        Return connection.StoreConnection.ConnectionString
    End Function

    ''' <summary>
    ''' Use ObjectQuery to get the Sql string.
    ''' </summary>
    Public Shared Function GetSqlString(ByVal query As ObjectQuery) As String
        If query Is Nothing Then
            Throw New ArgumentException("Paramter cannot be null", "query")
        End If

        Dim s As String = query.ToTraceString()

        Return s
    End Function

End Class

