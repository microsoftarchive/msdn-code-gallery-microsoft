'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Devices.Sensors
Imports Windows.Foundation
Imports System.Threading.Tasks
Imports Windows.UI.Core

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Private _sensor As LightSensor
    Private _desiredReportInterval As UInteger
    Private _dispatcherTimer As DispatcherTimer

    Public Sub New()
        Me.InitializeComponent()

        _sensor = LightSensor.GetDefault
        If _sensor IsNot Nothing Then
            ' Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
            ' This value will be used later to activate the sensor.
            Dim minReportInterval As UInteger = _sensor.MinimumReportInterval
            _desiredReportInterval = If(minReportInterval > 100, minReportInterval, 100)

            ' Set up a DispatchTimer
            _dispatcherTimer = New DispatcherTimer
            AddHandler _dispatcherTimer.Tick, AddressOf DisplayCurrentReading
            _dispatcherTimer.Interval = New TimeSpan(0, 0, 0, 0, CInt(_desiredReportInterval))
        Else
            rootPage.NotifyUser("No light sensor found", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ScenarioEnableButton.IsEnabled = True
        ScenarioDisableButton.IsEnabled = False
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
        If ScenarioDisableButton.IsEnabled Then
            RemoveHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged

            ' Stop the dispatcher
            _dispatcherTimer.Stop()

            ' Restore the default report interval to release resources while the sensor is not in use
            _sensor.ReportInterval = 0
        End If

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the event handler for VisibilityChanged events. You would register for these notifications
    ''' if handling sensor data when the app is not visible could cause unintended actions in the app.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e">
    ''' Event data that can be examined for the current visibility state.
    ''' </param>
    Private Sub VisibilityChanged(sender As Object, e As VisibilityChangedEventArgs)
        If ScenarioDisableButton.IsEnabled Then
            If e.Visible
                ' Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
                _dispatcherTimer.Start()
            Else
                ' Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                _dispatcherTimer.Stop()
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the dispatcher callback.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub DisplayCurrentReading(sender As Object, args As Object)
        Dim reading As LightSensorReading = _sensor.GetCurrentReading()
        If reading IsNot Nothing Then
            ScenarioOutput_LUX.Text = String.Format("{0,5:0.00}", reading.IlluminanceInLux)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Enable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioEnable(sender As Object, e As RoutedEventArgs)
        If _sensor IsNot Nothing Then
            ' Set the report interval to enable the sensor for polling
            _sensor.ReportInterval = _desiredReportInterval

            AddHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged
            _dispatcherTimer.Start()

            ScenarioEnableButton.IsEnabled = False
            ScenarioDisableButton.IsEnabled = True
        Else
            rootPage.NotifyUser("No light sensor found", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Disable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioDisable(sender As Object, e As RoutedEventArgs)
        RemoveHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged

        ' Stop the dispatcher
        _dispatcherTimer.Stop()

        ' Restore the default report interval to release resources while the sensor is not in use
        _sensor.ReportInterval = 0

        ScenarioEnableButton.IsEnabled = True
        ScenarioDisableButton.IsEnabled = False
    End Sub
End Class
