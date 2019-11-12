'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media.Imaging
Imports SDKTemplate
Imports System
Imports System.IO
Imports System.Numerics
Imports System.Runtime.InteropServices.WindowsRuntime
Imports Windows.Graphics.Imaging
Imports Windows.Storage.Pickers
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.System.Threading

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private Scenario4WriteableBitmap As WriteableBitmap

    Public Sub New()
        Me.InitializeComponent()
        Scenario4WriteableBitmap = New WriteableBitmap(CInt(Scenario4ImageContainer.Width), CInt(Scenario4ImageContainer.Height))
        Scenario4Image.Source = Scenario4WriteableBitmap
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)

        ' Set the source of the WriteableBitmap to a placeholder image
        Dim file As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///Assets/placeholder-sdk.png"))

        Using fileStream As IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
            Try
                Await Scenario4WriteableBitmap.SetSourceAsync(fileStream)
            Catch ex As TaskCanceledException
                ' The async action to set the WriteableBitmap's source may be canceled if the source is changed again while the action is in progress
            End Try
        End Using
    End Sub

    Private Async Sub LoadImageUsingSetSource_Click(sender As Object, e As RoutedEventArgs)
        ' This method loads an image into the WriteableBitmap using the SetSource method

        Dim picker As New FileOpenPicker()
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".png")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".bmp")

        Dim file As StorageFile = Await picker.PickSingleFileAsync()

        ' Ensure a file was selected
        If file IsNot Nothing Then
            ' Set the source of the WriteableBitmap to the image stream
            Using fileStream As IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
                Await Scenario4WriteableBitmap.SetSourceAsync(fileStream)
            End Using
        End If
    End Sub

    Private Async Sub LoadImageUsingPixelBuffer_Click(sender As Object, e As RoutedEventArgs)
        ' This method loads an image into the WriteableBitmap by decoding it into a byte stream
        ' and copying the result into the WriteableBitmap's pixel buffer

        Dim picker As New FileOpenPicker()
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".png")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".bmp")

        Dim file As StorageFile = Await picker.PickSingleFileAsync()

        ' Ensure a file was selected
        If file IsNot Nothing Then
            Using fileStream As IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
                Dim decoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(fileStream)

                ' Scale image to appropriate size
                Dim transform As New BitmapTransform() With { _
                    .ScaledWidth = Convert.ToUInt32(Scenario4WriteableBitmap.PixelWidth), _
                    .ScaledHeight = Convert.ToUInt32(Scenario4WriteableBitmap.PixelHeight) _
                }

                ' WriteableBitmap uses BGRA format
                ' This sample ignores Exif orientation
                Dim pixelData As PixelDataProvider = Await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage)

                ' An array containing the decoded image data, which could be modified before being displayed
                Dim sourcePixels As Byte() = pixelData.DetachPixelData()

                ' Open a stream to copy the image contents to the WriteableBitmap's pixel buffer
                Using stream As Stream = Scenario4WriteableBitmap.PixelBuffer.AsStream()
                    Await stream.WriteAsync(sourcePixels, 0, sourcePixels.Length)
                End Using
            End Using

            'Redraw the WriteableBitmap
            Scenario4WriteableBitmap.Invalidate()
        End If
    End Sub

    Private Async Sub DrawMandelbrotSet_Click(sender As Object, e As RoutedEventArgs)
        ' This method draws custom content to a WriteableBitmap then displays it in the Scenario4Image control

        Dim pixelWidth As Integer = Scenario4WriteableBitmap.PixelWidth
        Dim pixelHeight As Integer = Scenario4WriteableBitmap.PixelHeight

        ' Asynchronously graph the Mandelbrot set on a background thread
        Dim result As Byte() = Nothing
        Await ThreadPool.RunAsync(New WorkItemHandler(Sub(action As IAsyncAction)
                                                          result = DrawMandelbrotGraph(pixelWidth, pixelHeight)
                                                      End Sub))

        ' Open a stream to copy the graph to the WriteableBitmap's pixel buffer
        Using stream As Stream = Scenario4WriteableBitmap.PixelBuffer.AsStream()
            Await stream.WriteAsync(result, 0, result.Length)
        End Using

        'Redraw the WriteableBitmap
        Scenario4WriteableBitmap.Invalidate()
		End Sub

    Private Function DrawMandelbrotGraph(width As Integer, height As Integer) As Byte()
        ' 4 bytes required for each pixel
        Dim result As Byte() = New Byte(width * height * 4 - 1) {}
        Dim resultIndex As Integer = 0

        ' Max iterations when testing whether a point is in the set
        Dim maxIterationCount As Integer = 50

        ' Choose intervals
        Dim minimum As New Complex(-2.5, -1.0)
        Dim maximum As New Complex(1.0, 1)

        ' Normalize x and y values based on chosen interval and size of WriteableBitmap
        Dim xScaleFactor As Double = (maximum.Real - minimum.Real) / width
        Dim yScaleFactor As Double = (maximum.Imaginary - minimum.Imaginary) / height

        ' Plot the Mandelbrot set on x-y plane
        For y As Integer = 0 To height - 1
            For x As Integer = 0 To width - 1
                Dim c As New Complex(minimum.Real + x * xScaleFactor, maximum.Imaginary - y * yScaleFactor)
                Dim z As New Complex(c.Real, c.Imaginary)

                ' Iterate with simple escape-time algorithm
                Dim iteration As Integer = 0
                While Math.Pow(z.Magnitude, 2) < 4 AndAlso iteration < maxIterationCount
                    z = Complex.Pow(z, 2) + c
                    iteration += 1
                End While

                ' Shade pixel based on probability it's in the set
                Dim grayScaleValue As Byte = Convert.ToByte(255 - 255.0 * iteration / maxIterationCount)
                result(System.Math.Max(System.Threading.Interlocked.Increment(resultIndex), resultIndex - 1) - 1) = grayScaleValue
                ' Green value of pixel
                result(System.Math.Max(System.Threading.Interlocked.Increment(resultIndex), resultIndex - 1) - 1) = grayScaleValue
                ' Blue value of pixel
                result(System.Math.Max(System.Threading.Interlocked.Increment(resultIndex), resultIndex - 1) - 1) = grayScaleValue
                ' Red value of pixel
                ' Alpha value of pixel
                result(System.Math.Max(System.Threading.Interlocked.Increment(resultIndex), resultIndex - 1) - 1) = 255
            Next
        Next

        Return result
    End Function
End Class
