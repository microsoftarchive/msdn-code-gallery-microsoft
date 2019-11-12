'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports SDKTemplate
Imports System
Imports Windows.Media
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()

    Private rootPage As MainPage = MainPage.Current

    Private currentSongIndex As Integer = 0
    Private playlistCount As Integer = 0
    Private previousRegistered As Boolean = False
    Private nextRegistered As Boolean = False
    Private wasPlaying As Boolean = False
    Private playlist As New List(Of song)()


    Public Class song
        Public File As Windows.Storage.StorageFile
        Public Artist As String
        Public Track As String
        'public Windows.Storage.FileProperties.StorageItemThumbnail AlbumArt;

        Public Sub New(file__1 As Windows.Storage.StorageFile)
            File = file__1
        End Sub

        Public Async Function getMusicPropertiesAsync() As Task
            Dim properties = Await Me.File.Properties.GetMusicPropertiesAsync()
            'Windows.Storage.FileProperties.StorageItemThumbnail thumbnail = null;


            Artist = properties.Artist
            Track = properties.Title
        End Function
    End Class

    Public Sub New()

        Me.InitializeComponent()

        AddHandler DefaultOption.Click, AddressOf DefaultOption_Click

        AddHandler MediaControl.PlayPauseTogglePressed, AddressOf MediaControl_PlayPauseTogglePressed
        AddHandler MediaControl.PlayPressed, AddressOf MediaControl_PlayPressed
        AddHandler MediaControl.PausePressed, AddressOf MediaControl_PausePressed
        AddHandler MediaControl.StopPressed, AddressOf MediaControl_StopPressed

        AddHandler MediaControl.FastForwardPressed, AddressOf MediaControl_FastForwardPressed
        AddHandler MediaControl.RewindPressed, AddressOf MediaControl_RewindPressed
        AddHandler MediaControl.RecordPressed, AddressOf MediaControl_RecordPressed
        AddHandler MediaControl.ChannelDownPressed, AddressOf MediaControl_ChannelDownPressed
        AddHandler MediaControl.ChannelUpPressed, AddressOf MediaControl_ChannelUpPressed

        AddHandler PlayButton.Click, AddressOf MediaControl_PlayPauseTogglePressed
        AddHandler mediaElement.CurrentStateChanged, AddressOf mediaElement_CurrentStateChanged
        AddHandler mediaElement.MediaOpened, AddressOf mediaElement_MediaOpened
        AddHandler mediaElement.MediaEnded, AddressOf mediaElement_MediaEnded

        Scenario1Text.Text = "Events Registered"
    End Sub

    Private Async Sub DefaultOption_Click(sender As Object, e As RoutedEventArgs)
        Dim openPicker As New FileOpenPicker()
        openPicker.ViewMode = PickerViewMode.List
        openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary
        openPicker.FileTypeFilter.Add(".mp3")
        openPicker.FileTypeFilter.Add(".m4a")
        openPicker.FileTypeFilter.Add(".wma")
        openPicker.FileTypeFilter.Add(".MP4")
        Dim files As IReadOnlyList(Of StorageFile) = Await openPicker.PickMultipleFilesAsync()
        If files.Count > 0 Then
            Await createPlaylist(files)
            Await SetCurrentPlayingAsync(currentSongIndex)
        End If
    End Sub

    Private Async Sub mediaElement_MediaEnded(sender As Object, e As RoutedEventArgs)
        If currentSongIndex < playlistCount - 1 Then
            currentSongIndex += 1

            Await SetCurrentPlayingAsync(currentSongIndex)
            If wasPlaying Then
                mediaElement.Play()
            End If
        End If
    End Sub

    Private Sub mediaElement_MediaOpened(sender As Object, e As RoutedEventArgs)
        If wasPlaying Then
            MediaElement.Play()
        End If
    End Sub



    Private Sub mediaElement_CurrentStateChanged(sender As Object, e As RoutedEventArgs)
        If MediaElement.CurrentState = MediaElementState.Playing Then
            MediaControl.IsPlaying = True
            PlayButton.Content = "Pause"
        Else
            MediaControl.IsPlaying = False
            PlayButton.Content = "Play"
        End If
    End Sub

    Private Async Function SetMediaElementSourceAsync(Song As song) As Task
        Dim stream = Song.File.OpenAsync(Windows.Storage.FileAccessMode.Read)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     mediaElement.SetSource(stream, Song.File.ContentType)
                                                                 End Sub)



        MediaControl.ArtistName = Song.Artist
        MediaControl.TrackName = Song.Track

		End Function


    Private Async Function createPlaylist(files As IReadOnlyList(Of StorageFile)) As Task

        playlistCount = files.Count
        If previousRegistered Then
            RemoveHandler MediaControl.PreviousTrackPressed, AddressOf MediaControl_PreviousTrackPressed
            previousRegistered = False
        End If
        If nextRegistered Then
            RemoveHandler MediaControl.NextTrackPressed, AddressOf MediaControl_NextTrackPressed
            nextRegistered = False
        End If
        currentSongIndex = 0
        playlist.Clear()

        If files.Count > 0 Then
            ' Application now has read/write access to the picked file(s) 
            If files.Count > 1 Then
                AddHandler MediaControl.NextTrackPressed, AddressOf MediaControl_NextTrackPressed
                nextRegistered = True
            End If

            ' Create the playlist
            For Each file As StorageFile In files
                Dim newSong As New song(file)
                Await newSong.getMusicPropertiesAsync()
                playlist.Add(newSong)


            Next
        End If

    End Function

    Private Async Function SetCurrentPlayingAsync(playlistIndex As Integer) As Task
        wasPlaying = MediaControl.IsPlaying
        Dim stream As Windows.Storage.Streams.IRandomAccessStream = Nothing
        Try
            stream = Await playlist(playlistIndex).File.OpenAsync(Windows.Storage.FileAccessMode.Read)
        Catch e As Exception
            dispatchMessage("Error" & e.Message).Start()
        End Try

        Try
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                         mediaElement.SetSource(stream, playlist(playlistIndex).File.ContentType)
                                                                     End Sub)
        Catch e As Exception
            dispatchMessage("Error" & e.Message).Start()
        End Try

			MediaControl.ArtistName = playlist(playlistIndex).Artist
			MediaControl.TrackName = playlist(playlistIndex).Track


		End Function

    Private Async Sub MediaControl_ChannelUpPressed(sender As Object, e As Object)
        Await dispatchMessage("Channel Up Pressed")
    End Sub

    Private Async Sub MediaControl_ChannelDownPressed(sender As Object, e As Object)
        Await dispatchMessage("Channel Down Pressed")
    End Sub

    Private Async Sub MediaControl_RecordPressed(sender As Object, e As Object)
        Await dispatchMessage("Record Pressed")
    End Sub

    Private Async Sub MediaControl_RewindPressed(sender As Object, e As Object)
        Await dispatchMessage("Rewind Pressed")
    End Sub

    Private Async Sub MediaControl_FastForwardPressed(sender As Object, e As Object)
        Await dispatchMessage("Fast Forward Pressed")
    End Sub

    Private Async Sub MediaControl_PreviousTrackPressed(sender As Object, e As Object)
        Await dispatchMessage("Previous Track Pressed")
        If currentSongIndex > 0 Then
            If currentSongIndex = (playlistCount - 1) Then
                If Not nextRegistered Then
                    AddHandler MediaControl.NextTrackPressed, AddressOf MediaControl_NextTrackPressed
                    nextRegistered = True
                End If
            End If
            currentSongIndex -= 1

            If currentSongIndex = 0 Then
                RemoveHandler MediaControl.PreviousTrackPressed, AddressOf MediaControl_PreviousTrackPressed
                previousRegistered = False
            End If

            Await SetCurrentPlayingAsync(currentSongIndex)
        End If

    End Sub

    Private Async Sub MediaControl_NextTrackPressed(sender As Object, e As Object)
        Await dispatchMessage("Next Track Pressed")

        If currentSongIndex < (playlistCount - 1) Then
            currentSongIndex += 1
            Await SetCurrentPlayingAsync(currentSongIndex)
            If currentSongIndex > 0 Then
                If Not previousRegistered Then
                    AddHandler MediaControl.PreviousTrackPressed, AddressOf MediaControl_PreviousTrackPressed
                    previousRegistered = True

                End If
            End If
            If currentSongIndex = (playlistCount - 1) Then
                If nextRegistered Then
                    RemoveHandler MediaControl.NextTrackPressed, AddressOf MediaControl_NextTrackPressed
                    nextRegistered = False
                End If
            End If
        End If

    End Sub

    Private Async Sub MediaControl_StopPressed(sender As Object, e As Object)

        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     mediaElement.[Stop]()
                                                                                 End Sub)

        Await dispatchMessage("Stop Pressed")
		End Sub

    Private Async Sub MediaControl_PausePressed(sender As Object, e As Object)
        Await dispatchMessage("Pause Pressed")
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     mediaElement.Pause()
                                                                                 End Sub)
		End Sub

    Private Async Sub MediaControl_PlayPressed(sender As Object, e As Object)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     mediaElement.Play()
                                                                                 End Sub)
        Await dispatchMessage("Play Pressed")
		End Sub

    Private Async Sub MediaControl_PlayPauseTogglePressed(sender As Object, e As Object)
        If MediaControl.IsPlaying Then
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         mediaElement.Pause()
                                                                                     End Sub)
            Await dispatchMessage("Play/Pause Pressed - Pause")
			Else

            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         mediaElement.Play()
                                                                                     End Sub)
            Await dispatchMessage("Play/Pause Pressed - Play")
			End If

		End Sub

    Private Async Function dispatchMessage(message As String) As Task
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     Scenario1Text.Text = getTimeStampedMessage(message)
                                                                                 End Sub)
		End Function

    Private Function getTimeStampedMessage(eventText As String) As String
        Return eventText + "   " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString()
    End Function

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
