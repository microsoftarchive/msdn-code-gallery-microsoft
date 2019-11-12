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
    public sealed partial class InAppPurchase : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        LicenseChangedEventHandler licenseChangeHandler = null;

        public InAppPurchase()
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
            await LoadInAppPurchaseProxyFileAsync();
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

        private async Task LoadInAppPurchaseProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("data");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(InAppPurchaseRefreshScenario);
            CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

            // setup application upsell message
            try
            {
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
                var product1 = listing.ProductListings["product1"];
                var product2 = listing.ProductListings["product2"];
                Product1SellMessage.Text = "You can buy " + product1.Name + " for: " + product1.FormattedPrice + ".";
                Product2SellMessage.Text = "You can buy " + product2.Name + " for: " + product2.FormattedPrice + ".";
            }
            catch (Exception)
            {
                rootPage.NotifyUser("LoadListingInformationAsync API call failed", NotifyType.ErrorMessage);
            }
        }

        private void InAppPurchaseRefreshScenario()
        {
        }

        private void TestProduct1_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["product1"];
            if (productLicense.IsActive)
            {
                rootPage.NotifyUser("You can use Product 1.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType.ErrorMessage);
            }
        }

        private async void BuyProduct1Button_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (!licenseInformation.ProductLicenses["product1"].IsActive)
            {
                rootPage.NotifyUser("Buying Product 1...", NotifyType.StatusMessage);
                try
                {
                    await CurrentAppSimulator.RequestProductPurchaseAsync("product1");
                    if (licenseInformation.ProductLicenses["product1"].IsActive)
                    {
                        rootPage.NotifyUser("You bought Product 1.", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("Product 1 was not purchased.", NotifyType.StatusMessage);
                    }
                }
                catch (Exception)
                {
                    rootPage.NotifyUser("Unable to buy Product 1.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("You already own Product 1.", NotifyType.ErrorMessage);
            }
        }

        private void TestProduct2_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["product2"];
            if (productLicense.IsActive)
            {
                rootPage.NotifyUser("You can use Product 2.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("You don't own Product 2. You must buy Product 2 before you can use it.", NotifyType.ErrorMessage);
            }
        }

        private async void BuyProduct2_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (!licenseInformation.ProductLicenses["product2"].IsActive)
            {
                rootPage.NotifyUser("Buying Product 2...", NotifyType.StatusMessage);
                try
                {
                    await CurrentAppSimulator.RequestProductPurchaseAsync("product2");
                    if (licenseInformation.ProductLicenses["product2"].IsActive)
                    {
                        rootPage.NotifyUser("You bought Product 2.", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("Product 2 was not purchased.", NotifyType.StatusMessage);
                    }
                }
                catch (Exception)
                {
                    rootPage.NotifyUser("Unable to buy Product 2.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("You already own Product 2.", NotifyType.ErrorMessage);
            }
        }
    }
}
