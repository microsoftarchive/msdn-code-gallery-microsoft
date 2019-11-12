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

    Private _sensor As SimpleOrientationSensor

    Public Sub New()
        Me.InitializeComponent()

        _sensor = SimpleOrientationSensor.GetDefault
        If _sensor Is Nothing Then
            rootPage.NotifyUser("No simple orientation sensor found", NotifyType.StatusMessage)
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
            RemoveHandler _sensor.OrientationChanged, AddressOf OrientationChanged
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
                AddHandler _sensor.OrientationChanged, AddressOf OrientationChanged
            Else
                ' Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                RemoveHandler _sensor.OrientationChanged, AddressOf OrientationChanged
            End If
        End If
    End Sub

    ''' <summary>
    ''' Helper method to display the device orientation in the specified text box.
    ''' </summary>
    ''' <param name="tb">
    ''' The text box receiving the orientation value.
    ''' </param>
    ''' <param name="orientation">
    ''' The orientation value.
    ''' </param>
    Private Sub DisplayOrientation(tb As TextBlock, orientation As SimpleOrientation)
        Select Case orientation
            Case SimpleOrientation.NotRotated
                tb.Text = "Not Rotated"
                Exit Select
            Case SimpleOrientation.Rotated90DegreesCounterclockwise
                tb.Text = "Rotated 90 Degrees Counterclockwise"
                Exit Select
            Case SimpleOrientation.Rotated180DegreesCounterclockwise
                tb.Text = "Rotated 180 Degrees Counterclockwise"
                Exit Select
            Case SimpleOrientation.Rotated270DegreesCounterclockwise
                tb.Text = "Rotated 270 Degrees Counterclockwise"
                Exit Select
            Case SimpleOrientation.Faceup
                tb.Text = "Faceup"
                Exit Select
            Case SimpleOrientation.Facedown
                tb.Text = "Facedown"
                Exit Select
            Case Else
                tb.Text = "Unknown orientation"
                Exit Select
        End Select
    End Sub

    ''' <summary>
    ''' This is the event handler for OrientationChanged events.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub OrientationChanged(sender As Object, e As SimpleOrientationSensorOrientationChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     DisplayOrientation(ScenarioOutput_Orientation, e.Orientation)
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Enable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioEnable(sender As Object, e As RoutedEventArgs)
        If _sensor IsNot Nothing Then
            AddHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged
            AddHandler _sensor.OrientationChanged, AddressOf OrientationChanged

            ScenarioEnableButton.IsEnabled = False
            ScenarioDisableButton.IsEnabled = True

            ' Display the current orientation once while waiting for the next orientation change
            DisplayOrientation(ScenarioOutput_Orientation, _sensor.GetCurrentOrientation())
        Else
            rootPage.NotifyUser("No simple orientation sensor found", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Disable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioDisable(sender As Object, e As RoutedEventArgs)
        RemoveHandler Window.Current.VisibilityChanged, AddressOf VisibilityChanged
        RemoveHandler _sensor.OrientationChanged, AddressOf OrientationChanged

        ScenarioEnableButton.IsEnabled = True
        ScenarioDisableButton.IsEnabled = False
    End Sub
End Class
