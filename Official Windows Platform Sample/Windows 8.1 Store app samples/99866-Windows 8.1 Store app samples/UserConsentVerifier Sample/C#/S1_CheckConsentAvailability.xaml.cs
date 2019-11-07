//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials.UI;
using SDKTemplate;
using System;

namespace UserConsentVerifierCS
{
    /// <summary>t
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
  

        /// <summary>
        /// This is the click handler for the 'Check Availability' button. It checks the availability of User consent requisition 
        /// via registered fingerprints.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckAvailability_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            b.IsEnabled = false;
            try
            {
                // Check the availability of User Consent with fingerprints facility
                UserConsentVerifierAvailability consentAvailability = await UserConsentVerifier.CheckAvailabilityAsync();
                switch (consentAvailability)
                {
                    case UserConsentVerifierAvailability.Available:
                    {
                        rootPage.NotifyUser("User consent requisition facility is available.", NotifyType.StatusMessage);
                        break;
                    }

                    case UserConsentVerifierAvailability.DeviceBusy:
                    {
                        rootPage.NotifyUser("Biometric device is busy.", NotifyType.ErrorMessage);
                        break;
                    }

                    case UserConsentVerifierAvailability.DeviceNotPresent:
                    {
                        rootPage.NotifyUser("No biometric device found.", NotifyType.ErrorMessage);
                        break;
                    }

                    case UserConsentVerifierAvailability.DisabledByPolicy:
                    {
                        rootPage.NotifyUser("Biometrics is disabled by policy.", NotifyType.ErrorMessage);
                        break;
                    }

                    case UserConsentVerifierAvailability.NotConfiguredForUser:
                    {
                        rootPage.NotifyUser("User has no fingeprints registered.", NotifyType.ErrorMessage);
                        break;
                    }

                    default:
                    {
                        rootPage.NotifyUser("Consent verification with fingerprints is currently unavailable.", NotifyType.ErrorMessage);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Checking the availability of Consent feature failed with exception. Operation: CheckAvailabilityAsync, Exception: " + ex.ToString(), NotifyType.ErrorMessage);
            }
            finally
            {
                b.IsEnabled = true;
            }
        }
    }
}
