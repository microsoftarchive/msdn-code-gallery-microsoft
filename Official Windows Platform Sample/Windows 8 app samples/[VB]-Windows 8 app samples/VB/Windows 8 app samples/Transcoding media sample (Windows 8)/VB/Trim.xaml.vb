'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media
Imports SDKTemplate
Imports System
Imports Windows.Foundation
Imports Windows.Media
Imports Windows.Media.MediaProperties
Imports Windows.Media.Transcoding
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports System.Threading
Imports System.Threading.Tasks

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Trim
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private _dispatcher As Windows.UI.Core.CoreDispatcher = Window.Current.Dispatcher
    Private _cts As CancellationTokenSource
    Private _OutputFileName As String = "TranscodeSampleOutput.mp4"
    Private Profile As Windows.Media.MediaProperties.MediaEncodingProfile
    Private _InputFile As Windows.Storage.StorageFile = Nothing
    Private _OutputFile As Windows.Storage.StorageFile = Nothing
    Private _Transcoder As New Windows.Media.Transcoding.MediaTranscoder()
    Private _UseMp4 As Boolean = True

    Private _Start As New TimeSpan(0)
    Private _Stop As New TimeSpan(0)
    Private _c_sec0 As New TimeSpan(0, 0, 0)
    ' 0s
    Private _c_sec1 As New TimeSpan(0, 0, 1)
    ' 1s
    Public Sub New()
        Me.InitializeComponent()
        _cts = New CancellationTokenSource()

        ' Hook up UI
        AddHandler PickFileButton.Click, AddressOf PickFile
        AddHandler TargetFormat.SelectionChanged, AddressOf OnTargetFormatChanged
        AddHandler Transcode.Click, AddressOf TranscodeTrim
        AddHandler Cancel.Click, AddressOf TranscodeCancel
        AddHandler MarkInButton.Click, AddressOf MarkIn
        AddHandler MarkOutButton.Click, AddressOf MarkOut



        ' Disable manual manipulation of trim points
        StartTimeText.Text = (CSng(_Start.TotalMilliseconds) / 1000).ToString
        EndTimeText.Text = (CSng(_Stop.TotalMilliseconds) / 1000).ToString
        StartTimeText.IsEnabled = False
        EndTimeText.IsEnabled = False

        ' Media Controls
        AddHandler InputPlayButton.Click, AddressOf InputPlayButton_Click
        AddHandler InputPauseButton.Click, AddressOf InputPauseButton_Click
        AddHandler InputStopButton.Click, AddressOf InputStopButton_Click
        AddHandler OutputPlayButton.Click, AddressOf OutputPlayButton_Click
        AddHandler OutputPauseButton.Click, AddressOf OutputPauseButton_Click
        AddHandler OutputStopButton.Click, AddressOf OutputStopButton_Click



        ' File is not selected, disable all buttons but PickFileButton
        DisableButtons()
        SetPickFileButton(True)
        SetCancelButton(False)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

