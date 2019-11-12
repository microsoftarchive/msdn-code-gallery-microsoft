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
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Provider
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current
    Private fileToken As String = String.Empty

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickFileButton.Click, AddressOf PickFileButton_Click
        AddHandler OutputFileButton.Click, AddressOf OutputFileButton_Click
    End Sub

    Private Async Sub PickFileButton_Click(sender As Object, e As RoutedEventArgs)
        ' Clear previous returned file content, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputFileName)
        rootPage.ResetScenarioOutput(OutputFileContent)
        rootPage.NotifyUser("", NotifyType.StatusMessage)

        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Clear()
        fileToken = String.Empty

        If rootPage.EnsureUnsnapped() Then
            Dim openPicker As New FileOpenPicker()
            openPicker.FileTypeFilter.Add(".txt")
            Dim file As StorageFile = Await openPicker.PickSingleFileAsync()
            If file IsNot Nothing Then
                fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file)
                OutputFileButton.IsEnabled = True
                OutputFileAsync(file)
            Else
                rootPage.NotifyUser("Operation cancelled.", NotifyType.StatusMessage)
            End If
        End If
    End Sub

    Private Async Sub OutputFileButton_Click(sender As Object, e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(fileToken) Then
            rootPage.NotifyUser("", NotifyType.StatusMessage)

            ' Windows will call the server app to update the local version of the file
            Try
                Dim file As StorageFile = Await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken)
                OutputFileAsync(file)
            Catch ex As UnauthorizedAccessException
                rootPage.NotifyUser("Access is denied.", NotifyType.ErrorMessage)
            End Try
        End If
    End Sub

    Private Async Sub OutputFileAsync(file As StorageFile)
        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
        OutputFileName.Text = String.Format("Received file: {0}", file.Name)
        OutputFileContent.Text = String.Format("File content:{0}{1}", System.Environment.NewLine, fileContent)
    End Sub
End Class
