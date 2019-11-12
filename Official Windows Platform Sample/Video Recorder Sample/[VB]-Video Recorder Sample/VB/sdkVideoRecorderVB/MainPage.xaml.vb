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
Imports System.IO.IsolatedStorage
Imports System.IO

Partial Public Class MainPage
    Inherits PhoneApplicationPage
    ' Viewfinder for capturing video.
    Private videoRecorderBrush As VideoBrush

    ' Source and device for capturing video.
    Private captureSource As CaptureSource
    Private videoCaptureDevice As VideoCaptureDevice

    ' File details for storing the recording.        
    Private isoVideoFile As IsolatedStorageFileStream
    Private fileSink As FileSink
    Private isoVideoFileName As String = "CameraMovie.mp4"

    ' For managing button and application state.
    Private Enum ButtonState
        Initialized
        Ready
        Recording
        Playback
        Paused
        NoChange
        CameraNotSupported
    End Enum
    Private currentAppState As ButtonState

    ' Constructor
    Public Sub New()
        InitializeComponent()

        ' Prepare ApplicationBar and buttons.
        PhoneAppBar = CType(ApplicationBar, ApplicationBar)
        PhoneAppBar.IsVisible = True
        StartRecording = (CType(ApplicationBar.Buttons(0), ApplicationBarIconButton))
        StopPlaybackRecording = (CType(ApplicationBar.Buttons(1), ApplicationBarIconButton))
        StartPlayback = (CType(ApplicationBar.Buttons(2), ApplicationBarIconButton))
        PausePlayback = (CType(ApplicationBar.Buttons(3), ApplicationBarIconButton))
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        ' Initialize the video recorder.
        InitializeVideoRecorder()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Dispose of camera and media objects.
        DisposeVideoPlayer()
        DisposeVideoRecorder()

        MyBase.OnNavigatedFrom(e)
    End Sub

    ' Update the buttons and text on the UI thread based on app state.
    Private Sub UpdateUI(ByVal currentButtonState As ButtonState, ByVal statusMessage As String)
        ' Run code on the UI thread.
        ' When the camera is not supported by the device.
        ' First launch of the application, so no video is available.
        ' Ready to record, so video is available for viewing.
        ' Video recording is in progress.
        ' Video playback is in progress.
        ' Video playback has been paused.
        ' Display a message.
        ' Note the current application state.
        Dispatcher.BeginInvoke(Sub()
                                   Select Case currentButtonState
                                       Case ButtonState.CameraNotSupported
                                           StartRecording.IsEnabled = False
                                           StopPlaybackRecording.IsEnabled = False
                                           StartPlayback.IsEnabled = False
                                           PausePlayback.IsEnabled = False
                                       Case ButtonState.Initialized
                                           StartRecording.IsEnabled = True
                                           StopPlaybackRecording.IsEnabled = False
                                           StartPlayback.IsEnabled = False
                                           PausePlayback.IsEnabled = False
                                       Case ButtonState.Ready
                                           StartRecording.IsEnabled = True
                                           StopPlaybackRecording.IsEnabled = False
                                           StartPlayback.IsEnabled = True
                                           PausePlayback.IsEnabled = False
                                       Case ButtonState.Recording
                                           StartRecording.IsEnabled = False
                                           StopPlaybackRecording.IsEnabled = True
                                           StartPlayback.IsEnabled = False
                                           PausePlayback.IsEnabled = False
                                       Case ButtonState.Playback
                                           StartRecording.IsEnabled = False
                                           StopPlaybackRecording.IsEnabled = True
                                           StartPlayback.IsEnabled = False
                                           PausePlayback.IsEnabled = True
                                       Case ButtonState.Paused
                                           StartRecording.IsEnabled = False
                                           StopPlaybackRecording.IsEnabled = True
                                           StartPlayback.IsEnabled = True
                                           PausePlayback.IsEnabled = False
                                       Case Else
                                   End Select
                                   txtDebug.Text = statusMessage
                                   currentAppState = currentButtonState
                               End Sub)
    End Sub

    Public Sub InitializeVideoRecorder()
        If captureSource Is Nothing Then
            ' Create the VideoRecorder objects.
            captureSource = New CaptureSource()
            fileSink = New FileSink()

            videoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice()

            ' Add eventhandlers for captureSource.
            AddHandler captureSource.CaptureFailed, AddressOf OnCaptureFailed

            ' Initialize the camera if it exists on the device.
            If videoCaptureDevice IsNot Nothing Then
                ' Create the VideoBrush for the viewfinder.
                videoRecorderBrush = New VideoBrush()
                videoRecorderBrush.SetSource(captureSource)

                ' Display the viewfinder image on the rectangle.
                viewfinderRectangle.Fill = videoRecorderBrush

                ' Start video capture and display it on the viewfinder.
                captureSource.Start()

                ' Set the button state and the message.
                UpdateUI(ButtonState.Initialized, "Tap record to start recording...")
            Else
                ' Disable buttons when the camera is not supported by the device.
                UpdateUI(ButtonState.CameraNotSupported, "A camera is not supported on this device.")
            End If
        End If
    End Sub

    ' Set recording state: start recording.
    Private Sub StartVideoRecording()
        Try
            ' Connect fileSink to captureSource.
            If captureSource.VideoCaptureDevice IsNot Nothing AndAlso captureSource.State = CaptureState.Started Then
                captureSource.Stop()

                ' Connect the input and output of fileSink.
                fileSink.CaptureSource = captureSource
                fileSink.IsolatedStorageFileName = isoVideoFileName
            End If

            ' Begin recording.
            If captureSource.VideoCaptureDevice IsNot Nothing AndAlso captureSource.State = CaptureState.Stopped Then
                captureSource.Start()
            End If

            ' Set the button states and the message.
            UpdateUI(ButtonState.Recording, "Recording...")

            ' If recording fails, display an error.
        Catch e As Exception
            Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "ERROR: " & e.Message.ToString())
        End Try
    End Sub

    ' Set the recording state: stop recording.
    Private Sub StopVideoRecording()
        Try
            ' Stop recording.
            If captureSource.VideoCaptureDevice IsNot Nothing AndAlso captureSource.State = CaptureState.Started Then
                captureSource.Stop()

                ' Disconnect fileSink.
                fileSink.CaptureSource = Nothing
                fileSink.IsolatedStorageFileName = Nothing

                ' Set the button states and the message.
                UpdateUI(ButtonState.NoChange, "Preparing viewfinder...")

                StartVideoPreview()
            End If
            ' If stop fails, display an error.
        Catch e As Exception
            Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "ERROR: " & e.Message.ToString())
        End Try
    End Sub

    ' Set the recording state: display the video on the viewfinder.
    Private Sub StartVideoPreview()
        Try
            ' Display the video on the viewfinder.
            If captureSource.VideoCaptureDevice IsNot Nothing AndAlso captureSource.State = CaptureState.Stopped Then
                ' Add captureSource to videoBrush.
                videoRecorderBrush.SetSource(captureSource)

                ' Add videoBrush to the visual tree.
                viewfinderRectangle.Fill = videoRecorderBrush

                captureSource.Start()

                ' Set the button states and the message.
                UpdateUI(ButtonState.Ready, "Ready to record.")
            End If
            ' If preview fails, display an error.
        Catch e As Exception
            Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "ERROR: " & e.Message.ToString())
        End Try
    End Sub

    ' Start the video recording.
    Private Sub StartRecording_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Avoid duplicate taps.
        StartRecording.IsEnabled = False

        StartVideoRecording()
    End Sub

    ' Handle stop requests.
    Private Sub StopPlaybackRecording_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Avoid duplicate taps.
        StopPlaybackRecording.IsEnabled = False

        ' Stop during video recording.
        If currentAppState = ButtonState.Recording Then
            StopVideoRecording()

            ' Set the button state and the message.
            UpdateUI(ButtonState.NoChange, "Recording stopped.")

            ' Stop during video playback.
        Else
            ' Remove playback objects.
            DisposeVideoPlayer()

            StartVideoPreview()

            ' Set the button state and the message.
            UpdateUI(ButtonState.NoChange, "Playback stopped.")
        End If
    End Sub

    ' Start video playback.
    Private Sub StartPlayback_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Avoid duplicate taps.
        StartPlayback.IsEnabled = False

        ' Start video playback when the file stream exists.
        If isoVideoFile IsNot Nothing Then
            VideoPlayer.Play()
            ' Start the video for the first time.
        Else
            ' Stop the capture source.
            captureSource.Stop()

            ' Remove VideoBrush from the tree.
            viewfinderRectangle.Fill = Nothing

            ' Create the file stream and attach it to the MediaElement.
            isoVideoFile = New IsolatedStorageFileStream(isoVideoFileName, FileMode.Open, FileAccess.Read, IsolatedStorageFile.GetUserStoreForApplication())

            VideoPlayer.SetSource(isoVideoFile)

            ' Add an event handler for the end of playback.
            AddHandler VideoPlayer.MediaEnded, AddressOf VideoPlayerMediaEnded

            ' Start video playback.
            VideoPlayer.Play()
        End If

        ' Set the button state and the message.
        UpdateUI(ButtonState.Playback, "Playback started.")
    End Sub

    ' Pause video playback.
    Private Sub PausePlayback_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Avoid duplicate taps.
        PausePlayback.IsEnabled = False

        ' If mediaElement exists, pause playback.
        If VideoPlayer IsNot Nothing Then
            VideoPlayer.Pause()
        End If

        ' Set the button state and the message.
        UpdateUI(ButtonState.Paused, "Playback paused.")
    End Sub

    Private Sub DisposeVideoPlayer()
        If VideoPlayer IsNot Nothing Then
            ' Stop the VideoPlayer MediaElement.
            VideoPlayer.Stop()

            ' Remove playback objects.
            VideoPlayer.Source = Nothing
            isoVideoFile = Nothing

            ' Remove the event handler.
            RemoveHandler VideoPlayer.MediaEnded, AddressOf VideoPlayerMediaEnded
        End If
    End Sub

    Private Sub DisposeVideoRecorder()
        If captureSource IsNot Nothing Then
            ' Stop captureSource if it is running.
            If captureSource.VideoCaptureDevice IsNot Nothing AndAlso captureSource.State = CaptureState.Started Then
                captureSource.Stop()
            End If

            ' Remove the event handlers for capturesource and the shutter button.
            RemoveHandler captureSource.CaptureFailed, AddressOf OnCaptureFailed

            ' Remove the video recording objects.
            captureSource = Nothing
            videoCaptureDevice = Nothing
            fileSink = Nothing
            videoRecorderBrush = Nothing
        End If
    End Sub

    ' If recording fails, display an error message.
    Private Sub OnCaptureFailed(ByVal sender As Object, ByVal e As ExceptionRoutedEventArgs)
        Me.Dispatcher.BeginInvoke(Sub() txtDebug.Text = "ERROR: " & e.ErrorException.Message.ToString())
    End Sub

    ' Display the viewfinder when playback ends.
    Public Sub VideoPlayerMediaEnded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Remove the playback objects.
        DisposeVideoPlayer()

        StartVideoPreview()
    End Sub
End Class
