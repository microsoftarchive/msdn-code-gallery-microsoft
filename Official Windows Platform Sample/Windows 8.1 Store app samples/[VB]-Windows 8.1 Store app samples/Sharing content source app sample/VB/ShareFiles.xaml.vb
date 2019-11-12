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
Imports SDKTemplate
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml

Partial Public NotInheritable Class ShareFiles
    Inherits SDKTemplate.Common.SharePage

    Private storageItems As IReadOnlyList(Of StorageFile)

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        If storageItems IsNot Nothing Then
            Dim requestData As DataPackage = request.Data
            requestData.Properties.Title = TitleInputBox.Text
            requestData.Properties.Description = DescriptionInputBox.Text ' The description is optional.
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink
            requestData.SetStorageItems(Me.storageItems)
            succeeded = True
        Else
            request.FailWithDisplayText("Select the files you would like to share and try again.")
        End If
        Return succeeded
    End Function

    Private Async Sub SelectFilesButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim filePicker As FileOpenPicker = New FileOpenPicker With {.ViewMode = PickerViewMode.List, .SuggestedStartLocation = PickerLocationId.DocumentsLibrary}
        filePicker.FileTypeFilter.Add("*")

        Dim pickedFiles As IReadOnlyList(Of StorageFile) = Await filePicker.PickMultipleFilesAsync()

        If pickedFiles.Count > 0 Then
            Me.storageItems = pickedFiles

            ' Display the file names in the UI.
            Dim selectedFiles As String = String.Empty
            For index As Integer = 0 To pickedFiles.Count - 1
                selectedFiles &= pickedFiles(index).Name

                If index <> (pickedFiles.Count - 1) Then
                    selectedFiles &= ", "
                End If
            Next index
            Me.rootPage.NotifyUser("Picked files: " & selectedFiles & ".", NotifyType.StatusMessage)

            ShareStep.Visibility = Visibility.Visible
        End If
    End Sub
End Class
