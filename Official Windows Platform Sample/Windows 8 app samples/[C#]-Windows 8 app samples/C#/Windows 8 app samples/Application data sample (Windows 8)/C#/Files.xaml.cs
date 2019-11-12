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
using SDKTemplate;
using System;
using Windows.Storage;

namespace ApplicationDataSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Files : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        StorageFolder roamingFolder = null;
        int counter = 0;

        const string filename = "sampleFile.txt";

        public Files()
        {
            this.InitializeComponent();

            roamingFolder = ApplicationData.Current.RoamingFolder;

            DisplayOutput();
        }

        // Guidance for Local, Roaming, and Temporary files.
        //
        // Files are ideal for storing large data-sets, databases, or data that is
        // in a common file-format.
        //
        // Files can exist in either the Local, Roaming, or Temporary folders.
        //
        // Roaming files will be synchronized across machines on which the user has
        // singed in with a connected account.  Roaming of files is not instant; the
        // system weighs several factors when determining when to send the data.  Usage
        // of roaming data should be kept below the quota (available via the 
        // RoamingStorageQuota property), or else roaming of data will be suspended.
        // Files cannot be roamed while an application is writing to them, so be sure
        // to close your application's file objects when they are no longer needed.
        //
        // Local files are not synchronized and remain on the machine on which they
        // were originally written.
        //
        // Temporary files are subject to deletion when not in use.  The system 
        // considers factors such as available disk capacity and the age of a file when
        // determining when or whether to delete a temporary file.

        // This sample illustrates reading and writing a file in the Roaming folder, though a
        // Local or Temporary file could be used just as easily.

        async void Increment_Click(Object sender, RoutedEventArgs e)
        {
            counter++;

            StorageFile file = await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, counter.ToString());

            DisplayOutput();
        }

        async void ReadCounter()
        {
            try
            {
                StorageFile file = await roamingFolder.GetFileAsync(filename);
                string text = await FileIO.ReadTextAsync(file);

                OutputTextBlock.Text = "Counter: " + text;

                counter = int.Parse(text);
            }
            catch (Exception)
            {
                OutputTextBlock.Text = "Counter: <not found>";
            }
        }

        void DisplayOutput()
        {
            ReadCounter();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
