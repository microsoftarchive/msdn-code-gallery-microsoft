'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports SDKTemplate
Imports Windows.Devices.Sensors
Imports Windows.Graphics.Display
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rotationAngle As Double = 0.0
    Private Const toDegrees As Double = 180.0 / Math.PI
    Private accelerometer As Accelerometer

    Public Sub New()
        InitializeComponent()
        accelerometer = accelerometer.GetDefault()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        deviceRotation.Text = rotationAngle.ToString

        If accelerometer IsNot Nothing Then
            AddHandler accelerometer.ReadingChanged, AddressOf CalculateDeviceRotation
        End If


        If DisplayProperties.AutoRotationPreferences = DisplayOrientations.None Then
            LockButton.Content = "Lock"
        Else
            LockButton.Content = "Unlock"
        End If        
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        If accelerometer IsNot Nothing Then
            RemoveHandler accelerometer.ReadingChanged, AddressOf CalculateDeviceRotation
        End If
    End Sub

    Private Sub Scenario2Button_Click(sender As Object, e As RoutedEventArgs)
        If DisplayProperties.AutoRotationPreferences = DisplayOrientations.None Then
            ' since there is no preference currently set, get the current screen orientation and set it as the preference 
            DisplayProperties.AutoRotationPreferences = DisplayProperties.CurrentOrientation
            LockButton.Content = "Unlock"
        Else
            ' something is already set, so reset to no preference
            DisplayProperties.AutoRotationPreferences = DisplayOrientations.None
            LockButton.Content = "Lock"
        End If
    End Sub

    Private Async Sub CalculateDeviceRotation(sender As Accelerometer, args As AccelerometerReadingChangedEventArgs)
        ' Compute the rotation angle based on the accelerometer's position
        Dim angle = Math.Atan2(args.Reading.AccelerationY, args.Reading.AccelerationX) * toDegrees

        ' Since our arrow points upwards insted of the right, we rotate the coordinate system by 90 degrees
        angle += 90

        ' Ensure that the range of the value is between [0, 360)
        If angle < 0 Then
            angle += 360
        End If

        rotationAngle = angle

        ' Update the UI with the new value
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     deviceRotation.Text = rotationAngle.ToString
                                                                 End Sub)

        UpdateArrowForRotation()
    End Sub

    Private Async Sub UpdateArrowForRotation()
        ' Obtain current rotation taking into account a Landscape first or a Portrait first device
        Dim screenRotation = 0

        ' Native orientation can only be Landscape or Portrait
        If DisplayProperties.NativeOrientation = DisplayOrientations.Landscape Then
            Select Case DisplayProperties.CurrentOrientation
                Case DisplayOrientations.Landscape
                    screenRotation = 0
                    Exit Select

                Case DisplayOrientations.Portrait
                    screenRotation = 90
                    Exit Select

                Case DisplayOrientations.LandscapeFlipped
                    screenRotation = 180
                    Exit Select

                Case DisplayOrientations.PortraitFlipped
                    screenRotation = 270
                    Exit Select
                Case Else

                    screenRotation = 0
                    Exit Select
            End Select
        Else
            Select Case DisplayProperties.CurrentOrientation
                Case DisplayOrientations.Landscape
                    screenRotation = 270
                    Exit Select

                Case DisplayOrientations.Portrait
                    screenRotation = 0
                    Exit Select

                Case DisplayOrientations.LandscapeFlipped
                    screenRotation = 90
                    Exit Select

                Case DisplayOrientations.PortraitFlipped
                    screenRotation = 180
                    Exit Select
                Case Else

                    screenRotation = 270
                    Exit Select
            End Select
        End If

        Dim steeringAngle As Double = rotationAngle - screenRotation

        ' Keep the steering angle positive             
        If steeringAngle < 0 Then
            steeringAngle += 360
        End If

        ' Update the UI based on steering action
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     Dim transform As New RotateTransform()
                                                                     transform.Angle = -steeringAngle
                                                                     transform.CenterX = scenario2Image.ActualWidth / 2
                                                                     transform.CenterY = scenario2Image.ActualHeight / 2

                                                                     scenario2Image.RenderTransform = transform
                                                                 End Sub)
		End Sub
End Class
