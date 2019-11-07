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

Partial Public NotInheritable Class ShareText
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
        Dim dataPackageTitle As String = TitleInputBox.Text

        ' The title is required.
        If Not String.IsNullOrEmpty(dataPackageTitle) Then
            Dim dataPackageText As String = TextToShare.Text
            If Not String.IsNullOrEmpty(dataPackageText) Then
                Dim requestData As DataPackage = e.Request.Data
                requestData.Properties.Title = dataPackageTitle

                ' The description is optional.
                Dim dataPackageDescription As String = DescriptionInputBox.Text
                If dataPackageDescription IsNot Nothing Then
                    requestData.Properties.Description = dataPackageDescription
                End If
                requestData.SetText(dataPackageText)
            Else
                e.Request.FailWithDisplayText("Enter the text you would like to share and try again.")
            End If
        Else
            e.Request.FailWithDisplayText(MainPage.MissingTitleError)
        End If
    End Sub

    Private Sub ShowUIButton_Click(sender As Object, e As RoutedEventArgs)
        dataTransferManager.ShowShareUI()
    End Sub
End Class
