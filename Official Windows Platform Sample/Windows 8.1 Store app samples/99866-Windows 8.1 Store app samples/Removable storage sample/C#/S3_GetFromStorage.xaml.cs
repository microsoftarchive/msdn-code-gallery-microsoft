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
using Windows.Devices.Enumeration;
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
    public sealed partial class S3_GetFromStorage : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Contains the device information used for populating the device selection list
        private DeviceInformationCollection _deviceInfoCollection = null;

        public S3_GetFromStorage()
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
            DeviceSelector.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// This is the click handler for the 'Get Image' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void GetImage_Click(object sender, RoutedEventArgs e)
        {
            await ShowDeviceSelectorAsync();
        }

        /// <summary>
        /// Enumerates all storages and populates the device selection list.
        /// </summary>
        async private Task ShowDeviceSelectorAsync()
        {
            _deviceInfoCollection = null;

            // Find all storage devices using Windows.Devices.Enumeration
            _deviceInfoCollection = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
            if (_deviceInfoCollection.Count > 0)
            {
                var items = new List<object>();
                foreach (DeviceInformation deviceInfo in _deviceInfoCollection)
                {
                    items.Add(new
                    {
                        Name = deviceInfo.Name,
                    });
                }
                DeviceList.ItemsSource = items;
                DeviceSelector.Visibility = Visibility.Visible;
            }
            else
            {
                rootPage.NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// This is the tapped handler for the device selection list. It runs the scenario
        /// for the selected storage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void DeviceList_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            DeviceSelector.Visibility = Visibility.Collapsed;
            if (_deviceInfoCollection == null)
            {
                return; // not yet populated
            }

            var deviceInfo = _deviceInfoCollection[DeviceList.SelectedIndex];
            await GetFirstImageFromStorageAsync(deviceInfo);
        }

        /// <summary>
        /// Finds and displays the first image file on the storage referenced by the device information element.
        /// </summary>
        /// <param name="deviceInfoElement">Contains information about a selected device.</param>
        async private Task GetFirstImageFromStorageAsync(DeviceInformation deviceInfoElement)
        {
            // Convert the selected device information element to a StorageFolder
            var storage = StorageDevice.FromId(deviceInfoElement.Id);
            var storageName = deviceInfoElement.Name;

            // Construct the query for image files
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new List<string> { ".jpg", ".png", ".gif" });
            var imageFileQuery = storage.CreateFileQueryWithOptions(queryOptions);

            // Run the query for image files
            rootPage.NotifyUser("Looking for images on " + storageName + " ...", NotifyType.StatusMessage);
            var imageFiles = await imageFileQuery.GetFilesAsync();
            if (imageFiles.Count > 0)
            {
                var imageFile = imageFiles[0];
                rootPage.NotifyUser("Found " + imageFile.Name + " on " + storageName, NotifyType.StatusMessage);
                await DisplayImageAsync(imageFile);
            }
            else
            {
                rootPage.NotifyUser("No images were found on " + storageName + ". You can use scenario 2 to transfer an image to it", NotifyType.StatusMessage);
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
