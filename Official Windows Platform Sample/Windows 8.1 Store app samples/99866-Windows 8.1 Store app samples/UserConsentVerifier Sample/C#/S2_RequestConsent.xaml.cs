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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
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
        /// This is the click handler for the 'Request Consent' button. It requests consent from the current user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RequestConsent_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            b.IsEnabled = false;

            if(!String.IsNullOrEmpty(Message.Text))
            {
                try
                {
                    // Request the currently logged on user's consent via fingerprint swipe
                    UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(Message.Text);
                    switch (consentResult) 
				    { 
					    case UserConsentVerificationResult.Verified:
					    {
						    rootPage.NotifyUser("User's presence verified.", NotifyType.StatusMessage);
						    break;
                        }

					    case UserConsentVerificationResult.DeviceBusy:
                        {
						    rootPage.NotifyUser("Biometric device is busy.", NotifyType.ErrorMessage);
						    break;
                        }

					    case UserConsentVerificationResult.DeviceNotPresent: 
					    {
						    rootPage.NotifyUser("No biometric device found.", NotifyType.ErrorMessage);
						    break;
                        }

					    case UserConsentVerificationResult.DisabledByPolicy:
					    {
						    rootPage.NotifyUser("Biometrics is disabled by policy.", NotifyType.ErrorMessage);
						    break;
                        }
					    
                        case UserConsentVerificationResult.NotConfiguredForUser:
					    {
						    rootPage.NotifyUser("User has no fingeprints registered.", NotifyType.ErrorMessage);
						    break;
                        }

                        case UserConsentVerificationResult.RetriesExhausted:
					    {
						    rootPage.NotifyUser("Too many failed attempts.", NotifyType.ErrorMessage);
						    break;
					    }
					    case UserConsentVerificationResult.Canceled:
					    {
						    rootPage.NotifyUser("Consent request prompt was canceled.", NotifyType.ErrorMessage);
						    break;
                        }

                        default:
					    {
						    rootPage.NotifyUser("Consent verification with fingerprints is currently unavailable.", NotifyType.ErrorMessage);
						    break;
					    }
                    }
				}
                catch(Exception ex)
                {
			        rootPage.NotifyUser("Request current user's consent failed with exception. Operation: RequestVerificationAsync, Exception: " + ex.ToString() , NotifyType.ErrorMessage);
                }
                finally
                {
                    b.IsEnabled = true;
                }
            }
	        else
	        {
		        rootPage.NotifyUser("Empty Message String. Enter prompt string in the Message text field.", NotifyType.ErrorMessage);
		        b.IsEnabled = true;
            }
	    }
    }
}
