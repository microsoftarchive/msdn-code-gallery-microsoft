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
Imports SDKTemplate
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Media.Imaging

Partial Public NotInheritable Class ShareImage
    Inherits SDKTemplate.Common.SharePage

    Private imageFile As StorageFile

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        If Me.imageFile IsNot Nothing Then
            Dim requestData As DataPackage = request.Data
            requestData.Properties.Title = TitleInputBox.Text
            requestData.Properties.Description = DescriptionInputBox.Text ' The description is optional.
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink

            ' It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
            ' since the target app may only support one or the other.

            Dim imageItems As New List(Of IStorageItem)()
            imageItems.Add(Me.imageFile)
            requestData.SetStorageItems(imageItems)

            Dim imageStreamRef As RandomAccessStreamReference = RandomAccessStreamReference.CreateFromFile(Me.imageFile)
            requestData.Properties.Thumbnail = imageStreamRef
            requestData.SetBitmap(imageStreamRef)
            succeeded = True
        Else
            request.FailWithDisplayText("Select an image you would like to share and try again.")
        End If
        Return succeeded
    End Function

    Private Async Sub SelectImageButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim imagePicker As FileOpenPicker = New FileOpenPicker With {.ViewMode = PickerViewMode.Thumbnail, .SuggestedStartLocation = PickerLocationId.PicturesLibrary}
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
End Class
