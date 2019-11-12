'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Windows.Storage
Imports Windows.Storage.Search
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler SearchButton.Click, AddressOf SearchButton_Click
    End Sub

    Private Async Sub SearchButton_Click(sender As Object, e As RoutedEventArgs)
        Dim musicFolder As StorageFolder = KnownFolders.MusicLibrary

        Dim fileTypeFilter As New List(Of String)
        fileTypeFilter.Add("*")

        Dim queryOptions As New QueryOptions(CommonFileQuery.OrderBySearchRank, fileTypeFilter)
        'use the user's input to make a query
        queryOptions.UserSearchFilter = InputTextBox.Text
        Dim queryResult As StorageFileQueryResult = musicFolder.CreateFileQueryWithOptions(queryOptions)

        Dim outputText As New StringBuilder

        'find all files that match the query
        Dim files As IReadOnlyList(Of StorageFile) = Await queryResult.GetFilesAsync()
        'output how many files that match the query were found
        If files.Count = 0 Then
            outputText.Append("No files found for '" & queryOptions.UserSearchFilter & "'")
        ElseIf files.Count = 1 Then
            outputText.Append(files.Count & " file found:" & vbLf & vbLf)
        Else
            outputText.Append(files.Count & " files found:" & vbLf & vbLf)
        End If

        'output the name of each file that matches the query
        For Each file As StorageFile In files
            outputText.Append(file.Name & vbLf)
        Next

        OutputTextBlock.Text = outputText.ToString
    End Sub
End Class
