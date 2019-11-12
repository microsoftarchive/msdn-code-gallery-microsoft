'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports SDKTemplate
Imports Windows.Media.Playlists
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Add
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Loads a playlist picked by the user in the FilePicker
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PickPlaylistButton_Click(sender As Object, e As RoutedEventArgs)
        '' Make sure app is unsnapped before invoking the file picker
        If Not MainPage.EnsureUnsnapped() Then
            Me.OutputStatus.Text = "Unable to unsnap the app."
            Return
        End If

        Try
            MainPage.playlist = Await MainPage.PickPlaylistAsync()

            If MainPage.playlist IsNot Nothing Then
                Me.OutputStatus.Text = "Playlist loaded."
            Else
                Me.OutputStatus.Text = "No playlist picked."
            End If
        Catch ex As UnauthorizedAccessException
            Me.OutputStatus.Text = "Access is denied, cannot load playlist."
        End Try
    End Sub

    ''' <summary>
    ''' Adds a file to the end of the playlist loaded in PickPlaylistButton_Click
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PickAudioButton_Click(sender As Object, e As RoutedEventArgs)
        If MainPage.playlist IsNot Nothing Then
            '' Make sure app is unsnapped before invoking the file picker
            If Not MainPage.EnsureUnsnapped() Then
                Me.OutputStatus.Text = "Unable to unsnap the app."
                Return
            End If

            Dim picker As FileOpenPicker = MainPage.CreateFilePicker(MainPage.audioExtensions)
            Dim files As IReadOnlyList(Of StorageFile) = Await picker.PickMultipleFilesAsync()

            If files IsNot Nothing And files.Count > 0 Then
                For Each file As StorageFile In files
                    MainPage.playlist.Files.Add(file)
                Next

                Try
                    Await MainPage.playlist.SaveAsync()
                    Me.OutputStatus.Text = files.Count & " files added to playlist."
                Catch ErrorMessage As Exception
                    Me.OutputStatus.Text = ErrorMessage.Message
                End Try
            Else
                Me.OutputStatus.Text = "No files picked."
            End If
        Else
            Me.OutputStatus.Text = "Pick playlist first."
        End If
    End Sub

End Class
