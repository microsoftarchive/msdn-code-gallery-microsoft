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
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private _geolocator As Geolocator = Nothing

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
        StartTrackingButton.IsEnabled = True
        StopTrackingButton.IsEnabled = False
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
        If StopTrackingButton.IsEnabled Then
            RemoveHandler _geolocator.PositionChanged, AddressOf onPositionChanged
            RemoveHandler _geolocator.StatusChanged, AddressOf onStatusChanged
        End If

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the event handler for PositionChanged events.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub onPositionChanged(sender As Geolocator, e As PositionChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     Dim pos As Geoposition = e.Position

                                                                     rootPage.NotifyUser("Updated", NotifyType.StatusMessage)

                                                                     ScenarioOutput_Latitude.Text = pos.Coordinate.Latitude.ToString()
                                                                     ScenarioOutput_Longitude.Text = pos.Coordinate.Longitude.ToString()
                                                                     ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString()
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This is the event handler for StatusChanged events.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub onStatusChanged(sender As Geolocator, e As StatusChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     Select Case e.Status
                                                                         Case PositionStatus.Ready
                                                                             ' Location platform is providing valid data.
                                                                             ScenarioOutput_Status.Text = "Ready"
                                                                             Exit Select

                                                                         Case PositionStatus.Initializing
                                                                             ' Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                                                                             ScenarioOutput_Status.Text = "Initializing"
                                                                             Exit Select

                                                                         Case PositionStatus.NoData
                                                                             ' Location platform could not obtain location data.
                                                                             ScenarioOutput_Status.Text = "No data"
                                                                             Exit Select

                                                                         Case PositionStatus.Disabled
                                                                             ' The permission to access location data is denied by the user or other policies.
                                                                             ScenarioOutput_Status.Text = "Disabled"

                                                                             'Clear cached location data if any
                                                                             ScenarioOutput_Latitude.Text = "No data"
                                                                             ScenarioOutput_Longitude.Text = "No data"
                                                                             ScenarioOutput_Accuracy.Text = "No data"
                                                                             Exit Select

                                                                         Case PositionStatus.NotInitialized
                                                                             ' The location platform is not initialized. This indicates that the application has not made a request for location data.
                                                                             ScenarioOutput_Status.Text = "Not initialized"
                                                                             Exit Select

                                                                         Case PositionStatus.NotAvailable
                                                                             ' The location platform is not available on this version of the OS.
                                                                             ScenarioOutput_Status.Text = "Not available"
                                                                             Exit Select

                                                                         Case Else

                                                                             ScenarioOutput_Status.Text = "Unknown"
                                                                             Exit Select
                                                                     End Select
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'startTracking' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StartTracking(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage)

        AddHandler _geolocator.PositionChanged, AddressOf onPositionChanged
        AddHandler _geolocator.StatusChanged, AddressOf onStatusChanged

        StartTrackingButton.IsEnabled = False
        StopTrackingButton.IsEnabled = True
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'stopTracking' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StopTracking(sender As Object, e As RoutedEventArgs)
        RemoveHandler _geolocator.PositionChanged, AddressOf onPositionChanged
        RemoveHandler _geolocator.StatusChanged, AddressOf onStatusChanged

        StartTrackingButton.IsEnabled = True
        StopTrackingButton.IsEnabled = False
    End Sub
End Class
