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
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class CopyFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.current

    Public Sub New()
        Me.InitializeComponent()
        Me.Init()
    End Sub

#Region "Scenario Specific Code"

    Private Sub Init()
        AddHandler CopyButton.Click, AddressOf CopyButton_Click
        AddHandler PasteButton.Click, AddressOf PasteButton_Click
    End Sub

#End Region

#Region "Button Click"

    Private Async Sub CopyButton_Click(sender As Object, e As RoutedEventArgs)
        OutputText.Text = "Storage Items: "
        Dim filePicker = New FileOpenPicker() With {.ViewMode = PickerViewMode.List}
        filePicker.FileTypeFilter.Add("*")

        Dim storageItems = Await filePicker.PickMultipleFilesAsync()
        If storageItems.Count > 0 Then
            OutputText.Text &= storageItems.Count.ToString & " file(s) are copied into clipboard"
            Dim dataPackage = New DataPackage()
            dataPackage.SetStorageItems(storageItems)

            ' Request a copy operation from targets that support different file operations, like Windows Explorer
            dataPackage.RequestedOperation = DataPackageOperation.Copy
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage)
        Else
            OutputText.Text &= "No file was selected."
        End If
    End Sub

    Private Async Sub PasteButton_Click(sender As Object, e As RoutedEventArgs)
        ' Get data package from clipboard
        Dim dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent()
        If dataPackageView.Contains(StandardDataFormats.StorageItems) Then
            Dim storageItems = Await dataPackageView.GetStorageItemsAsync()
            If storageItems IsNot Nothing Then
                OutputText.Text = "Pasting following" & storageItems.Count & " file(s) to the folder " & ApplicationData.Current.LocalFolder.Path & Environment.NewLine
                Dim operation = dataPackageView.RequestedOperation
                OutputText.Text &= "Requested Operation: "
                Select Case operation
                    Case DataPackageOperation.Copy
                        OutputText.Text &= "Copy"
                        Exit Select
                    Case DataPackageOperation.Link
                        OutputText.Text &= "Link"
                        Exit Select
                    Case DataPackageOperation.Move
                        OutputText.Text &= "Move"
                        Exit Select
                    Case DataPackageOperation.None
                        OutputText.Text &= "None"
                        Exit Select
                    Case Else
                        OutputText.Text &= "Unknown"
                        Exit Select
                End Select

                OutputText.Text &= Environment.NewLine

                ' Iterate through each item in the collection
                For Each storageItem In storageItems
                    Dim file = TryCast(storageItem, StorageFile)
                    If file IsNot Nothing Then
                        ' Copy the file
                        Dim newFile = Await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting)
                        If newFile IsNot Nothing Then
                            OutputText.Text &= file.Path & Environment.NewLine
                        End If
                    Else
                        Dim folder = TryCast(storageItem, StorageFolder)
                        If folder IsNot Nothing Then
                            ' Skipping folders for brevity sake
                            OutputText.Text &= folder.Path & " is a folder, skipping" & Environment.NewLine
                        End If
                    End If
                Next
            Else
                OutputText.Text &= "Error: the storageItems are null " & Environment.NewLine
            End If
        End If
    End Sub

#End Region
End Class
