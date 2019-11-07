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
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Graphics.Imaging
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Media.Imaging

Partial Public NotInheritable Class ShareDelayRenderedFiles
    Inherits SDKTemplate.Common.SharePage

    Private selectedImage As StorageFile

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        If Me.selectedImage IsNot Nothing Then
            Dim requestData As DataPackage = request.Data
            requestData.Properties.Title = "Delay rendered image"
            requestData.Properties.Description = "Resized image from the Share Source sample"
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink
            requestData.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(Me.selectedImage)
            requestData.SetDataProvider(StandardDataFormats.Bitmap, Sub(providerRequest) Me.OnDeferredImageRequestedHandler(providerRequest, Me.selectedImage))
            succeeded = True
        Else
            request.FailWithDisplayText("Select an image you would like to share and try again.")
        End If
        Return succeeded
    End Function

    Private Async Sub OnDeferredImageRequestedHandler(ByVal providerRequest As DataProviderRequest, ByVal imageFile As StorageFile)
        ' In this delegate we provide updated Bitmap data using delayed rendering.

        If imageFile IsNot Nothing Then
            ' If the delegate is calling any asynchronous operations it needs to acquire the deferral first. This lets the
            ' system know that you are performing some operations that might take a little longer and that the call to
            ' SetData could happen after the delegate returns. Once you acquired the deferral object you must call Complete
            ' on it after your final call to SetData.
            Dim deferral As DataProviderDeferral = providerRequest.GetDeferral()
            Dim inMemoryStream As New InMemoryRandomAccessStream()

            ' Make sure to always call Complete when done with the deferral.
            Try
                ' Decode the image and re-encode it at 50% width and height.
                Dim imageStream As IRandomAccessStream = Await imageFile.OpenAsync(FileAccessMode.Read)
                Dim imageDecoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(imageStream)
                Dim imageEncoder As BitmapEncoder = Await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder)
                imageEncoder.BitmapTransform.ScaledWidth = CUInt(imageDecoder.OrientedPixelWidth * 0.5)
                imageEncoder.BitmapTransform.ScaledHeight = CUInt(imageDecoder.OrientedPixelHeight * 0.5)
                Await imageEncoder.FlushAsync()

                providerRequest.SetData(RandomAccessStreamReference.CreateFromStream(inMemoryStream))
            Finally
                deferral.Complete()
            End Try
        End If
    End Sub

    Private Async Sub SelectImageButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim imagePicker As FileOpenPicker = New FileOpenPicker With {.ViewMode = PickerViewMode.Thumbnail, .SuggestedStartLocation = PickerLocationId.PicturesLibrary}
        imagePicker.FileTypeFilter.Add(".jpg")
        imagePicker.FileTypeFilter.Add(".png")
        imagePicker.FileTypeFilter.Add(".bmp")
        imagePicker.FileTypeFilter.Add(".gif")
        imagePicker.FileTypeFilter.Add(".tif")

        Dim pickedImage As StorageFile = Await imagePicker.PickSingleFileAsync()

        If pickedImage IsNot Nothing Then
            Me.selectedImage = pickedImage

            ' Display the image in the UI.
            Dim displayStream As IRandomAccessStream = Await pickedImage.OpenAsync(FileAccessMode.Read)
            Dim bitmapImage As New BitmapImage()
            bitmapImage.SetSource(displayStream)
            ImageHolder.Source = bitmapImage
            Me.rootPage.NotifyUser("Selected " & pickedImage.Name & ".", NotifyType.StatusMessage)

            ShareStep.Visibility = Visibility.Visible
        End If
    End Sub
End Class
