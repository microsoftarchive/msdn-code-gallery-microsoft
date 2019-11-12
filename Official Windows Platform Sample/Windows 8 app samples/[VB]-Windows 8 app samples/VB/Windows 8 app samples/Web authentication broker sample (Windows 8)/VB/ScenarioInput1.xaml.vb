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
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

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
        ' ex: flipView1 = TryCast(outputFrame.FindName("FlipView1"), FlipView)

    End Sub
#End Region

#Region "Sample Click Handlers - modify if you need them, delete them otherwise"
    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Default_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Name & " button", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Other_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Name & " button", NotifyType.StatusMessage)
        End If
    End Sub
#End Region

    Private Sub DebugPrint(Trace As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim FacebookDebugArea As TextBox = TryCast(outputFrame.FindName("FacebookDebugArea"), TextBox)
        FacebookDebugArea.Text &= Trace & vbCr & vbCrLf
    End Sub

    Private Sub OutputToken(TokenUri As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim FacebookReturnedToken As TextBox = TryCast(outputFrame.FindName("FacebookReturnedToken"), TextBox)
        FacebookReturnedToken.Text = TokenUri
    End Sub

    Private Async Sub Launch_Click(sender As Object, e As RoutedEventArgs)
        If FacebookClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        ElseIf FacebookCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        End If

        Try
            Dim FacebookURL As String = "https://www.facebook.com/dialog/oauth?client_id=" & FacebookClientID.Text & "&redirect_uri=" & Uri.EscapeUriString(FacebookCallbackUrl.Text) & "&scope=read_stream&display=popup&response_type=token"

            Dim StartUri As System.Uri = New Uri(FacebookURL)
            Dim EndUri As System.Uri = New Uri(FacebookCallbackUrl.Text)

            DebugPrint("Navigating to: " & FacebookURL)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
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
