'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.Media
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private systemMediaControls As SystemMediaTransportControls = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Handle events from system transport contrtols when playing background-capable media
        systemMediaControls = SystemMediaTransportControls.GetForCurrentView()
        AddHandler systemMediaControls.ButtonPressed, AddressOf SystemMediaControls_ButtonPressed
        systemMediaControls.IsPlayEnabled = True
        systemMediaControls.IsPauseEnabled = True
        systemMediaControls.IsStopEnabled = True

        rootPage.SystemMediaTransportControlsInitialized = True

        AddHandler Scenario6MediaElement.CurrentStateChanged, AddressOf MediaElement_CurrentStateChanged
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be removed from a Frame.
    ''' </summary>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        If systemMediaControls IsNot Nothing Then
            RemoveHandler systemMediaControls.ButtonPressed, AddressOf SystemMediaControls_ButtonPressed
            systemMediaControls.IsPlayEnabled = False
            systemMediaControls.IsPauseEnabled = False
            systemMediaControls.IsStopEnabled = False
            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed
            systemMediaControls.DisplayUpdater.ClearAll()
            systemMediaControls.DisplayUpdater.Update()
            systemMediaControls = Nothing
        End If

        Scenario6MediaElement.Source = Nothing
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

        Dim file As StorageFile = Await fileOpenPicker.PickSingleFileAsync()

        ' Ensure a file was selected
        If file IsNot Nothing Then
            Dim mediaPlaybackType As MediaPlaybackType
            If (file.FileType = ".mp4") OrElse (file.FileType = ".wmv") Then
                mediaPlaybackType = mediaPlaybackType.Video
            Else
                mediaPlaybackType = mediaPlaybackType.Music
            End If

            ' Inform the system transport controls of the media information
            If Not (Await systemMediaControls.DisplayUpdater.CopyFromFileAsync(mediaPlaybackType, file)) Then
                '  Problem extracting metadata- just clear everything
                systemMediaControls.DisplayUpdater.ClearAll()
            End If
            systemMediaControls.DisplayUpdater.Update()

            Dim stream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)

            ' Set the selected file as the MediaElement's source
            Scenario6MediaElement.SetSource(stream, file.ContentType)
        End If

    End Sub

    ''' <summary>
    ''' Handler for the system transport controls button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub SystemMediaControls_ButtonPressed(ByVal sender As SystemMediaTransportControls, ByVal e As SystemMediaTransportControlsButtonPressedEventArgs)
        Select Case e.Button
            Case SystemMediaTransportControlsButton.Play
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Scenario6MediaElement.Play())

            Case SystemMediaTransportControlsButton.Pause
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Scenario6MediaElement.Pause())

            Case SystemMediaTransportControlsButton.Stop
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Scenario6MediaElement.Stop())

            Case Else
        End Select
    End Sub

    ''' <summary>
    ''' Handler for the media element state change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub MediaElement_CurrentStateChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim systemMediaControls As SystemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView()
        Select Case Scenario6MediaElement.CurrentState
            Case MediaElementState.Opening
                systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Changing

            Case MediaElementState.Buffering, MediaElementState.Playing
                systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing

            Case MediaElementState.Paused
                systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused

            Case MediaElementState.Stopped
                systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped

            Case Else
                systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed
        End Select
    End Sub

End Class
