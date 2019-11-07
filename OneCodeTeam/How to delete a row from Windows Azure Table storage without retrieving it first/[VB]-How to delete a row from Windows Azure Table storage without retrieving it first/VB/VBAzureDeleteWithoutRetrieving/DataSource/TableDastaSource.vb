'***************************** Module Header ******************************\
' Module Name:	TableDataSource.vb
' Project:		VBAzureDeleteWithoutRetrieving
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to reduce the request to Azure storage service.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports Microsoft.WindowsAzure.Storage.RetryPolicies
Imports Microsoft.WindowsAzure.Storage.Table
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Auth

Public NotInheritable Class TableDataSource
    Private Sub New()
    End Sub
    Public Shared ReadOnly cred As New StorageCredentials("{Account-Name}", "{Account-Key}")

    Private Shared tableName As String = "CSAzureDeleteWithoutRetrievingSampleTable"
    Private Shared conflictRetryPolicy As IRetryPolicy = New ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10)
    Private Shared client As New CloudTableClient(New Uri(String.Format("http://{0}.table.core.windows.net", cred.AccountName)), cred) With { _
         .RetryPolicy = conflictRetryPolicy _
    }


    Public Shared Async Function GetTableEntities() As Task(Of ObservableCollection(Of DynamicTableEntity))
        Dim table = client.GetTableReference(tableName)
        Dim query As New TableQuery()

        Dim results = Await table.ExecuteQuerySegmentedAsync(query, Nothing)
        Dim itemList As New ObservableCollection(Of DynamicTableEntity)()
        For Each item In results
            itemList.Add(DirectCast(item, DynamicTableEntity))
        Next
        Return itemList
    End Function

    Public Shared Async Function DeleteTable() As Task(Of Boolean)
        Try
            Dim table = client.GetTableReference(tableName)
            Dim requestOptions As New TableRequestOptions() With { _
                 .RetryPolicy = conflictRetryPolicy _
            }

            Await table.DeleteIfExistsAsync(requestOptions, Nothing)
            Return True
        Catch generatedExceptionName As Exception
            Throw
        End Try

    End Function
    Public Shared Async Function CreateSampleData() As Task(Of ObservableCollection(Of DynamicTableEntity))
        Dim table = client.GetTableReference(tableName)
        Try
            Dim requestOptions As New TableRequestOptions() With { _
                 .RetryPolicy = conflictRetryPolicy _
            }
            If Await table.CreateIfNotExistsAsync(requestOptions, Nothing) Then
                Dim bacthOperation As New TableBatchOperation()
                Dim entityList As ObservableCollection(Of DynamicTableEntity) = SampleData
                For Each entity In entityList
                    bacthOperation.Insert(entity)
                Next
                Await table.ExecuteBatchAsync(bacthOperation)
                Return entityList
            Else
                Return Await (GetTableEntities())
            End If
        Catch e As Exception
            Throw e
        End Try
    End Function

    Private Shared ReadOnly Property SampleData() As ObservableCollection(Of DynamicTableEntity)
        Get
            Dim entityList As New ObservableCollection(Of DynamicTableEntity)()
            Dim entity As DynamicTableEntity
            For i As Integer = 0 To 9
                Dim dic As New Dictionary(Of String, EntityProperty)()
                dic.Add("Time", EntityProperty.GeneratePropertyForDateTimeOffset(New DateTime(2001, 1, 1)))
                dic.Add("ToDoItem", EntityProperty.GeneratePropertyForString("Do the case" & i))

                entity = New DynamicTableEntity("Partition1", "Row" & i, DateTime.Now.ToString(), dic)
                entityList.Add(entity)
            Next
            Return entityList
        End Get
    End Property

    Public Shared Async Function DeleteEntity(entity As DynamicTableEntity) As Task(Of Boolean)
        Try
            Dim table = client.GetTableReference(tableName)

            Dim deleteOperation As TableOperation = TableOperation.Delete(entity)
            Dim result = Await table.ExecuteAsync(deleteOperation)

            Return True
        Catch e As Exception
            ' In windows store app, StorageException is an internal class.
            ' You can't convert Exception to StorageException, so you should use 
            ' RequestResult.TranslateFromExceptionMessage(e.Message) get the HttpStatusCode.
            Dim result = RequestResult.TranslateFromExceptionMessage(e.Message)

            ' We treat 404 as it has already been deleted.
            If result.HttpStatusCode = 404 Then
                Return True
            Else
                ' Throw new WebException(result.HttpStatusMessage);
                Return False
            End If
        End Try

    End Function

    Public Shared Async Function DeleteEntities(entities As ObservableCollection(Of DynamicTableEntity)) As Task(Of Boolean)
        If entities.Count > 0 Then
            Dim bacthOperation As New TableBatchOperation()

            Try
                Dim table = client.GetTableReference(tableName)


                For Each entity In entities
                    bacthOperation.Delete(entity)
                Next
                Await table.ExecuteBatchAsync(bacthOperation)

                Return True
            Catch e As Exception
                Dim result = RequestResult.TranslateFromExceptionMessage(e.Message)

                Return False
            End Try
        Else
            Return True
        End If

    End Function
End Class