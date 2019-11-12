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
    public sealed partial class Scenario5ScanFromFlatbed : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        CancellationTokenSource cancellationToken;

        public Scenario5ScanFromFlatbed()
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
        /// Even Handler for click on Start Scenario button. Starts the scenario for scanning from the Faltbed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartScenario_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                DisplayImage.Source = null;

                rootPage.NotifyUser("Starting scenario of scanning from Flatbed.", NotifyType.StatusMessage);		
                // Making scenario running as true before start of scenario
                ModelDataContext.ScenarioRunning = true;
                ScanToFolder(ModelDataContext.ScannerDataContext.CurrentScannerDeviceId, ModelDataContext.DestinationFolder);
            }
        }

        /// <summary>
        /// Scans image from the Flatbed source of the scanner
        /// The scanning is allowed only if the selected scanner is equipped with a Flatbed
        /// </summary>
        /// <param name="deviceId">scanner device id</param>
        /// <param name="folder">the folder that receives the scanned files</param>
        public async void ScanToFolder(string deviceId, StorageFolder folder)
        {
            try
            {
                // Get the scanner object for this device id
                ImageScanner myScanner = await ImageScanner.FromIdAsync(deviceId);
                // Check to see if the user has already canceled the scenario
                if (ModelDataContext.ScenarioRunning)
                {
                    if (myScanner.IsScanSourceSupported(ImageScannerScanSource.Flatbed))
                    {
                        // Set the scan file format to Device Independent Bitmap (DIB)
                        myScanner.FlatbedConfiguration.Format = ImageScannerFormat.DeviceIndependentBitmap;

                        cancellationToken = new CancellationTokenSource();

                        rootPage.NotifyUser("Scanning", NotifyType.StatusMessage);
                        // Scan API call to start scanning from the Flatbed source of the scanner.
                        var result = await myScanner.ScanFilesToFolderAsync(ImageScannerScanSource.Flatbed, folder).AsTask(cancellationToken.Token);
                        ModelDataContext.ScenarioRunning = false;
                        if (result.ScannedFiles.Count > 0)
                        {
                            Utils.DisplayImageAndScanCompleteMessage(result.ScannedFiles, DisplayImage);
                        }
                        else
                        {
                            rootPage.NotifyUser("There are no files scanned from the Flatbed.", NotifyType.ErrorMessage);
                        }
                    }
                    else
                    {
                        ModelDataContext.ScenarioRunning = false;
                        rootPage.NotifyUser("The selected scanner does not report to be equipped with a Flatbed.", NotifyType.ErrorMessage);
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
