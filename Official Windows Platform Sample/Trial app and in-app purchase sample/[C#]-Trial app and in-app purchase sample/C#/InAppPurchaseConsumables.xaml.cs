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
using System.Collections.Generic;
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
    public sealed partial class InAppPurchaseConsumables : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public InAppPurchaseConsumables()
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
            numberOfConsumablesPurchased = 0;
            grantedConsumableTransactionIds = new Dictionary<string, List<Guid>>();

            await LoadInAppPurchaseConsumablesProxyFileAsync();
        }

        /// <summary>
        /// Invoked when this page is about to unload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private async Task LoadInAppPurchaseConsumablesProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("data");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase-consumables.xml");
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

            // setup application upsell message
            try
            {
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
                var product1 = listing.ProductListings["product1"];
                Product1SellMessage.Text = "You can buy " + product1.Name + " for: " + product1.FormattedPrice + ".";
                Log("", NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("LoadListingInformationAsync API call failed", NotifyType.ErrorMessage);
            }
        }

        private async void BuyAndFulfillProduct1Button_Click(object sender, RoutedEventArgs e)
        {
            Log("Buying Product 1...", NotifyType.StatusMessage);
            try
            {
                PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync("product1");
                switch (purchaseResults.Status)
                {
                    case ProductPurchaseStatus.Succeeded:
                        GrantFeatureLocally("product1", purchaseResults.TransactionId);
                        FulfillProduct1("product1", purchaseResults.TransactionId);
                        break;
                    case ProductPurchaseStatus.NotFulfilled:
                        if (!IsLocallyFulfilled("product1", purchaseResults.TransactionId))
                        {
                            GrantFeatureLocally("product1", purchaseResults.TransactionId);
                        }
                        FulfillProduct1("product1", purchaseResults.TransactionId);
                        break;
                    case ProductPurchaseStatus.NotPurchased:
                        Log("Product 1 was not purchased.", NotifyType.StatusMessage);
                        break;
                }
            }
            catch (Exception)
            {
                Log("Unable to buy Product 1.", NotifyType.ErrorMessage);
            }
        }

        private async void FulfillProduct1(string productId, Guid transactionId)
        {
            try
            {
                FulfillmentResult result = await CurrentAppSimulator.ReportConsumableFulfillmentAsync(productId, transactionId);
                switch (result)
                {
                    case FulfillmentResult.Succeeded:
                        Log("You bought and fulfilled product 1.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.NothingToFulfill:
                        Log("There is no purchased product 1 to fulfill.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.PurchasePending:
                        Log("You bought product 1. The purchase is pending so we cannot fulfill the product.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.PurchaseReverted:
                        Log("You bought product 1. But your purchase has been reverted.", NotifyType.StatusMessage);
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.ServerError:
                        Log("You bought product 1. There was an error when fulfilling.", NotifyType.StatusMessage);
                        break;
                }
            }
            catch (Exception)
            {
                Log("You bought Product 1. There was an error when fulfilling.", NotifyType.ErrorMessage);
            }
        }

        private void Log(string message, NotifyType type)
        {
            string logMessage = message + "\n\n" + GetFulfillmentResults();
            rootPage.NotifyUser(logMessage, type);
        }

        private int numberOfConsumablesPurchased = 0;
        private Dictionary<string, List<Guid>> grantedConsumableTransactionIds = new Dictionary<string, List<Guid>>();

        private string GetFulfillmentResults()
        {
            string message = "Product 1 has been fulfilled " + numberOfConsumablesPurchased + " time" + (numberOfConsumablesPurchased == 1 ? "" : "s") + ".";
            return message;
        }

        private void GrantFeatureLocally(string productId, Guid transactionId)
        {
            if (!grantedConsumableTransactionIds.ContainsKey(productId))
            {
                grantedConsumableTransactionIds.Add(productId, new List<Guid>());
            }
            grantedConsumableTransactionIds[productId].Add(transactionId);

            // Grant the user their content. You will likely increase some kind of gold/coins/some other asset count.
            numberOfConsumablesPurchased++;
        }

        private Boolean IsLocallyFulfilled(string productId, Guid transactionId)
        {
            return grantedConsumableTransactionIds.ContainsKey(productId) && grantedConsumableTransactionIds[productId].Contains(transactionId);
        }

    }
}
