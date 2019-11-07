/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUnivsAppDownloadExtractZip.WindowsPhone
 * Copyright (c) Microsoft Corporation.
 * 
 * This code sample shows how to download and extract zip file in universal Windows apps.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

namespace CSUnvsAppDownloadExtractZip
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFolderPickerContinuable
    {
        private List<DownloadOperation> activeDownloads;
        private CancellationTokenSource cts;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            await DiscoverActiveDownloadsAsync();
        }

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };

        // Enumerate the downloads that were going on in the background while the app was closed.
        private async Task DiscoverActiveDownloadsAsync()
        {
            activeDownloads = new List<DownloadOperation>();

            IReadOnlyList<DownloadOperation> downloads = null;
            try
            {
                downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            }
            catch (Exception ex)
            {
                if (!IsExceptionHandled("Discovery error", ex))
                {
                    throw;
                }
                return;
            }

            Log("Loading background downloads: " + downloads.Count);

            if (downloads.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (DownloadOperation download in downloads)
                {
                    Log(String.Format(System.Globalization.CultureInfo.CurrentCulture,
                        "Discovered background download: {0}, Status: {1}", download.Guid,
                        download.Progress.Status));

                    // Attach progress and completion handlers.
                    tasks.Add(HandleDownloadAsync(download, false));
                }

                // Don't await HandleDownloadAsync() in the foreach loop since we would attach to the second
                // download only when the first one completed; attach to the third download when the second one
                // completes etc. We want to attach to all downloads immediately.
                // If there are actions that need to be taken once downloads complete, await tasks here, outside
                // the loop. 
                await Task.WhenAll(tasks);
            }
        }
        private Uri m_source;
        private void StartDownload()
        {
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.            
            if (!Uri.TryCreate(this.ZipFileUrlTextBox.Text.Trim(), UriKind.Absolute, out m_source))
            {
                NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add(".zip");

            folderPicker.PickFolderAndContinue();

            
        }
        public async void ContinueFolderPicker(Windows.ApplicationModel.Activation.FolderPickerContinuationEventArgs args)
        {
            // In this sample, we just use the default priority.
            // For more information about background transfer, please refer to the SDK Background transfer sample:
            // http://code.msdn.microsoft.com/windowsapps/Background-Transfer-Sample-d7833f61
            BackgroundTransferPriority priority = BackgroundTransferPriority.Default;
            bool requestUnconstrainedDownload = false;
            StorageFolder destinationFolder = args.Folder;
            if (destinationFolder != null)
            {
                // Application now has read/write access to all contents in the picked folder 
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", destinationFolder);
                Log("Picked folder: " + destinationFolder.Name);
            }
            else
            {
                Log("Operation cancelled.");
                return;
            }

            String localFileName = FileNameField.Text.Trim();

            String ext = Path.GetExtension(localFileName);
            if (!String.Equals(ext, ".zip", StringComparison.OrdinalIgnoreCase))
            {
                NotifyUser("Invalid file type. Please make sure the file type is zip.", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                StorageFile localFile = await destinationFolder.CreateFileAsync(localFileName, CreationCollisionOption.GenerateUniqueName);

                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(m_source, localFile);

                Log(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Downloading {0} to {1} with {2} priority, {3}",
                    m_source.AbsoluteUri, destinationFolder.Name, priority, download.Guid));

                download.Priority = priority;

                // In this sample, we do not show how to request unconstrained download.
                // For more information about background transfer, please refer to the SDK Background transfer sample:
                // http://code.msdn.microsoft.com/windowsapps/Background-Transfer-Sample-d7833f61
                if (!requestUnconstrainedDownload)
                {
                    // Attach progress and completion handlers.
                    await HandleDownloadAsync(download, true);

                    StorageFolder unzipFolder =
                        await destinationFolder.CreateFolderAsync(Path.GetFileNameWithoutExtension(localFile.Name),
                        CreationCollisionOption.GenerateUniqueName);

                    await UnZipFileAsync(localFile, unzipFolder);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogStatus(ex.Message, NotifyType.ErrorMessage);
            }
            
        }
        private void StartDownload_Click(object sender, RoutedEventArgs e)
        {            
            StartDownload();
        }

        // Note that this event is invoked on a background thread, so we cannot access the UI directly.
        private void DownloadProgress(DownloadOperation download)
        {
            MarshalLog(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Progress: {0}, Status: {1}", download.Guid,
                download.Progress.Status));

            double percent = 100;
            if (download.Progress.TotalBytesToReceive > 0)
            {
                percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
            }

            MarshalLog(String.Format(System.Globalization.CultureInfo.CurrentCulture, " - Transfered bytes: {0} of {1}, {2}%",
                download.Progress.BytesReceived, download.Progress.TotalBytesToReceive, percent));

            if (download.Progress.HasRestarted)
            {
                MarshalLog(" - Download restarted");
            }

            if (download.Progress.HasResponseChanged)
            {
                // We've received new response headers from the server.
                MarshalLog(" - Response updated; Header count: " + download.GetResponseInformation().Headers.Count);

                // If you want to stream the response data this is a good time to start.
                // download.GetResultStreamAt(0);
            }
        }

        private async Task HandleDownloadAsync(DownloadOperation download, bool start)
        {
            try
            {
                LogStatus("Running: " + download.Guid, NotifyType.StatusMessage);

                // Store the download so we can pause/resume.
                activeDownloads.Add(download);

                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                if (start)
                {
                    // Start the download and attach a progress handler.
                    await download.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The download was already running when the application started, re-attach the progress handler.
                    await download.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = download.GetResponseInformation();

                LogStatus(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Completed: {0}, Status Code: {1}",
                    download.Guid, response.StatusCode), NotifyType.StatusMessage);
            }
            catch (TaskCanceledException)
            {
                LogStatus("Canceled: " + download.Guid, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                if (!IsExceptionHandled("Execution error", ex, download))
                {
                    throw;
                }
            }
            finally
            {
                activeDownloads.Remove(download);
            }
        }

        /// <summary>
        /// Unzips the specified zipfile to a folder.
        /// </summary>
        /// <param name="zipFile">The zip file</param>
        /// <param name="unzipFolder">The destination folder</param>
        /// <returns></returns>
        private async Task UnZipFileAsync(StorageFile zipFile, StorageFolder unzipFolder)
        {
            try
            {
                LogStatus("Unziping file: " + zipFile.DisplayName + "...", NotifyType.StatusMessage);
                await ZipHelper.UnZipFileAsync(zipFile, unzipFolder);
                LogStatus("Unzip file '" + zipFile.DisplayName + "' successfully!", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                LogStatus("Failed to unzip file ..." + ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void PauseAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Downloads: " + activeDownloads.Count);

            foreach (DownloadOperation download in activeDownloads)
            {
                if (download.Progress.Status == BackgroundTransferStatus.Running)
                {
                    download.Pause();
                    Log("Paused: " + download.Guid);
                }
                else
                {
                    Log(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Skipped: {0}, Status: {1}", download.Guid,
                        download.Progress.Status));
                }
            }
        }

        private void ResumeAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Downloads: " + activeDownloads.Count);

            foreach (DownloadOperation download in activeDownloads)
            {
                if (download.Progress.Status == BackgroundTransferStatus.PausedByApplication)
                {
                    download.Resume();
                    Log("Resumed: " + download.Guid);
                }
                else
                {
                    Log(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Skipped: {0}, Status: {1}", download.Guid,
                        download.Progress.Status));
                }
            }
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Canceling Downloads: " + activeDownloads.Count);

            cts.Cancel();
            cts.Dispose();

            // Re-create the CancellationTokenSource and activeDownloads for future downloads.
            cts = new CancellationTokenSource();
            activeDownloads = new List<DownloadOperation>();
        }

        private bool IsExceptionHandled(string title, Exception ex, DownloadOperation download = null)
        {
            WebErrorStatus error = BackgroundTransferError.GetStatus(ex.HResult);
            if (error == WebErrorStatus.Unknown)
            {
                return false;
            }

            if (download == null)
            {
                LogStatus(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error: {0}: {1}", title, error),
                    NotifyType.ErrorMessage);
            }
            else
            {
                LogStatus(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error: {0} - {1}: {2}", download.Guid, title,
                    error), NotifyType.ErrorMessage);
            }

            return true;
        }

        // When operations happen on a background thread we have to marshal UI updates back to the UI thread.
        private void MarshalLog(string value)
        {
            var ignore = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Log(value);
            });
        }

        private void Log(string message)
        {
            outputField.Text += message + "\r\n";
        }

        private void LogStatus(string message, NotifyType type)
        {
            NotifyUser(message, type);
            Log(message);
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                // Use the status message style.
                case NotifyType.StatusMessage:
                    statusText.Style = Resources["StatusStyle"] as Style;
                    break;
                // Use the error message style.
                case NotifyType.ErrorMessage:
                    statusText.Style = Resources["ErrorStyle"] as Style;
                    break;
            }

            statusText.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            if (statusText.Text != String.Empty)
            {
                statusText.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                statusText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        async private void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }        
    }
}
