'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Activation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Imaging
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Automation
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Controls.Primitives
Imports SDKTemplate
Imports System
Imports System.Threading.Tasks

''' <summary>
''' A page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ImagingTransforms
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' Exception HResult constants
    ' This file format does not support the requested operation; for example, metadata or thumbnails.
    Const ERR_OPERATION_UNSUPPORTED As Integer = -2003292287  ' 0x88982F81
    ' This file format does not support the requested property/metadata query.
    Const ERR_PROPERTY_NOT_SUPPORTED As Integer = -2003292351 ' 0x88982F41
    ' There is no codec or component that can handle the requested operation; for example, encoding.
    Const ERR_COMPONENT_NOT_FOUND As Integer = -2003292336    ' 0x88982F50
    Private m_futureAccess As StorageItemAccessList = StorageApplicationPermissions.FutureAccessList
    Private m_localSettings As IPropertySet = Windows.Storage.ApplicationData.Current.LocalSettings.Values
    Private m_transform As New RotateTransform()
    Private m_fileToken As String
    Private m_displayWidthNonScaled As UInteger
    Private m_displayHeightNonScaled As UInteger
    Private m_scaleFactor As Double
    Private m_userRotation As PhotoOrientation
    Private m_exifOrientation As PhotoOrientation
    Private m_disableExifOrientation As Boolean

    ' A pointer back to the main page.
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        AddHandler App.Current.Suspending, AddressOf SaveDataToPersistedState

        ' Reset scenario state before starting.
        ResetSessionState()

        ' Attempt to load the previous scenario state.
        If (m_localSettings.ContainsKey("scenario2FileToken")) Then
            RestoreDataFromPersistedState()
        End If
    End Sub

    ''' <summary>
    ''' When the application is about to enter a suspended state, save the user edited properties text.
    ''' This method does not use the SuspensionManager helper class (defined in SuspensionManager.cs).
    ''' </summary>
    Private Sub SaveDataToPersistedState(sender As Object, args As SuspendingEventArgs)
        ' Only save state if we have valid data.
        If (m_fileToken <> Nothing) Then
            ' Requesting a deferral prevents the application from being immediately suspended.
            Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral()

            ' LocalSettings does not support overwriting existing items, so first clear the collection.
            ResetPersistedState()

            m_localSettings.Add("scenario2FileToken", m_fileToken)
            m_localSettings.Add("scenario2Width", m_displayWidthNonScaled)
            m_localSettings.Add("scenario2Height", m_displayHeightNonScaled)
            m_localSettings.Add("scenario2Scale", m_scaleFactor)
            m_localSettings.Add("scenario2UserRotation", Helpers.ConvertToExifOrientationFlag(m_userRotation))

            m_localSettings.Add("scenario2ExifOrientation", Helpers.ConvertToExifOrientationFlag(m_exifOrientation))

            m_localSettings.Add("scenario2DisableExif", m_disableExifOrientation)

            deferral.Complete()
        End If
    End Sub

    ''' <summary>
    ''' Reads the file token and image transform variables from the persisted state and 
    ''' restores the UI.
    ''' </summary>
    Private Async Sub RestoreDataFromPersistedState()
        Try
            rootPage.NotifyUser("Loading image file from persisted state...", NotifyType.StatusMessage)

            m_fileToken = DirectCast(m_localSettings("scenario2FileToken"), String)
            m_displayWidthNonScaled = CUInt(m_localSettings("scenario2Width"))
            m_displayHeightNonScaled = CUInt(m_localSettings("scenario2Height"))
            m_scaleFactor = CDbl(m_localSettings("scenario2Scale"))

            Dim desiredOrientation As PhotoOrientation = Helpers.ConvertToPhotoOrientation(CUShort(m_localSettings("scenario2UserRotation")))

            m_exifOrientation = Helpers.ConvertToPhotoOrientation(CUShort(m_localSettings("scenario2ExifOrientation")))

            m_disableExifOrientation = CBool(m_localSettings("scenario2DisableExif"))

            ' Display the image in the UI.
            Dim file As StorageFile = Await m_futureAccess.GetFileAsync(m_fileToken)
            Dim src As New BitmapImage()
            src.SetSource(Await file.OpenAsync(FileAccessMode.Read))
            Image1.Source = src
            AutomationProperties.SetName(Image1, file.Name)

            ' Display the image dimensions and transformation state in the UI.
            ExifOrientationTextblock.Text = Helpers.GetOrientationString(m_exifOrientation)
            ScaleSlider.Value = m_scaleFactor * 100
            UpdateImageDimensionsUI()

            ' Restore the image tag's rotation transform.
            While desiredOrientation <> m_userRotation
                RotateRight_Click(Nothing, Nothing)
            End While

            RotateRightButton.IsEnabled = True
            RotateLeftButton.IsEnabled = True
            SaveButton.IsEnabled = True
            CloseButton.IsEnabled = True
            SaveAsButton.IsEnabled = True
            ScaleSlider.IsEnabled = True
            rootPage.NotifyUser("Loaded image file from persisted state: " & file.Name, NotifyType.StatusMessage)
        Catch err As Exception
            rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            ResetSessionState()
            ResetPersistedState()
        End Try
    End Sub

    ''' <summary>
    ''' Load an image from disk and display properties. Invoked when the user clicks on the Open button.
    ''' </summary>
    Private Async Sub Open_Click(sender As Object, e As RoutedEventArgs)
        ResetPersistedState()
        ResetSessionState()

        Try
            rootPage.NotifyUser("Opening image file...", NotifyType.StatusMessage)

            Dim file As StorageFile = Await Helpers.GetFileFromOpenPickerAsync()

            ' Request persisted access permissions to the file the user selected.
            ' This allows the app to directly load the file in the future without relying on a
            ' broker such as the file picker.
            m_fileToken = m_futureAccess.Add(file)

            ' Display the image in the UI.
            Dim src As New BitmapImage()
            src.SetSource(Await file.OpenAsync(FileAccessMode.Read))
            Image1.Source = src
            AutomationProperties.SetName(Image1, file.Name)

            ' Use BitmapDecoder to attempt to read EXIF orientation and image dimensions.
            Await GetImageInformationAsync(file)

            ExifOrientationTextblock.Text = Helpers.GetOrientationString(m_exifOrientation)
            UpdateImageDimensionsUI()

            ScaleSlider.IsEnabled = True
            RotateLeftButton.IsEnabled = True
            RotateRightButton.IsEnabled = True
            SaveButton.IsEnabled = True
            SaveAsButton.IsEnabled = True
            CloseButton.IsEnabled = True
            rootPage.NotifyUser("Loaded file from picker: " & file.Name, NotifyType.StatusMessage)
        Catch err As Exception
            rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            ResetSessionState()
            ResetPersistedState()
        End Try
    End Sub

    ''' <summary>
    ''' Asynchronously attempts to get the oriented dimensions and EXIF orientation from the image file.
    ''' Sets member variables instead of returning a value with the Task.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Private Async Function GetImageInformationAsync(file As StorageFile) As task
        Using stream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
            Dim decoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(stream)

            ' The orientedPixelWidth and Height members provide the image dimensions
            ' reflecting any EXIF orientation.
            m_displayHeightNonScaled = decoder.OrientedPixelHeight
            m_displayWidthNonScaled = decoder.OrientedPixelWidth

            Try
                ' Property access using BitmapProperties is similar to using
                ' Windows.Storage.FileProperties (see Scenario 1); BitmapProperties accepts
                ' property keys such as "System.Photo.Orientation".
                ' The EXIF orientation flag can be also be read using native metadata queries
                ' such as "/app1/ifd/{ushort=274}" (JPEG) or "/ifd/{ushort=274}" (TIFF).
                Dim requestedProperties As String() = {"System.Photo.Orientation"}
                Dim retrievedProperties As BitmapPropertySet = Await decoder.BitmapProperties.GetPropertiesAsync(requestedProperties)

                ' Check to see if the property exists in the file.
                If retrievedProperties.ContainsKey("System.Photo.Orientation") Then
                    ' EXIF orientation ("System.Photo.Orientation") is stored as a 16-bit unsigned integer.
                    m_exifOrientation = Helpers.ConvertToPhotoOrientation(CUShort(retrievedProperties("System.Photo.Orientation").Value))
                End If
            Catch err As Exception
                Select Case err.HResult
                    ' If the file format does not support properties continue without applying EXIF orientation.
                    Case ERR_OPERATION_UNSUPPORTED, ERR_PROPERTY_NOT_SUPPORTED
                        m_disableExifOrientation = True
                        Exit Select
                    Case Else
                        Throw err
                End Select
            End Try
        End Using
    End Function

    ''' <summary>
    ''' When the user clicks Rotate Right, rotate the ImageViewbox by 90 degrees clockwise,
    ''' and update the dimensions.
    ''' </summary>
    Private Sub RotateRight_Click(sender As Object, e As RoutedEventArgs)
        m_userRotation = Helpers.Add90DegreesCW(m_userRotation)

        ' Swap width and height.
        Dim temp As UInteger = m_displayHeightNonScaled
        m_displayHeightNonScaled = m_displayWidthNonScaled
        m_displayWidthNonScaled = temp

        UpdateImageDimensionsUI()
        UpdateImageRotation(m_userRotation)
    End Sub

    ''' <summary>
    ''' When the user clicks Rotate Left, rotate the ImageViewbox by 90 degrees counterclockwise,
    ''' and update the dimensions.
    ''' </summary>
    Private Sub RotateLeft_Click(sender As Object, e As RoutedEventArgs)
        m_userRotation = Helpers.Add90DegreesCCW(m_userRotation)

        ' Swap width and height.
        Dim temp As UInteger = m_displayHeightNonScaled
        m_displayHeightNonScaled = m_displayWidthNonScaled
        m_displayWidthNonScaled = temp

        UpdateImageDimensionsUI()
        UpdateImageRotation(m_userRotation)
    End Sub

    ''' <summary>
    ''' Applies the user-provided scale and rotation operation to the original image file.
    ''' The "Transcoding" mode is used which preserves image metadata and performs other
    ''' optimizations when possible.
    ''' 
    ''' This method attempts to perform "soft" rotation using the EXIF orientation flag when possible,
    ''' but falls back to a hard rotation of the image pixels. The BitmapEncoder and
    ''' stream objects are closed in this method and cannot be used again.
    ''' </summary>
    Private Async Sub Save_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.NotifyUser("Saving file...", NotifyType.StatusMessage)

            ' Create a new encoder and initialize it with data from the original file.
            ' The encoder writes to an in-memory stream, we then copy the contents to the file.
            ' This allows the application to perform in-place editing of the file: any unedited data
            ' is copied to the destination, and the original file is overwritten
            ' with updated data.
            Dim file As StorageFile = Await m_futureAccess.GetFileAsync(m_fileToken)
            Using fileStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.ReadWrite), memStream As IRandomAccessStream = New InMemoryRandomAccessStream()
                Dim decoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(fileStream)

                ' Use the native (no orientation applied) image dimensions because we want to handle
                ' orientation ourselves.
                Dim originalWidth As UInteger = decoder.PixelWidth
                Dim originalHeight As UInteger = decoder.PixelHeight

                ' Set the encoder's destination to the temporary, in-memory stream.
                Dim encoder As BitmapEncoder = Await BitmapEncoder.CreateForTranscodingAsync(memStream, decoder)

                ' Scaling occurs before flip/rotation, therefore use the original dimensions
                ' (no orientation applied) as parameters for scaling.
                ' Dimensions are rounded down by BitmapEncoder to the nearest integer.
                If m_scaleFactor <> 1.0 Then
                    encoder.BitmapTransform.ScaledWidth = CUInt(originalWidth * m_scaleFactor)
                    encoder.BitmapTransform.ScaledHeight = CUInt(originalHeight * m_scaleFactor)
                End If

                ' If the file format supports EXIF orientation ("System.Photo.Orientation") then
                ' update the orientation flag to reflect any user-specified rotation.
                ' Otherwise, perform a hard rotate using BitmapTransform.
                If m_disableExifOrientation = False Then
                    Dim properties As New BitmapPropertySet()
                    Dim netExifOrientation As UShort = Helpers.ConvertToExifOrientationFlag(Helpers.AddPhotoOrientation(m_userRotation, m_exifOrientation))

                    ' BitmapProperties requires the application to explicitly declare the type
                    ' of the property to be written - this is different from FileProperties which
                    ' automatically coerces the value to the correct type. System.Photo.Orientation
                    ' is defined as an unsigned 16 bit integer.
                    Dim orientationTypedValue As New BitmapTypedValue(netExifOrientation, Windows.Foundation.PropertyType.UInt16)

                    properties.Add("System.Photo.Orientation", orientationTypedValue)
                    Await encoder.BitmapProperties.SetPropertiesAsync(properties)
                Else
                    encoder.BitmapTransform.Rotation = Helpers.ConvertToBitmapRotation(m_userRotation)
                End If

                ' Attempt to generate a new thumbnail to reflect any rotation operation.
                encoder.IsThumbnailGenerated = True

                Try
                    Await encoder.FlushAsync()
                Catch err As Exception
                    Select Case err.HResult
                        Case ERR_OPERATION_UNSUPPORTED
                            ' If the encoder does not support writing a thumbnail, then try again
                            ' but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = False
                            Exit Select
                        Case Else
                            Throw err
                    End Select
                End Try

                If encoder.IsThumbnailGenerated = False Then
                    Await encoder.FlushAsync()
                End If

                ' Now that the file has been written to the temporary stream, copy the data to the file.
                memStream.Seek(0)
                fileStream.Seek(0)
                fileStream.Size = 0
                Await RandomAccessStream.CopyAsync(memStream, fileStream)

                RotateRightButton.IsEnabled = False
                RotateLeftButton.IsEnabled = False
                SaveButton.IsEnabled = False
                SaveAsButton.IsEnabled = False
                CloseButton.IsEnabled = False
                rootPage.NotifyUser("Successfully saved file: " & file.Name, NotifyType.StatusMessage)
            End Using
        Catch err As Exception
            If err.HResult = ERR_COMPONENT_NOT_FOUND Then
                ' Some image formats (e.g. ICO) do not have encoders.
                rootPage.NotifyUser("Error: this file format may not support editing.", NotifyType.ErrorMessage)
            Else
                rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            End If
            ResetPersistedState()
            ResetSessionState()
        End Try
    End Sub

    ''' <summary>
    ''' Applies the user-provided scale and rotation operation to a new image file picked by the user.
    ''' This method writes the edited pixel data to the new file without
    ''' any regard to existing metadata or other information in the original file.
    ''' 
    ''' The BitmapEncoder and stream objects are closed in this method and cannot be used again.
    ''' </summary>
    Private Async Sub SaveAs_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.NotifyUser("Saving to a new file...", NotifyType.StatusMessage)

            Dim inputFile As StorageFile = Await m_futureAccess.GetFileAsync(m_fileToken)
            Dim outputFile As StorageFile = Await Helpers.GetFileFromSavePickerAsync()
            Dim encoderId As Guid

            Select Case outputFile.FileType
                Case ".png"
                    encoderId = BitmapEncoder.PngEncoderId
                    Exit Select
                Case ".bmp"
                    encoderId = BitmapEncoder.BmpEncoderId
                    Exit Select
                Case ".jpg"
                    encoderId = BitmapEncoder.JpegEncoderId
                    Exit Select
                Case Else
                    encoderId = BitmapEncoder.JpegEncoderId
                    Exit Select
            End Select

            Using inputStream As IRandomAccessStream = Await inputFile.OpenAsync(FileAccessMode.Read), outputStream As IRandomAccessStream = Await outputFile.OpenAsync(FileAccessMode.ReadWrite)
                ' Get pixel data from the decoder. We apply the user-requested transforms on the
                ' decoded pixels to take advantage of potential optimizations in the decoder.
                Dim decoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(inputStream)
                Dim transform As New BitmapTransform()

                ' Note that we are requesting the oriented pixel dimensions, and not applying
                ' EXIF orientation in the BitmapTransform. We will request oriented pixel data
                ' later in the BitmapDecoder.GetPixelDataAsync() call.
                transform.ScaledHeight = CUInt(decoder.OrientedPixelHeight * m_scaleFactor)
                transform.ScaledWidth = CUInt(decoder.OrientedPixelWidth * m_scaleFactor)
                transform.Rotation = Helpers.ConvertToBitmapRotation(m_userRotation)

                ' The BitmapDecoder indicates what pixel format and alpha mode best match the
                ' natively stored image data. This can provide a performance and/or quality gain.
                Dim format As BitmapPixelFormat = decoder.BitmapPixelFormat
                Dim alpha As BitmapAlphaMode = decoder.BitmapAlphaMode

                Dim pixelProvider As PixelDataProvider = Await decoder.GetPixelDataAsync(format, alpha, Transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb)

                Dim pixels As Byte() = pixelProvider.DetachPixelData()

                ' Write the pixel data onto the encoder. Note that we can't simply use the
                ' BitmapTransform.ScaledWidth and ScaledHeight members as the user may have
                ' requested a rotation (which is applied after scaling).
                Dim encoder As BitmapEncoder = Await BitmapEncoder.CreateAsync(encoderId, outputStream)
                encoder.SetPixelData(format, alpha, CUInt(CDbl(m_displayWidthNonScaled) * m_scaleFactor), CUInt(CDbl(m_displayHeightNonScaled) * m_scaleFactor), decoder.DpiX, decoder.DpiY, _
                    pixels)

                Await encoder.FlushAsync()

                RotateRightButton.IsEnabled = False
                RotateLeftButton.IsEnabled = False
                SaveButton.IsEnabled = False
                SaveAsButton.IsEnabled = False
                CloseButton.IsEnabled = False
                rootPage.NotifyUser("Successfully saved file: " & outputFile.Name, NotifyType.StatusMessage)
            End Using
        Catch err As Exception
            rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            ResetPersistedState()
            ResetSessionState()
        End Try
    End Sub

    Private Sub ScaleSlider_ValueChanged(sender As Object, e As Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs)
        ' The ValueChanged event is first fired before all controls are constructed; suppress any actions until 
        If ScaleTextblock IsNot Nothing Then
            ' The slider control uses percentages (0 to 100) while m_scaleFactor is a coefficient (0 to 1).
            m_scaleFactor = e.NewValue / 100
            UpdateImageDimensionsUI()
        End If
    End Sub

    ''' <summary>
    ''' Display the image dimensions and transformation state in the UI.
    ''' </summary>
    Private Sub UpdateImageDimensionsUI()
        ScaleTextblock.Text = (m_scaleFactor * 100).ToString & "%"
        WidthTextblock.Text = Math.Floor((CDbl(m_displayWidthNonScaled) * m_scaleFactor)).ToString & " pixels"
        HeightTextblock.Text = Math.Floor((CDbl(m_displayHeightNonScaled) * m_scaleFactor)).ToString & " pixels"
        UserRotationTextblock.Text = Helpers.GetOrientationString(m_userRotation)
    End Sub

    ''' <summary>
    ''' Sets the RotationTransform angle of the Image (technically, its parent Viewbox).
    ''' </summary>
    Private Sub UpdateImageRotation(rotation As PhotoOrientation)
        Select Case rotation
            Case PhotoOrientation.Rotate270
                ' Note that the PhotoOrientation enumeration uses a counterclockwise convention,
                ' while the RotationTransform uses a clockwise convention.
                m_transform.Angle = 90
                Exit Select
            Case PhotoOrientation.Rotate180
                m_transform.Angle = 180
                Exit Select
            Case PhotoOrientation.Rotate90
                m_transform.Angle = 270
                Exit Select
            Case PhotoOrientation.Normal
                m_transform.Angle = 0
                Exit Select
            Case Else
                m_transform.Angle = 0
                Exit Select
        End Select
    End Sub

    ''' <summary>
    ''' Closing the file brings the scenario back to the default initialized state.
    ''' </summary>
    Private Sub Close_Click(sender As Object, e As RoutedEventArgs)
        ResetSessionState()
        ResetPersistedState()
    End Sub

    ''' <summary>
    ''' Clear all of the state for this scenario that is stored in app data.
    ''' This method does not use the SuspensionManager helper class (defined in SuspensionManager.cs).
    ''' </summary>
    Private Sub ResetPersistedState()
        m_localSettings.Remove("scenario2FileToken")
        m_localSettings.Remove("scenario2Scale")
        m_localSettings.Remove("scenario2Rotation")
        m_localSettings.Remove("scenario2Width")
        m_localSettings.Remove("scenario2Height")
        m_localSettings.Remove("scenario2UserRotation")
        m_localSettings.Remove("scenario2ExifOrientation")
        m_localSettings.Remove("scenario2DisableExif")
    End Sub

    ''' <summary>
    ''' Clear all of the state that is stored in memory and in the UI
    ''' </summary>
    Private Async Sub ResetSessionState()
        m_fileToken = Nothing
        m_displayHeightNonScaled = 0
        m_displayWidthNonScaled = 0
        m_scaleFactor = 1
        m_userRotation = PhotoOrientation.Normal
        m_exifOrientation = PhotoOrientation.Normal
        m_disableExifOrientation = False

        RotateLeftButton.IsEnabled = False
        RotateRightButton.IsEnabled = False
        SaveButton.IsEnabled = False
        SaveAsButton.IsEnabled = False
        CloseButton.IsEnabled = False

        Dim placeholderImage As StorageFile = Await Package.Current.InstalledLocation.GetFileAsync("Assets\placeholder-sdk.png")
        Dim bitmapImage As New BitmapImage()
        bitmapImage.SetSource(Await placeholderImage.OpenAsync(FileAccessMode.Read))
        Image1.Source = bitmapImage
        AutomationProperties.SetName(Image1, "A placeholder image")

        m_transform.CenterX = ImageViewbox.Width / 2
        m_transform.CenterY = ImageViewbox.Height / 2
        ImageViewbox.RenderTransform = m_transform
        UpdateImageRotation(PhotoOrientation.Normal)

        ScaleTextblock.Text = ""
        ScaleSlider.Value = 100
        ScaleSlider.IsEnabled = False
        HeightTextblock.Text = ""
        WidthTextblock.Text = ""
        UserRotationTextblock.Text = ""
        ExifOrientationTextblock.Text = ""
    End Sub
End Class
