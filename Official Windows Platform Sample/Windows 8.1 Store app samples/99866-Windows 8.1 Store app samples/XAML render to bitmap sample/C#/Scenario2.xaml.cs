//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;

namespace RenderToBitmap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();

            rootPage.MainPageResized += rootPage_MainPageResized;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateOutputLayout();

            // Retrieve the active DataTransferManager to register for Share operations
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Unregister for Share operations
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
        }

        private void rootPage_MainPageResized(object sender, MainPageSizeChangedEventArgs e)
        {
            UpdateOutputLayout();
        }

        /// <summary>
        /// Event handler for the "Add Shape" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddShape_Click(object sender, RoutedEventArgs e)
        {
            // Create a new pseudo-random number generator to generate random colors
            Random randomGenerator = new Random();
            byte[] pixelValues = new byte[3]; // Represents the red, green, and blue channels of a color
            randomGenerator.NextBytes(pixelValues);
            Color color = new Color() { R = pixelValues[0], G = pixelValues[1], B = pixelValues[2], A = 255 };

            Rectangle rectangle = new Rectangle();
            rectangle.Width = randomGenerator.Next(50, 200);
            rectangle.Height = randomGenerator.Next(50, 200);
            rectangle.Fill = new SolidColorBrush(color);

            // Adds rectangle to RenderedGrid
            AddManipulableElement(rectangle);
        }

        /// <summary>
        /// Event handler for the "Add Text" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTextBlock_Click(object sender, RoutedEventArgs e)
        {
            // Create a TextBlock and add it to RenderedGrid
            TextBlock textBlock = new TextBlock();
            textBlock.Text = NewText.Text;
            textBlock.FontSize = 40;

            AddManipulableElement(textBlock);

            // Clear contents of the input TextBox
            NewText.Text = string.Empty;
        }

        /// <summary>
        /// Event handler for the "Save Image.." button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (RenderedGrid.Children.Count == 0)
            {
                rootPage.NotifyUser("You must add content before saving.", NotifyType.ErrorMessage);
                return;
            }

            // Render to an image at the current system scale and retrieve pixel contents
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(RenderedGrid);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            var savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".png";
            savePicker.FileTypeChoices.Add(".png", new List<string> { ".png" });
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.SuggestedFileName = "snapshot.png";

            // Prompt the user to select a file
            var saveFile = await savePicker.PickSaveFileAsync();

            // Verify the user selected a file
            if (saveFile == null)
                return;

            // Encode the image to the selected file on disk
            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }

            rootPage.NotifyUser("File saved!", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Provides an option to retrieve an image for the Share charm.
        /// </summary>
        /// <param name="request"></param>
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = "RenderToBitmap";
            e.Request.Data.Properties.Description = "Render to bitmap C# sample";

            e.Request.Data.SetDataProvider(StandardDataFormats.Bitmap, new DataProviderHandler(this.OnDeferredImageRequestedHandler));
        }

        /// <summary>
        /// Handles image requests from the Share charm.
        /// </summary>
        /// <param name="request"></param>
        private async void OnDeferredImageRequestedHandler(DataProviderRequest request)
        {
            // Request deferral to wait for async calls
            DataProviderDeferral deferral = request.GetDeferral();

            // XAML objects can only be accessed on the UI thread, and the call may come in on a background thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                    // Render to an image at the current system scale and retrieve pixel contents
                    await renderTargetBitmap.RenderAsync(RenderedGrid);
                    var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                    // Encode image to an in-memory stream
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint)renderTargetBitmap.PixelWidth,
                        (uint)renderTargetBitmap.PixelHeight,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        pixelBuffer.ToArray());

                    await encoder.FlushAsync();
                    
                    // Set content of the DataProviderRequest to the encoded image in memory
                    request.SetData(RandomAccessStreamReference.CreateFromStream(stream));
                }
                finally
                {
                    deferral.Complete();
                }
            });
        }

        /// <summary>
        /// Makes an element manipulable and adds it to the RenderedGrid panel.
        /// </summary>
        /// <param name="element"></param>
        private void AddManipulableElement(UIElement element)
        {
            ManipulableContainer container = new ManipulableContainer();
            container.Content = element;

            RenderedGrid.Children.Add(container);
        }

        /// <summary>
        /// Updates size and position of elements on the page when the size changes.
        /// </summary>
        private void UpdateOutputLayout()
        {
            Output.Width = (MainPage.Current.FindName("ContentRoot") as FrameworkElement).ActualWidth;
        }
    }
}
