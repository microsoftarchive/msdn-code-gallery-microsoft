//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Threading.Tasks;
using SDKTemplate;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrialMode : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        LicenseChangedEventHandler licenseChangeHandler = null;

        public TrialMode()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadTrialModeProxyFileAsync();
        }

        /// <summary>
        /// Invoked when this page is about to unload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (licenseChangeHandler != null)
            {
                CurrentAppSimulator.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }
            base.OnNavigatingFrom(e);
        }

        private async Task LoadTrialModeProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("data");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("trial-mode.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(TrialModeRefreshScenario);
            CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
            // setup application upsell message
            try
            {
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
                PurchasePrice.Text = "You can buy the full app for: " + listing.FormattedPrice + ".";
            }
            catch (Exception)
            {
                rootPage.NotifyUser("LoadListingInformationAsync API call failed", NotifyType.ErrorMessage);
            }
        }

        private void TrialModeRefreshScenario()
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    LicenseMode.Text = "Current license mode: Trial license";
                }
                else
                {
                    LicenseMode.Text = "Current license mode: Full license";
                }
            }
            else
            {
                LicenseMode.Text = "Current license mode: Inactive license";
            }
        }

        /// <summary>
        /// Invoked when remaining trial time needs to be calculated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrialTime_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    var remainingTrialTime = (licenseInformation.ExpirationDate - DateTime.Now).Days;
                    rootPage.NotifyUser("You can use this app for " + remainingTrialTime + " more days before the trial period ends.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("You have a full license. The trial time is not meaningful.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("You don't have a license. The trial time can't be determined.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked when Trial product is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrialProduct_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    rootPage.NotifyUser("You are using a trial version of this app.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("You no longer have a trial version of this app.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("You don't have a license for this app.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked when a product available in full version is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullProduct_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    rootPage.NotifyUser("You are using a trial version of this app.", NotifyType.ErrorMessage);
                }
                else
                {
                    rootPage.NotifyUser("You are using a fully-licensed version of this app.", NotifyType.StatusMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("You don't have a license for this app.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked when attempting to convert trial to full
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConvertTrial_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            rootPage.NotifyUser("Buying the full license...", NotifyType.StatusMessage);
            if (licenseInformation.IsTrial)
            {
                try
                {
                    await CurrentAppSimulator.RequestAppPurchaseAsync(false);
                    if (!licenseInformation.IsTrial && licenseInformation.IsActive)
                    {
                        rootPage.NotifyUser("You successfully upgraded your app to the fully-licensed version.", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("You still have a trial license for this app.", NotifyType.ErrorMessage);
                    }
                }
                catch (Exception)
                {
                    rootPage.NotifyUser("The upgrade transaction failed. You still have a trial license for this app.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("You already bought this app and have a fully-licensed version.", NotifyType.ErrorMessage);
            }
        }
    }
}
