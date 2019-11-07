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
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Navigation

Namespace Global.SDKTemplate.Common
    Public MustInherit Class SharePage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Protected rootPage As MainPage = MainPage.Current
        Private dataTransferManager As DataTransferManager

        Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
            ' Register the current page as a share source.
            Me.dataTransferManager = dataTransferManager.GetForCurrentView()
            AddHandler dataTransferManager.DataRequested, AddressOf OnDataRequested
        End Sub

        Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
            ' Unregister the current page as a share source.
            RemoveHandler dataTransferManager.DataRequested, AddressOf OnDataRequested
        End Sub

        ' When share is invoked (by the user or programatically) the event handler we registered will be called to populate the datapackage with the
        ' data to be shared.
        Private Sub OnDataRequested(ByVal sender As DataTransferManager, ByVal e As DataRequestedEventArgs)
            ' Call the scenario specific function to populate the datapackage with the data to be shared.
            If GetShareContent(e.Request) Then
                ' Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                ' to make sure the title is valid since the sample scenario gets the title from the user.
                If String.IsNullOrEmpty(e.Request.Data.Properties.Title) Then
                    e.Request.FailWithDisplayText(MainPage.MissingTitleError)
                End If
            End If
        End Sub

        Protected Sub ShowUIButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' If the user clicks the share button, invoke the share flow programatically.
            dataTransferManager.ShowShareUI()
        End Sub

        ' This function is implemented by each scenario to share the content specific to that scenario (text, link, image, etc.).
        Protected MustOverride Function GetShareContent(ByVal request As DataRequest) As Boolean

        Protected ReadOnly Property ApplicationLink() As Uri
            Get
                Return GetApplicationLink(Me.GetType().Name)
            End Get
        End Property

        Public Shared Function GetApplicationLink(ByVal sharePageName As String) As Uri
            Return New Uri("ms-sdk-sharesourcevb:navigate?page=" & sharePageName)
        End Function
    End Class
End Namespace
