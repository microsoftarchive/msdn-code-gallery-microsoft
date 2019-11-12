'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports HotspotAuthenticationTask
Imports Windows.Networking.NetworkOperators
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class AuthByForegroundApp
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed to call methods in MainPage such as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private authenticationContext As HotspotAuthenticationContext

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Configure background task handler to trigger foregound app for authentication
        ConfigStore.AuthenticateThroughBackgroundTask = False

        ' Setup completion handler
        ScenarioCommon.Instance.RegisteredCompletionHandlerForBackgroundTask()

        ' Register event to update UI state on authentication event
        ScenarioCommon.Instance.ForegroundAuthenticationCallback = AddressOf HandleForegroundAuthenticationCallback

        ' Check current authenticatino state
        InitializeForegroundAppAuthentication()
    End Sub

    ''' <summary>
    ''' Handle auhentication event triggered by background task
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub HandleForegroundAuthenticationCallback(sender As Object, e As EventArgs)
        InitializeForegroundAppAuthentication()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Authenticate' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub AuthenticateButton_Click(sender As Object, args As RoutedEventArgs)
        Try
            authenticationContext.IssueCredentials(ConfigStore.UserName, ConfigStore.Password, ConfigStore.ExtraParameters, ConfigStore.MarkAsManualConnect)
            rootPage.NotifyUser("Issuing credentials succeeded", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.StatusMessage)
        End Try
        ClearAuthenticationToken()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Skip' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SkipButton_Click(sender As Object, args As RoutedEventArgs)
        Try
            authenticationContext.SkipAuthentication()
            rootPage.NotifyUser("Authentication skipped", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try
        ClearAuthenticationToken()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Abort' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub AbortButton_Click(sender As Object, args As RoutedEventArgs)
        Try
            authenticationContext.AbortAuthentication(ConfigStore.MarkAsManualConnect)
            rootPage.NotifyUser("Authentication aborted", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try
        ClearAuthenticationToken()
    End Sub

    ''' <summary>
    ''' Query authentication token from application storage and upate the UI.
    ''' The token gets passed from the background task handler.
    ''' </summary>
    Private Sub InitializeForegroundAppAuthentication()
        Dim token As String = ConfigStore.AuthenticationToken
        If token = "" Then
            ' no token found
            Exit Sub
        End If
        If Not HotspotAuthenticationContext.TryGetAuthenticationContext(token, authenticationContext) Then
            rootPage.NotifyUser("TryGetAuthenticationContext failed", NotifyType.ErrorMessage)
            Return
        End If

        AuthenticateButton.IsEnabled = True
        SkipButton.IsEnabled = True
        AbortButton.IsEnabled = True
    End Sub

    ''' <summary>
    ''' Clear the authentication token in the application storage and update the UI.
    ''' </summary>
    Private Sub ClearAuthenticationToken()
        ConfigStore.AuthenticationToken = ""
        AuthenticateButton.IsEnabled = False
        SkipButton.IsEnabled = False
        AbortButton.IsEnabled = False
    End Sub
End Class
