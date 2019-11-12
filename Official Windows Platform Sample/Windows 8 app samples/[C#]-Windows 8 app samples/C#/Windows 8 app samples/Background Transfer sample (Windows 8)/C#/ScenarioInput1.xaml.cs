// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

namespace Microsoft.Samples.Networking.BackgroundTransfer
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed partial class ScenarioInput1 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        private MainPage rootPage = null;
        private TextBox outputField = null;

        private List<DownloadOperation> activeDownloads;
        private CancellationTokenSource cts;

        public ScenarioInput1()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        async void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            outputField = outputFrame.FindName("outputField") as TextBox;

            // An application must enumerate downloads when it gets started to prevent stale downloads/uploads.
            // Typically this can be done in the App class by overriding OnLaunched() and checking for
            // "args.Kind == ActivationKind.Launch" to detect an actual app launch.
            // We do it here in the sample to keep the sample code consolidated.
            await DiscoverActiveDownloadsAsync();
        }

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
                    Log(String.Format("Discovered background download: {0}, Status: {1}", download.Guid,
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

        private async void StartDownload_Click(object sender, RoutedEventArgs e)
        {
            // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
            // box validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            Uri source;
            if (!Uri.TryCreate(serverAddressField.Text.Trim(), UriKind.Absolute, out source))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            string destination = fileNameField.Text.Trim();

            if (string.IsNullOrWhiteSpace(destination))
            {
                rootPage.NotifyUser("A local file name is required.", NotifyType.ErrorMessage);
                return;
            }

            StorageFile destinationFile;
            try
            {
                destinationFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                    destination, CreationCollisionOption.GenerateUniqueName);
            }
            catch (FileNotFoundException ex)
            {
                rootPage.NotifyUser("Error while creating file: " + ex.Message, NotifyType.ErrorMessage);
                return;
            }

            BackgroundDownloader downloader = new BackgroundDownloader();
            DownloadOperation download = downloader.CreateDownload(source, destinationFile);

            Log(String.Format("Downloading {0} to {1}, {2}", source.AbsoluteUri, destinationFile.Name, download.Guid));

            // Attach progress and completion handlers.
            await HandleDownloadAsync(download, true);
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
                    Log(String.Format("Skipped: {0}, Status: {1}", download.Guid, download.Progress.Status));
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
                    Log(String.Format("Skipped: {0}, Status: {1}", download.Guid, download.Progress.Status));
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

        // Note that this event is invoked on a background thread, so we cannot access the UI directly.
        private void DownloadProgress(DownloadOperation download)
        {
            MarshalLog(String.Format("Progress: {0}, Status: {1}", download.Guid, download.Progress.Status));

            double percent = 100;
            if (download.Progress.TotalBytesToReceive > 0)
            {
                percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
            }

            MarshalLog(String.Format(" - Transfered bytes: {0} of {1}, {2}%",
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

                LogStatus(String.Format("Completed: {0}, Status Code: {1}", download.Guid, response.StatusCode),
                    NotifyType.StatusMessage);
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

        private bool IsExceptionHandled(string title, Exception ex, DownloadOperation download = null)
        {
            WebErrorStatus error = BackgroundTransferError.GetStatus(ex.HResult);
            if (error == WebErrorStatus.Unknown)
            {
                return false;
            }

            if (download == null)
            {
                LogStatus(String.Format("Error: {0}: {1}", title, error), NotifyType.ErrorMessage);
            }
            else
            {
                LogStatus(String.Format("Error: {0} - {1}: {2}", download.Guid, title, error), NotifyType.ErrorMessage);
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
            rootPage.NotifyUser(message, type);
            Log(message);
        }
    }
}
