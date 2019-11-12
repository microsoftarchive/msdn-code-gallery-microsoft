'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.Pickers

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class MainPage_PickFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickFileButton.Click, AddressOf PickFileButton_Click
    End Sub

    Private Async Sub PickFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim page As MainPage = MainPage.Current

        page.ResetScenarioOutput(OutputTextBlock)
        ' Set up and launch the Open Picker
        Dim fileOpenPicker As New FileOpenPicker()
        fileOpenPicker.ViewMode = PickerViewMode.Thumbnail
        fileOpenPicker.FileTypeFilter.Add(".png")

        Dim files As IReadOnlyList(Of StorageFile) = Await fileOpenPicker.PickMultipleFilesAsync()
        Dim fileNames As New StringBuilder()
        For Each file As StorageFile In files
            ' At this point, the app can begin reading from the provided file
            fileNames.AppendLine(file.Name)
        Next file

        OutputTextBlock.Text = fileNames.ToString()
    End Sub
End Class

