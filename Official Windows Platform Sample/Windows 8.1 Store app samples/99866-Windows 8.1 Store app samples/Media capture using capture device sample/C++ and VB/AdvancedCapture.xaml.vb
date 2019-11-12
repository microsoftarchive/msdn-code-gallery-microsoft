'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Devices.Enumeration
Imports Windows.Media.MediaProperties
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports Windows.Media
Imports Windows.Media.Capture
Imports Windows.Foundation
Imports SDKTemplate
Imports System
Imports System.Threading.Tasks

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class AdvancedCapture
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private m_mediaCaptureMgr As Windows.Media.Capture.MediaCapture = Nothing
    Private m_lowLagPhoto As Windows.Media.Capture.LowLagPhotoCapture = Nothing
    Private m_lowLagRecord As Windows.Media.Capture.LowLagMediaRecording = Nothing
    Private m_recordStorageFile As Windows.Storage.StorageFile
    Private m_devInfoCollection As DeviceInformationCollection
    Private m_microPhoneInfoCollection As DeviceInformationCollection
    Private m_bLowLagPrepared As Boolean
    Private m_bRecording As Boolean
    Private m_bSuspended As Boolean
    Private m_bPreviewing As Boolean
    Private m_bEffectAddedToRecord As Boolean = False
    Private m_bEffectAddedToPhoto As Boolean = False
    Private m_mediaPropertyChanged As TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)
    Private m_bRotateVideoOnOrientationChange As Boolean
    Private m_bReversePreviewRotation As Boolean
    Private m_displayOrientation As Windows.Graphics.Display.DisplayOrientations

    Private ReadOnly VIDEO_FILE_NAME As String = "video.mp4"
    Private ReadOnly PHOTO_FILE_NAME As String = "photo.jpg"

    Private m_rotHeight As Double
    Private m_rotWidth As Double
    Private Shared rotGUID As New Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1")

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        m_mediaPropertyChanged = New TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)(AddressOf SystemMediaControls_PropertyChanged)
        ScenarioInit()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        AddHandler systemMediaControls.PropertyChanged, m_mediaPropertyChanged
        Dim displayInfo As Windows.Graphics.Display.DisplayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView()
        m_displayOrientation = displayInfo.CurrentOrientation
        AddHandler displayInfo.OrientationChanged, AddressOf DisplayInfo_OrientationChanged
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        RemoveHandler systemMediaControls.PropertyChanged, m_mediaPropertyChanged
        RemoveHandler Windows.Graphics.Display.DisplayInformation.GetForCurrentView().OrientationChanged, AddressOf DisplayInfo_OrientationChanged

        ScenarioClose()

    End Sub

    Private Sub ScenarioInit()
        btnStartDevice2.IsEnabled = False
        btnStartPreview2.IsEnabled = False
        btnStartStopRecord2.IsEnabled = False
        btnStartStopRecord2.Content = "StartRecord"
        btnTakePhoto2.IsEnabled = False
        btnTakePhoto2.Content = "TakePhoto"

        m_bRecording = False
        m_bPreviewing = False
        m_bSuspended = False
        m_bLowLagPrepared = False
        chkAddRemoveEffect.IsChecked = False
        chkAddRemoveEffect.IsEnabled = False
        radTakePhoto.IsEnabled = False
        radRecord.IsEnabled = False
        radTakePhoto.IsChecked = False
        radRecord.IsChecked = False

        previewElement2.Source = Nothing
        playbackElement2.Source = Nothing
        imageElement2.Source = Nothing
        ShowStatusMessage("")

        EnumerateWebcamsAsync()
        EnumerateMicrophonesAsync()

        SceneModeList2.SelectedIndex = -1
        SceneModeList2.Items.Clear()

        m_rotHeight = previewElement2.Width
        m_rotWidth = previewElement2.Height
    End Sub

    Private Async Sub ScenarioClose()
        Try
            If m_bLowLagPrepared Then
                ShowStatusMessage("Stopping LowLagPhoto")
                Await m_lowLagPhoto.FinishAsync()
                m_bLowLagPrepared = False
            End If

            If m_bRecording Then
                ShowStatusMessage("Stopping LowLag Record")
                Await m_lowLagRecord.FinishAsync()
                m_bRecording = False
                If btnStartStopRecord2.Content.ToString() = "StartRecord" Then
                    Await m_recordStorageFile.DeleteAsync()
                End If
            End If
            If m_bPreviewing Then
                ShowStatusMessage("Stopping Preview")

                Await m_mediaCaptureMgr.StopPreviewAsync()
                m_bPreviewing = False
                previewElement2.Source = Nothing
            End If

            If m_mediaCaptureMgr IsNot Nothing Then
                ShowStatusMessage("Stopping Camera")
                previewElement2.Source = Nothing
                m_mediaCaptureMgr.Dispose()
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
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
        If btnStartStopRecord2.Content.ToString() = "StopRecord" Then
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Async Sub()
                                                                                         'if camera does not support lowlag record and lowlag photo at the same time
                                                                                         'enable the checkbox
                                                                                         'Prepare lowlag record for next round;
                                                                                         Try
                                                                                             ShowStatusMessage("Stopping Record on exceeding max record duration")
                                                                                             btnStartStopRecord2.IsEnabled = False
                                                                                             Await m_lowLagRecord.FinishAsync()
                                                                                             m_bRecording = False
                                                                                             ShowStatusMessage("Stopped record on exceeding max record duration:" & m_recordStorageFile.Path)
                                                                                             If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                                                                                                 radTakePhoto.IsEnabled = True
                                                                                                 radRecord.IsEnabled = True
                                                                                             End If
                                                                                             PrepareLowLagRecordAsync()
                                                                                         Catch exception As Exception
                                                                                             ShowExceptionMessage(exception)
                                                                                         End Try
                                                                                     End Sub)
        End If
    End Sub

    Public Async Sub Failed(ByVal currentCaptureObject As Windows.Media.Capture.MediaCapture, ByVal currentFailure As MediaCaptureFailedEventArgs)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() ShowStatusMessage("Fatal error" & currentFailure.Message))
    End Sub

    Private Async Sub PrepareLowLagRecordAsync()
        Try
            PrepareForVideoRecording()

            m_recordStorageFile = Await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(VIDEO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName)
            ShowStatusMessage("Create record file successful")
            Dim recordProfile As MediaEncodingProfile = Nothing
            recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto)
            m_lowLagRecord = Await m_mediaCaptureMgr.PrepareLowLagRecordToStorageFileAsync(recordProfile, m_recordStorageFile)
            m_bRecording = True
            btnStartStopRecord2.Content = "StartRecord"
            btnStartStopRecord2.IsEnabled = True
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub btnStartDevice_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try

            btnStartDevice2.IsEnabled = False
            m_bReversePreviewRotation = False
            ShowStatusMessage("Starting device")


            m_mediaCaptureMgr = New Windows.Media.Capture.MediaCapture()



            Dim settings = New Windows.Media.Capture.MediaCaptureInitializationSettings()
            Dim chosenDevInfo = m_devInfoCollection(EnumedDeviceList2.SelectedIndex)
            settings.VideoDeviceId = chosenDevInfo.Id

            If EnumedMicrophonesList2.SelectedIndex >= 0 AndAlso m_microPhoneInfoCollection.Count > 0 Then
                Dim chosenMicrophoneInfo = m_microPhoneInfoCollection(EnumedMicrophonesList2.SelectedIndex)
                settings.AudioDeviceId = chosenMicrophoneInfo.Id
            End If

            If chosenDevInfo.EnclosureLocation IsNot Nothing AndAlso chosenDevInfo.EnclosureLocation.Panel = Windows.Devices.Enumeration.Panel.Back Then
                m_bRotateVideoOnOrientationChange = True
                m_bReversePreviewRotation = False
            ElseIf chosenDevInfo.EnclosureLocation IsNot Nothing AndAlso chosenDevInfo.EnclosureLocation.Panel = Windows.Devices.Enumeration.Panel.Front Then
                m_bRotateVideoOnOrientationChange = True
                m_bReversePreviewRotation = True
            Else
                m_bRotateVideoOnOrientationChange = False
            End If


            Await m_mediaCaptureMgr.InitializeAsync(settings)

            If m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceId <> "" AndAlso m_mediaCaptureMgr.MediaCaptureSettings.AudioDeviceId <> "" Then
                btnStartPreview2.IsEnabled = True
                ShowStatusMessage("Device initialized successful")
                chkAddRemoveEffect.IsEnabled = True
                chkAddRemoveEffect.IsChecked = False
                AddHandler m_mediaCaptureMgr.RecordLimitationExceeded, AddressOf RecordLimitationExceeded
                AddHandler m_mediaCaptureMgr.Failed, AddressOf Failed

                If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                    radTakePhoto.IsEnabled = True
                    radRecord.IsEnabled = True
                    'choose TakePhoto Mode as defaul
                    radTakePhoto.IsChecked = True
                Else
                    'prepare lowlag photo, then prepare lowlag record
                    m_lowLagPhoto = Await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg())

                    btnTakePhoto2.IsEnabled = True
                    m_bLowLagPrepared = True
                    PrepareLowLagRecordAsync()
                    'disable check options
                    radTakePhoto.IsEnabled = False
                    radRecord.IsEnabled = False
                End If

                EnumerateSceneModeAsync()
            Else
                btnStartDevice2.IsEnabled = True
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
            btnStartPreview2.IsEnabled = False

            previewCanvas2.Visibility = Windows.UI.Xaml.Visibility.Visible
            previewCanvas2.Background.Opacity = 0
            previewElement2.Source = m_mediaCaptureMgr
            Await m_mediaCaptureMgr.StartPreviewAsync()
            m_bPreviewing = True
            OrientationChanged()
            ShowStatusMessage("Start preview successful")
        Catch exception As Exception
            m_bPreviewing = False
            previewElement2.Source = Nothing
            btnStartPreview2.IsEnabled = True
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub btnTakePhoto_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)

        Try
            ShowStatusMessage("Taking photo")
            btnTakePhoto2.IsEnabled = False

            If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                'disable check box while taking photo
                radTakePhoto.IsEnabled = False
                radRecord.IsEnabled = False
            End If

            Dim photo = Await m_lowLagPhoto.CaptureAsync()

            Dim currentRotation = GetCurrentPhotoRotation()
            Dim photoStorageFile = Await ReencodePhotoAsync(photo.Frame.CloneStream(), currentRotation)

            btnTakePhoto2.IsEnabled = True
            ShowStatusMessage("Photo taken")

            Dim photoStream = Await photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read)
            ShowStatusMessage("File open successful")
            Dim bmpimg = New BitmapImage()

            bmpimg.SetSource(photoStream)
            imageElement2.Source = bmpimg
            ShowStatusMessage(photoStorageFile.Path)

            If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                'reset check options 
                radTakePhoto.IsEnabled = True
                radTakePhoto.IsChecked = True
                radRecord.IsEnabled = True
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
            btnTakePhoto2.IsEnabled = True
            If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                'reset check options 
                radTakePhoto.IsEnabled = True
                radRecord.IsEnabled = True
            End If
        End Try

    End Sub

    Friend Async Sub btnStartStopRecord_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)

        Try
            If btnStartStopRecord2.Content.ToString() = "StartRecord" Then
                If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                    'disable check box while recording
                    radTakePhoto.IsEnabled = False
                    radRecord.IsEnabled = False
                End If
                btnStartStopRecord2.IsEnabled = False
                ShowStatusMessage("Starting Record")
                Await m_lowLagRecord.StartAsync()

                btnStartStopRecord2.Content = "StopRecord"
                btnStartStopRecord2.IsEnabled = True
                playbackElement2.Source = Nothing

            Else
                ShowStatusMessage("Stopping Record")
                btnStartStopRecord2.IsEnabled = False

                Await m_lowLagRecord.FinishAsync()
                ShowStatusMessage("Stop record successful")
                If Not m_bSuspended Then
                    Dim stream = Await m_recordStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read)
                    ShowStatusMessage("Record file opened")
                    ShowStatusMessage(Me.m_recordStorageFile.Path)
                    playbackElement2.AutoPlay = True
                    playbackElement2.SetSource(stream, Me.m_recordStorageFile.FileType)
                    playbackElement2.Play()
                End If

                If Not m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported Then
                    'reset check options 
                    radTakePhoto.IsEnabled = True
                    radRecord.IsEnabled = True
                    radRecord.IsChecked = True
                End If
                'prepare lowlag record for next round
                m_bRecording = False
                PrepareLowLagRecordAsync()
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Sub lstEnumedDevices_SelectionChanged(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Controls.SelectionChangedEventArgs)
        ScenarioClose()

        btnStartDevice2.IsEnabled = True
        btnStartPreview2.IsEnabled = False
        btnStartStopRecord2.IsEnabled = False
        btnStartStopRecord2.Content = "StartRecord"
        btnTakePhoto2.IsEnabled = False

        m_bRecording = False
        m_bPreviewing = False
        m_bSuspended = False
        m_bLowLagPrepared = False

        chkAddRemoveEffect.IsEnabled = False

        radTakePhoto.IsEnabled = False
        radRecord.IsEnabled = False
        radTakePhoto.IsChecked = False
        radRecord.IsChecked = False

        previewCanvas2.Background.Opacity = 100
        previewElement2.Source = Nothing
        playbackElement2.Source = Nothing
        imageElement2.Source = Nothing


        m_bEffectAddedToRecord = False
        m_bEffectAddedToPhoto = False
        SceneModeList2.SelectedIndex = -1
        SceneModeList2.Items.Clear()
        ShowStatusMessage("Device changed, Initialize")
    End Sub

    Private Async Sub EnumerateWebcamsAsync()
        Try
            ShowStatusMessage("Enumerating Webcams...")
            m_devInfoCollection = Nothing

            EnumedDeviceList2.Items.Clear()

            m_devInfoCollection = Await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)
            If m_devInfoCollection.Count = 0 Then
                ShowStatusMessage("No WebCams found.")
            Else
                For i As Integer = 0 To m_devInfoCollection.Count - 1
                    Dim devInfo = m_devInfoCollection(i)
                    Dim location = devInfo.EnclosureLocation

                    If location IsNot Nothing Then

                        If location.Panel = Windows.Devices.Enumeration.Panel.Front Then
                            EnumedDeviceList2.Items.Add(devInfo.Name & "-Front")
                        ElseIf location.Panel = Windows.Devices.Enumeration.Panel.Back Then
                            EnumedDeviceList2.Items.Add(devInfo.Name & "-Back")
                        Else
                            EnumedDeviceList2.Items.Add(devInfo.Name)
                        End If
                    Else
                        EnumedDeviceList2.Items.Add(devInfo.Name)
                    End If
                Next i
                EnumedDeviceList2.SelectedIndex = 0
                ShowStatusMessage("Enumerating Webcams completed successfully.")
                btnStartDevice2.IsEnabled = True
            End If
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Private Async Sub EnumerateMicrophonesAsync()
        Try
            ShowStatusMessage("Enumerating Microphones...")
            m_microPhoneInfoCollection = Nothing

            EnumedMicrophonesList2.Items.Clear()

            m_microPhoneInfoCollection = Await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture)

            Try
                If m_microPhoneInfoCollection Is Nothing OrElse m_microPhoneInfoCollection.Count = 0 Then
                    ShowStatusMessage("No Microphones found.")
                Else
                    For i As Integer = 0 To m_microPhoneInfoCollection.Count - 1
                        Dim devInfo = m_microPhoneInfoCollection(i)
                        Dim location = devInfo.EnclosureLocation
                        If location IsNot Nothing Then
                            If location.Panel = Windows.Devices.Enumeration.Panel.Front Then
                                EnumedMicrophonesList2.Items.Add(devInfo.Name & "-Front")
                            ElseIf location.Panel = Windows.Devices.Enumeration.Panel.Back Then
                                EnumedMicrophonesList2.Items.Add(devInfo.Name & "-Back")

                            Else
                                EnumedMicrophonesList2.Items.Add(devInfo.Name)
                            End If

                        Else
                            EnumedMicrophonesList2.Items.Add(devInfo.Name)
                        End If
                    Next i
                    EnumedMicrophonesList2.SelectedIndex = 0
                    ShowStatusMessage("Enumerating Microphones completed successfully.")
                End If
            Catch e As Exception
                ShowExceptionMessage(e)
            End Try

        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub

    Private Sub EnumerateSceneModeAsync()
        Try
            ShowStatusMessage("Enumerating SceneMode...")

            SceneModeList2.Items.Clear()

            Dim sceneModes = m_mediaCaptureMgr.VideoDeviceController.SceneModeControl.SupportedModes

            For Each mode In sceneModes
                Dim modeName As String = Nothing

                Select Case mode
                    Case Windows.Media.Devices.CaptureSceneMode.Auto
                        modeName = "Auto"
                    Case Windows.Media.Devices.CaptureSceneMode.Macro
                        modeName = "Macro"
                    Case Windows.Media.Devices.CaptureSceneMode.Portrait
                        modeName = "Portrait"
                    Case Windows.Media.Devices.CaptureSceneMode.Sport
                        modeName = "Sport"
                    Case Windows.Media.Devices.CaptureSceneMode.Snow
                        modeName = "Snow"
                    Case Windows.Media.Devices.CaptureSceneMode.Night
                        modeName = "Night"
                    Case Windows.Media.Devices.CaptureSceneMode.Beach
                        modeName = "Beach"
                    Case Windows.Media.Devices.CaptureSceneMode.Sunset
                        modeName = "Sunset"
                    Case Windows.Media.Devices.CaptureSceneMode.Candlelight
                        modeName = "Candlelight"
                    Case Windows.Media.Devices.CaptureSceneMode.Landscape
                        modeName = "Landscape"
                    Case Windows.Media.Devices.CaptureSceneMode.NightPortrait
                        modeName = "Night portrait"
                    Case Windows.Media.Devices.CaptureSceneMode.Backlit
                        modeName = "Backlit"
                End Select
                If modeName IsNot Nothing Then
                    SceneModeList2.Items.Add(modeName)
                End If
            Next mode

            If sceneModes.Count > 0 Then
                SceneModeList2.SelectedIndex = 0
            End If

        Catch e As Exception
            ShowExceptionMessage(e)
        End Try

    End Sub

    Friend Async Sub SceneMode_SelectionChanged(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Controls.SelectionChangedEventArgs)
        Try
            If SceneModeList2.SelectedIndex > -1 Then
                Dim modeName = SceneModeList2.Items.IndexOf(SceneModeList2.SelectedIndex).ToString()

                Dim mode = Windows.Media.Devices.CaptureSceneMode.Auto

                If modeName = "Macro" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Macro
                ElseIf modeName = "Portrait" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Portrait
                ElseIf modeName = "Sport" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Sport
                ElseIf modeName = "Snow" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Snow
                ElseIf modeName = "Night" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Night
                ElseIf modeName = "Beach" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Beach
                ElseIf modeName = "Sunset" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Sunset
                ElseIf modeName = "Candlelight" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Candlelight
                ElseIf modeName = "Landscape" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Landscape
                ElseIf modeName = "Night portrait" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.NightPortrait
                ElseIf modeName = "Backlight" Then
                    mode = Windows.Media.Devices.CaptureSceneMode.Backlit
                End If

                Await m_mediaCaptureMgr.VideoDeviceController.SceneModeControl.SetValueAsync(mode)
                Dim message = "SceneMode is set to " & modeName
                ShowStatusMessage(message)
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try

    End Sub

    Friend Async Sub addEffectToImageStream()
        Try
            If (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic <> Windows.Media.Capture.VideoDeviceCharacteristic.AllStreamsIdentical) AndAlso (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic <> Windows.Media.Capture.VideoDeviceCharacteristic.PreviewPhotoStreamsIdentical) AndAlso (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic <> Windows.Media.Capture.VideoDeviceCharacteristic.RecordPhotoStreamsIdentical) Then
                Dim props2 As IMediaEncodingProperties = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Photo)
                If props2.Type.Equals("Image") Then 'Cant add an effect to an image type
                    'Change the media type on the stream
                    Dim supportedPropsList As System.Collections.Generic.IReadOnlyList(Of IMediaEncodingProperties) = m_mediaCaptureMgr.VideoDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Photo)
                    Dim i As Integer = 0
                    Dim tempVar As Boolean = i < supportedPropsList.Count
                    i += 1
                    Do While tempVar
                        If supportedPropsList(i).Type.Equals("Video") Then
                            Dim bLowLagPrepare_tmp = m_bLowLagPrepared

                            'it is necessary to un-prepare the lowlag photo before adding effect, since adding effect needs to change mediaType if it was not "Video" type;   
                            If m_bLowLagPrepared Then
                                ShowStatusMessage("Stopping LowLagPhoto")
                                Await m_lowLagPhoto.FinishAsync()
                                btnTakePhoto2.IsEnabled = False
                                m_bLowLagPrepared = False
                            End If
                            Await m_mediaCaptureMgr.VideoDeviceController.SetMediaStreamPropertiesAsync(Windows.Media.Capture.MediaStreamType.Photo, supportedPropsList(i))
                            ShowStatusMessage("Change type on image pin successful")
                            Await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.Photo, "GrayscaleTransform.GrayscaleEffect", Nothing)
                            ShowStatusMessage("Add effect to photo successful")
                            m_bEffectAddedToPhoto = True
                            chkAddRemoveEffect.IsEnabled = True
                            'Prepare LowLag Photo again if LowLag Photo was prepared before adding effect;
                            If bLowLagPrepare_tmp Then
                                m_lowLagPhoto = Await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg())
                                btnTakePhoto2.IsEnabled = True
                                m_bLowLagPrepared = True
                            End If
                            Exit Do
                        End If
                        tempVar = i < supportedPropsList.Count
                        i += 1
                    Loop
                    chkAddRemoveEffect.IsEnabled = True
                Else
                    Await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.Photo, "GrayscaleTransform.GrayscaleEffect", Nothing)
                    ShowStatusMessage("Add effect to photo successful")
                    chkAddRemoveEffect.IsEnabled = True
                    m_bEffectAddedToPhoto = True

                End If
            Else
                chkAddRemoveEffect.IsEnabled = True
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub chkAddRemoveEffect_Checked(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            chkAddRemoveEffect.IsEnabled = False
            Await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.VideoPreview, "GrayscaleTransform.GrayscaleEffect", Nothing)
            ShowStatusMessage("Add effect to video preview successful")
            If (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic <> Windows.Media.Capture.VideoDeviceCharacteristic.AllStreamsIdentical) AndAlso (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic <> Windows.Media.Capture.VideoDeviceCharacteristic.PreviewRecordStreamsIdentical) Then
                Dim props As IMediaEncodingProperties = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoRecord)
                If Not props.Type.Equals("H264") Then 'Cant add an effect to an H264 stream
                    Await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.VideoRecord, "GrayscaleTransform.GrayscaleEffect", Nothing)
                    ShowStatusMessage("Add effect to video record successful")
                    m_bEffectAddedToRecord = True
                    addEffectToImageStream()
                Else
                    addEffectToImageStream()
                    chkAddRemoveEffect.IsEnabled = True
                End If
            Else
                addEffectToImageStream()
                chkAddRemoveEffect.IsEnabled = True
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub chkAddRemoveEffect_Unchecked(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            chkAddRemoveEffect.IsEnabled = False
            Await m_mediaCaptureMgr.ClearEffectsAsync(Windows.Media.Capture.MediaStreamType.VideoPreview)
            ShowStatusMessage("Remove effect from preview successful")
            If m_bEffectAddedToRecord Then
                Await m_mediaCaptureMgr.ClearEffectsAsync(Windows.Media.Capture.MediaStreamType.VideoRecord)
                ShowStatusMessage("Remove effect from preview successful")
                m_bEffectAddedToRecord = False
            End If
            If m_bEffectAddedToPhoto Then
                Await m_mediaCaptureMgr.ClearEffectsAsync(Windows.Media.Capture.MediaStreamType.Photo)
                ShowStatusMessage("Remove effect from preview successful")
                m_bEffectAddedToRecord = False
            End If
            chkAddRemoveEffect.IsEnabled = True
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub radTakePhoto_Checked(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            If Not m_bLowLagPrepared Then
                'if camera does not support lowlag record and lowlag photo at the same time
                'disable all buttons while preparing lowlag photo
                btnStartStopRecord2.IsEnabled = False
                btnTakePhoto2.IsEnabled = False
                'uncheck record Mode
                radRecord.IsChecked = False
                'disable check option while preparing lowlag photo
                radTakePhoto.IsEnabled = False
                radRecord.IsEnabled = False

                If m_bRecording Then
                    'if camera does not support lowlag record and lowlag photo at the same time
                    'but lowlag record is already prepared, un-prepare lowlag record first, before preparing lowlag photo 
                    m_bRecording = False
                    Await m_lowLagRecord.FinishAsync()

                    ShowStatusMessage("Stopped record on preparing lowlag Photo:" & m_recordStorageFile.Path)

                    m_lowLagPhoto = Await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg())
                    btnTakePhoto2.IsEnabled = True
                    m_bLowLagPrepared = True
                    're-enable check option, after lowlag record finish preparing
                    radTakePhoto.IsEnabled = True
                    radRecord.IsEnabled = True
                Else '(!m_bRecording)
                    'if camera does not support lowlag record and lowlag photo at the same time
                    'lowlag record is not prepared, go ahead to prepare lowlag photo 
                    m_lowLagPhoto = Await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg())
                    btnTakePhoto2.IsEnabled = True
                    m_bLowLagPrepared = True
                    're-enable check option, after lowlag record finish preparing
                    radTakePhoto.IsEnabled = True
                    radRecord.IsEnabled = True
                End If
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Friend Async Sub radRecord_Checked(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            If Not m_bRecording Then
                'if camera does not support lowlag record and lowlag photo at the same time
                'disable all buttons while preparing lowlag record
                btnTakePhoto2.IsEnabled = False
                btnStartStopRecord2.IsEnabled = False
                'uncheck TakePhoto Mode
                radTakePhoto.IsChecked = False
                'disable check option while preparing lowlag record
                radTakePhoto.IsEnabled = False
                radRecord.IsEnabled = False

                If m_bLowLagPrepared Then
                    'if camera does not support lowlag record and lowlag photo at the same time
                    'but lowlag photo is already prepared, un-prepare lowlag photo first, before preparing lowlag record 
                    Await m_lowLagPhoto.FinishAsync()

                    m_bLowLagPrepared = False
                    'prepare lowlag record
                    PrepareForVideoRecording()

                    m_recordStorageFile = Await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(VIDEO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName)
                    ShowStatusMessage("Create record file successful")
                    Dim recordProfile As MediaEncodingProfile = Nothing
                    recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto)
                    m_lowLagRecord = Await m_mediaCaptureMgr.PrepareLowLagRecordToStorageFileAsync(recordProfile, m_recordStorageFile)
                    btnStartStopRecord2.IsEnabled = True
                    m_bRecording = True
                    btnStartStopRecord2.Content = "StartRecord"

                    're-enable check option, after lowlag record finish preparing
                    radTakePhoto.IsEnabled = True
                    radRecord.IsEnabled = True

                Else '(!m_bLowLagPrepared)
                    'if camera does not support lowlag record and lowlag photo at the same time
                    'lowlag photo is not prepared, go ahead to prepare lowlag record 
                    PrepareForVideoRecording()

                    m_recordStorageFile = Await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(VIDEO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName)
                    ShowStatusMessage("Create record file successful")
                    Dim recordProfile As MediaEncodingProfile = Nothing
                    recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto)
                    m_lowLagRecord = Await m_mediaCaptureMgr.PrepareLowLagRecordToStorageFileAsync(recordProfile, m_recordStorageFile)
                    btnStartStopRecord2.IsEnabled = True
                    m_bRecording = True
                    btnStartStopRecord2.Content = "StartRecord"

                    're-enable check option, after lowlag record finish preparing
                    radTakePhoto.IsEnabled = True
                    radRecord.IsEnabled = True
                End If
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

    Private Async Function ReencodePhotoAsync(ByVal stream As Windows.Storage.Streams.IRandomAccessStream, ByVal photoRotation As Windows.Storage.FileProperties.PhotoOrientation) As Task(Of Windows.Storage.StorageFile)
        Dim inputStream As Windows.Storage.Streams.IRandomAccessStream = Nothing
        Dim outputStream As Windows.Storage.Streams.IRandomAccessStream = Nothing
        Dim photoStorage As Windows.Storage.StorageFile = Nothing

        Try
            inputStream = stream

            Dim decoder = Await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(inputStream)

            photoStorage = Await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName)

            outputStream = Await photoStorage.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)

            outputStream.Size = 0

            Dim encoder = Await Windows.Graphics.Imaging.BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder)

            Dim properties = New Windows.Graphics.Imaging.BitmapPropertySet()
            properties.Add("System.Photo.Orientation", New Windows.Graphics.Imaging.BitmapTypedValue(photoRotation, Windows.Foundation.PropertyType.UInt16))

            Await encoder.BitmapProperties.SetPropertiesAsync(properties)

            Await encoder.FlushAsync()
        Finally
            If inputStream IsNot Nothing Then
                inputStream.Dispose()
            End If

            If outputStream IsNot Nothing Then
                outputStream.Dispose()
            End If
        End Try

        Return photoStorage
    End Function

    Private Function GetCurrentPhotoRotation() As Windows.Storage.FileProperties.PhotoOrientation
        Dim counterclockwiseRotation As Boolean = m_bReversePreviewRotation

        If m_bRotateVideoOnOrientationChange Then
            Return PhotoRotationLookup(m_displayOrientation, counterclockwiseRotation)
        Else
            Return Windows.Storage.FileProperties.PhotoOrientation.Normal
        End If
    End Function

    Private Sub PrepareForVideoRecording()
        Try
            If m_mediaCaptureMgr Is Nothing Then
                Return
            End If

            Dim counterclockwiseRotation As Boolean = m_bReversePreviewRotation

            If m_bRotateVideoOnOrientationChange Then
                m_mediaCaptureMgr.SetRecordRotation(VideoRotationLookup(m_displayOrientation, counterclockwiseRotation))
            Else
                m_mediaCaptureMgr.SetRecordRotation(Windows.Media.Capture.VideoRotation.None)
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Private Async Sub OrientationChanged()
        Try
            If m_mediaCaptureMgr Is Nothing Then
                Return
            End If

            Dim videoEncodingProperties = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview)

            Dim previewMirroring As Boolean = m_mediaCaptureMgr.GetPreviewMirroring()
            Dim counterclockwiseRotation As Boolean = (previewMirroring AndAlso (Not m_bReversePreviewRotation)) OrElse ((Not previewMirroring) AndAlso m_bReversePreviewRotation)

            If m_bRotateVideoOnOrientationChange AndAlso m_bPreviewing Then
                Dim rotDegree = VideoPreviewRotationLookup(m_displayOrientation, counterclockwiseRotation)
                videoEncodingProperties.Properties.Add(rotGUID, rotDegree)
                Await m_mediaCaptureMgr.SetEncodingPropertiesAsync(Windows.Media.Capture.MediaStreamType.VideoPreview, videoEncodingProperties, Nothing)
                If rotDegree = 90 OrElse rotDegree = 270 Then
                    previewElement2.Height = m_rotHeight
                    previewElement2.Width = m_rotWidth
                Else
                    previewElement2.Height = m_rotWidth
                    previewElement2.Width = m_rotHeight
                End If
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

    Private Sub DisplayInfo_OrientationChanged(ByVal sender As Windows.Graphics.Display.DisplayInformation, ByVal args As Object)
        m_displayOrientation = sender.CurrentOrientation
        OrientationChanged()
    End Sub

    Private Function PhotoRotationLookup(ByVal displayOrientation As Windows.Graphics.Display.DisplayOrientations, ByVal counterclockwise As Boolean) As Windows.Storage.FileProperties.PhotoOrientation
        Select Case displayOrientation
            Case Windows.Graphics.Display.DisplayOrientations.Landscape
                Return Windows.Storage.FileProperties.PhotoOrientation.Normal

            Case Windows.Graphics.Display.DisplayOrientations.Portrait
                Return If(counterclockwise, Windows.Storage.FileProperties.PhotoOrientation.Rotate90, Windows.Storage.FileProperties.PhotoOrientation.Rotate270)

            Case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped
                Return Windows.Storage.FileProperties.PhotoOrientation.Rotate180

            Case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped
                Return If(counterclockwise, Windows.Storage.FileProperties.PhotoOrientation.Rotate270, Windows.Storage.FileProperties.PhotoOrientation.Rotate90)

            Case Else
                Return Windows.Storage.FileProperties.PhotoOrientation.Unspecified
        End Select
    End Function

    Private Function VideoRotationLookup(ByVal displayOrientation As Windows.Graphics.Display.DisplayOrientations, ByVal counterclockwise As Boolean) As Windows.Media.Capture.VideoRotation
        Select Case displayOrientation
            Case Windows.Graphics.Display.DisplayOrientations.Landscape
                Return Windows.Media.Capture.VideoRotation.None

            Case Windows.Graphics.Display.DisplayOrientations.Portrait
                Return If(counterclockwise, Windows.Media.Capture.VideoRotation.Clockwise270Degrees, Windows.Media.Capture.VideoRotation.Clockwise90Degrees)

            Case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped
                Return Windows.Media.Capture.VideoRotation.Clockwise180Degrees

            Case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped
                Return If(counterclockwise, Windows.Media.Capture.VideoRotation.Clockwise90Degrees, Windows.Media.Capture.VideoRotation.Clockwise270Degrees)

            Case Else
                Return Windows.Media.Capture.VideoRotation.None
        End Select
    End Function

    Private Function VideoPreviewRotationLookup(ByVal displayOrientation As Windows.Graphics.Display.DisplayOrientations, ByVal counterclockwise As Boolean) As UInteger
        Select Case displayOrientation
            Case Windows.Graphics.Display.DisplayOrientations.Landscape
                Return 0

            Case Windows.Graphics.Display.DisplayOrientations.Portrait
                If counterclockwise Then
                    Return 270
                Else
                    Return 90
                End If

            Case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped
                Return 180

            Case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped
                If counterclockwise Then
                    Return 90
                Else
                    Return 270
                End If

            Case Else
                Return 0
        End Select
    End Function
End Class