#Region "Trim specific"
    Private Async Sub TranscodeTrim(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        StopPlayers()
        DisableButtons()
        GetPresetProfile(ProfileSelect)

        ' Clear messages
        StatusMessage.Text = ""

        _Transcoder.TrimStartTime = _Start
        _Transcoder.TrimStopTime = _Stop

        Try
            If _InputFile IsNot Nothing Then
                _OutputFile = Await KnownFolders.VideosLibrary.CreateFileAsync(_OutputFileName, CreationCollisionOption.GenerateUniqueName)
                OutputPathText("Output (" & _OutputFile.Path & ")")

                Dim preparedTranscodeResult = Await _Transcoder.PrepareFileTranscodeAsync(_InputFile, _OutputFile, Profile)
                If preparedTranscodeResult.CanTranscode Then
                    SetCancelButton(True)
                    Dim progress = New Progress(Of Double)(AddressOf TranscodeProgress)
                    Await preparedTranscodeResult.TranscodeAsync().AsTask(_cts.Token, progress)
                    TranscodeComplete()
                Else
                    TranscodeFailure(preparedTranscodeResult.FailureReason)
                End If
            End If
        Catch generatedExceptionName As TaskCanceledException
            OutputText("")
            TranscodeError("Transcode Canceled")
        Catch exception As Exception
            TranscodeError(exception.Message)
        End Try
    End Sub

    Private Sub GetPresetProfile(combobox As ComboBox)
        Profile = Nothing
        Dim videoEncodingProfile As VideoEncodingQuality = VideoEncodingQuality.Wvga
        Select Case combobox.SelectedIndex
            Case 0
                videoEncodingProfile = VideoEncodingQuality.HD1080p
                Exit Select
            Case 1
                videoEncodingProfile = VideoEncodingQuality.HD720p
                Exit Select
            Case 2
                videoEncodingProfile = VideoEncodingQuality.Wvga
                Exit Select
            Case 3
                videoEncodingProfile = VideoEncodingQuality.Ntsc
                Exit Select
            Case 4
                videoEncodingProfile = VideoEncodingQuality.Pal
                Exit Select
            Case 5
                videoEncodingProfile = VideoEncodingQuality.Vga
                Exit Select
            Case 6
                videoEncodingProfile = VideoEncodingQuality.Qvga
                Exit Select
        End Select

        If _UseMp4 Then
            Profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(videoEncodingProfile)
        Else
            Profile = MediaEncodingProfile.CreateWmv(videoEncodingProfile)
        End If
    End Sub

    Private Sub MarkIn(sender As Object, e As RoutedEventArgs)
        _Start = InputVideo.Position
        StartTimeText.Text = (CSng(_Start.TotalMilliseconds) / 1000).ToString
        ' Make sure end time is after start time.
        If (_Start.CompareTo(_Stop) > 0) OrElse (_Start.CompareTo(_Stop) = 0 AndAlso _Start <> _c_sec0) Then
            _Stop = _Start.Add(_c_sec1)
            EndTimeText.Text = (CSng(_Stop.TotalMilliseconds) / 1000).ToString
        End If
    End Sub

    Private Sub MarkOut(sender As Object, e As RoutedEventArgs)
        _Stop = InputVideo.Position
        EndTimeText.Text = (CSng(_Stop.TotalMilliseconds) / 1000).ToString
        ' Make sure end time is after start time.
        If (_Start.CompareTo(_Stop) > 0) OrElse (_Start.CompareTo(_Stop) = 0 AndAlso _Start <> _c_sec0) Then
            If _Stop.CompareTo(_c_sec1) > 0 Then
                _Start = _Stop.Subtract(_c_sec1)
            Else
                _Start = _c_sec0
            End If
            StartTimeText.Text = (CSng(_Start.TotalMilliseconds) / 1000).ToString
        End If
    End Sub
#End Region

    Private Sub TranscodeProgress(percent As Double)
        OutputText("Progress:  " & percent.ToString.Split("."c)(0) & "%")
    End Sub

    Private Async Sub TranscodeComplete()
        OutputText("Transcode completed.")
        Dim stream As Windows.Storage.Streams.IRandomAccessStream = Await _OutputFile.OpenAsync(Windows.Storage.FileAccessMode.Read)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      OutputVideo.SetSource(stream, _OutputFile.ContentType)
                                                                                  End Sub)

        EnableButtons()
        SetCancelButton(False)
    End Sub

    Private Async Sub TranscodeCancel(sender As Object, e As RoutedEventArgs)
        Try
            _cts.Cancel()
            _cts.Dispose()
            _cts = New CancellationTokenSource()

            If _OutputFile IsNot Nothing Then
                Await _OutputFile.DeleteAsync()
            End If
        Catch exception As Exception
            TranscodeError(exception.Message)
        End Try
    End Sub

    Private Async Sub TranscodeFailure(reason As TranscodeFailureReason)
        Try
            If _OutputFile IsNot Nothing Then
                Await _OutputFile.DeleteAsync()
            End If
        Catch exception As Exception
            TranscodeError(exception.Message)
        End Try

        Select Case reason
            Case TranscodeFailureReason.CodecNotFound
                TranscodeError("Codec not found.")
                Exit Select
            Case TranscodeFailureReason.InvalidProfile
                TranscodeError("Invalid profile.")
                Exit Select
            Case Else
                TranscodeError("Unknown failure.")
                Exit Select
        End Select
    End Sub

    Private Async Sub PickFile(sender As Object, e As RoutedEventArgs)
        Dim picker As New Windows.Storage.Pickers.FileOpenPicker()
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
        picker.FileTypeFilter.Add(".wmv")
        picker.FileTypeFilter.Add(".mp4")

        Dim file As Windows.Storage.StorageFile = Await picker.PickSingleFileAsync()
        If file IsNot Nothing Then
            Dim stream As Windows.Storage.Streams.IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)

            _InputFile = file
            InputVideo.SetSource(stream, file.ContentType)
            InputVideo.Play()

            ' Enable buttons
            EnableButtons()
        End If
    End Sub

    Private Sub OnTargetFormatChanged(sender As Object, e As SelectionChangedEventArgs)
        If TargetFormat.SelectedIndex > 0 Then
            _OutputFileName = "TranscodeSampleOutput.wmv"
            _UseMp4 = False
        Else
            _OutputFileName = "TranscodeSampleOutput.mp4"
            _UseMp4 = True
        End If
        OutputPathText("Output (Libraries\Videos\" & _OutputFileName & ")")
    End Sub

    Private Sub InputPlayButton_Click(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        If InputVideo.DefaultPlaybackRate = 0 Then
            InputVideo.DefaultPlaybackRate = 1.0
            InputVideo.PlaybackRate = 1.0
        End If

        InputVideo.Play()
    End Sub

    Private Sub InputStopButton_Click(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        InputVideo.Stop()
    End Sub

    Private Sub InputPauseButton_Click(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        InputVideo.Pause()
    End Sub

    Private Sub OutputPlayButton_Click(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        If OutputVideo.DefaultPlaybackRate = 0 Then
            OutputVideo.DefaultPlaybackRate = 1.0
            OutputVideo.PlaybackRate = 1.0
        End If

        OutputVideo.Play()
    End Sub

    Private Sub OutputStopButton_Click(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        OutputVideo.Stop()
    End Sub

    Private Sub OutputPauseButton_Click(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        OutputVideo.Pause()
    End Sub

    Private Async Sub SetPickFileButton(isEnabled As Boolean)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      PickFileButton.IsEnabled = isEnabled
                                                                                  End Sub)
    End Sub

    Private Async Sub SetCancelButton(isEnabled As Boolean)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      Cancel.IsEnabled = isEnabled
                                                                                  End Sub)
    End Sub

    Private Async Sub EnableButtons()
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      PickFileButton.IsEnabled = True
                                                                                      TargetFormat.IsEnabled = True
                                                                                      ProfileSelect.IsEnabled = True
                                                                                      Transcode.IsEnabled = True
                                                                                      MarkInButton.IsEnabled = True
                                                                                      MarkOutButton.IsEnabled = True
                                                                                  End Sub)
    End Sub

    Private Async Sub DisableButtons()
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      ProfileSelect.IsEnabled = False
                                                                                      Transcode.IsEnabled = False
                                                                                      PickFileButton.IsEnabled = False
                                                                                      TargetFormat.IsEnabled = False
                                                                                      MarkInButton.IsEnabled = False
                                                                                      MarkOutButton.IsEnabled = False
                                                                                  End Sub)
    End Sub

    Private Async Sub StopPlayers()
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      If InputVideo.CurrentState <> MediaElementState.Paused Then
                                                                                          InputVideo.Pause()
                                                                                      End If
                                                                                      If OutputVideo.CurrentState <> MediaElementState.Paused Then
                                                                                          OutputVideo.Pause()
                                                                                      End If
                                                                                  End Sub)
    End Sub

    Private Async Sub PlayFile(MediaFile As Windows.Storage.StorageFile)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      Try
                                                                                          Dim stream As Windows.Storage.Streams.IRandomAccessStream = MediaFile.OpenAsync(FileAccessMode.Read)
                                                                                          OutputVideo.SetSource(stream, MediaFile.ContentType)
                                                                                          OutputVideo.Play()
                                                                                      Catch exception As Exception
                                                                                          TranscodeError(exception.Message)
                                                                                      End Try
                                                                                  End Sub)
    End Sub

    Private Async Sub TranscodeError(ErrorMessage As String)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      StatusMessage.Foreground = New Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red)
                                                                                      StatusMessage.Text = ErrorMessage
                                                                                  End Sub)

        EnableButtons()
        SetCancelButton(False)
    End Sub

    Private Async Sub OutputText(text As String)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      OutputMsg.Foreground = New Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green)
                                                                                      OutputMsg.Text = text
                                                                                  End Sub)
    End Sub

    Private Async Sub OutputPathText(text As String)
        Await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                      OutputPath.Text = text
                                                                                  End Sub)
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _cts.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region


End Class

