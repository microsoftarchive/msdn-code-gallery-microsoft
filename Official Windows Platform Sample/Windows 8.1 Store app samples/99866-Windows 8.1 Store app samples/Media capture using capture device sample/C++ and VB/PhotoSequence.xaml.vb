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
Partial Public NotInheritable Class PhotoSequence
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private m_capture As Windows.Media.Capture.MediaCapture
    Private m_photoSequenceCapture As Windows.Media.Capture.LowLagPhotoSequenceCapture
    Private m_framePtr() As Windows.Media.Capture.CapturedFrame

    Private m_photoStorageFile As Windows.Storage.StorageFile
    Private m_mediaPropertyChanged As TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)

    Private m_bPreviewing As Boolean
    Private m_bPhotoSequence As Boolean
    Private m_highLighted As Boolean

    Private m_selectedIndex As Integer
    Private m_frameNum As UInteger
    Private m_ThumbnailNum As UInteger
    Private m_pastFrame As UInteger
    Private m_futureFrame As UInteger

    Private ReadOnly PHOTOSEQ_FILE_NAME As String = "photoSequence.jpg"

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        ScenarioInit()
        m_mediaPropertyChanged = CType([Delegate].Combine(m_mediaPropertyChanged, New TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs)(AddressOf SystemMediaControls_PropertyChanged)), TypedEventHandler(Of SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs))
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

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        AddHandler systemMediaControls.PropertyChanged, m_mediaPropertyChanged
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        RemoveHandler systemMediaControls.PropertyChanged, m_mediaPropertyChanged

        ScenarioClose()
    End Sub

    Private Sub ShowStatusMessage(ByVal text As String)
        rootPage.NotifyUser(text, NotifyType.StatusMessage)
    End Sub

    Private Sub ShowExceptionMessage(ByVal ex As Exception)
        rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
    End Sub

    Private Sub ScenarioInit()
        Try
            btnStartDevice4.IsEnabled = True
            btnStartPreview4.IsEnabled = False
            btnStartStopPhotoSequence.IsEnabled = False
            btnStartStopPhotoSequence.Content = "Prepare PhotoSequence"
            btnSaveToFile.IsEnabled = False

            m_bPreviewing = False
            m_bPhotoSequence = False

            previewElement4.Source = Nothing

            m_photoSequenceCapture = Nothing
            Clear()

            'user can set the maximum history frame number
            m_pastFrame = 5
            '//user can set the maximum future frame number
            m_futureFrame = 5

            m_framePtr = New Windows.Media.Capture.CapturedFrame(CInt(m_pastFrame + m_futureFrame - 1)) {}

            m_frameNum = 0
            m_ThumbnailNum = 0
            m_selectedIndex = -1
            m_highLighted = False
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try

    End Sub

    Private Async Sub ScenarioClose()
        Try
            If m_bPhotoSequence Then
                ShowStatusMessage("Stopping PhotoSequence")
                Await m_photoSequenceCapture.FinishAsync()
                m_photoSequenceCapture = Nothing
                m_bPhotoSequence = False
                m_framePtr = Nothing
            End If
            If m_bPreviewing Then
                Await m_capture.StopPreviewAsync()
                m_bPreviewing = False
            End If

            If m_capture IsNot Nothing Then
                previewElement4.Source = Nothing
                m_capture.Dispose()
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try

    End Sub

    Private Async Sub Failed(ByVal currentCaptureObject As Windows.Media.Capture.MediaCapture, ByVal currentFailure As MediaCaptureFailedEventArgs)
        Try
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() ShowStatusMessage("Fatal error" & currentFailure.Message))
        Catch e As Exception
            ShowExceptionMessage(e)
        End Try
    End Sub



    Private Async Sub btnStartDevice_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            btnStartDevice4.IsEnabled = False
            ShowStatusMessage("Starting device")
            m_capture = New Windows.Media.Capture.MediaCapture()

            Await m_capture.InitializeAsync()

            If m_capture.MediaCaptureSettings.VideoDeviceId <> "" Then

                btnStartPreview4.IsEnabled = True

                ShowStatusMessage("Device initialized successful")

                AddHandler m_capture.Failed, AddressOf Failed
            Else
                btnStartDevice4.IsEnabled = True
                ShowStatusMessage("No Video Device Found")
            End If
        Catch ex As Exception
            btnStartPreview4.IsEnabled = False
            btnStartDevice4.IsEnabled = True
            ShowExceptionMessage(ex)
        End Try
    End Sub



    Private Async Sub btnStartPreview_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)

        Try
            ShowStatusMessage("Starting preview")
            btnStartPreview4.IsEnabled = False
            btnStartStopPhotoSequence.IsEnabled = True
            m_bPreviewing = True

            previewCanvas4.Visibility = Windows.UI.Xaml.Visibility.Visible
            previewElement4.Source = m_capture
            Await m_capture.StartPreviewAsync()

            ShowStatusMessage("Start preview successful")
        Catch ex As Exception
            previewElement4.Source = Nothing
            btnStartPreview4.IsEnabled = True
            btnStartStopPhotoSequence.IsEnabled = False
            m_bPreviewing = False
            ShowExceptionMessage(ex)
        End Try
    End Sub



    Private Sub Clear()
        PhotoGrid.Source = Nothing
        ThumbnailGrid.Items.Clear()
    End Sub


    Private Async Sub btnStartStopPhotoSequence_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            If btnStartStopPhotoSequence.Content.ToString() = "Prepare PhotoSequence" Then

                If Not m_capture.VideoDeviceController.LowLagPhotoSequence.Supported Then
                    rootPage.NotifyUser("Photo-sequence is not supported", NotifyType.ErrorMessage)
                Else
                    If m_capture.VideoDeviceController.LowLagPhotoSequence.MaxPastPhotos < m_pastFrame Then
                        m_pastFrame = m_capture.VideoDeviceController.LowLagPhotoSequence.MaxPastPhotos
                        rootPage.NotifyUser("pastFrame number is past limit, reset passFrame number", NotifyType.ErrorMessage)
                    End If

                    btnStartStopPhotoSequence.IsEnabled = False

                    m_capture.VideoDeviceController.LowLagPhotoSequence.ThumbnailEnabled = True
                    m_capture.VideoDeviceController.LowLagPhotoSequence.DesiredThumbnailSize = 300

                    m_capture.VideoDeviceController.LowLagPhotoSequence.PhotosPerSecondLimit = 4
                    m_capture.VideoDeviceController.LowLagPhotoSequence.PastPhotoLimit = m_pastFrame

                    Dim photoCapture As LowLagPhotoSequenceCapture = Await m_capture.PrepareLowLagPhotoSequenceCaptureAsync(ImageEncodingProperties.CreateJpeg())

                    AddHandler photoCapture.PhotoCaptured, AddressOf photoCapturedEventHandler

                    m_photoSequenceCapture = photoCapture

                    btnStartStopPhotoSequence.Content = "Take PhotoSequence"
                    btnStartStopPhotoSequence.IsEnabled = True
                    m_bPhotoSequence = True
                End If
            ElseIf btnStartStopPhotoSequence.Content.ToString() = "Take PhotoSequence" Then
                btnSaveToFile.IsEnabled = False
                m_frameNum = 0
                m_ThumbnailNum = 0
                m_selectedIndex = -1
                m_highLighted = False
                Clear()

                btnStartStopPhotoSequence.IsEnabled = False
                Await m_photoSequenceCapture.StartAsync()
            Else
                rootPage.NotifyUser("Bad photo-sequence state", NotifyType.ErrorMessage)
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub






    Private Async Sub photoCapturedEventHandler(ByVal senders As LowLagPhotoSequenceCapture, ByVal args As PhotoCapturedEventArgs)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Async Sub()
                                                                                     'first picture with timeSpam > 0 get highlighted 
                                                                                     Try
                                                                                         If m_frameNum = (m_pastFrame + m_futureFrame) Then
                                                                                             btnStartStopPhotoSequence.IsEnabled = False
                                                                                             Await m_photoSequenceCapture.StopAsync()
                                                                                             btnStartStopPhotoSequence.IsEnabled = True
                                                                                             btnSaveToFile.IsEnabled = True
                                                                                             ThumbnailGrid.SelectedIndex = m_selectedIndex
                                                                                         ElseIf m_frameNum < (m_pastFrame + m_futureFrame) Then
                                                                                             Dim bitmap = New BitmapImage()
                                                                                             m_framePtr(CInt(m_frameNum)) = args.Frame
                                                                                             bitmap.SetSource(args.Thumbnail)
                                                                                             Dim image = New Image()
                                                                                             image.Source = bitmap
                                                                                             image.Width = 160
                                                                                             image.Height = 120
                                                                                             Dim ThumbnailItem = New Windows.UI.Xaml.Controls.GridViewItem()
                                                                                             ThumbnailItem.Content = image
                                                                                             ThumbnailGrid.Items.Add(ThumbnailItem)
                                                                                             If ((Not m_highLighted)) AndAlso (args.CaptureTimeOffset.Ticks > 0) Then
                                                                                                 m_highLighted = True
                                                                                                 ThumbnailItem.BorderThickness = New Thickness(1)
                                                                                                 ThumbnailItem.BorderBrush = New Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red)
                                                                                                 m_selectedIndex = CInt(m_ThumbnailNum)
                                                                                             End If
                                                                                             m_ThumbnailNum += CUInt(1)
                                                                                         End If
                                                                                         m_frameNum += CUInt(1)
                                                                                     Catch ex As Exception
                                                                                         ShowExceptionMessage(ex)
                                                                                     End Try
                                                                                 End Sub)

    End Sub



    Private Async Sub ItemSelected_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        m_selectedIndex = ThumbnailGrid.SelectedIndex
        If m_selectedIndex > -1 Then
            Dim bitmap = New BitmapImage()
            Try
                Await bitmap.SetSourceAsync(m_framePtr(m_selectedIndex).CloneStream())
                PhotoGrid.Source = bitmap
            Catch ex As Exception
                ShowExceptionMessage(ex)
                rootPage.NotifyUser("Display selected photo fail", NotifyType.ErrorMessage)
            End Try
        End If
    End Sub




    Private Async Sub btnSaveToFile_Click(ByVal sender As Object, ByVal e As Windows.UI.Xaml.RoutedEventArgs)
        Try
            m_selectedIndex = ThumbnailGrid.SelectedIndex

            If m_selectedIndex > -1 Then
                m_photoStorageFile = Await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTOSEQ_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName)

                If Nothing Is m_photoStorageFile Then
                    rootPage.NotifyUser("PhotoFile creation fails", NotifyType.ErrorMessage)
                End If

                Dim OutStream = Await m_photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)

                If Nothing Is OutStream Then
                    rootPage.NotifyUser("PhotoStream creation fails", NotifyType.ErrorMessage)
                End If

                Dim ContentStream = m_framePtr(m_selectedIndex).CloneStream()

                Await Windows.Storage.Streams.RandomAccessStream.CopyAndCloseAsync(ContentStream.GetInputStreamAt(0), OutStream.GetOutputStreamAt(0))

                ShowStatusMessage("Photo save complete")
            Else
                rootPage.NotifyUser("Please select a photo to display", NotifyType.ErrorMessage)
            End If
        Catch exception As Exception
            ShowExceptionMessage(exception)
        End Try
    End Sub

End Class
