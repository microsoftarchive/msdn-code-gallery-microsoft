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
Imports System.IO
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario7
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler CopyFileButton.Click, AddressOf CopyFileButton_Click
    End Sub

    Private Async Sub CopyFileButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Dim fileCopy As StorageFile = Await file.CopyAsync(KnownFolders.DocumentsLibrary, "sample - Copy.dat", NameCollisionOption.ReplaceExisting)
                OutputTextBlock.Text = "The file '" & file.Name & "' was copied and the new file was named '" & fileCopy.Name & "'."
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub
End Class
