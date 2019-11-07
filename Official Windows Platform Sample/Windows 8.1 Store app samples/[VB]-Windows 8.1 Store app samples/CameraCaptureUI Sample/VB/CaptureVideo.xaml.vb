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
Imports Windows.Media.Capture
Imports Windows.Storage
Imports Windows.Storage.Streams

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class CaptureVideo
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private appSettings As Windows.Foundation.Collections.IPropertySet
    Private Const videoKey As String = "capturedVideo"

    Public Sub New()
        Me.InitializeComponent()
        appSettings = ApplicationData.Current.LocalSettings.Values
        ResetButton.Visibility = Visibility.Collapsed
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Async Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Reload previously taken video
        If appSettings.ContainsKey(videoKey) Then
            Dim filePath As Object = Nothing
            If appSettings.TryGetValue(videoKey, filePath) AndAlso filePath.ToString() <> "" Then
                CaptureButton.IsEnabled = False
                Await ReloadVideo(filePath.ToString())
                CaptureButton.IsEnabled = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'CaptureButton' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub CaptureVideo_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            rootPage.NotifyUser("", NotifyType.StatusMessage)

            ' Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
            Dim dialog As New CameraCaptureUI()
            dialog.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4

            Dim file As StorageFile = Await dialog.CaptureFileAsync(CameraCaptureUIMode.Video)
            If file IsNot Nothing Then
                Dim fileStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
                CapturedVideo.SetSource(fileStream, "video/mp4")
                ResetButton.Visibility = Visibility.Visible

                ' Store the file path in Application Data
                appSettings(videoKey) = file.Path
            Else
                rootPage.NotifyUser("No video captured.", NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Reset' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Reset_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ResetButton.Visibility = Visibility.Collapsed
        CapturedVideo.Source = Nothing

        ' Clear file path in Application Data
        appSettings.Remove(videoKey)
    End Sub

    ''' <summary>
    ''' Loads the video from file path
    ''' </summary>
    ''' <param name="filePath">The path to load the video from</param>
    Private Async Function ReloadVideo(ByVal filePath As String) As Task
        Try
            Dim file As StorageFile = Await StorageFile.GetFileFromPathAsync(filePath)
            Dim fileStream As IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
            CapturedVideo.SetSource(fileStream, "video/mp4")
            ResetButton.Visibility = Visibility.Visible
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch ex As Exception
            appSettings.Remove(videoKey)
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Function
End Class
