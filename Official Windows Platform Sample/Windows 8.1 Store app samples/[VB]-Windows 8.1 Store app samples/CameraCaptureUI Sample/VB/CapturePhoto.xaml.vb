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
Imports Windows.UI.Xaml.Media.Imaging

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class CapturePhoto
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private appSettings As Windows.Foundation.Collections.IPropertySet
    Private Const photoKey As String = "capturedPhoto"

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
        ' Reload previously taken photo
        If appSettings.ContainsKey(photoKey) Then
            Dim filePath As Object = Nothing
            If appSettings.TryGetValue(photoKey, filePath) AndAlso filePath.ToString() <> "" Then
                CaptureButton.IsEnabled = False
                Await ReloadPhoto(filePath.ToString())
                CaptureButton.IsEnabled = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'CaptureButton' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub CapturePhoto_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            rootPage.NotifyUser("", NotifyType.StatusMessage)

            ' Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
            Dim dialog As New CameraCaptureUI()
            Dim aspectRatio As New Size(16, 9)
            dialog.PhotoSettings.CroppedAspectRatio = aspectRatio

            Dim file As StorageFile = Await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo)
            If file IsNot Nothing Then
                Dim bitmapImage As New BitmapImage()
                Using fileStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
                    bitmapImage.SetSource(fileStream)
                End Using
                CapturedPhoto.Source = bitmapImage
                ResetButton.Visibility = Visibility.Visible

                ' Store the file path in Application Data
                appSettings(photoKey) = file.Path
            Else
                rootPage.NotifyUser("No photo captured.", NotifyType.StatusMessage)
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
        CapturedPhoto.Source = New BitmapImage(New Uri(Me.BaseUri, "Assets/placeholder-sdk.png"))

        ' Clear file path in Application Data
        appSettings.Remove(photoKey)
    End Sub

    ''' <summary>
    ''' Loads the photo from file path
    ''' </summary>
    ''' <param name="filePath">The path to load the photo from</param>
    Private Async Function ReloadPhoto(ByVal filePath As String) As Task
        Try
            Dim file As StorageFile = Await StorageFile.GetFileFromPathAsync(filePath)
            Dim bitmapImage As New BitmapImage()
            Using fileStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
                bitmapImage.SetSource(fileStream)
            End Using
            CapturedPhoto.Source = bitmapImage
            ResetButton.Visibility = Visibility.Visible
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch ex As Exception
            appSettings.Remove(photoKey)
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Function
End Class
