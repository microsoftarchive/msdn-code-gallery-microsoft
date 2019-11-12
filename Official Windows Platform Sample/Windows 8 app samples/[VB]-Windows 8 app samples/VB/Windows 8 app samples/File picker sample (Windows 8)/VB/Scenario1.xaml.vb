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
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickAFileButton.Click, AddressOf PickAFileButton_Click
    End Sub

    Private Async Sub PickAFileButton_Click(sender As Object, e As RoutedEventArgs)
        ' Clear previous returned file name, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputTextBlock)

        If rootPage.EnsureUnsnapped Then
            Dim openPicker As New FileOpenPicker
            openPicker.ViewMode = PickerViewMode.Thumbnail
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
            openPicker.FileTypeFilter.Add(".jpg")
            openPicker.FileTypeFilter.Add(".jpeg")
            openPicker.FileTypeFilter.Add(".png")
            Dim file As StorageFile = Await openPicker.PickSingleFileAsync
            If file IsNot Nothing Then
                ' Application now has read/write access to the picked file
                OutputTextBlock.Text = "Picked Photo: " & file.Name
            Else
                OutputTextBlock.Text = "Operation cancelled."
            End If
        End If
    End Sub
End Class
