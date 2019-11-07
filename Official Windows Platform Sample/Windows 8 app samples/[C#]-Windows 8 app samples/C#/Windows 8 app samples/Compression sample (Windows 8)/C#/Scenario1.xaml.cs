//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.Storage.Compression;
using SDKTemplate;
using System;

namespace Compression
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// This is the main scenario worker.
        /// </summary>
        /// <param name="Algorithm">
        /// Comression algorithm to use. If no value is provided compressor will be created using
        /// Compressor(IInputStream) constructor, otherwise extended version will be used:
        /// Compressor(IInputStream, CompressAlgorithm, uint)
        /// </param>
        private async void DoScenario(CompressAlgorithm? Algorithm)
        {
            try
            {
                Progress.Text = "";

                // This scenario uses File Picker which doesn't work in snapped mode - try unsnap first
                // and fail gracefully if we can't
                if ((ApplicationView.Value == ApplicationViewState.Snapped) &&
                    !ApplicationView.TryUnsnap())
                {
                    throw new NotSupportedException("Sample doesn't work in snapped mode");
                }
                
                rootPage.NotifyUser("Working...", NotifyType.StatusMessage);

                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add("*");
                var originalFile = await picker.PickSingleFileAsync();
                if (originalFile == null)
                {
                    throw new OperationCanceledException("No file has been selected");
                }

                Progress.Text += String.Format("\"{0}\" has been picked\n", originalFile.Name);

                var compressedFilename = originalFile.Name + ".compressed";
                var compressedFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(compressedFilename, CreationCollisionOption.GenerateUniqueName);
                Progress.Text += String.Format("\"{0}\" has been created to store compressed data\n", compressedFile.Name);

                // ** DO COMPRESSION **
                // Following code actually performs compression from original file to the newly created
                // compressed file. In order to do so it:
                // 1. Opens input for the original file.
                // 2. Opens output stream on the file to be compressed and wraps it into Compressor object.
                // 3. Copies original stream into Compressor wrapper.
                // 4. Finalizes compressor - it puts termination mark into stream and flushes all intermediate
                //    buffers.
                using (var originalInput = await originalFile.OpenReadAsync())
                using (var compressedOutput = await compressedFile.OpenAsync(FileAccessMode.ReadWrite))
                using (var compressor = !Algorithm.HasValue ?
                    new Compressor(compressedOutput.GetOutputStreamAt(0)) :
                    new Compressor(compressedOutput.GetOutputStreamAt(0), Algorithm.Value, 0))
                {
                    Progress.Text += "All streams wired for compression\n";
                    var bytesCompressed = await RandomAccessStream.CopyAsync(originalInput, compressor);
                    var finished = await compressor.FinishAsync();
                    Progress.Text += String.Format("Compressed {0} bytes into {1}\n", bytesCompressed, compressedOutput.Size);
                }

                var decompressedFilename = originalFile.Name + ".decompressed";
                var decompressedFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(decompressedFilename, CreationCollisionOption.GenerateUniqueName);
                Progress.Text += String.Format("\"{0}\" has been created to store decompressed data\n", decompressedFile.Name);

                // ** DO DECOMPRESSION **
                // Following code performs decompression from the just compressed file to the
                // decompressed file. In order to do so it:
                // 1. Opens input stream on compressed file and wraps it into Decompressor object.
                // 2. Opens output stream from the file that will store decompressed data.
                // 3. Copies data from Decompressor stream into decompressed file stream.
                using (var compressedInput = await compressedFile.OpenSequentialReadAsync())
                using (var decompressor = new Decompressor(compressedInput))
                using (var decompressedOutput = await decompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Progress.Text += "All streams wired for decompression\n";
                    var bytesDecompressed = await RandomAccessStream.CopyAsync(decompressor, decompressedOutput);
                    Progress.Text += String.Format("Decompressed {0} bytes of data\n", bytesDecompressed);
                }

                rootPage.NotifyUser("All done", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            DoScenario(null);
        }

        private void Xpress_Click(object sender, RoutedEventArgs e)
        {
            DoScenario(CompressAlgorithm.Xpress);
        }

        private void XpressHuff_Click(object sender, RoutedEventArgs e)
        {
            DoScenario(CompressAlgorithm.XpressHuff);
        }

        private void Mszip_Click(object sender, RoutedEventArgs e)
        {
            DoScenario(CompressAlgorithm.Mszip);
        }

        private void Lzms_Click(object sender, RoutedEventArgs e)
        {
            DoScenario(CompressAlgorithm.Lzms);
        }
    }
}
