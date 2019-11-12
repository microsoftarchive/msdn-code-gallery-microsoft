/***************************** Module Header ******************************\
* Module Name:	MainPage.xaml.cs
* Project:		CSUnvsAppAzureBlob.Windows
* Copyright (c) Microsoft Corporation.
* 
* This sample will show you how to operate Azure blob storage in universal Windows apps, 
* including upload/download/delete file from blob storage.
*
* MainPage.xaml.cs
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace CSUnvsAppAzureBlob
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ProcessBegin()
        {
            processRing.IsActive = true;
            btnSave.IsEnabled = false;
            btnDelete.IsEnabled = false;
            lvwBlobs.IsEnabled = false;
            statusText.Text = "";
        }

        private void ProcessEnd()
        {
            processRing.IsActive = false;
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
            lvwBlobs.IsEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ProcessBegin();
            refreshListview();
        }

        /// <summary>
        /// Refresh the Listview.
        /// </summary>
        private async void refreshListview()
        {
            try
            {
                await App.container.CreateIfNotExistsAsync();
                BlobContinuationToken token = null;
                var tast = App.container.ListBlobsSegmentedAsync(token);
                var blobsSegmented = await tast;
                tast.Completed = (IAsyncOperation<BlobResultSegment> asyncInfo, AsyncStatus asyncStatus) =>
                {
                    ProcessEnd();
                };
                lvwBlobs.ItemsSource = blobsSegmented.Results;
            }
            catch (Exception ex)
            {
                statusText.Text = (ex.Message + "\n");
            }
        }

        /// <summary>
        /// Select a image file, and save it to Azure blob.
        /// </summary>
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            ProcessBegin();
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                using (var fileStream = await file.OpenSequentialReadAsync())
                {
                    try
                    {
                        await App.container.CreateIfNotExistsAsync();
                        var blob = App.container.GetBlockBlobReference(file.Name);
                        await blob.DeleteIfExistsAsync();
                        await blob.UploadFromStreamAsync(fileStream);
                        
                        statusText.Text = DateTime.Now.ToString() + ": Save picture '" + file.Name + "' successfully!\n";
                        refreshListview();
                    }
                    catch (Exception ex)
                    {
                        statusText.Text = (ex.Message + "\n");
                    }
                }
            }
        }

        /// <summary>
        /// Delete the Item in blob.
        /// </summary>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvwBlobs.SelectedIndex != -1)
            {
                var item = lvwBlobs.SelectedItem as CloudBlockBlob;
                var blob = App.container.GetBlockBlobReference(item.Name);
                try
                {
                    ProcessBegin();
                    await blob.DeleteIfExistsAsync();
                    imgBlobItem.Source = null;
                }
                catch (Exception ex)
                {
                    statusText.Text = (ex.Message + "\n");
                }

                refreshListview();
                statusText.Text = DateTime.Now.ToString() + item.Name + " has been removed from blob\n";
            }
            else
            {
                statusText.Text = "No item selected.";
            }
        }


        /// <summary>
        /// Download file from blob, save it to tempfolder and show it in screen.
        /// </summary>
        private async void lvwBlobs_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as CloudBlockBlob;
            var blob = App.container.GetBlockBlobReference(item.Name);
            StorageFile file;
            try
            {
                Windows.Storage.StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
                file = await temporaryFolder.CreateFileAsync(item.Name,
                   CreationCollisionOption.ReplaceExisting);

                ProcessBegin();

                var downloadTask = blob.DownloadToFileAsync(file);

                await downloadTask;

                downloadTask.Completed = (IAsyncAction asyncInfo, AsyncStatus asyncStatus) =>
                {
                    ProcessEnd();
                };
       
                // Make sure it's an image file.
                imgBlobItem.Source = new BitmapImage(new Uri(file.Path));
            }
            catch (Exception ex)
            {
                statusText.Text = (ex.Message + "\n");
            }
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 800.0)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width < e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
        }
    }
}
