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
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Provider
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler SaveFileButton.Click, AddressOf SaveFileButton_Click
    End Sub

    Private Async Sub SaveFileButton_Click(sender As Object, e As RoutedEventArgs)
        ' Clear previous returned file name, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputTextBlock)

        If rootPage.EnsureUnsnapped Then
            Dim savePicker As New FileSavePicker
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            ' Dropdown of file types the user can save the file as

            savePicker.FileTypeChoices.Add("Plain Text", New List(Of String) From {".txt"})

            ' Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document"
            Dim file As StorageFile = Await savePicker.PickSaveFileAsync
            If file IsNot Nothing Then
                ' Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync

                CachedFileManager.DeferUpdates(file)
                ' Write to file
                Await FileIO.WriteTextAsync(file, file.Name)
               ' Let Windows know that we are finished changing the file so the other app can update the remote version of the file.
               ' Completing updates may require windows to ask for user input
                Dim status As FileUpdateStatus = Await CachedFileManager.CompleteUpdatesAsync(file)
                If status = FileUpdateStatus.Complete Then
                    OutputTextBlock.Text = "File " & file.Name & " was saved."
                Else
                    OutputTextBlock.Text = "File " & file.Name & " couldn't be saved."
                End If
            Else
                OutputTextBlock.Text = "Operation cancelled."
            End If
        End If
    End Sub
End Class
