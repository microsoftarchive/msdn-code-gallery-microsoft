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
using Windows.UI.Core;

namespace MagneticStripeReaderSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        MagneticStripeReader _reader = null;
        ClaimedMagneticStripeReader _claimedReader = null;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Creates the default magnetic stripe reader.
        /// </summary>
        /// <returns>true if magnetic stripe reader is created. Otherwise returns false</returns>
        private async Task<bool> CreateDefaultMagneticStripeReaderObject()
        {
            if (_reader == null)
            {
                UpdateReaderStatusTextBlock("Creating Magnetic Stripe Reader.");
                _reader = await MagneticStripeReader.GetDefaultAsync();

                if (_reader != null)
                {
                    UpdateReaderStatusTextBlock("Default Magnetic Stripe Reader created.");
                    UpdateReaderStatusTextBlock("Device Id is:" + _reader.DeviceId);
                }
                else
                {
                    UpdateReaderStatusTextBlock("Magnetic Stripe Reader not found. Please connect a Magnetic Stripe Reader.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Update the status in the UI with the string passed.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateReaderStatusTextBlock(string message)
        {
            ReaderStatusText.Text += Environment.NewLine;
            ReaderStatusText.Text += message;
        }

        /// <summary>
        /// Claim the magnetic stripe reader
        /// </summary>
        /// <returns>true if claim is successful. Otherwise returns false</returns>
        private async Task<bool> ClaimReader()
        {
            if (_claimedReader == null)
            {
                // claim the magnetic stripe reader
                _claimedReader = await _reader.ClaimReaderAsync();

                // enable the claimed magnetic stripe reader
                if (_claimedReader != null)
                {
                    UpdateReaderStatusTextBlock("Claim Magnetic Stripe Reader succeeded.");
                }
                else
                {
                    UpdateReaderStatusTextBlock("Claim Magnetic Stripe Reader failed.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Enable the magnetic stripe reader.
        /// </summary>
        /// <returns>true if claim is successful. Otherwise returns false</returns>
        private async Task<bool> EnableReader()
        {
    
            await _claimedReader.EnableAsync();
            UpdateReaderStatusTextBlock("Enable Magnetic Stripe Reader succeeded.");
            return true;
        }


        private async void ScenarioStartReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (await CreateDefaultMagneticStripeReaderObject())
            {
                if (_reader != null)
                {
                    // after successful creation, claim the reader for exclusive use and enable it so that data reveived events are received.
                    if (await ClaimReader())
                    {
                        if (_claimedReader != null)
                        {
                            // It is always a good idea to have a release device requested event handler. If this event is not handled, there are chances of another app can 
                            // claim ownsership of the magnetic stripe reader.
                            _claimedReader.ReleaseDeviceRequested += OnReleaseDeviceRequested;

                            // after successfully claiming and enabling, attach the AamvaCardDataReceived event handler.
                            // Note: If the scanner is not enabled (i.e. EnableAsync not called), attaching the event handler will not be any useful because the API will not fire the event 
                            // if the claimedScanner has not beed Enabled
                            _claimedReader.AamvaCardDataReceived += OnAamvaCardDataReceived;
                            UpdateReaderStatusTextBlock("Attached the AamvaCardDataReceived Event handler.");

                            // Ask the API to decode the data by default. By setting this, API will decode the raw data from the magnetic stripe reader
                            _claimedReader.IsDecodeDataEnabled = true;

                            if (await EnableReader())
                            {
                                UpdateReaderStatusTextBlock("Ready to Swipe.");

                                // reset the button state
                                ScenarioEndReadButton.IsEnabled = true;
                                ScenarioStartReadButton.IsEnabled = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for End button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScenarioEndReadButton_Click(object sender, RoutedEventArgs e)
        {
            ResetTheScenarioState();
        }

        /// <summary>
        /// Event handler for the Release Device Requested event fired when magnetic stripe reader receives Claim request from another application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> Contains the ClaimedMagneticStripeReader that is sending this request</param>
        void OnReleaseDeviceRequested(object sender, ClaimedMagneticStripeReader args)
        {
            // let us retain the device always. If it is not retained, this exclusive claim will be lost.
            args.RetainDevice(); 
        }

        /// <summary>
        /// Event handler for the DataReceived event fired when a AAMVA card is read by the magnetic stripe reader 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> Contains the MagneticStripeReaderAamvaCardDataReceivedEventArgs which contains the data obtained in the scan</param>
        async void OnAamvaCardDataReceived(object sender, MagneticStripeReaderAamvaCardDataReceivedEventArgs args)
        {
            // read the data and display
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ScenarioOutputAddress.Text = args.Address;
                ScenarioOutputBirthDate.Text = args.BirthDate;
                ScenarioOutputCity.Text = args.City;
                ScenarioOutputClass.Text = args.Class;
                ScenarioOutputEndorsements.Text = args.Endorsements;
                ScenarioOutputExpirationDate.Text = args.ExpirationDate;
                ScenarioOutputEyeColor.Text = args.EyeColor;
                ScenarioOutputFirstName.Text = args.FirstName;
                ScenarioOutputGender.Text = args.Gender;
                ScenarioOutputHairColor.Text = args.HairColor;
                ScenarioOutputHeight.Text = args.Height;
                ScenarioOutputLicenseNumber.Text = args.LicenseNumber;
                ScenarioOutputPostalCode.Text = args.PostalCode;
                ScenarioOutputRestrictions.Text = args.Restrictions;
                ScenarioOutputState.Text = args.State;
                ScenarioOutputSuffix.Text = args.Suffix;
                ScenarioOutputSurname.Text = args.Surname;
                ScenarioOutputWeight.Text = args.Weight;
            });

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
        /// Invoked when this page is no longer displayed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ResetTheScenarioState();
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Reset the Scenario state
        /// </summary>
        void ResetTheScenarioState()
        {
            if (_claimedReader != null)
            {
                // Detach the datareceived event handler and releasedevicerequested event handler
                _claimedReader.AamvaCardDataReceived -= OnAamvaCardDataReceived;
                _claimedReader.ReleaseDeviceRequested -= OnReleaseDeviceRequested;
        
                // release the Claimed Magnetic Stripe Reader and set to null
                _claimedReader.Dispose();		
                _claimedReader = null;
            }

            if (_reader != null)
            {
                // release the Magnetic Stripe Reader and set to null
                _reader = null;
            }

            // Reset the strings in the UI
            ReaderStatusText.Text = "Click the Start Receiving Data Button.";
            ScenarioOutputAddress.Text = "No data";
            ScenarioOutputBirthDate.Text = "No data";
            ScenarioOutputCity.Text = "No data";
            ScenarioOutputClass.Text = "No data";
            ScenarioOutputEndorsements.Text = "No data";
            ScenarioOutputExpirationDate.Text = "No data";
            ScenarioOutputEyeColor.Text = "No data";
            ScenarioOutputFirstName.Text = "No data";
            ScenarioOutputGender.Text = "No data";
            ScenarioOutputHairColor.Text = "No data";
            ScenarioOutputHeight.Text = "No data";
            ScenarioOutputLicenseNumber.Text = "No data";
            ScenarioOutputPostalCode.Text = "No data";
            ScenarioOutputRestrictions.Text = "No data";
            ScenarioOutputState.Text = "No data";
            ScenarioOutputSuffix.Text = "No data";
            ScenarioOutputSurname.Text = "No data";
            ScenarioOutputWeight.Text = "No data";

            // reset the button state
            ScenarioEndReadButton.IsEnabled = false;
            ScenarioStartReadButton.IsEnabled = true;
        }
    }
}
