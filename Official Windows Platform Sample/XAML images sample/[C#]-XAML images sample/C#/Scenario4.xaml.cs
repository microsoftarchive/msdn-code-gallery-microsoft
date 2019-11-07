//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using SDKTemplate;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;

namespace Images
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private WriteableBitmap Scenario4WriteableBitmap;

        public Scenario4()
        {
            this.InitializeComponent();
            Scenario4WriteableBitmap = new WriteableBitmap((int)Scenario4ImageContainer.Width, (int)Scenario4ImageContainer.Height);
            Scenario4Image.Source = Scenario4WriteableBitmap;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the source of the WriteableBitmap to a placeholder image
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/placeholder-sdk.png"));
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                try
                {
                    await Scenario4WriteableBitmap.SetSourceAsync(fileStream);
                }
                catch (TaskCanceledException)
                {
                    // The async action to set the WriteableBitmap's source may be canceled if the source is changed again while the action is in progress
                }
            }
        }

        private async void LoadImageUsingSetSource_Click(object sender, RoutedEventArgs e)
        {
            // This method loads an image into the WriteableBitmap using the SetSource method

            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");

            StorageFile file = await picker.PickSingleFileAsync();

            // Ensure a file was selected
            if (file != null)
            {
                // Set the source of the WriteableBitmap to the image stream
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    try
                    {
                        await Scenario4WriteableBitmap.SetSourceAsync(fileStream);
                    }
                    catch (TaskCanceledException)
                    {
                        // The async action to set the WriteableBitmap's source may be canceled if the user clicks the button repeatedly
                    }
                }
            }
        }

        private async void LoadImageUsingPixelBuffer_Click(object sender, RoutedEventArgs e)
        {
            // This method loads an image into the WriteableBitmap by decoding it into a byte stream
            // and copying the result into the WriteableBitmap's pixel buffer

            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");

            StorageFile file = await picker.PickSingleFileAsync();

            // Ensure a file was selected
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                    
                    // Scale image to appropriate size
                    BitmapTransform transform = new BitmapTransform() { 
                        ScaledWidth = Convert.ToUInt32(Scenario4WriteableBitmap.PixelWidth),
                        ScaledHeight = Convert.ToUInt32(Scenario4WriteableBitmap.PixelHeight)};
                    
                    PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,    // WriteableBitmap uses BGRA format
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation, // This sample ignores Exif orientation
                        ColorManagementMode.DoNotColorManage);

                    // An array containing the decoded image data, which could be modified before being displayed
                    byte[] sourcePixels = pixelData.DetachPixelData();

                    // Open a stream to copy the image contents to the WriteableBitmap's pixel buffer
                    using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
                    {
                        await stream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                    }
                }

                // Redraw the WriteableBitmap
                Scenario4WriteableBitmap.Invalidate();
            }
        }

        private async void DrawMandelbrotSet_Click(object sender, RoutedEventArgs e)
        {
            // This method draws custom content to a WriteableBitmap then displays it in the Scenario4Image control

            int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
            int pixelHeight = Scenario4WriteableBitmap.PixelHeight;

            // Asynchronously graph the Mandelbrot set on a background thread
            byte[] result = null;
            await ThreadPool.RunAsync(new WorkItemHandler(
                (IAsyncAction action) =>
                {
                    result = DrawMandelbrotGraph(pixelWidth, pixelHeight);
                }
                ));

            // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
            using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(result, 0, result.Length);
            }

            // Redraw the WriteableBitmap
            Scenario4WriteableBitmap.Invalidate();
        }

        private byte[] DrawMandelbrotGraph(int width, int height)
        {
            // 4 bytes required for each pixel
            byte[] result = new byte[width * height * 4];
            int resultIndex = 0;

            // Max iterations when testing whether a point is in the set
            int maxIterationCount = 50;

            // Choose intervals
            Complex minimum = new Complex(-2.5, -1.0);
            Complex maximum = new Complex(1.0, 1);

            // Normalize x and y values based on chosen interval and size of WriteableBitmap
            double xScaleFactor = (maximum.Real - minimum.Real) / width;
            double yScaleFactor = (maximum.Imaginary - minimum.Imaginary) / height;

            // Plot the Mandelbrot set on x-y plane
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Complex c = new Complex(minimum.Real + x * xScaleFactor, maximum.Imaginary - y * yScaleFactor);
                    Complex z = new Complex(c.Real, c.Imaginary);

                    // Iterate with simple escape-time algorithm
                    int iteration = 0;
                    while (z.Magnitude < 2 && iteration < maxIterationCount)
                    {
                        z = (z * z) + c;
                        iteration++;
                    }

                    // Shade pixel based on probability it's in the set
                    byte grayScaleValue = Convert.ToByte(255 - 255.0 * iteration / maxIterationCount);
                    result[resultIndex++] = grayScaleValue; // Green value of pixel
                    result[resultIndex++] = grayScaleValue; // Blue value of pixel
                    result[resultIndex++] = grayScaleValue; // Red value of pixel
                    result[resultIndex++] = 255;            // Alpha value of pixel
                }
            }

            return result;
        }
    }
}
