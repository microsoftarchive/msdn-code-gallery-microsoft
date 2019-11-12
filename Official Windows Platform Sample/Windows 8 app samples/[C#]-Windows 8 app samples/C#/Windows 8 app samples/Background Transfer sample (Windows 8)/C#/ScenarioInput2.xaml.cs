// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Pickers;
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
    public sealed partial class ScenarioInput2 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        private MainPage rootPage = null;
        private TextBox outputField = null;

        private CancellationTokenSource cts;

        public ScenarioInput2()
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

            // An application must enumerate uploads when it gets started to prevent stale downloads/uploads.
            // Typically this can be done in the App class by overriding OnLaunched() and checking for
            // "args.Kind == ActivationKind.Launch" to detect an actual app launch.
            // We do it here in the sample to keep the sample code consolidated.
            await DiscoverActiveUploadsAsync();
        }

        // Enumerate the uploads that were going on in the background while the app was closed.
        private async Task DiscoverActiveUploadsAsync()
        {
            IReadOnlyList<UploadOperation> uploads = null;
            try
            {
                uploads = await BackgroundUploader.GetCurrentUploadsAsync();
            }
            catch (Exception ex)
            {
                if (!IsExceptionHandled("Discovery error", ex))
                {
                    throw;
                }
                return;
            }

            Log("Loading background uploads: " + uploads.Count);

            if (uploads.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (UploadOperation upload in uploads)
                {
                    Log(String.Format("Discovered background upload: {0}, Status: {1}", upload.Guid, upload.Progress.Status));

                    // Attach progress and completion handlers.
                    tasks.Add(HandleUploadAsync(upload, false));
                }

                // Don't await HandleUploadAsync() in the foreach loop since we would attach to the second
                // upload only when the first one completed; attach to the third upload when the second one
                // completes etc. We want to attach to all uploads immediately.
                // If there are actions that need to be taken once uploads complete, await tasks here, outside
                // the loop.
                await Task.WhenAll(tasks);
            }
        }

        private async void StartUpload_Click(object sender, RoutedEventArgs e)
        {
            // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
            // box validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            Uri uri;
            if (!Uri.TryCreate(serverAddressField.Text.Trim(), UriKind.Absolute, out uri))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            // Verify that we are currently not snapped, or that we can unsnap to open the picker.
            if (ApplicationView.Value == ApplicationViewState.Snapped && !ApplicationView.TryUnsnap())
            {
                rootPage.NotifyUser("File picker cannot be opened in snapped mode. Please unsnap first.", NotifyType.ErrorMessage);
                return;
            }

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            StorageFile file = await picker.PickSingleFileAsync();

            if (file == null)
            {
                rootPage.NotifyUser("No file selected.", NotifyType.ErrorMessage);
                return;
            }

            BackgroundUploader uploader = new BackgroundUploader();
            uploader.SetRequestHeader("Filename", file.Name);

            UploadOperation upload = uploader.CreateUpload(uri, file);
            Log(String.Format("Uploading {0} to {1}, {2}", file.Name, uri.AbsoluteUri, upload.Guid));

            // Attach progress and completion handlers.
            await HandleUploadAsync(upload, true);
        }

        private async void StartMultipartUpload_Click(object sender, RoutedEventArgs e)
        {
            // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
            // box validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            Uri uri;
            if (!Uri.TryCreate(serverAddressField.Text.Trim(), UriKind.Absolute, out uri))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            // Verify that we are currently not snapped, or that we can unsnap to open the picker.
            if (ApplicationView.Value == ApplicationViewState.Snapped && !ApplicationView.TryUnsnap())
            {
                rootPage.NotifyUser("File picker cannot be opened in snapped mode. Please unsnap first.", NotifyType.ErrorMessage);
                return;
            }

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();

            if (files.Count == 0)
            {
                rootPage.NotifyUser("No file selected.", NotifyType.ErrorMessage);
                return;
            }

            List<BackgroundTransferContentPart> parts = new List<BackgroundTransferContentPart>();
            for (int i = 0; i < files.Count; i++)
            {
                BackgroundTransferContentPart part = new BackgroundTransferContentPart("File" + i, files[i].Name);
                part.SetFile(files[i]);
                parts.Add(part);
            }

            BackgroundUploader uploader = new BackgroundUploader();
            UploadOperation upload = await uploader.CreateUploadAsync(uri, parts);

            String fileNames = files[0].Name;
            for (int i = 1; i < files.Count; i++)
            {
                fileNames += ", " + files[i].Name;
            }

            Log(String.Format("Uploading {0} to {1}, {2}", fileNames, uri.AbsoluteUri, upload.Guid));

            // Attach progress and completion handlers.
            await HandleUploadAsync(upload, true);
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Canceling all active uploads");

            cts.Cancel();
            cts.Dispose();

            // Re-create the CancellationTokenSource and activeUploads for future uploads.
            cts = new CancellationTokenSource();
        }

        // Note that this event is invoked on a background thread, so we cannot access the UI directly.
        private void UploadProgress(UploadOperation upload)
        {
            MarshalLog(String.Format("Progress: {0}, Status: {1}", upload.Guid, upload.Progress.Status));

            BackgroundUploadProgress progress = upload.Progress;

            double percentSent = 100;
            if (progress.TotalBytesToSend > 0)
            {
                percentSent = progress.BytesSent * 100 / progress.TotalBytesToSend;
            }

            MarshalLog(String.Format(" - Sent bytes: {0} of {1} ({2}%), Received bytes: {3} of {4}",
                progress.BytesSent, progress.TotalBytesToSend, percentSent,
                progress.BytesReceived, progress.TotalBytesToReceive));

            if (progress.HasRestarted)
            {
                MarshalLog(" - Upload restarted");
            }

            if (progress.HasResponseChanged)
            {
                // We've received new response headers from the server.
                MarshalLog(" - Response updated; Header count: " + upload.GetResponseInformation().Headers.Count);

                // If you want to stream the response data this is a good time to start.
                // upload.GetResultStreamAt(0);
            }
        }

        private async Task HandleUploadAsync(UploadOperation upload, bool start)
        {
            try
            {
                LogStatus("Running: " + upload.Guid, NotifyType.StatusMessage);

                Progress<UploadOperation> progressCallback = new Progress<UploadOperation>(UploadProgress);
                if (start)
                {
                    // Start the upload and attach a progress handler.
                    await upload.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The upload was already running when the application started, re-attach the progress handler.
                    await upload.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = upload.GetResponseInformation();

                LogStatus(String.Format("Completed: {0}, Status Code: {1}", upload.Guid, response.StatusCode),
                    NotifyType.StatusMessage);
            }
            catch (TaskCanceledException)
            {
                LogStatus("Canceled: " + upload.Guid, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                if (!IsExceptionHandled("Error", ex, upload))
                {
                    throw;
                }
            }
        }

        private bool IsExceptionHandled(string title, Exception ex, UploadOperation upload = null)
        {
            WebErrorStatus error = BackgroundTransferError.GetStatus(ex.HResult);
            if (error == WebErrorStatus.Unknown)
            {
                return false;
            }

            if (upload == null)
            {
                LogStatus(String.Format("Error: {0}: {1}", title, error), NotifyType.ErrorMessage);
            }
            else
            {
                LogStatus(String.Format("Error: {0} - {1}: {2}", upload.Guid, title, error), NotifyType.ErrorMessage);
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
