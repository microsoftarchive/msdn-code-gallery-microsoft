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
Imports Windows.Foundation
Imports Windows.Graphics.Imaging
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.Storage.Pickers
Imports Windows.ApplicationModel.DataTransfer
Imports SDKTemplate

Partial Public NotInheritable Class ShareDelayRenderedFiles
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current
    Private dataTransferManager As DataTransferManager
    Private imageFile As StorageFile

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Register this page as a share source.
        Me.dataTransferManager = dataTransferManager.GetForCurrentView()
        AddHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ' Unregister this page as a share source.
        RemoveHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Private Sub OnDataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        If imageFile IsNot Nothing Then
            Dim requestData As DataPackage = e.Request.Data
            requestData.Properties.Title = "Delay rendered image"
            requestData.Properties.Description = "Resized image from the Share Source sample"
            requestData.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(imageFile)
            requestData.SetDataProvider(StandardDataFormats.Bitmap, AddressOf Me.OnDeferredImageRequestedHandler)
        Else
            e.Request.FailWithDisplayText("Select an image you would like to share and try again.")
        End If
    End Sub

    Private Async Sub OnDeferredImageRequestedHandler(request As DataProviderRequest)
        ' Here we provide updated Bitmap data using delayed rendering.

        If Me.imageFile IsNot Nothing Then
            Dim deferral As DataProviderDeferral = request.GetDeferral()
            Dim inMemoryStream As New InMemoryRandomAccessStream()

            ' Decode the image and re-encode it at 50% width and height.
            Dim imageStream As IRandomAccessStream = Await Me.imageFile.OpenAsync(FileAccessMode.Read)
            Dim imageDecoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(imageStream)
            Dim imageEncoder As BitmapEncoder = Await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder)
            imageEncoder.BitmapTransform.ScaledWidth = CUInt(imageDecoder.OrientedPixelWidth * 0.5)
            imageEncoder.BitmapTransform.ScaledHeight = CUInt(imageDecoder.OrientedPixelHeight * 0.5)
            Await imageEncoder.FlushAsync()

            request.SetData(RandomAccessStreamReference.CreateFromStream(inMemoryStream))
            deferral.Complete()
        End If
    End Sub

    Private Async Sub SelectImageButton_Click(sender As Object, e As RoutedEventArgs)
        Dim imagePicker As New FileOpenPicker() With { _
            .ViewMode = PickerViewMode.Thumbnail, _
            .SuggestedStartLocation = PickerLocationId.PicturesLibrary
        }

        imagePicker.FileTypeFilter.Add(".jpg")
        imagePicker.FileTypeFilter.Add(".png")
        imagePicker.FileTypeFilter.Add(".bmp")
        imagePicker.FileTypeFilter.Add(".gif")
        imagePicker.FileTypeFilter.Add(".tif")
        Dim pickedImage As StorageFile = Await imagePicker.PickSingleFileAsync()

        If pickedImage IsNot Nothing Then
            Me.imageFile = pickedImage

            ' Display the image in the UI.
            Dim displayStream As IRandomAccessStream = Await pickedImage.OpenAsync(FileAccessMode.Read)
            Dim bitmapImage As New BitmapImage()
            bitmapImage.SetSource(displayStream)
            ImageHolder.Source = bitmapImage
            Me.rootPage.NotifyUser("Selected " & pickedImage.Name & ".", NotifyType.StatusMessage)

            ShareStep.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub ShowUIButton_Click(sender As Object, e As RoutedEventArgs)
        dataTransferManager.ShowShareUI()
    End Sub
End Class
