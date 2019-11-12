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
Imports System.Threading
Imports System.Threading.Tasks
Imports Windows.Devices.Geolocation
Imports Windows.Foundation
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private _geolocator As Geolocator = Nothing
    Private _cts As CancellationTokenSource = Nothing

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        _geolocator = New Geolocator
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        GetGeolocationButton.IsEnabled = True
        CancelGetGeolocationButton.IsEnabled = False
    End Sub

    ''' <summary>
    ''' Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
    ''' </summary>
    ''' <param name="e">
    ''' Event data that can be examined by overriding code. The event data is representative
    ''' of the navigation that will unload the current Page unless canceled. The
    ''' navigation can potentially be canceled by setting Cancel.
    ''' </param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'getGeolocation' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub GetGeolocation(sender As Object, e As RoutedEventArgs)
        GetGeolocationButton.IsEnabled = False
        CancelGetGeolocationButton.IsEnabled = True

        Try
            ' Get cancellation token
            _cts = New CancellationTokenSource()
            Dim token As CancellationToken = _cts.Token

            rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage)

            ' Carry out the operation
            Dim pos As Geoposition = Await _geolocator.GetGeopositionAsync().AsTask(token)

            rootPage.NotifyUser("Updated", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = pos.Coordinate.Latitude.ToString()
            ScenarioOutput_Longitude.Text = pos.Coordinate.Longitude.ToString()
            ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString()
        Catch generatedExceptionName As System.UnauthorizedAccessException
            rootPage.NotifyUser("Disabled", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = "No data"
            ScenarioOutput_Longitude.Text = "No data"
            ScenarioOutput_Accuracy.Text = "No data"
        Catch generatedExceptionName As TaskCanceledException
            rootPage.NotifyUser("Canceled", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.StatusMessage)
        Finally
            _cts = Nothing
        End Try

        GetGeolocationButton.IsEnabled = True
        CancelGetGeolocationButton.IsEnabled = False
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'unregisterBackgroundTask' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CancelGetGeolocation(sender As Object, e As RoutedEventArgs)
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If

        GetGeolocationButton.IsEnabled = True
        CancelGetGeolocationButton.IsEnabled = False
    End Sub
End Class
