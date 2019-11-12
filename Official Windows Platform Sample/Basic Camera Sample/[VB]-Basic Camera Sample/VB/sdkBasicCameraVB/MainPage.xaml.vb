'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports Microsoft.Devices
Imports System.IO
Imports System.IO.IsolatedStorage
Imports Microsoft.Xna.Framework.Media


Partial Public Class MainPage
    Inherits PhoneApplicationPage
    ' Variables
    Private savedCounter As Integer = 0
    Private cam As PhotoCamera
    Private library As New MediaLibrary()

    ' Holds the current flash mode.
    Private currentFlashMode As String

    ' Holds the current resolution index.
    Private currentResIndex As Integer = 0

    ' Constructor
    Public Sub New()
        InitializeComponent()
    End Sub

    'Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
    Protected Overrides Sub OnNavigatedTo(ByVal e As System.Windows.Navigation.NavigationEventArgs)

        ' Check to see if the camera is available on the device.
        If (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) = True) OrElse (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) = True) Then
            ' Initialize the camera, when available.
            If PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) Then
                ' Use front-facing camera if available.
                cam = New Microsoft.Devices.PhotoCamera(CameraType.FrontFacing)
            Else
                ' Otherwise, use standard camera on back of device.
                cam = New Microsoft.Devices.PhotoCamera(CameraType.Primary)
            End If

            ' Event is fired when the PhotoCamera object has been initialized.
            AddHandler cam.Initialized, AddressOf cam_Initialized

            ' Event is fired when the capture sequence is complete.
            AddHandler cam.CaptureCompleted, AddressOf cam_CaptureCompleted

            ' Event is fired when the capture sequence is complete and an image is available.
            AddHandler cam.CaptureImageAvailable, AddressOf cam_CaptureImageAvailable

            ' Event is fired when the capture sequence is complete and a thumbnail image is available.
            AddHandler cam.CaptureThumbnailAvailable, AddressOf cam_CaptureThumbnailAvailable

            ' The event is fired when auto-focus is complete.
            AddHandler cam.AutoFocusCompleted, AddressOf cam_AutoFocusCompleted

            ' The event is fired when the viewfinder is tapped (for focus).
            AddHandler viewfinderCanvas.Tap, AddressOf focus_Tapped

            ' The event is fired when the shutter button receives a half press.
            AddHandler CameraButtons.ShutterKeyHalfPressed, AddressOf OnButtonHalfPress

            ' The event is fired when the shutter button receives a full press.
            AddHandler CameraButtons.ShutterKeyPressed, AddressOf OnButtonFullPress

            ' The event is fired when the shutter button is released.
            AddHandler CameraButtons.ShutterKeyReleased, AddressOf OnButtonRelease

            'Set the VideoBrush source to the camera.
            viewfinderBrush.SetSource(cam)
        Else
            ' The camera is not supported on the device.
            ' Write message.
            Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "A Camera is not available on this device.")

            ' Disable UI.
            ShutterButton.IsEnabled = False
            FlashButton.IsEnabled = False
            AFButton.IsEnabled = False
            ResButton.IsEnabled = False
        End If
    End Sub

    Protected Overrides Sub OnNavigatingFrom(ByVal e As System.Windows.Navigation.NavigatingCancelEventArgs)
        If cam IsNot Nothing Then
            ' Dispose camera to minimize power consumption and to expedite shutdown.
            cam.Dispose()

            ' Release memory, ensure garbage collection.
            RemoveHandler cam.Initialized, AddressOf cam_Initialized
            RemoveHandler cam.CaptureCompleted, AddressOf cam_CaptureCompleted
            RemoveHandler cam.CaptureImageAvailable, AddressOf cam_CaptureImageAvailable
            RemoveHandler cam.CaptureThumbnailAvailable, AddressOf cam_CaptureThumbnailAvailable
            RemoveHandler cam.AutoFocusCompleted, AddressOf cam_AutoFocusCompleted
            RemoveHandler CameraButtons.ShutterKeyHalfPressed, AddressOf OnButtonHalfPress
            RemoveHandler CameraButtons.ShutterKeyPressed, AddressOf OnButtonFullPress
            RemoveHandler CameraButtons.ShutterKeyReleased, AddressOf OnButtonRelease
        End If
    End Sub

    ' Update the UI if initialization succeeds.
    Private Sub cam_Initialized(ByVal sender As Object, ByVal e As Microsoft.Devices.CameraOperationCompletedEventArgs)
        If e.Succeeded Then
            ' Write message.
            ' Set flash button text.
            Me.Dispatcher.BeginInvoke(Sub()
                                          txtDebug.Text = "Camera initialized."
                                          FlashButton.Content = "Fl:" & cam.FlashMode.ToString()
                                      End Sub)
        End If
    End Sub

    ' Ensure that the viewfinder is upright in LandscapeRight.
    Protected Overrides Sub OnOrientationChanged(ByVal e As OrientationChangedEventArgs)
        If cam IsNot Nothing Then
            ' LandscapeRight rotation when camera is on back of device.
            Dim landscapeRightRotation As Integer = 180

            ' Change LandscapeRight rotation for front-facing camera.
            If cam.CameraType = CameraType.FrontFacing Then
                landscapeRightRotation = -180
            End If

            ' Rotate video brush from camera.
            If e.Orientation = PageOrientation.LandscapeRight Then
                ' Rotate for LandscapeRight orientation.
                viewfinderBrush.RelativeTransform = New CompositeTransform() With {.CenterX = 0.5, .CenterY = 0.5, .Rotation = landscapeRightRotation}
            Else
                ' Rotate for standard landscape orientation.
                viewfinderBrush.RelativeTransform = New CompositeTransform() With {.CenterX = 0.5, .CenterY = 0.5, .Rotation = 0}
            End If
        End If

        MyBase.OnOrientationChanged(e)
    End Sub

    Private Sub ShutterButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cam IsNot Nothing Then
            Try
                ' Start image capture.
                cam.CaptureImage()
            Catch ex As Exception
                ' Cannot capture an image until the previous capture has completed.
                Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = ex.Message)
            End Try
        End If
    End Sub

    Private Sub cam_CaptureCompleted(ByVal sender As Object, ByVal e As CameraOperationCompletedEventArgs)
        ' Increments the savedCounter variable used for generating JPEG file names.
        savedCounter += 1
    End Sub


    ' Informs when full resolution picture has been taken, saves to local media library and isolated storage.
    Private Sub cam_CaptureImageAvailable(ByVal sender As Object, ByVal e As Microsoft.Devices.ContentReadyEventArgs)
        Dim fileName As String = savedCounter & ".jpg"

        Try
            Deployment.Current.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Captured image available, saving picture.")

            ' Save picture to the library camera roll.
            library.SavePictureToCameraRoll(fileName, e.ImageStream)

            ' Write message to the UI thread.
            Deployment.Current.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Picture has been saved to camera roll.")

            ' Set the position of the stream back to start
            e.ImageStream.Seek(0, SeekOrigin.Begin)

            ' Save picture as JPEG to isolated storage.
            Using isStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
                Using targetStream As IsolatedStorageFileStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write)
                    ' Initialize the buffer for 4KB disk pages.
                    Dim readBuffer(4095) As Byte
                    Dim bytesRead As Integer = -1

                    ' Copy the image to isolated storage. 
                    bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)
                    Do While bytesRead > 0
                        targetStream.Write(readBuffer, 0, bytesRead)
                        bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)
                    Loop
                End Using
            End Using

            ' Write message to the UI thread.
            Deployment.Current.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Picture has been saved to isolated storage.")
        Finally
            ' Close image stream
            e.ImageStream.Close()
        End Try

    End Sub

    ' Informs when thumbnail picture has been taken, saves to isolated storage
    ' User will select this image in the pictures application to bring up the full-resolution picture. 
    Public Sub cam_CaptureThumbnailAvailable(ByVal sender As Object, ByVal e As ContentReadyEventArgs)
        Dim fileName As String = savedCounter & "_th.jpg"

        Try
            ' Write message to UI thread.
            Deployment.Current.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Captured image available, saving thumbnail.")

            ' Save thumbnail as JPEG to isolated storage.
            Using isStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
                Using targetStream As IsolatedStorageFileStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write)
                    ' Initialize the buffer for 4KB disk pages.
                    Dim readBuffer(4095) As Byte
                    Dim bytesRead As Integer = -1

                    ' Copy the thumbnail to isolated storage. 
                    bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)
                    Do While bytesRead > 0
                        targetStream.Write(readBuffer, 0, bytesRead)
                        bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)
                    Loop
                End Using
            End Using

            ' Write message to UI thread.
            Deployment.Current.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Thumbnail has been saved to isolated storage.")
        Finally
            ' Close image stream
            e.ImageStream.Close()
        End Try
    End Sub

    ' Activate a flash mode.
    ' Cycle through flash mode options when the flash button is pressed.
    Private Sub changeFlash_Clicked(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Select Case cam.FlashMode
            Case FlashMode.Off
                If cam.IsFlashModeSupported(FlashMode.On) Then
                    ' Specify that flash should be used.
                    cam.FlashMode = FlashMode.On
                    FlashButton.Content = "Fl:On"
                    currentFlashMode = "Flash mode: On"
                End If
            Case FlashMode.On
                If cam.IsFlashModeSupported(FlashMode.RedEyeReduction) Then
                    ' Specify that the red-eye reduction flash should be used.
                    cam.FlashMode = FlashMode.RedEyeReduction
                    FlashButton.Content = "Fl:RER"
                    currentFlashMode = "Flash mode: RedEyeReduction"
                ElseIf cam.IsFlashModeSupported(FlashMode.Auto) Then
                    ' If red-eye reduction is not supported, specify automatic mode.
                    cam.FlashMode = FlashMode.Auto
                    FlashButton.Content = "Fl:Auto"
                    currentFlashMode = "Flash mode: Auto"
                Else
                    ' If automatic is not supported, specify that no flash should be used.
                    cam.FlashMode = FlashMode.Off
                    FlashButton.Content = "Fl:Off"
                    currentFlashMode = "Flash mode: Off"
                End If
            Case FlashMode.RedEyeReduction
                If cam.IsFlashModeSupported(FlashMode.Auto) Then
                    ' Specify that the flash should be used in the automatic mode.
                    cam.FlashMode = FlashMode.Auto
                    FlashButton.Content = "Fl:Auto"
                    currentFlashMode = "Flash mode: Auto"
                Else
                    ' If automatic is not supported, specify that no flash should be used.
                    cam.FlashMode = FlashMode.Off
                    FlashButton.Content = "Fl:Off"
                    currentFlashMode = "Flash mode: Off"
                End If
            Case FlashMode.Auto
                If cam.IsFlashModeSupported(FlashMode.Off) Then
                    ' Specify that no flash should be used.
                    cam.FlashMode = FlashMode.Off
                    FlashButton.Content = "Fl:Off"
                    currentFlashMode = "Flash mode: Off"
                End If
        End Select

        ' Display current flash mode.
        Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = currentFlashMode)
    End Sub

    ' Provide auto-focus in the viewfinder.
    Private Sub focus_Clicked(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        If cam.IsFocusSupported = True Then
            'Focus when a capture is not in progress.
            Try
                cam.Focus()
            Catch focusError As Exception
                ' Cannot focus when a capture is in progress.
                Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = focusError.Message)
            End Try
        Else
            ' Write message to UI.
            Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Camera does not support programmable auto focus.")
        End If
    End Sub

    Private Sub cam_AutoFocusCompleted(ByVal sender As Object, ByVal e As CameraOperationCompletedEventArgs)
        ' Write message to UI.
        ' Hide the focus brackets.
        Deployment.Current.Dispatcher.BeginInvoke(Sub()
                                                      txtDebug.Text = "Auto focus has completed."
                                                      focusBrackets.Visibility = Visibility.Collapsed
                                                  End Sub)
    End Sub

    ' Provide touch focus in the viewfinder.
    Private Sub focus_Tapped(ByVal sender As Object, ByVal e As GestureEventArgs)
        If cam IsNot Nothing Then
            If cam.IsFocusAtPointSupported = True Then
                Try
                    ' Determine location of tap.
                    Dim tapLocation As Point = e.GetPosition(viewfinderCanvas)

                    ' Position focus brackets with estimated offsets.
                    focusBrackets.SetValue(Canvas.LeftProperty, tapLocation.X - 30)
                    focusBrackets.SetValue(Canvas.TopProperty, tapLocation.Y - 28)

                    ' Determine focus point.
                    Dim focusXPercentage As Double = tapLocation.X \ viewfinderCanvas.Width
                    Dim focusYPercentage As Double = tapLocation.Y \ viewfinderCanvas.Height

                    ' Show focus brackets and focus at point
                    focusBrackets.Visibility = Visibility.Visible
                    cam.FocusAtPoint(focusXPercentage, focusYPercentage)

                    ' Write a message to the UI.
                    Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = String.Format("Camera focusing at point: {0:N2} , {1:N2}", focusXPercentage, focusYPercentage))
                Catch focusError As Exception
                    ' Cannot focus when a capture is in progress.
                    ' Write a message to the UI.
                    ' Hide focus brackets.
                    Me.Dispatcher.BeginInvoke(Sub()
                                                  txtDebug.Text = focusError.Message
                                                  focusBrackets.Visibility = Visibility.Collapsed
                                              End Sub)
                End Try
            Else
                ' Write a message to the UI.
                Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Camera does not support FocusAtPoint().")
            End If
        End If
    End Sub

    Private Sub changeRes_Clicked(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        ' Variables
        Dim resList As IEnumerable(Of Size) = cam.AvailableResolutions
        Dim resCount As Integer = resList.Count()
        Dim res As Size

        ' Poll for available camera resolutions.
        For i = 0 To resCount - 1
            res = resList.ElementAt(i)
        Next i

        ' Set the camera resolution.
        res = resList.ElementAt((currentResIndex + 1) Mod resCount)
        cam.Resolution = res
        currentResIndex = (currentResIndex + 1) Mod resCount

        ' Update the UI.
        txtDebug.Text = String.Format("Setting capture resolution: {0}x{1}", res.Width, res.Height)
        ResButton.Content = "R" & res.Width
    End Sub


    ' Provide auto-focus with a half button press using the hardware shutter button.
    Private Sub OnButtonHalfPress(ByVal sender As Object, ByVal e As EventArgs)
        If cam IsNot Nothing Then
            ' Focus when a capture is not in progress.
            Try
                Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "Half Button Press: Auto Focus")

                cam.Focus()
            Catch focusError As Exception
                ' Cannot focus when a capture is in progress.
                Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = focusError.Message)
            End Try
        End If
    End Sub

    ' Capture the image with a full button press using the hardware shutter button.
    Private Sub OnButtonFullPress(ByVal sender As Object, ByVal e As EventArgs)
        If cam IsNot Nothing Then
            cam.CaptureImage()
        End If
    End Sub

    ' Cancel the focus if the half button press is released using the hardware shutter button.
    Private Sub OnButtonRelease(ByVal sender As Object, ByVal e As EventArgs)

        If cam IsNot Nothing Then
            cam.CancelFocus()
        End If
    End Sub


End Class
