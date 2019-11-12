'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.ApplicationModel.DataTransfer
Imports SDKTemplate

Partial Public NotInheritable Class SetErrorMessage
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current
    Private dataTransferManager As DataTransferManager

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Register this page as a share source.
        Me.dataTransferManager = dataTransferManager.GetForCurrentView()
        AddHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ' Unregister this page as a share source.
        RemoveHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Private Sub OnDataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        Dim customErrorMessage As String = CustomErrorText.Text
        If Not String.IsNullOrEmpty(customErrorMessage) Then
            e.Request.FailWithDisplayText(customErrorMessage)
        Else
            e.Request.FailWithDisplayText("Enter a failure display text and try again.")
        End If
    End Sub
End Class
