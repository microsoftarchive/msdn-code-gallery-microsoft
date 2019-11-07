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

using ScanRuntimeAPI.Sample_Utils;
using SDKTemplate;
using SDKTemplate.Common;

namespace ScanRuntimeAPI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class Scenario6ScanFromFeeder : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        CancellationTokenSource cancellationToken;



        public Scenario6ScanFromFeeder()
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
        /// Even Handler for click on Start Scenario button. Starts the scenario for scanning from the Feeder.
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
                    
                rootPage.NotifyUser("Starting scenario of scanning from Feeder.", NotifyType.StatusMessage);		
                // Making scenario running as true before start of scenario
                ModelDataContext.ScenarioRunning = true;
                ScanToFolder(ModelDataContext.ScannerDataContext.CurrentScannerDeviceId, ModelDataContext.DestinationFolder);
            }
        }

        /// <summary>
        /// Scans all the images from Feeder source of the scanner
        /// The scanning of the image is allowed only if the selected scanner has Feeder source
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
                        // Update Feeder Configuration
                        // Set the scan file format to PNG, if available, if not to DIB
                        if (myScanner.FeederConfiguration.IsFormatSupported(ImageScannerFormat.Png))
                        {
                            myScanner.FeederConfiguration.Format = ImageScannerFormat.Png;
                        }
                        else
                        {
                            myScanner.FeederConfiguration.Format = ImageScannerFormat.DeviceIndependentBitmap;
                        }
                        // Set the color mode to Grayscale, if available
                        if (myScanner.FeederConfiguration.IsColorModeSupported(ImageScannerColorMode.Grayscale))
                        {
                            myScanner.FeederConfiguration.ColorMode = ImageScannerColorMode.Grayscale;
                        }
                        // Set feeder to scan duplex
                        myScanner.FeederConfiguration.Duplex = myScanner.FeederConfiguration.CanScanDuplex;
                        // Set MaxNumberOfPages to zero to scanning all the pages that are present in the feeder
                        myScanner.FeederConfiguration.MaxNumberOfPages = 0;
                        
                        rootPage.NotifyUser("Scanning", NotifyType.StatusMessage);

                        var progress = new Progress<UInt32>(ScanProgress);
                        cancellationToken = new CancellationTokenSource();
                        // Scan API call to start scanning from the Feeder source of the scanner.
                        var result = await myScanner.ScanFilesToFolderAsync(ImageScannerScanSource.Feeder, folder).AsTask(cancellationToken.Token, progress);
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
                            rootPage.NotifyUser("There are no files scanned from the Feeder.", NotifyType.ErrorMessage);
                        }
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
        private void ScanProgress(UInt32 numberOfScannedFiles)
        {
            rootPage.NotifyUser("The Number of files scanned so far: " + numberOfScannedFiles, NotifyType.StatusMessage);
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
