' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Security.Authentication.Web

Partial Public NotInheritable Class ScenarioInput4
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As Global.SDKTemplate.MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, Global.SDKTemplate.MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        'ex:flipView1 = TryCast(inputFrame.FindName("FlipView1"), FlipView)

    End Sub
#End Region

    Private Sub DebugPrint(Trace As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim GoogleDebugArea As TextBox = TryCast(outputFrame.FindName("GoogleDebugArea"), TextBox)
        GoogleDebugArea.Text &= Trace & vbCr & vbCrLf
    End Sub

    Private Sub OutputToken(TokenUri As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim GoogleReturnedToken As TextBox = TryCast(outputFrame.FindName("GoogleReturnedToken"), TextBox)
        GoogleReturnedToken.Text = TokenUri
    End Sub

    Private Async Sub Launch_Click(sender As Object, e As RoutedEventArgs)
        If GoogleClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        ElseIf GoogleCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        End If

        Try
            Dim GoogleURL As String = "https://accounts.google.com/o/oauth2/auth?client_id=" & GoogleClientID.Text & "&redirect_uri=" & Uri.EscapeUriString(GoogleCallbackUrl.Text) & "&response_type=code&scope=http://picasaweb.google.com/data"

            Dim StartUri As System.Uri = New Uri(GoogleURL)
            Dim EndUri As System.Uri = New Uri("https://accounts.google.com/o/oauth2/approval?")

            DebugPrint("Navigating to: " & GoogleURL)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, StartUri, EndUri)
            If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                OutputToken(WebAuthenticationResult.ResponseData.ToString)
            ElseIf WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.ErrorHttp Then
                OutputToken("HTTP Error returned by AuthenticateAsync : " & WebAuthenticationResult.ResponseErrorDetail.ToString)
            Else
                OutputToken("Error returned by AuthenticateAsync : " & WebAuthenticationResult.ResponseStatus.ToString)
            End If
        Catch Ex As Exception
            '
            ' Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            '
            DebugPrint(Ex.ToString)
        End Try
    End Sub
End Class
