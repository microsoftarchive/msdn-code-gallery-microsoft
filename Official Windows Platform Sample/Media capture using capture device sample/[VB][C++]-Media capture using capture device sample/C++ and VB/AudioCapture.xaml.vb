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
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.Media
Imports Windows.Media.Capture
Imports Windows.Media.Devices
Imports Windows.Devices.Enumeration
Imports Windows.Foundation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class AudioCapture
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private m_mediaCaptureMgr As Windows.Media.Capture.MediaCapture
    Private m_recordStorageFile As Windows.Storage.StorageFile
    Private m_bRecording As Boolean
    Private m_bSuspended As Boolean
    Private m_bUserRequestedRaw As Boolean
    Private m_bRawAudioSupported As Boolean
    Private m_mediaPropertyChanged As TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)

    Private ReadOnly AUDIO_FILE_NAME As String = "audio.mp4"

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        ScenarioInit()
        m_mediaPropertyChanged = New TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)(AddressOf SystemMediaControls_PropertyChanged)
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

    Private Async Sub ScenarioInit()
        btnStartDevice3.IsEnabled = True
        btnStartStopRecord3.IsEnabled = False
        m_bRecording = False
        recordRawAudio.IsChecked = False
        recordRawAudio.IsEnabled = False
        m_bUserRequestedRaw = False
        m_bRawAudioSupported = False
        playbackElement3.Source = Nothing
        m_bSuspended = False
        ShowStatusMessage("")


        'Read system's raw audio stream support
        Dim propertiesToRetrieve() As String = {"System.Devices.AudioDevice.RawProcessingSupported"}
        Try
            Dim device = Await DeviceInformation.CreateFromIdAsync(MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Communications), propertiesToRetrieve)
            m_bRawAudioSupported = device.Properties("System.Devices.AudioDevice.RawProcessingSupported").Equals(True)
            If m_bRawAudioSupported Then
                recordRawAudio.IsEnabled = True
                ShowStatusMessage("Raw audio recording is supported")
            Else
                ShowStatusMessage("Raw audio recording is not supported")
            End If
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Private Async Sub ScenarioClose()
        If m_bRecording Then
            ShowStatusMessage("Stopping Record on invisibility")

            Await m_mediaCaptureMgr.StopRecordAsync()
            m_bRecording = False
            EnableButton(True, "StartStopRecord")
            m_mediaCaptureMgr.Dispose()
        End If
        If m_mediaCaptureMgr IsNot Nothing Then
            m_mediaCaptureMgr.Dispose()
        End If

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
        Try
            If m_bRecording Then
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Async Sub()
                                                                                             Try
                                                                                                 ShowStatusMessage("Stopping Record on exceeding max record duration")
                                                                                                 Await m_mediaCaptureMgr.StopRecordAsync()
                                                                                                 m_bRecording = False
                                                                                                 SwitchRecordButtonContent()
                                                                                                 EnableButton(True, "StartStopRecord")
                                                                                                 ShowStatusMessage("Stopped record on exceeding max record duration:" & m_recordStorageFile.Path)
                                                                                             Catch e As Exception
                                                                                                 ShowExceptionMessage(e)
                                                                                             End Try
                                                                                         End Sub)
            End If
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Public Sub Failed(ByVal currentCaptureObject As Windows.Media.Capture.MediaCapture, ByVal currentFailure As MediaCaptureFailedEventArgs)
        Try
            Dim ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                                 Try
                                                                                                     ShowStatusMessage("Fatal error" & currentFailure.Message)
                                                                                                 Catch e As Exception
                                                                                                     ShowExceptionMessage(e)
                                                                                                 End Try
                                                                                             End Sub)
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Private Async Sub startAudioCapture()
        m_mediaCaptureMgr = New Windows.Media.Capture.MediaCapture()
        Dim settings = New Windows.Media.Capture.MediaCaptureInitializationSettings()
        settings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio
        settings.MediaCategory = Windows.Media.Capture.MediaCategory.Other
        settings.AudioProcessing = If(m_bRawAudioSupported AndAlso m_bUserRequestedRaw, Windows.Media.AudioProcessing.Raw, Windows.Media.AudioProcessing.Default)
        Await m_mediaCaptureMgr.InitializeAsync(settings)

        EnableButton(True, "StartPreview")
        EnableButton(True, "StartStopRecord")
        EnableButton(True, "TakePhoto")
        ShowStatusMessage("Device initialized successfully")
        AddHandler m_mediaCaptureMgr.RecordLimitationExceeded, AddressOf RecordLimitationExceeded

        AddHandler m_mediaCaptureMgr.Failed, AddressOf Failed

    End Sub

    Private Sub btnStartDevice_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            m_bUserRequestedRaw = If(recordRawAudio.IsChecked.Value, True, False)
            recordRawAudio.IsEnabled = False
            EnableButton(False, "StartDevice")
            ShowStatusMessage("Starting device")
            startAudioCapture()
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub btnStartStopRecord_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            Dim fileName As String
            EnableButton(False, "StartStopRecord")

            If Not m_bRecording Then
                ShowStatusMessage("Starting Record")

                fileName = AUDIO_FILE_NAME

                m_recordStorageFile = Await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName)

                ShowStatusMessage("Create record file successful")

                Dim recordProfile As MediaEncodingProfile = Nothing
                recordProfile = MediaEncodingProfile.CreateM4a(Windows.Media.MediaProperties.AudioEncodingQuality.Auto)

                Await m_mediaCaptureMgr.StartRecordToStorageFileAsync(recordProfile, Me.m_recordStorageFile)

                m_bRecording = True
                SwitchRecordButtonContent()
                EnableButton(True, "StartStopRecord")

                ShowStatusMessage("Start Record successful")

            Else
                ShowStatusMessage("Stopping Record")

                Await m_mediaCaptureMgr.StopRecordAsync()

                m_bRecording = False
                EnableButton(True, "StartStopRecord")
                SwitchRecordButtonContent()

                ShowStatusMessage("Stop record successful")
                If Not m_bSuspended Then
                    Dim stream = Await m_recordStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read)

                    ShowStatusMessage("Record file opened")
                    ShowStatusMessage(Me.m_recordStorageFile.Path)
                    playbackElement3.AutoPlay = True
                    playbackElement3.SetSource(stream, Me.m_recordStorageFile.FileType)
                    playbackElement3.Play()

                End If

            End If
        Catch exception As Exception
            EnableButton(True, "StartStopRecord")
            ShowExceptionMessage(exception)
            m_bRecording = False
        End Try
    End Sub

    Private Sub ShowStatusMessage(ByVal text As String)
        rootPage.NotifyUser(text, NotifyType.StatusMessage)
    End Sub

    Private Sub ShowExceptionMessage(ByVal ex As Exception)
        rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
    End Sub

    Private Sub SwitchRecordButtonContent()
        If m_bRecording Then
            btnStartStopRecord3.Content = "StopRecord"
        Else
            btnStartStopRecord3.Content = "StartRecord"
        End If
    End Sub

    Private Sub EnableButton(ByVal enabled As Boolean, ByVal name As String)
        If name.Equals("StartDevice") Then
            btnStartDevice3.IsEnabled = enabled

        ElseIf name.Equals("StartStopRecord") Then
            btnStartStopRecord3.IsEnabled = enabled
        End If

    End Sub
End Class
