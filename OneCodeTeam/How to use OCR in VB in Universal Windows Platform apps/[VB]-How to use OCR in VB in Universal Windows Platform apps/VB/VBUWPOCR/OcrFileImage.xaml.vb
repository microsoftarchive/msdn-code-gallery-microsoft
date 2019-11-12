' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238



Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Windows.Foundation
Imports Windows.Globalization
Imports Windows.Graphics.Imaging
Imports Windows.Media.Ocr
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class OcrFileImage
    Inherits Page
    ' A pointer back to the main page.
    ' This Is needed if you want to call methods in MainPage such as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' Bitmap holder of currently loaded image.
    Private bitmap As SoftwareBitmap

    ' Recognized words overlay boxes.
    Private wordBoxes As New List(Of WordOverlay)()

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        UpdateAvailableLanguages()
        Await LoadSampleImage()
    End Sub

    Private Sub UpdateAvailableLanguages()
        If (Not UserLanguageToggle.IsOn) Then

            ' Check if any OCR language Is available on device.
            If (OcrEngine.AvailableRecognizerLanguages.Count > 0) Then
                LanguageList.ItemsSource = OcrEngine.AvailableRecognizerLanguages
                LanguageList.SelectedIndex = 0
                LanguageList.IsEnabled = True
            Else
                ' Prevent OCR if no OCR languages are available on device.
                UserLanguageToggle.IsEnabled = False
                LanguageList.IsEnabled = False
                ExtractButton.IsEnabled = False
                rootPage.NotifyUser("No available OCR languages.", NotifyType.ErrorMessage)
            End If
        Else
            LanguageList.ItemsSource = Nothing
            LanguageList.IsEnabled = False
            rootPage.NotifyUser(
                    "Run OCR in first OCR available language from UserProfile.GlobalizationPreferences.Languages list.",
                    NotifyType.StatusMessage)
        End If
    End Sub

    Private Async Function LoadSampleImage() As Task
        ' Load sample "Hello World" image.
        Dim File = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\splash-sdk.png")
        Await LoadImage(File)
    End Function

    Private Async Function LoadImage(file As StorageFile) As Task
        Using stream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
            Dim decoder = Await BitmapDecoder.CreateAsync(stream)
            bitmap = Await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied)
            Dim imgSource = New WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight)
            bitmap.CopyToBuffer(imgSource.PixelBuffer)
            PreviewImage.Source = imgSource
        End Using
    End Function

    ''' <summary>
    ''' Clears extracted text and text overlay from previous OCR.
    ''' </summary>
    Private Sub ClearResults()
        TextOverlay.RenderTransform = Nothing
        ExtractedTextBox.Text = String.Empty
        TextOverlay.Children.Clear()
        wordBoxes.Clear()
    End Sub

    ''' <summary>
    '''  Update word box transform to match current UI size.
    ''' </summary>
    Private Sub UpdateWordBoxTransform()
        ' Used for text overlay.
        ' Prepare scale transform for words since image Is Not displayed in original size.
        Dim scaleTrasform = New ScaleTransform()


        scaleTrasform.CenterX = 0
        scaleTrasform.CenterY = 0
        scaleTrasform.ScaleX = PreviewImage.ActualWidth / bitmap.PixelWidth
        scaleTrasform.ScaleY = PreviewImage.ActualHeight / bitmap.PixelHeight

        For Each item In wordBoxes
            item.Transform(scaleTrasform)
        Next

    End Sub

    Private Async Sub ExtractButton_Tapped(sender As Object, e As TappedRoutedEventArgs)
        ClearResults()
        ' Check if OcrEngine supports image resoulution.
        If (bitmap.PixelWidth > OcrEngine.MaxImageDimension Or bitmap.PixelHeight > OcrEngine.MaxImageDimension) Then
            rootPage.NotifyUser(
                    String.Format("Bitmap dimensions ({0}x{1}) are too big for OCR.", bitmap.PixelWidth, bitmap.PixelHeight) +
                    "Max image dimension is " + OcrEngine.MaxImageDimension + ".",
                    NotifyType.ErrorMessage)
            Return
        End If
        Dim myOcrEngine As OcrEngine = Nothing
        If (UserLanguageToggle.IsOn) Then
            ' Try to create OcrEngine for first supported language from UserProfile.GlobalizationPreferences.Languages list.
            ' If none of the languages are available on device, method returns null.
            myOcrEngine = OcrEngine.TryCreateFromUserProfileLanguages()
        Else
            ' Try to create OcrEngine for specified language.
            ' If language Is Not supported on device, method returns null.
            myOcrEngine = OcrEngine.TryCreateFromLanguage(LanguageList.SelectedValue)
        End If

        If (myOcrEngine IsNot Nothing) Then

            ' Recognize text from image.
            Dim myOcrResult = Await myOcrEngine.RecognizeAsync(bitmap)

            ' Display recognized text.
            ExtractedTextBox.Text = myOcrResult.Text

            If (myOcrResult.TextAngle IsNot Nothing) Then

                ' If text Is detected under some angle in this sample scenario we want to
                ' overlay word boxes over original image, so we rotate overlay boxes.
                Dim transform = New RotateTransform()
                transform.Angle = myOcrResult.TextAngle
                transform.CenterX = PreviewImage.ActualWidth / 2
                transform.CenterY = PreviewImage.ActualHeight / 2
                TextOverlay.RenderTransform = transform
            End If
            ' Create overlay boxes over recognized words.
            For Each line In myOcrResult.Lines
                Dim lineRect = Rect.Empty
                For Each word In line.Words
                    lineRect.Union(word.BoundingRect)
                Next
                ' Determine if line Is horizontal Or vertical.
                ' Vertical lines are supported only in Chinese Traditional And Japanese languages.
                Dim isVerticalLine As Boolean = lineRect.Height > lineRect.Width
                For Each word In line.Words
                    Dim wordBoxOverlay As New WordOverlay(word)
                    ' Keep references to word boxes.
                    wordBoxes.Add(wordBoxOverlay)
                    ' Define overlay style.
                    Dim overlay = New Border()
                    If (isVerticalLine) Then
                        overlay.Style = Resources.Item("HighlightedWordBoxVerticalLine")
                    Else
                        overlay.Style = Resources.Item("HighlightedWordBoxHorizontalLine")
                    End If
                    ' Bind word boxes to UI.
                    overlay.SetBinding(Border.MarginProperty, wordBoxOverlay.CreateWordPositionBinding())
                    overlay.SetBinding(Border.WidthProperty, wordBoxOverlay.CreateWordWidthBinding())
                    overlay.SetBinding(Border.HeightProperty, wordBoxOverlay.CreateWordHeightBinding())
                    ' Put the filled textblock in the results grid.
                    TextOverlay.Children.Add(overlay)
                Next
            Next
            ' Rescale word boxes to match current UI size.
            UpdateWordBoxTransform()
            rootPage.NotifyUser(
                    "Image is OCRed for " + myOcrEngine.RecognizerLanguage.DisplayName + " language.",
                    NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Selected language is not available.", NotifyType.ErrorMessage)
                    End If
    End Sub

    Private Sub UserLanguageToggle_Toggled(sender As Object, e As RoutedEventArgs)
        UpdateAvailableLanguages()
    End Sub

    ''' <summary>
    ''' This is event handler for 'Sample' button.
    ''' It loads image with 'Hello World' text and displays it in UI.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub SampleButton_Tapped(sender As Object, e As TappedRoutedEventArgs)
        ClearResults()
        ' Load sample "Hello World" image.
        Await LoadSampleImage()
        rootPage.NotifyUser("Loaded sample image.", NotifyType.StatusMessage)

    End Sub

    ''' <summary>
    ''' This is event handler for 'Load' button.
    ''' It opens file picked and load selected image in UI..
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub LoadButton_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dim picker = New FileOpenPicker()
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".png")

        Dim File = Await picker.PickSingleFileAsync()
        If (File IsNot Nothing) Then
            ClearResults()
            Await LoadImage(File)
            rootPage.NotifyUser(
                    String.Format("Loaded image from file: {0} ({1}x{2}).", File.Name, bitmap.PixelWidth, bitmap.PixelHeight),
                    NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' Occures when selected language is changed in available languages combo box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LanguageList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        ClearResults()
        Dim lang As Language = LanguageList.SelectedValue
        If (lang IsNot Nothing) Then
            rootPage.NotifyUser(
                    "Selected OCR language is " + lang.DisplayName + ". " +
                        OcrEngine.AvailableRecognizerLanguages.Count.ToString() + " OCR language(s) are available. " +
                        "Check combo box for full list.",
                    NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' Occurs when displayed image size changes.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub PreviewImage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        ' Update word overlay boxes.
        UpdateWordBoxTransform()

        ' Update image rotation center.

        Dim rotate As RotateTransform = New RotateTransform()
        If (TextOverlay.RenderTransform IsNot Nothing) Then
            rotate.CenterX = PreviewImage.ActualWidth / 2
            rotate.CenterY = PreviewImage.ActualHeight / 2
            TextOverlay.RenderTransform = rotate
        End If
    End Sub


End Class
