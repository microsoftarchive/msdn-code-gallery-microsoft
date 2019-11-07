//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Devices.PointOfService;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Text;

namespace BarcodeScannerCS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        BarcodeScanner scanner = null;
        ClaimedBarcodeScanner claimedScanner = null;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Creates the default barcode scanner.
        /// </summary>
        /// <returns>true if barcode scanner is created. Otherwise returns false</returns>
        private async Task<bool> CreateDefaultScannerObject()
        {
            if (scanner == null)
            {
                UpdateOutput("Creating Barcode Scanner object.");
                scanner = await BarcodeScanner.GetDefaultAsync();

                if (scanner != null)
                {
                    UpdateOutput("Default Barcode Scanner created.");
                    UpdateOutput("Device Id is:" + scanner.DeviceId);
                }
                else
                {
                    UpdateOutput("Barcode Scanner not found. Please connect a Barcode Scanner.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method claims the barcode scanner 
        /// </summary>
        /// <returns>true if claim is successful. Otherwise returns false</returns>
        private async Task<bool> ClaimScanner()
        {
            if (claimedScanner == null)
            {
                // claim the barcode scanner
                claimedScanner = await scanner.ClaimScannerAsync();

                // enable the claimed barcode scanner
                if (claimedScanner != null)
                {
                    UpdateOutput("Claim Barcode Scanner succeeded.");
                   
                }
                else
                {
                    UpdateOutput("Claim Barcode Scanner failed.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method claims the barcode scanner and enables it 
        /// </summary>
        /// <returns>true if enable is successful</returns>
        private async Task<bool> EnableScanner()
        {
           // enable the claimed barcode scanner
            if (claimedScanner == null)
            {
                return false;   
            }
            else
            {   
                await claimedScanner.EnableAsync();
                
                UpdateOutput("Enable Barcode Scanner succeeded.");
                
                return true;
            }            
          
        }


        /// <summary>
        /// Update the status in the UI with the string passed.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateOutput(string message)
        {
            StatusBlock.Text += message;
            StatusBlock.Text += Environment.NewLine;
        }

      
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ResetTheScenarioState();
        }

        /// <summary>
        /// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame
        /// </summary>
        /// <param name="e">Provides data for the OnNavigatingFrom callback that can be used to cancel a navigation 
        /// request from origination</param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ResetTheScenarioState();
        }

        /// <summary>
        /// Event Handler for Start Scan Button Click.
        /// Sets up the barcode scanner to be ready to receive the data events from the scan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScenarioStartScanButton_Click(object sender, RoutedEventArgs e)
        {
            // create the barcode scanner. 
            if (await CreateDefaultScannerObject())
            {
                // after successful creation, claim the scanner for exclusive use and enable it so that data reveived events are received.
                if (await ClaimScanner())
                {
                    
                    // It is always a good idea to have a release device requested event handler. If this event is not handled, there are chances of another app can 
                    // claim ownsership of the barcode scanner.
                    claimedScanner.ReleaseDeviceRequested += claimedScanner_ReleaseDeviceRequested;

                    // after successfully claiming, attach the datareceived event handler.
                    claimedScanner.DataReceived += claimedScanner_DataReceived;
                    UpdateOutput("Attached the DataReceived Event handler.");

                    // Ask the API to decode the data by default. By setting this, API will decode the raw data from the barcode scanner and 
                    // send the ScanDataLabel and ScanDataType in the DataReceived event
                    claimedScanner.IsDecodeDataEnabled = true;

                    // enable the scanner.
                    // Note: If the scanner is not enabled (i.e. EnableAsync not called), attaching the event handler will not be any useful because the API will not fire the event 
                    // if the claimedScanner has not beed Enabled
                    if (await EnableScanner())
                    {
                        // reset the button state
                        ScenarioEndScanButton.IsEnabled = true;
                        ScenarioStartScanButton.IsEnabled = false;

                        UpdateOutput("Ready to Scan.");
                    }
                } 
            }
            else
            {
                UpdateOutput("No Barcode Scanner found");
            }
        }

        /// <summary>
        /// Event handler for the Release Device Requested event fired when barcode scanner receives Claim request from another application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> Contains the ClamiedBarcodeScanner that is sending this request</param>
        async void claimedScanner_ReleaseDeviceRequested(object sender, ClaimedBarcodeScanner e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // alsways retain the device
                e.RetainDevice();

                UpdateOutput("Event ReleaseDeviceRequest received. Retaining the Barcode Scanner.");
            }); 
        }

        string GetDataString(IBuffer data)
        {
            StringBuilder result = new StringBuilder();

            if (data == null)
            {
                result.Append("No data");
            }
            else
            {
                // Just to show that we have the raw data, we'll print the value of the bytes.
                // Arbitrarily limit the number of bytes printed to 20 so the UI isn't overloaded.
                const uint MAX_BYTES_TO_PRINT = 20;
                uint bytesToPrint = Math.Min(data.Length, MAX_BYTES_TO_PRINT);

                DataReader reader = DataReader.FromBuffer(data);
                byte[] dataBytes = new byte[bytesToPrint];
                reader.ReadBytes(dataBytes);

                for (uint byteIndex = 0; byteIndex < bytesToPrint; ++byteIndex)
                {
                    result.AppendFormat("{0:X2} ", dataBytes[byteIndex]);
                }

                if (bytesToPrint < data.Length)
                {
                    result.Append("...");
                }
            }

            return result.ToString();
        }

        string GetDataLabelString(IBuffer data, uint scanDataType)
        {
            string result = null;
            // Only certain data types contain encoded text.
            //   To keep this simple, we'll just decode a few of them.
            if (data == null)
            {
                result = "No data";
            }
            else
            {
                switch (BarcodeSymbologies.GetName(scanDataType))
                {
                    case "Upca":
                    case "UpcaAdd2":
                    case "UpcaAdd5":
                    case "Upce":
                    case "UpceAdd2":
                    case "UpceAdd5":
                    case "Ean8":
                    case "TfStd":
                        // The UPC, EAN8, and 2 of 5 families encode the digits 0..9
                        // which are then sent to the app in a UTF8 string (like "01234")

                        // This is not an exhaustive list of symbologies that can be converted to a string

                        DataReader reader = DataReader.FromBuffer(data);
                        result = reader.ReadString(data.Length);
                        break;
                    default:
                        // Some other symbologies (typically 2-D symbologies) contain binary data that
                        //  should not be converted to text.
                        result = string.Format("Decoded data unavailable. Raw label data: {0}", GetDataString(data));
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Event handler for the DataReceived event fired when a barcode is scanned by the barcode scanner 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> Contains the BarcodeScannerReport which contains the data obtained in the scan</param>
        async void claimedScanner_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            // need to update the UI data on the dispatcher thread.
            // update the UI with the data received from the scan.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // read the data from the buffer and convert to string.
                ScenarioOutputScanDataLabel.Text = GetDataLabelString(args.Report.ScanDataLabel, args.Report.ScanDataType);

                ScenarioOutputScanData.Text = GetDataString(args.Report.ScanData);

                ScenarioOutputScanDataType.Text = BarcodeSymbologies.GetName(args.Report.ScanDataType);
            }); 
        }

        /// <summary>
        /// Reset the Scenario state
        /// </summary>
        private void ResetTheScenarioState()
        {
            if (claimedScanner != null)
            {
                // Detach the event handlers
                claimedScanner.DataReceived -= claimedScanner_DataReceived;
                claimedScanner.ReleaseDeviceRequested -= claimedScanner_ReleaseDeviceRequested;
                // Release the Barcode Scanner and set to null
                claimedScanner.Dispose();
                claimedScanner = null;
            }

            scanner = null;

            // Reset the strings in the UI
            UpdateOutput("Click the Start Scanning Button.");
            this.ScenarioOutputScanData.Text = "No data";
            this.ScenarioOutputScanDataLabel.Text = "No data";
            this.ScenarioOutputScanDataType.Text = "No data";

            // reset the button state
            ScenarioEndScanButton.IsEnabled = false;
            ScenarioStartScanButton.IsEnabled = true;
        }

        /// <summary>
        /// Event handler for End Scan Button Click. 
        /// Releases the Barcode Scanner and resets the text in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioEndScanButton_Click(object sender, RoutedEventArgs e)
        {
            // reset the scenario state
            this.ResetTheScenarioState();
        }

    }
}
