'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage

    ''' <summary>
    ''' Timer used to update the media position Slider.
    ''' </summary>
    Private _timer As DispatcherTimer
    ''' <summary>
    ''' Tracks whether the user is currently interacting with the media position Slider.
    ''' </summary>
    Private _sliderPressed As Boolean = False

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        TimelineSlider.AddHandler(Slider.PointerPressedEvent, New PointerEventHandler(AddressOf TimelineSlider_PointerPressed), True)
        TimelineSlider.AddHandler(Slider.PointerReleasedEvent, New PointerEventHandler(AddressOf TimelineSlider_PointerReleased), True)

        _timer = New DispatcherTimer()
        SetFullWindowMode(False)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' Invoked when this page will no longer be displayed in a Frame.
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Set source to null to ensure it stops playing to a Play To device if applicable.
        Scenario4MediaElement.Source = Nothing
    End Sub

    ''' <summary>
    ''' Click handler for the "Select a media file" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub OpenFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fileOpenPicker As New FileOpenPicker()

        ' Filter to include a sample subset of file types
        fileOpenPicker.FileTypeFilter.Add(".wmv")
        fileOpenPicker.FileTypeFilter.Add(".mp4")
        fileOpenPicker.FileTypeFilter.Add(".mp3")
        fileOpenPicker.FileTypeFilter.Add(".wma")
        fileOpenPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary

        ' Prompt user to select a file            
        Dim file As StorageFile = Await fileOpenPicker.PickSingleFileAsync()

        ' Ensure a file was selected
        If file IsNot Nothing Then
            ' Open the selected file and set it as the MediaElement's source
            Dim stream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
            Scenario4MediaElement.SetSource(stream, file.ContentType)
        End If
    End Sub

    ''' <summary>
    ''' Click handler for the "Play" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub PlayButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Scenario4MediaElement.PlaybackRate = 1.0

        Scenario4MediaElement.Play()
        SetupTimer()
    End Sub

    ''' <summary>
    ''' Click handler for the "Pause" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub PauseButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Scenario4MediaElement.Pause()
    End Sub

    ''' <summary>
    ''' Click handler for the "Stop" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StopButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Scenario4MediaElement.Stop()
    End Sub

    ''' <summary>
    ''' Click handler for the "Rewind" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RewindButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Play media in reverse at 1x speed
        Scenario4MediaElement.PlaybackRate = -1.0
    End Sub

    ''' <summary>
    ''' Click handler for the "Forward" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ForwardButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Fast forward media at 2x speed
        Scenario4MediaElement.PlaybackRate = 2.0
    End Sub

    ''' <summary>
    ''' Click handler for the "Zoom" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ZoomButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Scenario4MediaElement.Stretch = Stretch.Uniform Then
            ' Stretch to fill, which will zoom in and clip content to remove letterboxing if the media file's aspect ratio does not match the MediaElement's aspect ratio
            Scenario4MediaElement.Stretch = Stretch.UniformToFill
        Else
            ' Stretch uniformly, which will result in letterboxing if the media file's aspect ratio does not match the MediaElement's aspect ratio
            Scenario4MediaElement.Stretch = Stretch.Uniform
        End If
    End Sub

    ''' <summary>
    ''' Click handler for the "Full Window" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub FullWindowButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SetFullWindowMode((Not Scenario4MediaElement.IsFullWindow))
    End Sub

    ''' <summary>
    ''' Double tap handler for the MediaElement.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub MediaElement_DoubleTapped(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs)
        SetFullWindowMode((Not Scenario4MediaElement.IsFullWindow))
    End Sub

    ''' <summary>
    ''' Toggles full-window mode and updates the position of transport controls
    ''' </summary>
    ''' <param name="isFullWindow"></param>
    Private Sub SetFullWindowMode(ByVal isFullWindow As Boolean)
        Scenario4MediaElement.IsFullWindow = isFullWindow

        If isFullWindow Then
            ' When displaying in full-window mode, center transport controls at the bottom of the window

            ' Since the Popup is in the Output Grid, the offset must account for its parent
            Dim rootFrame = TryCast(Window.Current.Content, Frame)
            Dim outputGridOffset = Output.TransformToVisual(rootFrame).TransformPoint(New Point(0, 0))

            TransportControlsPopup.HorizontalOffset = (rootFrame.ActualWidth - TransportControlsPopup.ActualWidth) / 2 - outputGridOffset.X
            TransportControlsPopup.VerticalOffset = rootFrame.ActualHeight - TransportControlsPopup.ActualHeight - outputGridOffset.Y
        Else
            ' When displaying in embedded mode, center transport controls at the bottom of the MediaElement
            TransportControlsPopup.HorizontalOffset = (Scenario4MediaElement.Width - TransportControlsPopup.ActualWidth) / 2
            TransportControlsPopup.VerticalOffset = Scenario4MediaElement.Height - TransportControlsPopup.ActualHeight
        End If
    End Sub

    ''' <summary>
    ''' MediaOpened event handler for the MediaElement.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub MediaElement_MediaOpened(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Set slider maximum to the media's duration in seconds
        TimelineSlider.Maximum = Math.Round(Scenario4MediaElement.NaturalDuration.TimeSpan.TotalSeconds, MidpointRounding.AwayFromZero)
        TimelineSlider.StepFrequency = CalculateSliderFrequency(Scenario4MediaElement.NaturalDuration.TimeSpan)

        SetupTimer()
    End Sub

    Private Sub MediaElement_MediaEnded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _timer.Stop()
        RemoveHandler _timer.Tick, AddressOf Timer_Tick

        TimelineSlider.Value = 0.0
    End Sub

#Region "Timeline Slider interaction"

    Private Sub TimelineSlider_PointerPressed(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        _sliderPressed = True
    End Sub

    Private Sub TimelineSlider_PointerReleased(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        If _sliderPressed Then
            Scenario4MediaElement.Position = TimeSpan.FromSeconds(TimelineSlider.Value)
            _sliderPressed = False
        End If
    End Sub

    Private Function CalculateSliderFrequency(ByVal timevalue As TimeSpan) As Double
        ' Calculate the slider step frequency based on the timespan length
        Dim stepFrequency As Double = 1

        If timevalue.TotalHours >= 1 Then
            stepFrequency = 60
        ElseIf timevalue.TotalMinutes > 30 Then
            stepFrequency = 30
        ElseIf timevalue.TotalMinutes > 10 Then
            stepFrequency = 10
        Else
            stepFrequency = Math.Round(timevalue.TotalSeconds / 100, MidpointRounding.AwayFromZero)
        End If

        Return stepFrequency
    End Function

    Private Sub SetupTimer()
        If Not _timer.IsEnabled Then
            _timer.Interval = TimeSpan.FromSeconds(TimelineSlider.StepFrequency)
            AddHandler _timer.Tick, AddressOf Timer_Tick
            _timer.Start()
        End If
    End Sub

    Private Sub Timer_Tick(ByVal sender As Object, ByVal e As Object)
        ' Don't update the Slider's position while the user is interacting with it
        If Not _sliderPressed Then
            TimelineSlider.Value = Scenario4MediaElement.Position.TotalSeconds
        End If
    End Sub

#End Region

End Class
