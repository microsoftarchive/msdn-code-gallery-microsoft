'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.Storage.Pickers
Imports Windows.ApplicationModel.DataTransfer
Imports SDKTemplate

Partial Public NotInheritable Class ShareFiles
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current
    Private dataTransferManager As DataTransferManager
    Private storageItems As IReadOnlyList(Of StorageFile)

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Register this page as a share source.
        Me.dataTransferManager = dataTransferManager.GetForCurrentView()
        AddHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ' Unregister this page as a share source.
        RemoveHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Private Sub OnDataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        Dim dataPackageTitle As String = TitleInputBox.Text

        ' The title is required.
        If Not String.IsNullOrEmpty(dataPackageTitle) Then
            If Me.storageItems IsNot Nothing Then
                Dim requestData As DataPackage = e.Request.Data
                requestData.Properties.Title = dataPackageTitle

                ' The description is optional.
                Dim dataPackageDescription As String = DescriptionInputBox.Text
                If dataPackageDescription IsNot Nothing Then
                    requestData.Properties.Description = dataPackageDescription
                End If
                requestData.SetStorageItems(Me.storageItems)
            Else
                e.Request.FailWithDisplayText("Select the files you would like to share and try again.")
            End If
        Else
            e.Request.FailWithDisplayText(MainPage.MissingTitleError)
        End If
    End Sub

    Private Async Sub SelectFilesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim filePicker As New FileOpenPicker() With { _
            .ViewMode = PickerViewMode.List, _
            .SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        }

        filePicker.FileTypeFilter.Add("*")


        Dim pickedFiles As IReadOnlyList(Of StorageFile) = Await filePicker.PickMultipleFilesAsync()

        If pickedFiles.Count > 0 Then
            Me.storageItems = pickedFiles

            ' Display the file names in the UI.
            Dim selectedFiles As String = String.Empty
            For index As Integer = 0 To pickedFiles.Count - 1
                selectedFiles += pickedFiles(index).Name

                If index <> (pickedFiles.Count - 1) Then
                    selectedFiles += ", "
                End If
            Next
            Me.rootPage.NotifyUser("Picked files: " & selectedFiles & ".", NotifyType.StatusMessage)

            ShareStep.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub ShowUIButton_Click(sender As Object, e As RoutedEventArgs)
        dataTransferManager.ShowShareUI()
    End Sub
End Class
