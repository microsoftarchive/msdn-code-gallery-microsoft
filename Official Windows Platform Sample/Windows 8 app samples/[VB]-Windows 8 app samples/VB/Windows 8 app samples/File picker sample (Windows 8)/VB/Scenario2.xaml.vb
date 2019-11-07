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
Imports System.Text
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickFilesButton.Click, AddressOf PickFilesButton_Click
    End Sub

    Private Async Sub PickFilesButton_Click(sender As Object, e As RoutedEventArgs)
        ' Clear any previously returned files between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputTextBlock)

        If rootPage.EnsureUnsnapped Then
            Dim openPicker As New FileOpenPicker
            openPicker.ViewMode = PickerViewMode.List
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            openPicker.FileTypeFilter.Add("*")
            Dim files As IReadOnlyList(Of StorageFile) = Await openPicker.PickMultipleFilesAsync
            If files.Count > 0 Then
                Dim output As New StringBuilder("Picked files:" & vbCrLf)
                ' Application now has read/write access to the picked file(s)
                For Each file In files
                    output.Append(file.Name & vbCrLf)
                Next
                OutputTextBlock.Text = output.ToString
            Else
                OutputTextBlock.Text = "Operation cancelled."
            End If
        End If
    End Sub
End Class
