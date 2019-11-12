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
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickFolderButton.Click, AddressOf PickFolderButton_Click
    End Sub

    Private Async Sub PickFolderButton_Click(sender As Object, e As RoutedEventArgs)
        ' Clear previous returned folder name, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputTextBlock)

        If rootPage.EnsureUnsnapped Then
            Dim folderPicker As New FolderPicker
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop
            folderPicker.FileTypeFilter.Add(".docx")
            folderPicker.FileTypeFilter.Add(".xlsx")
            folderPicker.FileTypeFilter.Add(".pptx")
            Dim folder As StorageFolder = Await folderPicker.PickSingleFolderAsync
            If folder IsNot Nothing Then
                ' Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder)
                OutputTextBlock.Text = "Picked folder: " & folder.Name
            Else
                OutputTextBlock.Text = "Operation cancelled."
            End If
        End If
    End Sub
End Class
