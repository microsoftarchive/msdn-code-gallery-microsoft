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
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Private _compass As Compass
    Private _desiredReportInterval As UInteger

    Public Sub New()
        Me.InitializeComponent()

        _compass = Compass.GetDefault()
        If _compass IsNot Nothing Then
            ' Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
            ' This value will be used later to activate the sensor.
            Dim minReportInterval As UInteger = _compass.MinimumReportInterval
            _desiredReportInterval = If(minReportInterval > 16, minReportInterval, 16)
        Else
            rootPage.NotifyUser("No compass found", NotifyType.StatusMessage)
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
            RemoveHandler _compass.ReadingChanged, AddressOf ReadingChanged

            ' Restore the default report interval to release resources while the sensor is not in use
            _compass.ReportInterval = 0
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
                AddHandler _compass.ReadingChanged, AddressOf ReadingChanged
            Else
                ' Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                RemoveHandler _compass.ReadingChanged, AddressOf ReadingChanged
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the event handler for ReadingChanged events.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub ReadingChanged(sender As Object, e As CompassReadingChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     Dim reading As CompassReading = e.Reading
                                                                     ScenarioOutput_MagneticNorth.Text = String.Format("{0,5:0.00}", reading.HeadingMagneticNorth)
                                                                     If reading.HeadingTrueNorth IsNot Nothing Then
                                                                         ScenarioOutput_TrueNorth.Text = String.Format("{0,5:0.00}", reading.HeadingTrueNorth)
                                                                     Else
                                                                         ScenarioOutput_TrueNorth.Text = "No data"
                                                                     End If
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Enable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioEnable(sender As Object, e As RoutedEventArgs)
        If _compass IsNot Nothing Then
            ' Establish the report interval
            _compass.ReportInterval = _desiredReportInterval

            AddHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged
            AddHandler _compass.ReadingChanged, AddressOf ReadingChanged

            ScenarioEnableButton.IsEnabled = False
            ScenarioDisableButton.IsEnabled = True
        Else
            rootPage.NotifyUser("No compass found", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Disable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioDisable(sender As Object, e As RoutedEventArgs)
        RemoveHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged
        RemoveHandler _compass.ReadingChanged, AddressOf ReadingChanged

        ' Restore the default report interval to release resources while the sensor is not in use
        _compass.ReportInterval = 0

        ScenarioEnableButton.IsEnabled = True
        ScenarioDisableButton.IsEnabled = False
    End Sub
End Class
