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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Msappdata
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
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
    Dim appData As ApplicationData = ApplicationData.Current

        Try
            Dim sourceFile As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///assets/appDataLocal.png"))
            Await sourceFile.CopyAsync(appData.LocalFolder)
        Catch ex As Exception
    ' If the image has already been copied the CopyAsync() method above will fail.
    ' Ignore this error.
        End Try

        Try
            Dim sourceFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///assets/appDataRoaming.png"))
            Await sourceFile.CopyAsync(appData.RoamingFolder)
        Catch ex As Exception
    ' If the image has already been copied the CopyAsync() method above will fail.
    ' Ignore this error.
        End Try

        Try
            Dim sourceFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///assets/appDataTemp.png"))
            Await sourceFile.CopyAsync(appData.TemporaryFolder)
        Catch ex As Exception
    ' If the image has already been copied the CopyAsync() method above will fail.
    ' Ignore this error.
        End Try

        LocalImage.Source = New Windows.UI.Xaml.Media.Imaging.BitmapImage(New Uri("ms-appdata:///local/appDataLocal.png"))
        RoamingImage.Source = New Windows.UI.Xaml.Media.Imaging.BitmapImage(New Uri("ms-appdata:///roaming/appDataRoaming.png"))
        TempImage.Source = New Windows.UI.Xaml.Media.Imaging.BitmapImage(New Uri("ms-appdata:///temp/appDataTemp.png"))
    End Sub

    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        LocalImage.Source = Nothing
        RoamingImage.Source = Nothing
        TempImage.Source = Nothing
    End Sub
End Class
