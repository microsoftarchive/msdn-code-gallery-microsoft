//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.ApplicationModel.Store;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpiringProduct : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        LicenseChangedEventHandler licenseChangeHandler = null;

        public ExpiringProduct()
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
            await LoadExpiringProductProxyFileAsync();
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

        private async Task LoadExpiringProductProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("data");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("expiring-product.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(ExpiringProductRefreshScenario);
            CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
        }

        private void ExpiringProductRefreshScenario()
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    rootPage.NotifyUser("You don't have full license", NotifyType.ErrorMessage);
                }
                else
                {
                    var productLicense1 = licenseInformation.ProductLicenses["product1"];
                    if (productLicense1.IsActive)
                    {
                        var longdateTemplate = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate");
                        Product1Message.Text = "Product 1 expires on: " + longdateTemplate.Format(productLicense1.ExpirationDate);
                        var remainingDays = (productLicense1.ExpirationDate - DateTime.Now).Days;
                        rootPage.NotifyUser("Product 1 expires in: " + remainingDays + " days.", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("Product 1 is not available.", NotifyType.ErrorMessage);
                    }
                }
            }
            else
            {
                rootPage.NotifyUser("You don't have active license.", NotifyType.ErrorMessage);
            }
        }
    }
}
