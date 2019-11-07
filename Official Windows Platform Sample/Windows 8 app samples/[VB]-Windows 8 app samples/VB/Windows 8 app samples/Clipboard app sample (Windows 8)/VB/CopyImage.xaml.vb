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
Imports System.Threading.Tasks
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Graphics.Imaging
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class CopyImage
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private rootPage As Global.SDKTemplate.MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Me.Init()
    End Sub

#Region "Scenario Specific Code"

    Private Sub Init()
        AddHandler CopyButton.Click, AddressOf CopyButton_Click
        AddHandler PasteButton.Click, AddressOf PasteButton_Click
        AddHandler CopyWithDelayRenderingButton.Click, AddressOf CopyWithDelayRenderingButton_Click
    End Sub

#End Region

#Region "Button click"

    Private Sub CopyButton_Click(sender As Object, e As RoutedEventArgs)
        Me.CopyBitmap(False)
    End Sub

    Private Sub CopyWithDelayRenderingButton_Click(sender As Object, e As RoutedEventArgs)
        Me.CopyBitmap(True)
    End Sub

    Private Async Sub PasteButton_Click(sender As Object, e As RoutedEventArgs)
        ' Get the bitmap and display it.
        Dim dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent()
        If dataPackageView.Contains(StandardDataFormats.Bitmap) Then
            Dim imageReceived = Await dataPackageView.GetBitmapAsync()
            If imageReceived IsNot Nothing Then
                Dim imageStream = Await imageReceived.OpenReadAsync()
                If imageStream IsNot Nothing Then
                    Dim bitmapImage = New BitmapImage()
                    bitmapImage.SetSource(imageStream)
                    ImageHolder.Source = bitmapImage
                    ImageHolder.Visibility = Visibility.Visible
                    OutputText.Text = "Image is retrieved from the clipboard and pasted successfully."
                Else
                    OutputText.Text = "Error pasting image, the IRandomAccessStream retrieved from DataPackageView is null"
                End If
            Else
                OutputText.Text = "Error pasting image, the IRandomAccessStreamReference retrieved from DataPackageView is null"
            End If
        Else
            OutputText.Text = "No available image in clipboard"
            ImageHolder.Visibility = Visibility.Collapsed
        End If
    End Sub

#End Region

#Region "private helper methods"

    Private Async Sub OnDeferredImageRequestedHandler(request As DataProviderRequest)
        If Me.imageFile IsNot Nothing Then
            ' Since this method is using "await" prior to setting the data in DataPackage,
            ' deferral object must be used
            Dim deferral = request.GetDeferral()

            Dim imageStream = Await Me.imageFile.OpenAsync(FileAccessMode.Read)

            ' Decode the image
            Dim imageDecoder = Await BitmapDecoder.CreateAsync(imageStream)

            ' Re-encode the image at 50% width and height
            Dim inMemoryStream = New InMemoryRandomAccessStream()
            Dim imageEncoder = Await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder)
            imageEncoder.BitmapTransform.ScaledWidth = CUInt(imageDecoder.OrientedPixelWidth * 0.5)
            imageEncoder.BitmapTransform.ScaledHeight = CUInt(imageDecoder.OrientedPixelHeight * 0.5)
            Await imageEncoder.FlushAsync()

            request.SetData(RandomAccessStreamReference.CreateFromStream(inMemoryStream))

            Await log(OutputText, "Image has been set via deferral")

            ' data has been set, now we need to signal completion of the operation
            deferral.Complete()
        Else
            Await log(OutputText, "Error: imageFile is null")
        End If
    End Sub

    Private Async Sub CopyBitmap(isDelayRendered As Boolean)
        Dim imagePicker = New FileOpenPicker() With {.ViewMode = PickerViewMode.Thumbnail, .SuggestedStartLocation = PickerLocationId.PicturesLibrary}
        imagePicker.FileTypeFilter.Add(".jpg")
        imagePicker.FileTypeFilter.Add(".png")
        imagePicker.FileTypeFilter.Add(".bmp")
        imagePicker.FileTypeFilter.Add(".gif")
        imagePicker.FileTypeFilter.Add(".tif")

        Dim imageFile = Await imagePicker.PickSingleFileAsync()
        If imageFile IsNot Nothing Then
            Dim dataPackage = New DataPackage()

            ' Use one click handler for two operations: regular copy and copy using delayed rendering
            ' Differentiate the case by the button name
            If isDelayRendered Then
                Me.imageFile = imageFile
                dataPackage.SetDataProvider(StandardDataFormats.Bitmap, AddressOf OnDeferredImageRequestedHandler)
                OutputText.Text = "Image has been copied using delayed rendering"
            Else
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromFile(imageFile))
                OutputText.Text = "Image has been copied"
            End If
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage)
        Else
            OutputText.Text = "No image was selected."
        End If
    End Sub

    Private Async Function log(textBlock As TextBlock, msg As String) As Task
        ' This function should be called when a back-ground thread tries to output log to the UI
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     textBlock.Text &= Environment.NewLine & msg
                                                                 End Sub)
    End Function

#End Region

#Region "private member variables"

    Private imageFile As StorageFile = Nothing

#End Region
End Class
