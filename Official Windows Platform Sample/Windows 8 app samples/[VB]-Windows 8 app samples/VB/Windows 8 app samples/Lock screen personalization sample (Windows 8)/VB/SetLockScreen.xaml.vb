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
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.Storage.Pickers
Imports Windows.Storage
Imports System.Threading.Tasks
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.Storage.Streams
Imports Windows.System.UserProfile

Partial Public NotInheritable Class SetLockScreen
    Inherits SDKTemplate.Common.LayoutAwarePage
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
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub PickAndSetButton_Click(sender As Object, e As RoutedEventArgs)
        Dim imagePicker As New FileOpenPicker() With { _
            .ViewMode = PickerViewMode.Thumbnail, _
            .SuggestedStartLocation = PickerLocationId.PicturesLibrary
        }

        imagePicker.FileTypeFilter.Add(".jpg")
        imagePicker.FileTypeFilter.Add(".jpeg")
        imagePicker.FileTypeFilter.Add(".png")
        imagePicker.FileTypeFilter.Add(".bmp")

        Dim imageFile As StorageFile = Await imagePicker.PickSingleFileAsync()
        If imageFile IsNot Nothing Then
            ' Application now has access to the picked file, setting image to lockscreen.
            Await LockScreen.SetImageFileAsync(imageFile)

            Try
                ' Retrieve the lock screen image that was set
                Dim imageStream As IRandomAccessStream = LockScreen.GetImageStream()

                If imageStream IsNot Nothing Then
                    Dim lockScreen__1 As New BitmapImage()
                    lockScreen__1.SetSource(imageStream)
                    LockScreenImage.Source = lockScreen__1
                    LockScreenImage.Visibility = Visibility.Visible
                Else
                    LockScreenImage.Visibility = Visibility.Collapsed
                    rootPage.NotifyUser("Setting the lock screen image failed.  Make sure your copy of Windows is activated.", NotifyType.StatusMessage)
                End If
            Catch ex As Exception
                rootPage.NotifyUser("Error opening stream: " & ex.ToString(), NotifyType.ErrorMessage)
            End Try
        Else
            LockScreenImage.Visibility = Visibility.Collapsed
            rootPage.NotifyUser("No file was selected using the picker.", NotifyType.StatusMessage)
        End If
    End Sub
End Class
