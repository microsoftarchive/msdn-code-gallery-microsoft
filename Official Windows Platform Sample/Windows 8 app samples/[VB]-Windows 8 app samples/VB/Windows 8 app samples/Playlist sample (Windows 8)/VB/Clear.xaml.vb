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
Imports Windows.UI.Xaml

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Clear
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Clears the playlist picked by the user in the FilePicker
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
                MainPage.playlist.Files.Clear()

                Try
                    Await MainPage.playlist.SaveAsync()
                    Me.OutputStatus.Text = "Playlist cleared."
                Catch ErrorMessage As Exception
                    Me.OutputStatus.Text = ErrorMessage.Message
                End Try
            Else
                Me.OutputStatus.Text = "No playlist picked."
            End If
        Catch ex As UnauthorizedAccessException
            Me.OutputStatus.Text = "Access is denied, cannot load playlist."
        End Try
    End Sub

End Class
