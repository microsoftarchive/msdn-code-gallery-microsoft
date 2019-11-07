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
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class MainPage_SaveFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler SaveFileButton.Click, AddressOf SaveFileButton_Click
    End Sub

    Private Async Sub SaveFileButton_Click(sender As Object, e As RoutedEventArgs)
        Dim page As MainPage = MainPage.Current

        page.ResetScenarioOutput(OutputTextBlock)
        If page.EnsureUnsnapped() Then
            ' Set up and launch the Save Picker
            Dim fileSavePicker As New FileSavePicker()
            fileSavePicker.FileTypeChoices.Add("PNG", New String() {".png"})

            Dim file As StorageFile = Await fileSavePicker.PickSaveFileAsync()
            If file IsNot Nothing Then
                ' At this point, the app can begin writing to the provided save file
                OutputTextBlock.Text = file.Name
            End If
        End If
    End Sub
End Class
