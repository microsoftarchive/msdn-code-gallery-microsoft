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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class FileOpenPicker_PickAppFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private Const id As String = "MyLocalFile"
    Private fileOpenPickerUI As FileOpenPickerUI = FileOpenPickerPage.Current.fileOpenPickerUI
    Private _dispatcher As CoreDispatcher = Window.Current.Dispatcher

    Public Sub New()
        Me.InitializeComponent()
        AddHandler AddLocalFileButton.Click, AddressOf AddLocalFileButton_Click
        AddHandler RemoveLocalFileButton.Click, AddressOf RemoveLocalFileButton_Click
    End Sub

    Private Sub UpdateButtonState(fileInBasket As Boolean)
        AddLocalFileButton.IsEnabled = Not fileInBasket
        RemoveLocalFileButton.IsEnabled = fileInBasket
    End Sub

    Private Async Sub OnFileRemoved(sender As FileOpenPickerUI, args As FileRemovedEventArgs)
        ' The event handler may be invoked on a background thread, so use the Dispatcher to run the UI-related code on the UI thread.
        Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                      OutputTextBlock.Text = Status.FileRemoved
                                                                      UpdateButtonState(False)
                                                                  End Sub)

    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        UpdateButtonState(fileOpenPickerUI.ContainsFile(id))
        AddHandler fileOpenPickerUI.FileRemoved, AddressOf OnFileRemoved
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler fileOpenPickerUI.FileRemoved, AddressOf OnFileRemoved
    End Sub

    Private Async Sub AddLocalFileButton_Click(sender As Object, e As RoutedEventArgs)
        Dim file As StorageFile = Await Package.Current.InstalledLocation.GetFileAsync("Assets\squareTile-sdk.png")
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

    Private Sub RemoveLocalFileButton_Click(sender As Object, e As RoutedEventArgs)
        If fileOpenPickerUI.ContainsFile(id) Then
            fileOpenPickerUI.RemoveFile(id)
            OutputTextBlock.Text = Status.FileRemoved
        End If
        UpdateButtonState(False)
    End Sub
End Class
