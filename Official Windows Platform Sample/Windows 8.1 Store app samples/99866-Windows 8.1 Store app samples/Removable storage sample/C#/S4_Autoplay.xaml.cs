//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Portable;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Portable.RemovableStorageSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S4_Autoplay : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public S4_Autoplay()
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
            // Enable the button only when launched from Autoplay or File Activation
            ScenarioInput.IsEnabled = (rootPage.AutoplayFileSystemDeviceFolder != null ||
                                       rootPage.AutoplayNonFileSystemDeviceId  != null ||
                                       rootPage.FileActivationFiles            != null);
        }

        /// <summary>
        /// This is the click handler for the 'Get Image' button.
        /// If launched by Autoplay, this will find and display the first image file on the storage.
        /// If launched by file activation, this will display the first image file from the activation file list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void GetImage_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.FileActivationFiles != null)
            {
                if (rootPage.FileActivationFiles.Count > 0)
                {
                    // Because this sample only supports image file types in its manifest,
                    // we can know that all files in the array of files will be image files.
                    rootPage.NotifyUser("[File Activation] Displaying first image file ...", NotifyType.StatusMessage);
                    var imageFile = rootPage.FileActivationFiles[0] as StorageFile; // Pick the first file to display
                    await DisplayImageAsync(imageFile);
                }
                else
                {
                    rootPage.NotifyUser("[File Activation] File activation occurred but 0 files were received", NotifyType.ErrorMessage);
                }
            }
            else
            {
                if (rootPage.AutoplayFileSystemDeviceFolder != null)
                {
                    await GetFirstImageFromStorageAsync(rootPage.AutoplayFileSystemDeviceFolder);
                }
                else
                {
                    var storage = StorageDevice.FromId(rootPage.AutoplayNonFileSystemDeviceId);
                    await GetFirstImageFromStorageAsync(storage);
                }
            }
        }

        /// <summary>
        /// Finds and displays the first image file on the storage.
        /// </summary>
        /// <param name="storage"></param>
        async private Task GetFirstImageFromStorageAsync(StorageFolder storage)
        {
            var storageName = storage.Name;

            // Construct the query for image files
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new List<string> { ".jpg", ".png", ".gif" });
            var imageFileQuery = storage.CreateFileQueryWithOptions(queryOptions);

            // Run the query for image files
            rootPage.NotifyUser("[Launched by Autoplay] Looking for images on " + storageName + " ...", NotifyType.StatusMessage);
            var imageFiles = await imageFileQuery.GetFilesAsync();
            if (imageFiles.Count > 0)
            {
                var imageFile = imageFiles[0];
                rootPage.NotifyUser("[Launched by Autoplay] Found " + imageFile.Name + " on " + storageName, NotifyType.StatusMessage);
                await DisplayImageAsync(imageFile);
            }
            else
            {
                rootPage.NotifyUser("[Launched by Autoplay] No images were found on " + storageName + ". You can use scenario 2 to transfer an image to it", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// Displays an image file in the 'ScenarioOutputImage' element.
        /// </summary>
        /// <param name="imageFile">The image file to display.</param>
        async private Task DisplayImageAsync(StorageFile imageFile)
        {
            var imageProperties = await imageFile.GetBasicPropertiesAsync();
            if (imageProperties.Size > 0)
            {
                rootPage.NotifyUser("Displaying: " + imageFile.Name + ", date modified: " + imageProperties.DateModified + ", size: " + imageProperties.Size + " bytes", NotifyType.StatusMessage);
                var stream = await imageFile.OpenAsync(FileAccessMode.Read);

                // BitmapImage.SetSource needs to be called in the UI thread
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    ScenarioOutputImage.SetValue(Image.SourceProperty, bitmap);
                });
            }
            else
            {
                rootPage.NotifyUser("Cannot display " + imageFile.Name + " because its size is 0", NotifyType.ErrorMessage);
            }
        }
    }
}
