' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.Globalization
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Windows.ApplicationModel
Imports Windows.Devices.Enumeration
Imports Windows.Graphics.Display
Imports Windows.Graphics.Imaging
Imports Windows.Media
Imports Windows.Media.Capture
Imports Windows.Media.MediaProperties
Imports Windows.Media.Ocr
Imports Windows.System.Display
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class OcrCapturedImage
    Inherits Page
    Private _rootPage As MainPage = MainPage.Current
    Private _ocrLanguage As New Language("en")
    Private _wordBoxes As New List(Of WordOverlay)()
    Private ReadOnly _displayInformation As DisplayInformation = DisplayInformation.GetForCurrentView()
    Private Shared ReadOnly RotationKey As New Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1")
    Private Shared ReadOnly _displayRequest As New DisplayRequest()
    Private _mediaCapture As MediaCapture
    Private _isInitialized As Boolean = False
    Private _isPreviewing As Boolean = False

    Private _mirroringPreview As Boolean = False
    Private _externalCamera As Boolean = False

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler Application.Current.Suspending, AddressOf Application_SuspendingAsync
        AddHandler Application.Current.Resuming, AddressOf Application_ResumingAsync

    End Sub

    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)

        AddHandler _displayInformation.OrientationChanged, AddressOf DisplayInformation_OrientationChanged
        If (Not OcrEngine.IsLanguageSupported(_ocrLanguage)) Then
            _rootPage.NotifyUser(_ocrLanguage.DisplayName + " is not supported.", NotifyType.ErrorMessage)
            Return
        End If
        Await StartCameraAsync()
        If (_isInitialized) Then
            ExtractButton.Visibility = Visibility.Visible
            CameraButton.Visibility = Visibility.Collapsed
        Else
            ExtractButton.Visibility = Visibility.Collapsed
            CameraButton.Visibility = Visibility.Visible
        End If

    End Sub

    Protected Overrides Async Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        RemoveHandler _displayInformation.OrientationChanged, AddressOf DisplayInformation_OrientationChanged
        Await CleanupCameraAsync()
    End Sub



    Protected Async Sub Application_SuspendingAsync(sender As Object, e As SuspendingEventArgs)
        If (Frame.CurrentSourcePageType.Equals(GetType(MainPage))) Then
            Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
            Await CleanupCameraAsync()
            RemoveHandler _displayInformation.OrientationChanged, AddressOf DisplayInformation_OrientationChanged
            deferral.Complete()
        End If
    End Sub
    Protected Async Sub Application_ResumingAsync(sender As Object, e As Object)
        If (Frame.CurrentSourcePageType.Equals(GetType(MainPage))) Then
            AddHandler _displayInformation.OrientationChanged, AddressOf DisplayInformation_OrientationChanged
            Await StartCameraAsync()
        End If
    End Sub

    Private Async Sub DisplayInformation_OrientationChanged(sender As DisplayInformation, args As Object)
        If (_isPreviewing) Then
            Await SetPreviewRotationAsync()
        End If
    End Sub

    Private Async Sub ExtractButton_Tapped(sender As Object, e As TappedRoutedEventArgs)
        'Get information about the preview.
        Dim previewProperties As VideoEncodingProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview)
        Dim videoFrameWidth As Integer = previewProperties.Width
        Dim videoFrameHeight As Integer = previewProperties.Height


        ' In portrait modes, the width And height must be swapped for the VideoFrame to have the correct aspect ratio And avoid letterboxing / black bars.
        If (Not _externalCamera And (_displayInformation.CurrentOrientation = DisplayOrientations.Portrait Or _displayInformation.CurrentOrientation = DisplayOrientations.PortraitFlipped)) Then
            videoFrameWidth = previewProperties.Height
            videoFrameHeight = previewProperties.Width
        End If


        ' Create the video frame to request a SoftwareBitmap preview frame.
        Dim VideoFrame As New VideoFrame(BitmapPixelFormat.Bgra8, videoFrameWidth, videoFrameHeight)

        ' Capture the preview frame.
        Using currentFrame As VideoFrame = Await _mediaCapture.GetPreviewFrameAsync(VideoFrame)
            'Collect the resulting frame.
            Dim bitmap As SoftwareBitmap = currentFrame.SoftwareBitmap

            Dim myOcrEngine As OcrEngine = OcrEngine.TryCreateFromLanguage(_ocrLanguage)

            If (myOcrEngine Is Nothing) Then

                _rootPage.NotifyUser(_ocrLanguage.DisplayName + " is not supported.", NotifyType.ErrorMessage)

                Return
            End If

            Dim imgSource As New WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight)
            bitmap.CopyToBuffer(imgSource.PixelBuffer)
            PreviewImage.Source = imgSource

            Dim myOcrResult As OcrResult = Await myOcrEngine.RecognizeAsync(bitmap)

            ' Used for text overlay.
            ' Prepare scale transform for words since image Is Not displayed in original format.
            Dim myScaleTrasform As New ScaleTransform()

            myScaleTrasform.CenterX = 0
            myScaleTrasform.CenterY = 0
            myScaleTrasform.ScaleX = PreviewControl.ActualWidth / bitmap.PixelWidth
            myScaleTrasform.ScaleY = PreviewControl.ActualHeight / bitmap.PixelHeight


            If (myOcrResult.TextAngle IsNot Nothing) Then

                ' If text Is detected under some angle in this sample scenario we want to
                ' overlay word boxes over original image, so we rotate overlay boxes.
                Dim myRotateTransform As New RotateTransform()
                TextOverlay.RenderTransform = myRotateTransform
                myRotateTransform.Angle = myOcrResult.TextAngle
                myRotateTransform.CenterX = PreviewImage.ActualWidth / 2
                myRotateTransform.CenterY = PreviewImage.ActualHeight / 2

            End If

            ' Iterate over recognized lines of text.
            For Each line In myOcrResult.Lines

                ' Iterate over words in line.
                For Each word In line.Words

                    ' Define the TextBlock.
                    Dim wordTextBlock As New TextBlock()

                    wordTextBlock.Text = word.Text
                    wordTextBlock.Style = Me.Resources.Item("ExtractedWordTextStyle")

                    Dim wordBoxOverlay As New WordOverlay(word)

                    ' Keep references to word boxes.
                    _wordBoxes.Add(wordBoxOverlay)

                    ' Define position, background, etc.
                    Dim overlay As New Border()

                    overlay.Child = wordTextBlock
                    overlay.Style = Me.Resources.Item("HighlightedWordBoxHorizontalLine")

                    ' Bind word boxes to UI.
                    overlay.SetBinding(Border.MarginProperty, wordBoxOverlay.CreateWordPositionBinding())
                    overlay.SetBinding(Border.WidthProperty, wordBoxOverlay.CreateWordWidthBinding())
                    overlay.SetBinding(Border.HeightProperty, wordBoxOverlay.CreateWordHeightBinding())

                    ' Put the filled textblock in the results grid.
                    TextOverlay.Children.Add(overlay)
                Next
            Next
            _rootPage.NotifyUser("Image processed using " + myOcrEngine.RecognizerLanguage.DisplayName + " language.", NotifyType.StatusMessage)
        End Using
        UpdateWordBoxTransform()
        PreviewControl.Visibility = Visibility.Collapsed
        Image.Visibility = Visibility.Visible
        ExtractButton.Visibility = Visibility.Collapsed
        CameraButton.Visibility = Visibility.Visible
    End Sub

    Private Async Sub CameraButton_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Await StartCameraAsync()
    End Sub

    Private Sub UpdateWordBoxTransform()
        Dim bitmap As WriteableBitmap = PreviewImage.Source

        If (bitmap IsNot Nothing) Then

            ' Used for text overlay.
            ' Prepare scale transform for words since image Is Not displayed in original size.
            Dim myScaleTrasform As New ScaleTransform()
            myScaleTrasform.CenterX = 0
            myScaleTrasform.CenterY = 0
            myScaleTrasform.ScaleX = PreviewImage.ActualWidth / bitmap.PixelWidth
            myScaleTrasform.ScaleY = PreviewImage.ActualHeight / bitmap.PixelHeight

            For Each item In _wordBoxes
                item.Transform(myScaleTrasform)
            Next
        End If

    End Sub

    Private Sub PreviewImage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        UpdateWordBoxTransform()
        ' Update image rotation center.

        If (TextOverlay.RenderTransform IsNot Nothing) Then
            Dim rotate As New RotateTransform()
            rotate.CenterX = PreviewImage.ActualWidth / 2
            rotate.CenterY = PreviewImage.ActualHeight / 2
            TextOverlay.RenderTransform = rotate
        End If
    End Sub

    Private Async Function SetPreviewRotationAsync() As Task
        If (_externalCamera) Then
            Return
        End If

        ' Calculate which way And how far to rotate the preview.
        Dim rotationDegrees As Integer
        Dim sourceRotation As VideoRotation
        CalculatePreviewRotation(sourceRotation, rotationDegrees)

        ' Set preview rotation in the preview source.
        _mediaCapture.SetPreviewRotation(sourceRotation)

        ' Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering And getting preview frames
        Dim props As IMediaEncodingProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview)
        props.Properties.Add(RotationKey, rotationDegrees)
        Await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, Nothing)
    End Function

    Private Sub CalculatePreviewRotation(ByRef sourceRotation As VideoRotation, ByRef rotationDegrees As Integer)
        If (_displayInformation.CurrentOrientation = DisplayOrientations.Portrait) Then
            If (_mirroringPreview) Then
                rotationDegrees = 270
                sourceRotation = VideoRotation.Clockwise270Degrees
            Else
                rotationDegrees = 90
                sourceRotation = VideoRotation.Clockwise90Degrees
            End If
        ElseIf (_displayInformation.CurrentOrientation = DisplayOrientations.LandscapeFlipped) Then
            rotationDegrees = 180
            sourceRotation = VideoRotation.Clockwise180Degrees
        ElseIf (_displayInformation.CurrentOrientation = DisplayOrientations.PortraitFlipped) Then
            If (Not _mirroringPreview) Then
                rotationDegrees = 270
                sourceRotation = VideoRotation.Clockwise270Degrees
            Else
                rotationDegrees = 90
                sourceRotation = VideoRotation.Clockwise90Degrees
            End If
        Else
            rotationDegrees = 0
            sourceRotation = VideoRotation.None
        End If
    End Sub

    ''' <summary>
    ''' Starts the camera. Initializes resources and starts preview.
    ''' </summary>
    ''' <returns></returns>
    Private Async Function StartCameraAsync() As Task
        If (Not _isInitialized) Then

            Await InitializeCameraAsync()
        End If

        If (_isInitialized) Then

            TextOverlay.Children.Clear()
            _wordBoxes.Clear()

            PreviewImage.Source = Nothing

            PreviewControl.Visibility = Visibility.Visible
            Image.Visibility = Visibility.Collapsed

            ExtractButton.Visibility = Visibility.Visible
            CameraButton.Visibility = Visibility.Collapsed

            _rootPage.NotifyUser("Camera started.", NotifyType.StatusMessage)
        End If
    End Function

    Private Async Function InitializeCameraAsync() As Task
        If (_mediaCapture Is Nothing) Then

            ' Attempt to get the back camera if one Is available, but use any camera device if Not.
            Dim cameraDevice As DeviceInformation = Await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back)

            If (cameraDevice Is Nothing) Then
                _rootPage.NotifyUser("No camera device!", NotifyType.ErrorMessage)
                Return
            End If

            ' Create MediaCapture And its settings.
            _mediaCapture = New MediaCapture()

            ' Register for a notification when something goes wrong
            AddHandler _mediaCapture.Failed, AddressOf MediaCapture_Failed

            Dim settings = New MediaCaptureInitializationSettings()
            settings.VideoDeviceId = cameraDevice.Id

            ' Initialize MediaCapture
            Try
                Await _mediaCapture.InitializeAsync(settings)
                _isInitialized = True

            Catch uaEx As UnauthorizedAccessException
                _rootPage.NotifyUser("Denied access to the camera.", NotifyType.ErrorMessage)
            Catch ex As Exception
                _rootPage.NotifyUser("Exception when init MediaCapture. " + ex.Message, NotifyType.ErrorMessage)
            End Try

            ' If initialization succeeded, start the preview.
            If (_isInitialized) Then

                ' Figure out where the camera Is located
                If (cameraDevice.EnclosureLocation Is Nothing Or cameraDevice.EnclosureLocation.Panel = Windows.Devices.Enumeration.Panel.Unknown) Then

                    ' No information on the location of the camera, assume it's an external camera, not integrated on the device.
                    _externalCamera = True

                Else

                    ' Camera Is fixed on the device.
                    _externalCamera = False

                    ' Only mirror the preview if the camera Is on the front panel.
                    _mirroringPreview = (cameraDevice.EnclosureLocation.Panel = Windows.Devices.Enumeration.Panel.Front)
                End If

                Await StartPreviewAsync()
            End If
        End If
    End Function

    Private Async Function StartPreviewAsync() As Task
        ' Prevent the device from sleeping while the preview Is running.
        _displayRequest.RequestActive()

        ' Set the preview source in the UI And mirror it if necessary.
        PreviewControl.Source = _mediaCapture
        If (_mirroringPreview) Then
            PreviewControl.FlowDirection = Windows.UI.Xaml.FlowDirection.RightToLeft
        Else
            PreviewControl.FlowDirection = Windows.UI.Xaml.FlowDirection.LeftToRight
        End If

        ' Start the preview.
        Try
            Await _mediaCapture.StartPreviewAsync()
            _isPreviewing = True
        Catch ex As Exception
            _rootPage.NotifyUser("Exception starting preview." + ex.Message, NotifyType.ErrorMessage)
        End Try

        ' Initialize the preview to the current orientation.
        If (_isPreviewing) Then
            Await SetPreviewRotationAsync()
        End If

    End Function

    Private Async Function StopPreviewAsync() As Task
        Dim catched As Boolean = False
        Dim exceptionMessage As String = String.Empty

        Try
            _isPreviewing = False
            Await _mediaCapture.StopPreviewAsync()
        Catch ex As Exception
            'Use the dispatcher because this method Is sometimes called from non-UI threads.
            catched = True
            exceptionMessage = ex.Message
        End Try
        If (catched) Then
            Await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                Sub()
                    _rootPage.NotifyUser("Exception stopping preview. " + exceptionMessage, NotifyType.ErrorMessage)
                End Sub)
        End If

        ' Use the dispatcher because this method Is sometimes called from non-UI threads.
        Await Dispatcher.RunAsync(
            CoreDispatcherPriority.Normal,
            Sub()
                PreviewControl.Source = Nothing
                ' Allow the device to sleep now that the preview Is stopped.
                _displayRequest.RequestRelease()
            End Sub)
    End Function

    Private Async Function CleanupCameraAsync() As Task
        If (_isInitialized) Then
            If (_isPreviewing) Then
                ' The call to stop the preview Is included here for completeness, but can be
                ' safely removed if a call to MediaCapture.Dispose() Is being made later,
                ' as the preview will be automatically stopped at that point
                Await StopPreviewAsync()
            End If
            _isInitialized = False
        End If

        If (_mediaCapture IsNot Nothing) Then
            RemoveHandler _mediaCapture.Failed, AddressOf MediaCapture_Failed
            _mediaCapture.Dispose()
            _mediaCapture = Nothing
        End If
    End Function

    Private Async Function FindCameraDeviceByPanelAsync(desiredPanel As Windows.Devices.Enumeration.Panel) As Task(Of DeviceInformation)
        ' Get available devices for capturing pictures.
        Dim allVideoDevices = Await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)

        ' Get the desired camera by panel.
        Dim desiredDevice = allVideoDevices.FirstOrDefault(
            Function(x)
                Return x.EnclosureLocation IsNot Nothing And x.EnclosureLocation.Panel = desiredPanel
            End Function)

        ' If there Is no device mounted on the desired panel, return the first device found.
        If (desiredDevice IsNot Nothing) Then
            Return desiredDevice
        End If
        Return allVideoDevices.FirstOrDefault()

    End Function
    Private Async Sub MediaCapture_Failed(sender As MediaCapture, errorEventArgs As MediaCaptureFailedEventArgs)
        Await CleanupCameraAsync()

        Await Dispatcher.RunAsync(
            CoreDispatcherPriority.Normal,
            Sub()
                ExtractButton.Visibility = Visibility.Collapsed
                CameraButton.Visibility = Visibility.Visible
                _rootPage.NotifyUser("MediaCapture Failed. " + errorEventArgs.Message, NotifyType.ErrorMessage)
            End Sub
            )
    End Sub

End Class
