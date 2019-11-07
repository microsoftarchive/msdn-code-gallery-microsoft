// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

namespace OCR
{
    /// <summary>
    /// A page that can be used on its own or navigated to within a Frame.
    /// On Windows Phone, the page implements IFileOpenPickerContinuable to support
    /// Windows Phone-style continuable file pickers. This interface is invoked when
    /// the app is re-activated after the file picker process returns.
    /// For more information about continuable file pickers, see:
    /// http://go.microsoft.com/fwlink/?LinkId=393345
    /// </summary>
#if WINDOWS_PHONE_APP
    public sealed partial class ExtractText : Page, IFileOpenPickerContinuable
#else
    public sealed partial class ExtractText : Page
#endif
    {
        // A pointer back to main page.
        private MainPage rootPage;

        // Bitmap holder of currently loaded image.
        private WriteableBitmap bitmap;

        // OCR engine instance used to extract text from images.
        private OcrEngine ocrEngine;

        public ExtractText()
        {
            rootPage = MainPage.Current;

            this.InitializeComponent();

            ocrEngine = new OcrEngine(OcrLanguage.English);

            // Load all available languages from OcrLanguage enum in combo box.
            LanguageList.ItemsSource = Enum.GetNames(typeof(OcrLanguage)).OrderBy(name => name.ToString());
            LanguageList.SelectedItem = ocrEngine.Language.ToString();
            LanguageList.SelectionChanged += LanguageList_SelectionChanged;
        }

        /// <summary>
        /// This is selection changed handler for language list.
        /// Tries to change language for Optical Character Recognition.
        /// If language resources are not present reverts selected language.
        /// Check MSDN docs or readme.txt in NuGet Package to learn how to produce 
        /// resource file that contains language specific resources.
        /// </summary>
        void LanguageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var languageName = LanguageList.SelectedItem as string;

            try
            {
                // Parse the selected language as an OcrLanguage value.
                OcrLanguage parseResult;
                if (Enum.TryParse(languageName, out parseResult))
                {
                    // Set the OCR language to the result.
                    ocrEngine.Language = parseResult;

                    rootPage.NotifyUser(
                        String.Format("OCR engine set to extract text in {0} language.", LanguageList.SelectedItem),
                        NotifyType.StatusMessage);

                    ClearResults();
                }
            }
            catch (ArgumentException)
            {
                LanguageList.SelectedItem = e.RemovedItems.First();

                rootPage.NotifyUser(
                    String.Format(
                        "Resource file 'MsOcrRes.opr' does not contain required resources for {0} language. " +
                        Environment.NewLine +
                        "Check MSDN docs or readme.txt in NuGet Package to learn how to produce resource file " +
                        "that contains {0} language specific resources. " +
                        Environment.NewLine +
                        "OCR language is now reverted to {1} language.",
                        languageName,
                        e.RemovedItems.First()),
                    NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked when the user clicks on the Load button.
        /// </summary>
#if WINDOWS_PHONE_APP
        private void Load_Click(object sender, RoutedEventArgs e)
#else
        private async void Load_Click(object sender, RoutedEventArgs e)
#endif
        {
            var picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" },
            };

            // On Windows Phone, after the picker is launched the app is closed.
#if WINDOWS_PHONE_APP
            picker.PickSingleFileAndContinue();
#else
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadImage(file);
            }
#endif
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Implements IFileOpenPickerContinuable. This method is marked async but cannot be awaited
        /// by the caller and does not follow the "Async" naming convention.  It is intended to be
        /// called in a "fire and forget" manner.
        /// </summary>
        /// <param name="args">Contains the file(s) returned by the continuable file picker.</param>
        public async void ContinueFileOpenPicker(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count != 0)
            {
                await LoadImage(args.Files[0]);
            }
        }
#endif

        /// <summary>
        /// Invoked when the user clicks on the Sample button.
        /// </summary>
        private async void Sample_Click(object sender, RoutedEventArgs e)
        {
            // Load sample "Hello World" image.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("sample\\sample.png");
            await LoadImage(file);
        }

        /// <summary>
        /// Loads image from file to bitmap and displays it in UI.
        /// </summary>
        private async Task LoadImage(StorageFile file)
        {
            ImageProperties imgProp = await file.Properties.GetImagePropertiesAsync();

            using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                bitmap.SetSource(imgStream);
                PreviewImage.Source = bitmap;
            }

            rootPage.NotifyUser(
                String.Format("Loaded image from file: {0} ({1}x{2}).", file.Name, imgProp.Width, imgProp.Height),
                NotifyType.StatusMessage);

            ClearResults();
        }

