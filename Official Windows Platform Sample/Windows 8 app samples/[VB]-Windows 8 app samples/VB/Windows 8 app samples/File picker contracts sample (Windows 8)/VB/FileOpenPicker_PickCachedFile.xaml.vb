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
Imports Windows.ApplicationModel
Imports Windows.Foundation
Imports Windows.Storage
Imports Windows.Storage.Pickers.Provider
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Storage.Provider

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class FileOpenPicker_PickCachedFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private Const id As String = "MyCachedFile"

    Private fileOpenPickerUI As FileOpenPickerUI = FileOpenPickerPage.Current.fileOpenPickerUI
    Private _dispatcher As CoreDispatcher = Window.Current.Dispatcher

    Public Sub New()
        Me.InitializeComponent()
        AddHandler AddFileButton.Click, AddressOf AddFileButton_Click
        AddHandler RemoveFileButton.Click, AddressOf RemoveFileButton_Click
    End Sub

    Private Sub UpdateButtonState(fileInBasket As Boolean)
        AddFileButton.IsEnabled = Not fileInBasket
        RemoveFileButton.IsEnabled = fileInBasket
    End Sub

    Private Async Sub OnFileRemoved(sender As FileOpenPickerUI, args As FileRemovedEventArgs)
        ' make sure that the item got removed matches the one we added.
        If args.Id = id Then
            ' The event handler may be invoked on a background thread, so use the Dispatcher to run the UI-related code on the UI thread.
            Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                          OutputTextBlock.Text = Status.FileRemoved
                                                                          UpdateButtonState(False)
                                                                      End Sub)
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        UpdateButtonState(fileOpenPickerUI.ContainsFile(id))
        AddHandler fileOpenPickerUI.FileRemoved, AddressOf OnFileRemoved
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler fileOpenPickerUI.FileRemoved, AddressOf OnFileRemoved
    End Sub

    Private Async Sub AddFileButton_Click(sender As Object, e As RoutedEventArgs)
        Dim file As StorageFile = Await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("CachedFile.txt", CreationCollisionOption.ReplaceExisting)
        Await Windows.Storage.FileIO.WriteTextAsync(file, "Cached file created...")
        CachedFileUpdater.SetUpdateInformation(file, "CachedFile", ReadActivationMode.BeforeAccess, WriteActivationMode.NotNeeded, CachedFileOptions.RequireUpdateOnAccess)

        Dim inBasket As Boolean
        Select Case fileOpenPickerUI.AddFile(id, file)
            Case AddFileResult.Added, AddFileResult.AlreadyAdded
                inBasket = True
                OutputTextBlock.Text = Status.FileAdded
                Exit Select
            Case Else
                inBasket = False
                OutputTextBlock.Text = Status.FileAddFailed
                Exit Select
        End Select
        UpdateButtonState(inBasket)
    End Sub

    Private Sub RemoveFileButton_Click(sender As Object, e As RoutedEventArgs)
        If fileOpenPickerUI.ContainsFile(id) Then
            fileOpenPickerUI.RemoveFile(id)
            OutputTextBlock.Text = Status.FileRemoved
        End If
        UpdateButtonState(False)
    End Sub
End Class
