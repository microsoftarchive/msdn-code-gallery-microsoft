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
Imports Windows.UI.Xaml.Controls

Partial Public NotInheritable Class ShareHtml
    Inherits SDKTemplate.Common.SharePage

    Public Sub New()
        Me.InitializeComponent()
        ShareWebView.Navigate(New Uri("http://msdn.microsoft.com"))
    End Sub

    Private Sub ShareWebView_NavigationCompleted(ByVal sender As Object, ByVal e As WebViewNavigationCompletedEventArgs)
        ShareWebView.Visibility = Windows.UI.Xaml.Visibility.Visible
        BlockingRect.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        LoadingProgressRing.IsActive = False
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        ' Get the user's selection from the WebView.
        Dim requestData As DataPackage = ShareWebView.CaptureSelectedContentToDataPackageAsync().GetResults()
        Dim dataPackageView As DataPackageView = requestData.GetView()

        If (dataPackageView IsNot Nothing) AndAlso (dataPackageView.AvailableFormats.Count > 0) Then
            requestData.Properties.Title = "A web snippet for you"
            requestData.Properties.Description = "HTML selection from a WebView control" ' The description is optional.
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink
            requestData.Properties.ContentSourceWebLink = New Uri("http://msdn.microsoft.com")
            request.Data = requestData
            succeeded = True
        Else
            request.FailWithDisplayText("Make a selection in the WebView control and try again.")
        End If
        Return succeeded
    End Function
End Class