        /// <summary>
        /// Clears extracted text results.
        /// Removes extracted text and text overlay.
        /// </summary>
        void ClearResults()
        {
            // Retrieve initial state.
            PreviewImage.RenderTransform = null;
            ImageText.Text = "Text not extracted.";
            ImageText.Style = (Style)Application.Current.Resources["YellowTextStyle"];

            ExtractTextButton.IsEnabled = true;
            LanguageList.IsEnabled = true;

            // Clear text overlay from image.
            TextOverlay.Children.Clear();
        }

        /// <summary>
        /// This is click handler for Extract Text button.
        /// If image size is supported text is extracted and overlaid over displayed image.
        /// Supported image dimensions are between 40 and 2600 pixels.
        /// </summary>
        private async void ExtractText_Click(object sender, RoutedEventArgs e)
        {
            // Prevent another OCR request, since only image can be processed at the time at same OCR engine instance.
            ExtractTextButton.IsEnabled = false;

            // Check whether is loaded image supported for processing.
            // Supported image dimensions are between 40 and 2600 pixels.
            if (bitmap.PixelHeight < 40 ||
                bitmap.PixelHeight > 2600 ||
                bitmap.PixelWidth < 40 ||
                bitmap.PixelWidth > 2600)
            {
                ImageText.Text = "Image size is not supported." +
                                    Environment.NewLine +
                                    "Loaded image size is " + bitmap.PixelWidth + "x" + bitmap.PixelHeight + "." +
                                    Environment.NewLine +
                                    "Supported image dimensions are between 40 and 2600 pixels.";
                ImageText.Style = (Style)Application.Current.Resources["RedTextStyle"];

                rootPage.NotifyUser(
                    String.Format("OCR was attempted on image with unsupported size. " +
                                  Environment.NewLine +
                                  "Supported image dimensions are between 40 and 2600 pixels."),
                    NotifyType.ErrorMessage);

                return;
            }

            // This main API call to extract text from image.
            var ocrResult = await ocrEngine.RecognizeAsync((uint)bitmap.PixelHeight, (uint)bitmap.PixelWidth, bitmap.PixelBuffer.ToArray());

            // OCR result does not contain any lines, no text was recognized. 
            if (ocrResult.Lines != null)
            {
                // Used for text overlay.
                // Prepare scale transform for words since image is not displayed in original format.
                var scaleTrasform = new ScaleTransform
                {
                    CenterX = 0,
                    CenterY = 0,
                    ScaleX = PreviewImage.ActualWidth / bitmap.PixelWidth,
                    ScaleY = PreviewImage.ActualHeight / bitmap.PixelHeight,
                };

                if (ocrResult.TextAngle != null)
                {
                    // If text is detected under some angle then
                    // apply a transform rotate on image around center.
                    PreviewImage.RenderTransform = new RotateTransform
                    {
                        Angle = (double)ocrResult.TextAngle,
                        CenterX = PreviewImage.ActualWidth / 2,
                        CenterY = PreviewImage.ActualHeight / 2
                    };
                }

                string extractedText = "";

                // Iterate over recognized lines of text.
                foreach (var line in ocrResult.Lines)
                {
                    // Iterate over words in line.
                    foreach (var word in line.Words)
                    {
                        var originalRect = new Rect(word.Left, word.Top, word.Width, word.Height);
                        var overlayRect = scaleTrasform.TransformBounds(originalRect);

                        // Define the TextBlock.
                        var wordTextBlock = new TextBlock()
                        {
                            Height = overlayRect.Height,
                            Width = overlayRect.Width,
                            FontSize = overlayRect.Height * 0.8,
                            Text = word.Text,
                            Style = (Style)Application.Current.Resources["ExtractedWordTextStyle"]
                        };

                        // Define position, background, etc.
                        var border = new Border()
                        {
                            Margin = new Thickness(overlayRect.Left, overlayRect.Top, 0, 0),
                            Height = overlayRect.Height,
                            Width = overlayRect.Width,
                            Child = wordTextBlock,
                            Style = (Style)Application.Current.Resources["ExtractedWordBorderStyle"]
                        };

                        // Put the filled textblock in the results grid.
                        TextOverlay.Children.Add(border);

                        extractedText += word.Text + " ";
                    }
                    extractedText += Environment.NewLine;
                }

                ImageText.Text = extractedText;
                ImageText.Style = (Style)Application.Current.Resources["GreenTextStyle"];
            }
            else
            {
                ImageText.Text = "No text.";
                ImageText.Style = (Style)Application.Current.Resources["RedTextStyle"];
            }

            rootPage.NotifyUser(
                    String.Format("Image successfully processed in {0} language.", ocrEngine.Language.ToString()),
                    NotifyType.StatusMessage);
        }

        /// <summary>
        /// This is click event handler for Overlay button.
        /// Check state of this this control determines whether extracted text will be overlaid over image. 
        /// </summary>
        private void Overlay_Click(object sender, RoutedEventArgs e)
        {
            TextOverlay.Visibility = OverlayResults.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
