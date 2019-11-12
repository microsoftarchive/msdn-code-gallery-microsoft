'***************************** Module Header ******************************\
' Module Name:	MainPage.xaml.vb
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
Imports Microsoft.WindowsAzure.Storage.Table

' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238


Partial Public NotInheritable Class MainPage
    Inherits Page
    Private tablelist As New ObservableCollection(Of DynamicTableEntity)()

    Public Sub New()
        Me.InitializeComponent()
        Onload()
    End Sub

    Private Async Function Onload() As Task
        tablelist = Await TableDataSource.CreateSampleData()
        lstEntities.ItemsSource = tablelist
    End Function

    Private Async Function btnDelete_Click(sender As Object, e As RoutedEventArgs) As Task
        Dim entity = DirectCast(lstEntities.SelectedItem, DynamicTableEntity)
        If Await TableDataSource.DeleteEntity(entity) Then
            tablelist.Remove(entity)
            lstEntities.ItemsSource = tablelist
        End If
    End Function

    Private Async Function btnDeleteAll_Click(sender As Object, e As RoutedEventArgs) As Task
        If Await TableDataSource.DeleteEntities(tablelist) Then
            tablelist = Nothing
            lstEntities.ItemsSource = Nothing
        End If
    End Function

    'This will take a long time, because it require to delete the Table first, 
    'then recreate it and add data to it.
    Private Async Function btnRegenerate_Click(sender As Object, e As RoutedEventArgs) As Task
        btnDelete.IsEnabled = False
        btnDeleteAll.IsEnabled = False
        btnRegenerate.IsEnabled = False

        If Await TableDataSource.DeleteTable() Then
            tablelist = Await TableDataSource.CreateSampleData()

            lstEntities.ItemsSource = tablelist
        End If
        btnDelete.IsEnabled = True
        btnDeleteAll.IsEnabled = True
        btnRegenerate.IsEnabled = True
    End Function

End Class