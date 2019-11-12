//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LibraryManagement
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
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
        /// Displays the folder Picker to request that the user select a folder that will be added to the
        /// Pictures library.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            StorageLibrary picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            StorageFolder folderAdded = await picturesLibrary.RequestAddFolderAsync();
            if (folderAdded != null)
            {
                OutputTextBlock.Text = folderAdded.DisplayName + " was added to the Pictures library.";
            }
            else
            {
                OutputTextBlock.Text = "Operation canceled.";
            }
        }
    }
}
