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
Imports Windows.Storage.Pickers
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Media.Imaging

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler Scenario2Button1.Click, AddressOf Scenario2Button1_Click
        Scenario2DecodePixelHeight.Text = "100"
        Scenario2DecodePixelWidth.Text = "100"
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub Scenario2Button1_Click(sender As Object, e As RoutedEventArgs) 
        Dim decodePixelHeight As Integer
        Dim decodePixelWidth As Integer

        ' Try to parse an integer from the given text. If invalid, default to 100px
        If Not Integer.TryParse(Scenario2DecodePixelHeight.Text, decodePixelHeight) Then
            Scenario2DecodePixelHeight.Text = "100"
            decodePixelHeight = 100
        End If

        ' Try to parse an integer from the given text. If invalid, default to 100px
        If Not Integer.TryParse(Scenario2DecodePixelWidth.Text, decodePixelWidth) Then
            Scenario2DecodePixelWidth.Text = "100"
            decodePixelWidth = 100
        End If

        Dim open As New FileOpenPicker()
        open.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        open.ViewMode = PickerViewMode.Thumbnail

        ' Filter to include a sample subset of file types
        open.FileTypeFilter.Clear()
        open.FileTypeFilter.Add(".bmp")
        open.FileTypeFilter.Add(".png")
        open.FileTypeFilter.Add(".jpeg")
        open.FileTypeFilter.Add(".jpg")

        ' Open a stream for the selected file
        Dim file As StorageFile = Await open.PickSingleFileAsync()
        If file IsNot Nothing Then
            Dim fileStream As IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)

            ' Set the image source to the selected bitmap
            Dim bitmapImage As New BitmapImage()
            bitmapImage.DecodePixelHeight = decodePixelHeight
            bitmapImage.DecodePixelWidth = decodePixelWidth

            Await bitmapImage.SetSourceAsync(fileStream)
            Scenario2Image.Source = bitmapImage
        End If
    End Sub

End Class
