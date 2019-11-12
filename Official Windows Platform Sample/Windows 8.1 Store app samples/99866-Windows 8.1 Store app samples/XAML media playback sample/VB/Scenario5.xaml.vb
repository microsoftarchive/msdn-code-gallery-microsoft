'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
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
        Scenario5MediaElement.Source = Nothing
    End Sub

    ''' <summary>
    ''' Click handler for the "Select multiple media files" button.
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

        ' Prompt user to select one or more files            
        Dim files As IReadOnlyList(Of StorageFile) = Await fileOpenPicker.PickMultipleFilesAsync()

        ' Add selected files to the playlist ListBox
        For Each file As StorageFile In files
            Playlist.Items.Add(file)
        Next file

    End Sub

    ''' <summary>
    ''' Handler for the MediaEnded event of the MediaElement.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub MediaElement_MediaEnded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Advance to the next file in the playlist if any remain
        If Playlist.SelectedIndex < Playlist.Items.Count - 1 Then
            Playlist.SelectedIndex += 1
        End If
    End Sub

    ''' <summary>
    ''' Selection changed handler for the playlist ListBox.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Playlist_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim file As StorageFile = TryCast(Playlist.SelectedItem, StorageFile)

        If file IsNot Nothing Then
            ' Open the selected file and set it as the MediaElement's source
            Dim stream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
            Scenario5MediaElement.SetSource(stream, file.ContentType)
        End If
    End Sub

End Class
