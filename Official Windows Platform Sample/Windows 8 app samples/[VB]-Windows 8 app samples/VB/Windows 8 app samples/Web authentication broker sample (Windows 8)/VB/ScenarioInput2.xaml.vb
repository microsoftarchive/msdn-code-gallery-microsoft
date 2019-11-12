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
Imports Windows.Security.Cryptography.Core
Imports Windows.Security.Cryptography
Imports Windows.Storage.Streams
Imports System.Text
Imports System.Net
Imports System.IO
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput2
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

        ' Get a pointer to the content within the OutputFrame
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = TryCast(outputFrame.FindName("FlipView1"), FlipView)

    End Sub
#End Region

#Region "Sample Click Handlers - modify if you need them, delete them otherwise"
    ''' <summary>
    ''' This is the click handler for the 'DoSomething' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DoSomething_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " + b.Name + " button", NotifyType.StatusMessage)
        End If
    End Sub
#End Region

    Private Async Function SendDataAsync(Url As String) As Task(Of String)
        Try
            Dim httpClient As Http.HttpClient = New Http.HttpClient()
            Return Await httpClient.GetStringAsync(Url)
        Catch Err As Exception
            rootPage.NotifyUser("Error getting data from server." & Err.Message, Global.SDKTemplate.NotifyType.StatusMessage)
        End Try
        Return vbNullString
    End Function

    Private Sub DebugPrint(Trace As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim TwitterDebugArea As TextBox = TryCast(outputFrame.FindName("TwitterDebugArea"), TextBox)
        TwitterDebugArea.Text &= Trace & vbCr & vbCrLf
    End Sub

    Private Sub OutputToken(TokenUri As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim TwitterReturnedToken As TextBox = TryCast(outputFrame.FindName("TwitterReturnedToken"), TextBox)
        TwitterReturnedToken.Text = TokenUri
    End Sub

    Private Async Sub Launch_Click(sender As Object, e As RoutedEventArgs)
        If TwitterClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        ElseIf TwitterCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        ElseIf TwitterClientSecret.Text = "" Then
            rootPage.NotifyUser("Please enter an Client Secret.", Global.SDKTemplate.NotifyType.StatusMessage)
            Exit Sub
        End If

        Try
            '
            ' Acquiring a request token
            '
            Dim SinceEpoch As TimeSpan = (DateTime.UtcNow - New DateTime(1970, 1, 1))
            Dim Rand As New Random
            Dim TwitterUrl As String = "https://api.twitter.com/oauth/request_token"
            Dim Nonce As Int32 = Rand.Next(1000000000)
            '
            ' Compute base signature string and sign it.
            '    This is a common operation that is required for all requests even after the token is obtained.
            '    Parameters need to be sorted in alphabetical order
            '    Keys and values should be URL Encoded.
            '
            Dim SigBaseStringParams As String = "oauth_callback=" & Uri.EscapeDataString(TwitterCallbackUrl.Text)
            SigBaseStringParams &= "&" & "oauth_consumer_key=" & TwitterClientID.Text
            SigBaseStringParams &= "&" & "oauth_nonce=" & Nonce.ToString
            SigBaseStringParams &= "&" & "oauth_signature_method=HMAC-SHA1"
            SigBaseStringParams &= "&" & "oauth_timestamp=" & Math.Round(SinceEpoch.TotalSeconds)
            SigBaseStringParams &= "&" & "oauth_version=1.0"
            Dim SigBaseString As String = "GET&"
            SigBaseString &= Uri.EscapeDataString(TwitterUrl) & "&" & Uri.EscapeDataString(SigBaseStringParams)

            Dim KeyMaterial As IBuffer = CryptographicBuffer.ConvertStringToBinary(TwitterClientSecret.Text & "&", BinaryStringEncoding.Utf8)
            Dim HmacSha1Provider As MacAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1")
            Dim MacKey As CryptographicKey = HmacSha1Provider.CreateKey(KeyMaterial)
            Dim DataToBeSigned As IBuffer = CryptographicBuffer.ConvertStringToBinary(SigBaseString, BinaryStringEncoding.Utf8)
            Dim SignatureBuffer As IBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned)
            Dim Signature As String = CryptographicBuffer.EncodeToBase64String(SignatureBuffer)
            Dim DataToPost As String = "OAuth oauth_callback=""" & Uri.EscapeDataString(TwitterCallbackUrl.Text) & """, oauth_consumer_key=""" & TwitterClientID.Text & """, oauth_nonce=""" & Nonce.ToString & """, oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""" & Math.Round(SinceEpoch.TotalSeconds) & """, oauth_version=""1.0"", oauth_signature=""" & Uri.EscapeDataString(Signature) & """"

            TwitterUrl &= "?" & SigBaseStringParams & "&oauth_signature=" & Uri.EscapeDataString(Signature)

            Dim GetResponse = Await SendDataAsync(TwitterUrl)
            DebugPrint("Received Data: " & GetResponse)

            If GetResponse IsNot Nothing Then
                Dim oauth_token As String = Nothing
                Dim oauth_token_secret As String = Nothing
                Dim keyValPairs() As String = GetResponse.Split("&"c)
                For i = 0 To keyValPairs.Length - 1
                    Dim splits() As String = keyValPairs(i).Split("="c)
                    Select Case splits(0)
                        Case "oauth_token"
                            oauth_token = splits(1)
                            Exit Select
                        Case "oauth_token_secret"
                            oauth_token_secret = splits(1)
                            Exit Select
                    End Select
                Next

                If oauth_token IsNot Nothing Then

                    TwitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" & oauth_token
                    Dim StartUri As System.Uri = New Uri(TwitterUrl)
                    Dim EndUri As System.Uri = New Uri(TwitterCallbackUrl.Text)

                    DebugPrint("Navigating to: " & TwitterUrl)

                    Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
                    If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                        OutputToken(WebAuthenticationResult.ResponseData.ToString)
                    ElseIf WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.ErrorHttp Then
                        OutputToken("HTTP Error returned by AuthenticateAsync : " & WebAuthenticationResult.ResponseErrorDetail.ToString)
                    Else
                        OutputToken("Error returned by AuthenticateAsync : " & WebAuthenticationResult.ResponseStatus.ToString)
                    End If
                End If
            End If
        Catch ex As Exception
            '
            ' Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            '
            DebugPrint(ex.ToString)
        End Try
    End Sub
End Class
