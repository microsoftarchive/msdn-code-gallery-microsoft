//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using System.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Devices.Scanners;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;
using Windows.Foundation;

using ScanRuntimeAPI.Sample_Utils;
using SDKTemplate;
using SDKTemplate.Common;

namespace ScanRuntimeAPI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario7MultipleResultsWithProgress : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        CancellationTokenSource cancellationToken;



        public Scenario7MultipleResultsWithProgress()
        {
            this.InitializeComponent();            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!ModelDataContext.ScannerDataContext.WatcherStarted)
            {
                ModelDataContext.ScannerDataContext.StartScannerWatcher();
            }
        }

        /// <summary>
        /// Invoked when user nagivates away from this page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If a scan job is in progress, cancel it now
            CancelScanning();
            ModelDataContext.UnLoad();
        }

        /// <summary>
        /// Even Handler for click on Start Scenario button. Starts the scenario for scanning from the Feeder and getting multiple results with progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartScenario_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                DisplayImage.Source = null;
                ModelDataContext.ClearFileList();

                rootPage.NotifyUser("Starting scenario of scanning from Feeder and getting multiple results with progress.", NotifyType.StatusMessage);		
                // Making scenario running as true before start of scenario
                ModelDataContext.ScenarioRunning = true;
                ScanToFolder(ModelDataContext.ScannerDataContext.CurrentScannerDeviceId, ModelDataContext.DestinationFolder);
            }
        }

        /// <summary>
        /// Scans all the images from Feeder source of the scanner
        /// The scanning is allowed only if the selected scanner is equipped with a Feeder
        /// </summary>
        /// <param name="deviceId">scanner device id</param>
        /// <param name="folder">the folder that receives the scanned files</param>        
        public async void ScanToFolder(string deviceId, StorageFolder folder)
        {
            try
            {
                // Get the scanner object for this device id
                ImageScanner myScanner = await ImageScanner.FromIdAsync(deviceId);
                // Check to see if the use has already cancelled the scenario
                if (ModelDataContext.ScenarioRunning)
                {
                    if (myScanner.IsScanSourceSupported(ImageScannerScanSource.Feeder))
                    {
                        // Set MaxNumberOfPages to zero to scanning all the pages that are present in the feeder
                        myScanner.FeederConfiguration.MaxNumberOfPages = 0;
                        cancellationToken = new CancellationTokenSource();
                        rootPage.NotifyUser("Scanning", NotifyType.StatusMessage);

                        // Scan API call to start scanning from the Feeder source of the scanner.
                        var operation = myScanner.ScanFilesToFolderAsync(ImageScannerScanSource.Feeder, folder);
                        operation.Progress = new AsyncOperationProgressHandler<ImageScannerScanResult, uint>(ScanProgress);
                        var result = await operation.AsTask<ImageScannerScanResult, UInt32>(cancellationToken.Token);

                        // Number of scanned files should be zero here since we already processed during scan progress notifications all the files that have been scanned
                        rootPage.NotifyUser("Scanning is complete.", NotifyType.StatusMessage);
                        ModelDataContext.ScenarioRunning = false;
                    }
                    else
                    {
                        ModelDataContext.ScenarioRunning = false;
                        rootPage.NotifyUser("The selected scanner does not report to be equipped with a Feeder.", NotifyType.ErrorMessage);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Utils.DisplayScanCancelationMessage();
            }
            catch (Exception ex)
            {
                Utils.OnScenarioException(ex, ModelDataContext);
            }
        }
        
        /// <summary>
        /// Event Handler for progress of scanning 
        /// </summary>
        /// <param name="operation">async operation for scanning</param>
        /// <param name="numberOfFiles">The Number of files scanned so far</param>
        private async void ScanProgress(IAsyncOperationWithProgress<ImageScannerScanResult, UInt32> operation, UInt32 numberOfScannedFiles) 
        {
            
            ImageScannerScanResult result = null;
            try
            {
                result = operation.GetResults();
            }
            catch (OperationCanceledException)
            {
                // The try catch is placed here for scenarios in which operation has already been cancelled when progress call is made
                Utils.DisplayScanCancelationMessage();
            }

            if (result != null && result.ScannedFiles.Count > 0)
            {
                IReadOnlyList<StorageFile> fileStorageList = result.ScannedFiles;
                await MainPage.Current.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        StorageFile file = fileStorageList[0];
                        Utils.SetImageSourceFromFile(file, DisplayImage);

                        rootPage.NotifyUser("Scanning is in progress. The Number of files scanned so far: " + numberOfScannedFiles + ". Below is the latest scanned image. \n" +
                        "All the files that have been scanned are saved to local My Pictures folder.", NotifyType.StatusMessage);
                        Utils.UpdateFileListData(fileStorageList, ModelDataContext);
                    }
                ));
            }
        }

       
        /// <summary>
        /// Cancels the current scenario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelScenario_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                CancelScanning();
            }
        }

        /// <summary>
        /// Cancels the current scanning task.
        /// </summary>
        void CancelScanning()
        {
            if (ModelDataContext.ScenarioRunning)
            {
                if (cancellationToken != null)
                {
                    cancellationToken.Cancel();
                }                
                DisplayImage.Source = null;
                ModelDataContext.ScenarioRunning = false;
                ModelDataContext.ClearFileList();
            }
        }

    }
}
