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

using ScanRuntimeAPI.Sample_Utils;
using SDKTemplate;



namespace ScanRuntimeAPI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2JustScan : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        CancellationTokenSource cancellationToken;

        public Scenario2JustScan()
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
        /// Event Handler for click on Start Scenario button. Starts the scenario of Just Scan.
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

                rootPage.NotifyUser("Starting scenario of Just Scan.", NotifyType.StatusMessage);
                // Making scenario running as true before start of scenario
                ModelDataContext.ScenarioRunning = true;
                ScanToFolder(ModelDataContext.ScannerDataContext.CurrentScannerDeviceId, ModelDataContext.DestinationFolder);
            }
        }

        /// <summary>
        /// Scans the images from the scanner with default settings
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
                    cancellationToken = new CancellationTokenSource();

                    rootPage.NotifyUser("Scanning", NotifyType.StatusMessage);
                    var progress = new Progress<UInt32>(ScanProgress);
                    // Scan API call to start scanning 
                    var result = await myScanner.ScanFilesToFolderAsync(ImageScannerScanSource.Default, folder).AsTask(cancellationToken.Token, progress);
                    ModelDataContext.ScenarioRunning = false;
                    if (result.ScannedFiles.Count > 0)
                    {
                        Utils.DisplayImageAndScanCompleteMessage(result.ScannedFiles, DisplayImage);
                        if (result.ScannedFiles.Count > 1)
                        {
                            Utils.UpdateFileListData(result.ScannedFiles, ModelDataContext);
                        }
                    }
                    else
                    {
                        rootPage.NotifyUser("There were no files scanned.", NotifyType.ErrorMessage);
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
        private void ScanProgress(UInt32 numberOfScannedFiles)
        {
            rootPage.NotifyUser("The number of files scanned so far:" + numberOfScannedFiles, NotifyType.StatusMessage);
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
