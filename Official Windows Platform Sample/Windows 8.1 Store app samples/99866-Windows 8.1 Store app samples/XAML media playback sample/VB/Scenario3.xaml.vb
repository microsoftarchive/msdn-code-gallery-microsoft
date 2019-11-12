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
Partial Public NotInheritable Class Scenario3
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
        Scenario3MediaElement.Source = Nothing
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
            Scenario3MediaElement.SetSource(stream, file.ContentType)
        End If
    End Sub

    ''' <summary>
    ''' Click handler for the "Add marker" button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub AddMarkerButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Add a marker to the MediaElement at 2 seconds past the current time
        Dim marker As New TimelineMarker()
        marker.Text = "Marker Fired"
        marker.Time = TimeSpan.FromSeconds(Scenario3MediaElement.Position.TotalSeconds + 2)
        Scenario3MediaElement.Markers.Add(marker)

        Scenario3Text.Text += String.Format("Marker added at time = {0:g}" & vbLf, Math.Round(Scenario3MediaElement.Position.TotalSeconds + 2, 2))
    End Sub

    ''' <summary>
    ''' Handler for the MediaElement.MarkerReached event.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub MediaElement_MarkerReached(ByVal sender As Object, ByVal e As TimelineMarkerRoutedEventArgs)
        ' Display marker info when a marker is reached
        Scenario3Text.Text += e.Marker.Text & ": " & String.Format("{0:g}" & vbLf, Math.Round(e.Marker.Time.TotalSeconds, 2))
    End Sub
End Class
