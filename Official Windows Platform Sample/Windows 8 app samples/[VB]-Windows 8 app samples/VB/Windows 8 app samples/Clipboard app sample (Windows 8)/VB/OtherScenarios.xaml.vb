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
Imports System.Collections.Generic
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Foundation
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Navigation


Partial Public NotInheritable Class OtherScenarios
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Me.Init()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        If rootPage.isClipboardContentChangeChecked Then
            RegisterClipboardContentChange.IsChecked = True
        End If

        If rootPage.needToPrintClipboardFormat Then
            Me.DisplayFormats(Windows.ApplicationModel.DataTransfer.Clipboard.GetContent())
        End If
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        rootPage.needToPrintClipboardFormat = False
    End Sub

#Region "Scenario Specific Code"

    Private Sub Init()
        AddHandler ShowFormatButton.Click, AddressOf ShowFormatButton_Click
        AddHandler EmptyClipboardButton.Click, AddressOf EmptyClipboardButton_Click
        AddHandler RegisterClipboardContentChange.Checked, AddressOf RegisterClipboardContentChange_Check
        AddHandler RegisterClipboardContentChange.Unchecked, AddressOf RegisterClipboardContentChange_UnCheck
        AddHandler ClearOutputButton.Click, AddressOf ClearOutputButton_Click
    End Sub

#End Region

#Region "Button Click"

    Private Sub ShowFormatButton_Click(sender As Object, e As RoutedEventArgs)
        Dim dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent()
        Me.DisplayFormats(dataPackageView)
    End Sub

    Private Sub EmptyClipboardButton_Click(sender As Object, e As RoutedEventArgs)
        Me.DisplayFormatOutputText.Text = ""
        Windows.ApplicationModel.DataTransfer.Clipboard.Clear()
        OutputText.Text = "Clipboard has been emptied."
    End Sub

    Private Sub RegisterClipboardContentChange_Check(sender As Object, e As RoutedEventArgs)
        If Not rootPage.isClipboardContentChangeChecked Then
            rootPage.isClipboardContentChangeChecked = True
            AddHandler Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged, AddressOf Me.RegisterClipboardChangeEventHandler
            AddHandler Window.Current.CoreWindow.VisibilityChanged, AddressOf CoreWindow_VisibilityChanged
            OutputText.Text = "Successfully registered for clipboard update notification."
        End If
    End Sub

    Private Sub RegisterClipboardContentChange_UnCheck(sender As Object, e As RoutedEventArgs)
        Me.ClearOutput()
        rootPage.isClipboardContentChangeChecked = False
        RemoveHandler Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged, AddressOf Me.RegisterClipboardChangeEventHandler
        RemoveHandler Window.Current.CoreWindow.VisibilityChanged, AddressOf CoreWindow_VisibilityChanged
        OutputText.Text = "Successfully un-registered for clipboard update notification."
    End Sub

    Private Sub ClearOutputButton_Click(sender As Object, e As RoutedEventArgs)
        Me.ClearOutput()
    End Sub

#End Region

#Region "Private helper methods"

    Private Sub ClearOutput()
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        OutputText.Text = ""
        DisplayFormatOutputText.Text = ""
    End Sub

    Private Sub DisplayFormats(dataPackageView As DataPackageView)
        If dataPackageView IsNot Nothing AndAlso dataPackageView.AvailableFormats.Count > 0 Then
            DisplayFormatOutputText.Text = "Available formats in the clipboard: "
            Dim availableFormats = dataPackageView.AvailableFormats.GetEnumerator()
            While availableFormats.MoveNext()
                DisplayFormatOutputText.Text &= Environment.NewLine & availableFormats.Current
            End While
        Else
            OutputText.Text = "Clipboard is empty"
        End If
    End Sub

    Private Sub RegisterClipboardChangeEventHandler(sender As Object, e As Object)
        ' If user is not in  when clipboard content is changed, this flag will ensure the clipboard format gets printed out when user navigates to it
        rootPage.needToPrintClipboardFormat = True
        If Me.isApplicationWindowActive Then
            rootPage.NotifyUser(String.Format("Clipboard content has changed, available clipboard format is shown in ."), NotifyType.StatusMessage)
            Me.DisplayFormats(Windows.ApplicationModel.DataTransfer.Clipboard.GetContent())
        Else
            rootPage.NotifyUser(String.Format("Clipboard content has changed, the app is currently not at foreground and the available format will be shown once user returns to app."), NotifyType.StatusMessage)

            ' Background applications can't access clipboard
            ' Deferring processing of update notification till later
            Me.containsUnprocessedNotifications = True
        End If
    End Sub

    Private Sub CoreWindow_VisibilityChanged(sender As CoreWindow, e As VisibilityChangedEventArgs)
        If e.Visible Then
            ' Application's main window has been activated (received focus), and therefore the application can now access clipboard
            Me.isApplicationWindowActive = True
            If Me.containsUnprocessedNotifications Then
                Me.DisplayFormats(Windows.ApplicationModel.DataTransfer.Clipboard.GetContent())
                Me.containsUnprocessedNotifications = False
            End If
        Else
            ' Application's main window has been deactivated (lost focus), and therefore the application can no longer access Clipboard
            Me.isApplicationWindowActive = False
        End If
    End Sub

#End Region

#Region "private member variables"

    Private isApplicationWindowActive As Boolean = True
    Private containsUnprocessedNotifications As Boolean = False

#End Region
End Class
