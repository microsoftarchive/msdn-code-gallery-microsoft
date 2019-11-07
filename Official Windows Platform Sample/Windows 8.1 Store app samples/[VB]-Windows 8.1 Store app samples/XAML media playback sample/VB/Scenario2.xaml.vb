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
Partial Public NotInheritable Class Scenario2
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
        If rootPage.SystemMediaTransportControlsInitialized Then
            ' PlayTo handling requires either no support for SystemMediaTransportControls or
            ' support for Play and Pause similar to background audio.  If the SystemMediaTransportControls
            ' have been initialized in another scenario than we also need to hook them up here or PlayTo
            ' will fail.
            systemMediaControls = SystemMediaTransportControls.GetForCurrentView()
            AddHandler systemMediaControls.ButtonPressed, AddressOf SystemMediaControls_ButtonPressed
            systemMediaControls.IsPlayEnabled = True
            systemMediaControls.IsPauseEnabled = True
            systemMediaControls.IsStopEnabled = True

            AddHandler Scenario2MediaElement.CurrentStateChanged, AddressOf MediaElement_CurrentStateChanged
        End If
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

        ' Set source to null to ensure it stops playing to a Play To device if applicable.
        Scenario2MediaElement.Source = Nothing
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
            If systemMediaControls IsNot Nothing Then
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
            End If

            ' Open the selected file and set it as the MediaElement's source
            Dim stream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
            Scenario2MediaElement.SetSource(stream, file.ContentType)
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
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Scenario2MediaElement.Play())

            Case SystemMediaTransportControlsButton.Pause
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Scenario2MediaElement.Pause())

            Case SystemMediaTransportControlsButton.Stop
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Scenario2MediaElement.Stop())

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
        Select Case Scenario2MediaElement.CurrentState
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
