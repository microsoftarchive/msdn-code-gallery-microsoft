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

    Private _sensor As SimpleOrientationSensor

    Public Sub New()
        Me.InitializeComponent()

        _sensor = SimpleOrientationSensor.GetDefault
        If _sensor Is Nothing Then
            rootPage.NotifyUser("No simple orientation sensor found", NotifyType.StatusMessage)
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
    ''' This is the click handler for the 'Enable' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ScenarioGet(sender As Object, e As RoutedEventArgs)
        If _sensor IsNot Nothing Then
            DisplayOrientation(ScenarioOutput_Orientation, _sensor.GetCurrentOrientation())
        Else
            rootPage.NotifyUser("No simple orientation sensor found", NotifyType.StatusMessage)
        End If
    End Sub
End Class
