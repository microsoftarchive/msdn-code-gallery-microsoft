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
Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current
    Private fileToken As String = String.Empty

    Public Sub New()
        Me.InitializeComponent()
        AddHandler SaveFileButton.Click, AddressOf SaveFileButton_Click
        AddHandler WriteFileButton.Click, AddressOf WriteFileButton_Click
    End Sub

    Private Async Sub SaveFileButton_Click(sender As Object, e As RoutedEventArgs)
        ' Clear previous returned file name, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputFileName)
        rootPage.ResetScenarioOutput(OutputFileContent)

        If rootPage.EnsureUnsnapped() Then
            Dim savePicker As New FileSavePicker()
            ' Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", New List(Of String)() From {".txt"})

            ' Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document"
            Dim file As StorageFile = Await savePicker.PickSaveFileAsync()
            If file IsNot Nothing Then
                fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file)
                MainPage.Current.NotifyUser(String.Format("Received file: {0}", file.Name), NotifyType.StatusMessage)
                WriteFileButton.IsEnabled = True
            Else
                MainPage.Current.NotifyUser("Operation cancelled.", NotifyType.StatusMessage)
            End If
        End If
    End Sub

    Private Async Sub WriteFileButton_Click(sender As Object, e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(fileToken) Then
            Dim file As StorageFile = Await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken)
            ' Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file)
            ' write to file
            Await FileIO.AppendTextAsync(file, String.Format("{0}Text Added @ {1}.", System.Environment.NewLine, DateTime.Now.ToString))
            ' Let Windows know that we're finished changing the file so the server app can update the remote version of the file.
            ' Completing updates may require Windows to ask for user input.
            Dim status As FileUpdateStatus = Await CachedFileManager.CompleteUpdatesAsync(file)
            Select Case status
                Case FileUpdateStatus.Complete
                    MainPage.Current.NotifyUser("File " & file.Name & " was saved.", NotifyType.StatusMessage)
                    OutputFileAsync(file)
                    Exit Select

                Case FileUpdateStatus.CompleteAndRenamed
                    MainPage.Current.NotifyUser("File " & file.Name & " was renamed and saved.", NotifyType.StatusMessage)
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace(fileToken, file)
                    OutputFileAsync(file)
                    Exit Select
                Case Else

                    MainPage.Current.NotifyUser("File " & file.Name & " couldn't be saved.", NotifyType.StatusMessage)
                    Exit Select
            End Select
        End If
    End Sub

    Private Async Sub OutputFileAsync(file As StorageFile)
        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
        OutputFileName.Text = String.Format("Received file: {0}", file.Name)
        OutputFileContent.Text = String.Format("File content:{0}{1}", System.Environment.NewLine, fileContent)
    End Sub
End Class
