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
Imports System.Collections.Generic
Imports SDKTemplate
Imports Windows.Media.Playlists
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Create
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Creates a playlist with the audio picked by the user in the FilePicker
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PickAudioButton_Click(sender As Object, e As RoutedEventArgs)
        '' Make sure app is unsnapped before invoking the file picker
        If Not MainPage.EnsureUnsnapped() Then
            Me.OutputStatus.Text = "Unable to unsnap the app."
            Return
        End If

        Dim picker As FileOpenPicker = MainPage.CreateFilePicker(MainPage.audioExtensions)
        Dim files As IReadOnlyList(Of StorageFile) = Await picker.PickMultipleFilesAsync()

        If files IsNot Nothing AndAlso files.Count > 0 Then
            MainPage.playlist = New Playlist()

            For Each file As StorageFile In files
                MainPage.playlist.Files.Add(file)
            Next

            Dim savedFile As StorageFile = Await MainPage.playlist.SaveAsAsync(KnownFolders.MusicLibrary,
                                                                               "Sample",
                                                                               NameCollisionOption.ReplaceExisting,
                                                                               PlaylistFormat.WindowsMedia)

            Me.OutputStatus.Text = savedFile.Name & " was created and saved with " & MainPage.playlist.Files.Count & " files."

            ''Reset playlist so subsequent SaveAsync calls don't fail
            MainPage.playlist = Nothing
        Else
            Me.OutputStatus.Text = "No files picked."
        End If
    End Sub

End Class
