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
Imports SDKTemplate
Imports System
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Core
Imports Windows.Media


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PlaybackControl
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private cw As CoreWindow = Window.Current.CoreWindow
    Private IsPlaying As Boolean = False

    Shared IsMediaControlRegistered As Boolean = False
    Shared SoundLevelChangedHandler As EventHandler(Of Object) = Nothing
    Shared PlayPauseTogglePressedHandler As EventHandler(Of Object) = Nothing
    Shared PlayPressedHandler As EventHandler(Of Object) = Nothing
    Shared PausePressedHandler As EventHandler(Of Object) = Nothing
    Shared StopPressedHandler As EventHandler(Of Object) = Nothing

    Public Sub New()
        Me.InitializeComponent()

        If IsMediaControlRegistered Then
            ' remove previous handlers
            RemoveHandler MediaControl.SoundLevelChanged, AddressOf MediaControl_SoundLevelChanged
            RemoveHandler MediaControl.PlayPauseTogglePressed, AddressOf MediaControl_PlayPauseTogglePressed
            RemoveHandler MediaControl.PlayPressed, AddressOf MediaControl_PlayPressed
            RemoveHandler MediaControl.PausePressed, AddressOf MediaControl_PausePressed
            RemoveHandler MediaControl.StopPressed, AddressOf MediaControl_StopPressed
        End If

        ' add new handlers
        AddHandler MediaControl.SoundLevelChanged, AddressOf MediaControl_SoundLevelChanged
        AddHandler MediaControl.PlayPauseTogglePressed, AddressOf MediaControl_PlayPauseTogglePressed
        AddHandler MediaControl.PlayPressed, AddressOf MediaControl_PlayPressed
        AddHandler MediaControl.PausePressed, AddressOf MediaControl_PausePressed
        AddHandler MediaControl.StopPressed, AddressOf MediaControl_StopPressed

        '' save current handlers
        'SoundLevelChangedHandler = MediaControl_SoundLevelChanged()
        'PlayPauseTogglePressedHandler = MediaControl_PlayPauseTogglePressed()
        'PlayPressedHandler = MediaControl_PlayPressed()
        'PausePressedHandler = MediaControl_PausePressed()
        'StopPressedHandler = MediaControl_StopPressed()

        IsMediaControlRegistered = True

        MediaControl.IsPlaying = False
    End Sub

    Public Async Sub Play()
        Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        OutputMedia.Play()
                                                                                        MediaControl.IsPlaying = True
                                                                                    End Sub)
		End Sub

    Public Async Sub Pause()
        Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        OutputMedia.Pause()
                                                                                        MediaControl.IsPlaying = False
                                                                                    End Sub)
    End Sub

    Public Async Sub [Stop]()
        Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        OutputMedia.Stop()
                                                                                        MediaControl.IsPlaying = False
                                                                                    End Sub)
    End Sub

    Public Sub SetAudioCategory(category As AudioCategory)
        OutputMedia.AudioCategory = category
    End Sub

    Public Sub SetAudioDeviceType(devicetype As AudioDeviceType)
        OutputMedia.AudioDeviceType = devicetype
    End Sub

    Public Async Sub SelectFile()
        Dim picker As New Windows.Storage.Pickers.FileOpenPicker()
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary
        picker.FileTypeFilter.Add(".mp3")
        picker.FileTypeFilter.Add(".mp4")
        picker.FileTypeFilter.Add(".m4a")
        picker.FileTypeFilter.Add(".wma")
        picker.FileTypeFilter.Add(".wav")
        Dim file As Windows.Storage.StorageFile = Await picker.PickSingleFileAsync()
        If file IsNot Nothing Then
            Dim stream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
            OutputMedia.AutoPlay = False
            Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                            OutputMedia.SetSource(stream, file.ContentType)
                                                                                        End Sub)
			End If
		End Sub

    Private Async Sub DisplayStatus(text As String)
        Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        Status.Text &= text & vbLf
                                                                                    End Sub)
		End Sub

    Private Function GetTimeStampedMessage(eventText As String) As String
        Return eventText & "   " & DateTime.Now.Hour.ToString & ":" & DateTime.Now.Minute.ToString & ":" & DateTime.Now.Second.ToString
    End Function

    Private Function SoundLevelToString(level As SoundLevel) As String
        Dim LevelString As String

        Select Case level
            Case SoundLevel.Muted
                LevelString = "Muted"
                Exit Select
            Case SoundLevel.Low
                LevelString = "Low"
                Exit Select
            Case SoundLevel.Full
                LevelString = "Full"
                Exit Select
            Case Else
                LevelString = "Unknown"
                Exit Select
        End Select
        Return LevelString
    End Function

    Private Async Sub AppMuted()
        Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        If OutputMedia.CurrentState <> MediaElementState.Paused Then
                                                                                            IsPlaying = True
                                                                                            Pause()
                                                                                        Else
                                                                                            IsPlaying = False
                                                                                        End If
                                                                                    End Sub)
			DisplayStatus(GetTimeStampedMessage("Muted"))
		End Sub

    Private Async Sub AppUnmuted()
        Await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        If IsPlaying Then
                                                                                            Play()
                                                                                        End If
                                                                                    End Sub)
			DisplayStatus(GetTimeStampedMessage("Unmuted"))
		End Sub

    Private Sub MediaControl_SoundLevelChanged(sender As Object, e As Object)
        Dim soundLevelState = MediaControl.SoundLevel

        DisplayStatus(GetTimeStampedMessage("App sound level is [" & SoundLevelToString(soundLevelState) & "]"))
        If soundLevelState = SoundLevel.Muted Then
            AppMuted()
        Else
            AppUnmuted()
        End If
    End Sub

    Private Sub MediaControl_PlayPauseTogglePressed(sender As Object, e As Object)
        If MediaControl.IsPlaying Then
            Pause()
            DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Pause"))
        Else
            Play()
            DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Play"))
        End If
    End Sub

    Private Sub MediaControl_PlayPressed(sender As Object, e As Object)
        Play()
        DisplayStatus(GetTimeStampedMessage("Play Pressed"))
    End Sub

    Private Sub MediaControl_PausePressed(sender As Object, e As Object)
        Pause()
        DisplayStatus(GetTimeStampedMessage("Pause Pressed"))
    End Sub

    Private Sub MediaControl_StopPressed(sender As Object, e As Object)
        [Stop]()
        DisplayStatus(GetTimeStampedMessage("Stop Pressed"))
    End Sub

    Private Sub Button_Play_Click(sender As Object, e As RoutedEventArgs)
        Play()
    End Sub

    Private Sub Button_Pause_Click(sender As Object, e As RoutedEventArgs)
        Pause()
    End Sub

    Private Sub Button_Stop_Click(sender As Object, e As RoutedEventArgs)
        [Stop]()
    End Sub
End Class
