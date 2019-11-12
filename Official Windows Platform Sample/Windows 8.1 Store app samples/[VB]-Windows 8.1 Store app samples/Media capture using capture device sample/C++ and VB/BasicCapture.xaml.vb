'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Media.MediaProperties
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.Media
Imports Windows.Media.Capture
Imports Windows.Foundation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class BasicCapture
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private m_mediaCaptureMgr As Windows.Media.Capture.MediaCapture
    Private m_photoStorageFile As Windows.Storage.StorageFile
    Private m_recordStorageFile As Windows.Storage.StorageFile
    Private m_bRecording As Boolean
    Private m_bSuspended As Boolean
    Private m_bPreviewing As Boolean
    Private m_mediaPropertyChanged As TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)

    Private ReadOnly PHOTO_FILE_NAME As String = "photo.jpg"
    Private ReadOnly VIDEO_FILE_NAME As String = "video.mp4"



    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        ScenarioInit()
        m_mediaPropertyChanged = CType([Delegate].Combine(m_mediaPropertyChanged, New TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)(AddressOf SystemMediaControls_PropertyChanged)), TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs))
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        AddHandler systemMediaControls.PropertyChanged, m_mediaPropertyChanged
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        RemoveHandler systemMediaControls.PropertyChanged, m_mediaPropertyChanged
        ScenarioClose()
    End Sub

    Private Sub ScenarioInit()
        btnStartDevice1.IsEnabled = True
        btnStartPreview1.IsEnabled = False
        btnStartStopRecord1.IsEnabled = False
        btnStartStopRecord1.Content = "StartRecord"
        btnTakePhoto1.IsEnabled = False
        btnTakePhoto1.Content = "TakePhoto"

        m_bRecording = False
        m_bPreviewing = False
        m_bSuspended = False

        previewElement1.Source = Nothing
        playbackElement1.Source = Nothing
        imageElement1.Source = Nothing
        sldBrightness.IsEnabled = False
        sldContrast.IsEnabled = False

        ShowStatusMessage("")
    End Sub

    Private Async Sub ScenarioClose()
        Try
            If m_bRecording Then
                ShowStatusMessage("Stopping Record")

                Await m_mediaCaptureMgr.StopRecordAsync()
                m_bRecording = False
            End If
            If m_bPreviewing Then
                ShowStatusMessage("Stopping preview")
                Await m_mediaCaptureMgr.StopPreviewAsync()
                m_bPreviewing = False
            End If

            If m_mediaCaptureMgr IsNot Nothing Then
                ShowStatusMessage("Stopping Camera")
                previewElement1.Source = Nothing
                m_mediaCaptureMgr.Dispose()
            End If
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Private Async Sub SystemMediaControls_PropertyChanged(ByVal sender As SystemMediaTransportControls, ByVal e As SystemMediaTransportControlsPropertyChangedEventArgs)
        Select Case e.Property
            Case SystemMediaTransportControlsProperty.SoundLevel
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                             If sender.SoundLevel <> Windows.Media.SoundLevel.Muted Then
                                                                                                 ScenarioInit()
                                                                                             Else
                                                                                                 ScenarioClose()
                                                                                             End If
                                                                                         End Sub)

            Case Else
        End Select
    End Sub

    Public Async Sub RecordLimitationExceeded(ByVal currentCaptureObject As Windows.Media.Capture.MediaCapture)

        If m_bRecording Then
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Async Sub()
                                                                                         'if camera does not support record and Takephoto at the same time
                                                                                         'enable TakePhoto button again, after record finished
                                                                                         Try
                                                                                             ShowStatusMessage("Stopping Record on exceeding max record duration")
                                                                                             Await m_mediaCaptureMgr.StopRecordAsync()
                                                                                             m_bRecording = False
                                                                                             btnStartStopRecord1.Content = "StartRecord"
                                                                                             btnStartStopRecord1.IsEnabled = True
                                                                                             ShowStatusMessage("Stopped record on exceeding max record duration:" & m_recordStorageFile.Path)
                                                                                             If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                                                                                                 btnTakePhoto1.Content = "TakePhoto"
                                                                                                 btnTakePhoto1.IsEnabled = True
                                                                                             End If
                                                                                         Catch e As Exception
                                                                                             ShowExceptionMessage(e)
                                                                                         End Try
                                                                                     End Sub)
        End If
    End Sub

    Public Async Sub Failed(ByVal currentCaptureObject As Windows.Media.Capture.MediaCapture, ByVal currentFailure As MediaCaptureFailedEventArgs)
        Try
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() ShowStatusMessage("Fatal error" & currentFailure.Message))
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Friend Async Sub btnStartDevice_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            btnStartDevice1.IsEnabled = False
            ShowStatusMessage("Starting device")
            m_mediaCaptureMgr = New Windows.Media.Capture.MediaCapture()
            Await m_mediaCaptureMgr.InitializeAsync()

            If m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceId <> "" AndAlso m_mediaCaptureMgr.MediaCaptureSettings.AudioDeviceId <> "" Then

                btnStartPreview1.IsEnabled = True
                btnStartStopRecord1.IsEnabled = True
                btnTakePhoto1.IsEnabled = True

                ShowStatusMessage("Device initialized successful")

                AddHandler m_mediaCaptureMgr.RecordLimitationExceeded, AddressOf RecordLimitationExceeded
                AddHandler m_mediaCaptureMgr.Failed, AddressOf Failed
            Else
                btnStartDevice1.IsEnabled = True
                ShowStatusMessage("No VideoDevice/AudioDevice Found")
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub btnStartPreview_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        m_bPreviewing = False
        Try
            ShowStatusMessage("Starting preview")
            btnStartPreview1.IsEnabled = False

            previewCanvas1.Visibility = Windows.UI.Xaml.Visibility.Visible
            previewElement1.Source = m_mediaCaptureMgr
            Await m_mediaCaptureMgr.StartPreviewAsync()
            If (m_mediaCaptureMgr.VideoDeviceController.Brightness IsNot Nothing) AndAlso m_mediaCaptureMgr.VideoDeviceController.Brightness.Capabilities.Supported Then
                SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Brightness, sldBrightness)
            End If
            If (m_mediaCaptureMgr.VideoDeviceController.Contrast IsNot Nothing) AndAlso m_mediaCaptureMgr.VideoDeviceController.Contrast.Capabilities.Supported Then
                SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Contrast, sldContrast)
            End If
            m_bPreviewing = True
            ShowStatusMessage("Start preview successful")

        Catch exception As Exception
            m_bPreviewing = False
            previewElement1.Source = Nothing
            btnStartPreview1.IsEnabled = True
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub btnTakePhoto_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)

        Try
            ShowStatusMessage("Taking photo")
            btnTakePhoto1.IsEnabled = False

            If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                'if camera does not support record and Takephoto at the same time
                'disable Record button when taking photo
                btnStartStopRecord1.IsEnabled = False
            End If

            m_photoStorageFile = Await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName)

            ShowStatusMessage("Create photo file successful")
            Dim imageProperties As ImageEncodingProperties = ImageEncodingProperties.CreateJpeg()

            Await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile)

            btnTakePhoto1.IsEnabled = True
            ShowStatusMessage("Photo taken")

            If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                'if camera does not support record and Takephoto at the same time
                'enable Record button after taking photo
                btnStartStopRecord1.IsEnabled = True
            End If

            Dim photoStream = Await m_photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read)

            ShowStatusMessage("File open successful")
            Dim bmpimg = New BitmapImage()

            bmpimg.SetSource(photoStream)
            imageElement1.Source = bmpimg
            ShowStatusMessage(Me.m_photoStorageFile.Path)

        Catch exception As Exception
            ShowExceptionMessage(exception)
            btnTakePhoto1.IsEnabled = True
        End Try
    End Sub

    Private Async Sub StartRecord()
        Try
            ShowStatusMessage("Starting Record")
            Dim fileName As String
            fileName = VIDEO_FILE_NAME

            m_recordStorageFile = Await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName)

            ShowStatusMessage("Create record file successful")

            Dim recordProfile As MediaEncodingProfile = Nothing
            recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto)

            Await m_mediaCaptureMgr.StartRecordToStorageFileAsync(recordProfile, m_recordStorageFile)
            m_bRecording = True
            btnStartStopRecord1.IsEnabled = True
            btnStartStopRecord1.Content = "StopRecord"

            ShowStatusMessage("Start Record successful")
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub


    Friend Async Sub btnStartStopRecord_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            btnStartStopRecord1.IsEnabled = False
            playbackElement1.Source = Nothing

            If btnStartStopRecord1.Content.ToString() = "StartRecord" Then
                If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                    'if camera does not support record and Takephoto at the same time
                    'disable TakePhoto button when recording
                    btnTakePhoto1.IsEnabled = False
                End If

                StartRecord()

            Else '(btnStartStopRecord1.Content.ToString() == "StopRecord")
                ShowStatusMessage("Stopping Record")

                Await m_mediaCaptureMgr.StopRecordAsync()

                m_bRecording = False
                btnStartStopRecord1.IsEnabled = True
                btnStartStopRecord1.Content = "StartRecord"

                If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                    'if camera does not support lowlag record and lowlag photo at the same time
                    'enable TakePhoto button after recording
                    btnTakePhoto1.IsEnabled = True
                End If

                ShowStatusMessage("Stop record successful")
                If Not m_bSuspended Then
                    Dim stream = Await m_recordStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read)

                    ShowStatusMessage("Record file opened")
                    ShowStatusMessage(Me.m_recordStorageFile.Path)
                    playbackElement1.AutoPlay = True
                    playbackElement1.SetSource(stream, Me.m_recordStorageFile.FileType)
                    playbackElement1.Play()
                End If

            End If
        Catch ex As Exception
            ShowExceptionMessage(ex)
            m_bRecording = False
            btnStartStopRecord1.IsEnabled = True
            btnStartStopRecord1.Content = "StartRecord"
        End Try

    End Sub

    Private Sub SetupVideoDeviceControl(ByVal videoDeviceControl As Windows.Media.Devices.MediaDeviceControl, ByVal slider As Slider)
        Try
            If (videoDeviceControl.Capabilities).Supported Then
                slider.IsEnabled = True
                slider.Maximum = videoDeviceControl.Capabilities.Max
                slider.Minimum = videoDeviceControl.Capabilities.Min
                slider.StepFrequency = videoDeviceControl.Capabilities.Step
                Dim controlValue As Double = 0
                If videoDeviceControl.TryGetValue(controlValue) Then
                    slider.Value = controlValue
                End If
            Else
                slider.IsEnabled = False
            End If
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    ' VideoDeviceControllers
    Friend Sub sldBrightness_ValueChanged(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs)
        Try
            Dim succeeded As Boolean = m_mediaCaptureMgr.VideoDeviceController.Brightness.TrySetValue(sldBrightness.Value)
            If Not succeeded Then
                ShowStatusMessage("Set Brightness failed")
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Sub sldContrast_ValueChanged(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs)
        Try
            Dim succeeded As Boolean = m_mediaCaptureMgr.VideoDeviceController.Contrast.TrySetValue(sldContrast.Value)
            If Not succeeded Then
                ShowStatusMessage("Set Contrast failed")
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Private Sub ShowStatusMessage(ByVal text As String)
        rootPage.NotifyUser(text, NotifyType.StatusMessage)
    End Sub

    Private Sub ShowExceptionMessage(ByVal ex As Exception)
        rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
    End Sub
End Class
