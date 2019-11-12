/****************************** Module Header ******************************\
* Module Name:  MainPage.cs
* Project:	    CSWindowsStoreAppFTPDownloader
* Copyright (c) Microsoft Corporation.
* 
* The main UI of this app.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CSWindowsStoreAppFTPDownloader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Common.LayoutAwarePage
    {
        private string serverUrl;
        Stack<string> pathStack;
        NetworkCredential credential;

        public MainPage()
        {
            this.InitializeComponent();
        }


        #region Page methods

        /// <summary>
        ///  Subscribe download completed event.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            FTP.FTPDownloadManager.Instance.DownloadCompleted += FTPItem_DownloadCompleted;
            FTP.FTPDownloadManager.Instance.Initialize();
        }



        /// <summary>
        /// Unsubscribe download completed event.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            FTP.FTPDownloadManager.Instance.DownloadCompleted -= FTPItem_DownloadCompleted;
        }

        #endregion

        #region UI event handlers

        /// <summary>
        /// Connect to FTP server and list the sub folders and file.
        /// </summary>
        private void ConnectFTPServerButton_Click(object sender, RoutedEventArgs e)
        {
            Uri serverUri = null;
            string serverUriStr = serverName.Text.Trim();

            if (Uri.TryCreate(serverUriStr, UriKind.Absolute, out serverUri))
            {
                serverUrl = serverUri.ToString();
                pathStack = new Stack<string>();

                if (!string.IsNullOrEmpty(userName.Text.Trim()) &&
                   !string.IsNullOrEmpty(password.Password.Trim()))
                {
                    credential = new System.Net.NetworkCredential(userName.Text.Trim(),
                        password.Password.Trim());
                }
                else
                {
                    credential = null;
                }

                // List the sub folders and file.
                ListDirectory();
            }
            else
            {
                NotifyUser(serverUriStr + " is not a valid FTP server");
            }

        }

        /// <summary>
        /// When use click an item:
        /// 1. If it is a directory, navigate to the sub folder.
        /// 2. If it is a file, select it.
        /// </summary>
        private void itemsView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (DataModel.SampleDataItem)e.ClickedItem;
            if (clickedItem.Content.IsDirectory)
            {
                this.pathStack.Push(clickedItem.Content.Name);
                this.ListDirectory();
            }
            else
            {
                (sender as ListViewBase).SelectedItems.Clear();
                (sender as ListViewBase).SelectedItems.Add(
                    (sender as ListViewBase).Items.First(i => i == clickedItem));
            }
        }

        /// <summary>
        /// Back to parent folder.
        /// </summary>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.pathStack.Count > 0)
            {
                this.pathStack.Pop();
                this.ListDirectory();
            }
        }

        private bool isSyncing = false;

        /// <summary>
        /// When selection changed
        /// 1. Sync the selection of GridView and ListView.
        /// 2. Show the app bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isSyncing)
            {
                return;
            }

            isSyncing = true;
            if (sender == itemGridView)
            {
                itemListView.SelectedItems.Clear();
                foreach (var item in itemGridView.SelectedItems)
                {
                    itemListView.SelectedItems.Add(itemListView.Items.First(i => i == item));
                }
            }
            else if (sender == itemListView)
            {
                itemGridView.SelectedItems.Clear();
                foreach (var item in itemListView.SelectedItems)
                {
                    itemGridView.SelectedItems.Add(itemGridView.Items.First(i => i == item));
                }
            }
            isSyncing = false;

            downloadButton.Visibility = (sender as ListViewBase).SelectedItems.Count > 0 ?
                Visibility.Visible : Visibility.Collapsed;
            manageAppBar.IsOpen = true;
        }

        /// <summary>
        /// Download the selected items.
        /// </summary>
        private async void downloadButton_Click(object sender, RoutedEventArgs e)
        {

            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) ||
                ApplicationView.TryUnsnap());

            if (!unsnapped)
            {
                NotifyUser("Cannot unsnap the sample.");
                return;
            }


            // Select a folder as target.
            Windows.Storage.Pickers.FolderPicker picker = new Windows.Storage.Pickers.FolderPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {

                // Download the items.
                var itemsToDownload = itemGridView.SelectedItems.Select(i => (i as DataModel.SampleDataItem).Content);
                FTP.FTPDownloadManager.Instance.DownloadFTPItemsAsync(
                    itemsToDownload, folder, credential);
            }

            // Clear the selection.
            itemGridView.SelectedItems.Clear();
            itemListView.SelectedItems.Clear();
        }

        /// <summary>
        /// Refresh the explorer.
        /// </summary>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(serverUrl) && pathStack != null)
            {
                this.ListDirectory();
            }
        }

        #endregion

        /// <summary>
        /// Show toast notification when a file is downloaded.
        /// </summary>
        async void FTPItem_DownloadCompleted(object sender, FTP.DownloadCompletedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    if (e.Error == null)
                    {
                        progressInfo.Items.Insert(0, string.Format("{0} is completed.",
                            e.RequestFile.ToString()));
                    }
                    else
                    {
                        progressInfo.Items.Insert(0, string.Format("{0} is not completed: {1}.", 
                            e.RequestFile.ToString(),
                            e.Error.Message));
                    }
                    progressInfo.SelectedIndex = 0;
                }
            );
        }

        #region FTP methods

        /// <summary>
        ///  List the sub folders and file.
        ///  Generate the data source and then bind to the Controls.
        /// </summary>
        async void ListDirectory()
        {
            NotifyUser("");
            getDataProgress.Visibility = Windows.UI.Xaml.Visibility.Visible;
            string relativePath = GenerateRelativePath(pathStack);

            try
            {
                IEnumerable<FTP.FTPFileSystem> items = await FTP.FTPDownloadManager.Instance.ListFtpContentAsync(
                    serverUrl + relativePath, credential);
                DataModel.FTPFileDataSource source = new DataModel.FTPFileDataSource(items);
                this.DefaultViewModel["Groups"] = source.AllGroups;
            }
            catch (Exception ex)
            {
                this.DefaultViewModel["Groups"] = null;
                NotifyUser(ex.Message);
            }

            this.getDataProgress.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.backButton.IsEnabled = this.pathStack.Count > 0;
            this.ftpPath.Text = relativePath;
        }

        /// <summary>
        /// Generate the current relative path.
        /// </summary>
        /// <param name="pathStack"></param>
        /// <returns></returns>
        string GenerateRelativePath(IEnumerable<string> pathStack)
        {
            StringBuilder relativePath = new StringBuilder("/");
            foreach (var path in pathStack.Reverse())
            {
                string encodedUrl = FTP.FTPFileSystem.EncodeUrl(path);
                relativePath.AppendFormat("{0}/", encodedUrl);
            }

            return relativePath.ToString();
        }

        #endregion

        #region Common methods

        async private void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }

        public void NotifyUser(string message)
        {
            textStatus.Text = message;
        }

        #endregion

    }
}
