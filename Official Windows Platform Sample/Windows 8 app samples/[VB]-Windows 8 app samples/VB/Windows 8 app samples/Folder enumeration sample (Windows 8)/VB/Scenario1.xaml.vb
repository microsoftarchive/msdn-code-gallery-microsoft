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
Imports System.Text
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        Me.InitializeComponent()
        AddHandler GetFilesAndFoldersButton.Click, AddressOf GetFilesAndFolders_Click
    End Sub

    Private Async Sub GetFilesAndFolders_Click(sender As Object, e As RoutedEventArgs)
        Dim picturesFolder As StorageFolder = KnownFolders.PicturesLibrary

        Dim fileList As IReadOnlyList(Of StorageFile) = Await picturesFolder.GetFilesAsync()
        Dim folderList As IReadOnlyList(Of StorageFolder) = Await picturesFolder.GetFoldersAsync()

        Dim count = fileList.Count + folderList.Count
        Dim outputText As New StringBuilder(picturesFolder.Name & " (" & count & ")" & vbLf & vbLf)

        For Each folder As StorageFolder In folderList
            outputText.AppendLine("    " & folder.DisplayName & "\")
        Next

        For Each file As StorageFile In fileList
            outputText.AppendLine("    " & file.Name)
        Next

        OutputTextBlock.Text = outputText.ToString
    End Sub
End Class
