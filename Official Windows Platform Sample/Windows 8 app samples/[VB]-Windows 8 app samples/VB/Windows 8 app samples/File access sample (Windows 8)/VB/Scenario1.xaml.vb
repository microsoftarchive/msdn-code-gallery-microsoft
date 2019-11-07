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
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler CreateFileButton.Click, AddressOf CreateFileButton_Click
    End Sub

    ''' <summary>
    ''' Creates a new file
    ''' </summary>
    Private Async Sub CreateFileButton_Click(sender As Object, e As RoutedEventArgs)
        Dim storageFolder As StorageFolder = KnownFolders.DocumentsLibrary
        rootPage.sampleFile = Await storageFolder.CreateFileAsync(MainPage.filename, CreationCollisionOption.ReplaceExisting)
        OutputTextBlock.Text = "The file '" & rootPage.sampleFile.Name & "' was created."
    End Sub
End Class
