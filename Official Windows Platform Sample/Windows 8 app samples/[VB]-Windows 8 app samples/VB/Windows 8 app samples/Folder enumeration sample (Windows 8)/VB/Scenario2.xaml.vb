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
Imports System.Threading.Tasks
Imports Windows.Storage
Imports Windows.Storage.Search
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        Me.InitializeComponent()
        AddHandler GroupByMonthButton.Click, AddressOf GroupByMonth_Click
        AddHandler GroupByRatingButton.Click, AddressOf GroupByRating_Click
        AddHandler GroupByTagButton.Click, AddressOf GroupByTag_Click
    End Sub

    ''' <summary>
    ''' list all the files and folders in Pictures library by month
    ''' </summary>
    Private Async Sub GroupByMonth_Click(sender As Object, e As RoutedEventArgs)
        Await GroupByHelperAsync(New QueryOptions(CommonFolderQuery.GroupByMonth))
    End Sub

    ''' <summary>
    ''' list all the files and folders in Pictures library by rating
    ''' </summary>
    Private Async Sub GroupByRating_Click(sender As Object, e As RoutedEventArgs)
        Await GroupByHelperAsync(New QueryOptions(CommonFolderQuery.GroupByRating))
    End Sub

    ''' <summary>
    ''' list all the files and folders in Pictures library by tag
    ''' </summary>
    Private Async Sub GroupByTag_Click(sender As Object, e As RoutedEventArgs)
        Await GroupByHelperAsync(New QueryOptions(CommonFolderQuery.GroupByTag))
    End Sub

    ''' <summary>
    ''' helper for all list by functions
    ''' </summary>
    Private Async Function GroupByHelperAsync(queryOptions As QueryOptions) As Task
        OutputPanel.Children.Clear()

        Dim picturesFolder As StorageFolder = KnownFolders.PicturesLibrary
        Dim queryResult As StorageFolderQueryResult = picturesFolder.CreateFolderQueryWithOptions(queryOptions)

        Dim folderList As IReadOnlyList(Of StorageFolder) = Await queryResult.GetFoldersAsync()
        For Each folder As StorageFolder In folderList
            Dim fileList As IReadOnlyList(Of StorageFile) = Await folder.GetFilesAsync()
            OutputPanel.Children.Add(CreateHeaderTextBlock(folder.Name & " (" & fileList.Count & ")"))
            For Each file As StorageFile In fileList
                OutputPanel.Children.Add(CreateLineItemTextBlock(file.Name))
            Next
        Next
    End Function

    Private Function CreateHeaderTextBlock(contents As String) As TextBlock
        Dim textBlock As New TextBlock()
        textBlock.Text = contents
        textBlock.Style = DirectCast(Application.Current.Resources("H2Style"), Style)
        textBlock.TextWrapping = TextWrapping.Wrap
        Return textBlock
    End Function

    Private Function CreateLineItemTextBlock(contents As String) As TextBlock
        Dim textBlock As New TextBlock()
        textBlock.Text = contents
        textBlock.Style = DirectCast(Application.Current.Resources("BasicTextStyle"), Style)
        textBlock.TextWrapping = TextWrapping.Wrap
        Dim margin As Thickness = textBlock.Margin
        margin.Left = 20
        textBlock.Margin = margin
        Return textBlock
    End Function
End Class
