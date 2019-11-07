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
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.System.UserProfile

Partial Public NotInheritable Class GetAccountPicture
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Async Sub GetSmallImageButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' The small picture returned by GetAccountPicture() is 96x96 pixels in size.
        Dim image As StorageFile = TryCast(UserInformation.GetAccountPicture(AccountPictureKind.SmallImage), StorageFile)
        If image IsNot Nothing Then
            rootPage.NotifyUser("SmallImage path = " & image.Path, NotifyType.StatusMessage)

            Try
                Dim imageStream As IRandomAccessStream = Await image.OpenReadAsync()
                Dim bitmapImage As New BitmapImage()
                bitmapImage.SetSource(imageStream)
                smallImage.Source = bitmapImage

                smallImage.Visibility = Visibility.Visible
                largeImage.Visibility = Visibility.Collapsed
                mediaPlayer.Visibility = Visibility.Collapsed
            Catch ex As Exception
                rootPage.NotifyUser("Error opening stream: " & ex.ToString(), NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("Small Account Picture is not available", NotifyType.StatusMessage)
            mediaPlayer.Visibility = Visibility.Collapsed
            smallImage.Visibility = Visibility.Collapsed
            largeImage.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Async Sub GetLargeImageButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' The large picture returned by GetAccountPicture() is 448x448 pixels in size.
        Dim image As StorageFile = TryCast(UserInformation.GetAccountPicture(AccountPictureKind.LargeImage), StorageFile)
        If image IsNot Nothing Then
            rootPage.NotifyUser("LargeImage path = " & image.Path, NotifyType.StatusMessage)

            Try
                Dim imageStream As IRandomAccessStream = Await image.OpenReadAsync()
                Dim bitmapImage As New BitmapImage()
                bitmapImage.SetSource(imageStream)
                largeImage.Source = bitmapImage
                largeImage.Visibility = Visibility.Visible
                smallImage.Visibility = Visibility.Collapsed
                mediaPlayer.Visibility = Visibility.Collapsed
            Catch ex As Exception
                rootPage.NotifyUser("Error opening stream: " & ex.ToString(), NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("Large Account Picture is not available", NotifyType.StatusMessage)
            mediaPlayer.Visibility = Visibility.Collapsed
            smallImage.Visibility = Visibility.Collapsed
            largeImage.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Async Sub GetVideoButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' The video returned from getAccountPicture is 448x448 pixels in size.
        Dim video As StorageFile = TryCast(UserInformation.GetAccountPicture(AccountPictureKind.Video), StorageFile)
        If video IsNot Nothing Then
            rootPage.NotifyUser("Video path = " & video.Path, NotifyType.StatusMessage)

            Try
                Dim videoStream As IRandomAccessStream = Await video.OpenAsync(FileAccessMode.Read)

                mediaPlayer.SetSource(videoStream, "video/mp4")
                mediaPlayer.Visibility = Visibility.Visible
                smallImage.Visibility = Visibility.Collapsed
                largeImage.Visibility = Visibility.Collapsed
            Catch ex As Exception
                rootPage.NotifyUser("Error opening stream: " & ex.ToString(), NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("Video is not available", NotifyType.StatusMessage)
            mediaPlayer.Visibility = Visibility.Collapsed
            smallImage.Visibility = Visibility.Collapsed
            largeImage.Visibility = Visibility.Collapsed
        End If
    End Sub
End Class