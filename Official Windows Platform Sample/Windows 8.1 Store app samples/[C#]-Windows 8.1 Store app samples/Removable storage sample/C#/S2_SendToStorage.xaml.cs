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
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Portable.RemovableStorageSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S2_SendToStorage : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Contains the device information used for populating the device selection list
        private DeviceInformationCollection _deviceInfoCollection = null;

        public S2_SendToStorage()
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
        /// This is the click handler for the 'Send Image' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void SendImage_Click(object sender, RoutedEventArgs e)
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
        /// This is the tapped handler for the device selection list. It runs this scenario
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
            await SendImageFileToStorageAsync(deviceInfo);
        }

        /// <summary>
        /// Sends a user-selected image file to the storage referenced by the device information element.
        /// </summary>
        /// <param name="deviceInfoElement">Contains information about a selected device.</param>
        async private Task SendImageFileToStorageAsync(DeviceInformation deviceInfoElement)
        {
            // Launch the picker to select an image file
            var picker = new FileOpenPicker
            {
                FileTypeFilter = { ".jpg", ".png", ".gif" },
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            var sourceFile = await picker.PickSingleFileAsync();
            if (sourceFile != null)
            {
                // Convert the selected device information element to a StorageFolder
                var storage = StorageDevice.FromId(deviceInfoElement.Id);
                var storageName = deviceInfoElement.Name;

                rootPage.NotifyUser("Copying image: " + sourceFile.Name + " to " + storageName + " ...", NotifyType.StatusMessage);
                await CopyFileToFolderOnStorageAsync(sourceFile, storage);
            }
            else
            {
                rootPage.NotifyUser("No file was selected", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// Copies a file to the first folder on a storage.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="storage"></param>
        async private Task CopyFileToFolderOnStorageAsync(StorageFile sourceFile, StorageFolder storage)
        {
            var storageName = storage.Name;

            // Construct a folder search to find sub-folders under the current storage.
            // The default (shallow) query should be sufficient in finding the first level of sub-folders.
            // If the first level of sub-folders are not writable, a deep query + recursive copy may be needed.
            var folders = await storage.GetFoldersAsync();
            if (folders.Count > 0)
            {
                var destinationFolder = folders[0];
                var destinationFolderName = destinationFolder.Name;

                rootPage.NotifyUser("Trying the first sub-folder: " + destinationFolderName + "...", NotifyType.StatusMessage);
                try
                {
                    var newFile = await sourceFile.CopyAsync(destinationFolder, sourceFile.Name, NameCollisionOption.GenerateUniqueName);
                    rootPage.NotifyUser("Image " + newFile.Name + " created in folder: " + destinationFolderName + " on " + storageName, NotifyType.StatusMessage);
                }
                catch (Exception e)
                {
                    rootPage.NotifyUser("Failed to copy image to the first sub-folder: " + destinationFolderName + ", " + storageName + " may not allow sending files to its top level folders. Error: " + e.Message, NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("No sub-folders found on " + storageName + " to copy to", NotifyType.StatusMessage);
            }
        }
    }
}
