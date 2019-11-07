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
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Portable.RemovableStorageSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S1_ListStorages : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public S1_ListStorages()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This is the click handler for the 'List Storages' button.
        /// </summary>
        /// <remarks>
        /// There are two ways to find removable storages:
        /// The first way uses the Removable Devices KnownFolder to get a snapshot of the currently
        /// connected devices as StorageFolders.  This is demonstrated in this scenario.
        /// The second way uses Windows.Devices.Enumeration and is demonstrated in the second scenario.
        /// Windows.Devices.Enumeration supports more advanced scenarios such as subscibing for device
        /// arrival, removal and updates. Refer to the DeviceEnumeration sample for details on
        /// Windows.Devices.Enumeration.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void ListStorages_Click(object sender, RoutedEventArgs e)
        {
            ScenarioOutput.Text = "";

            // Find all storage devices using the known folder
            var removableStorages = await KnownFolders.RemovableDevices.GetFoldersAsync();
            if (removableStorages.Count > 0)
            {
                // Display each storage device
                foreach (StorageFolder storage in removableStorages)
                {
                    ScenarioOutput.Text += storage.DisplayName + "\n";
                }
            }
            else
            {
                rootPage.NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType.StatusMessage);
            }
        }
    }
}
