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
    public sealed partial class InAppPurchaseConsumablesAdvanced : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public InAppPurchaseConsumablesAdvanced()
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
            product1TempTransactionId = Guid.Empty;
            product1NumPurchases = 0;
            product1NumFulfillments = 0;
            product2TempTransactionId = Guid.Empty;
            product2NumPurchases = 0;
            product2NumFulfillments = 0;
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
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase-consumables-advanced.xml");
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

            // setup application upsell message
            try
            {
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
                ProductListing product1 = listing.ProductListings["product1"];
                Product1SellMessage.Text = "You can buy " + product1.Name + " for: " + product1.FormattedPrice + ".";
                ProductListing product2 = listing.ProductListings["product2"];
                Product2SellMessage.Text = "You can buy " + product2.Name + " for: " + product2.FormattedPrice + ".";
                Log("", NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("LoadListingInformationAsync API call failed", NotifyType.ErrorMessage);
            }

            // recover any unfulfilled consumables
            try
            {
                IReadOnlyList<UnfulfilledConsumable> products = await CurrentAppSimulator.GetUnfulfilledConsumablesAsync();
                foreach (UnfulfilledConsumable product in products)
                {
                    if (product.ProductId == "product1")
                    {
                        product1TempTransactionId = product.TransactionId;
                    }
                    else if (product.ProductId == "product2")
                    {
                        product2TempTransactionId = product.TransactionId;
                    }
                }
            }
            catch (Exception)
            {
                rootPage.NotifyUser("GetUnfulfilledConsumablesAsync API call failed", NotifyType.ErrorMessage);
            }
        }

        private async void GetUnfulfilledConsumables()
        {
            try
            {
                IReadOnlyList<UnfulfilledConsumable> products = await CurrentAppSimulator.GetUnfulfilledConsumablesAsync();
                string logMessage = "List of unfulfilled consumables:";

                foreach (UnfulfilledConsumable product in products)
                {
                    logMessage += "\nProduct Id: " + product.ProductId + " Transaction Id: " + product.TransactionId;
                    // This is where you would grant the user their consumable content and call currentApp.reportConsumableFulfillment
                }

                if (products.Count == 0)
                {
                    logMessage = "There are no consumable purchases awaiting fulfillment.";
                }
                Log(logMessage, NotifyType.StatusMessage, products.Count);
            }
            catch (Exception)
            {
                Log("GetUnfulfilledConsumablesAsync API call failed", NotifyType.ErrorMessage);
            }
        }

        private async void GetUnfulfilledButton1_Click(object sender, RoutedEventArgs e)
        {
            GetUnfulfilledConsumables();
        }

        private async void GetUnfulfilledButton2_Click(object sender, RoutedEventArgs e)
        {
            GetUnfulfilledConsumables();
        }

        private async void BuyProduct1Button_Click(object sender, RoutedEventArgs e)
        {
            Log("Buying Product 1...", NotifyType.StatusMessage);
            try
            {
                PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync("product1");
                switch (purchaseResults.Status)
                {
                    case ProductPurchaseStatus.Succeeded:
                        product1NumPurchases++;
                        product1TempTransactionId = purchaseResults.TransactionId;
                        Log("You bought product 1. Transaction Id: " + purchaseResults.TransactionId, NotifyType.StatusMessage);

                        // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                        break;
                    case ProductPurchaseStatus.NotFulfilled:
                        product1TempTransactionId = purchaseResults.TransactionId;
                        Log("You have an unfulfilled copy of product 1. Hit \"Fulfill product 1\" before you can purchase a second copy.", NotifyType.StatusMessage);

                        // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
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

        private async void BuyProduct2Button_Click(object sender, RoutedEventArgs e)
        {
            Log("Buying Product 2...", NotifyType.StatusMessage);
            try
            {
                PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync("product2");
                switch (purchaseResults.Status)
                {
                    case ProductPurchaseStatus.Succeeded:
                        product2NumPurchases++;
                        product2TempTransactionId = purchaseResults.TransactionId;
                        Log("You bought product 2. Transaction Id: " + purchaseResults.TransactionId, NotifyType.StatusMessage);

                        // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                        break;
                    case ProductPurchaseStatus.NotFulfilled:
                        product2TempTransactionId = purchaseResults.TransactionId;
                        Log("You have an unfulfilled copy of product 2. Hit \"Fulfill product 2\" before you can purchase a second copy.", NotifyType.StatusMessage);

                        // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                        break;
                    case ProductPurchaseStatus.NotPurchased:
                        Log("Product 2 was not purchased.", NotifyType.StatusMessage);
                        break;
                }
            }
            catch (Exception)
            {
                Log("Unable to buy Product 2.", NotifyType.ErrorMessage);
            }
        }

        private async void FulfillProduct1Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLocallyFulfilled("product1", product1TempTransactionId))
            {
                GrantFeatureLocally("product1", product1TempTransactionId);
            }

            try {
                FulfillmentResult result = await CurrentAppSimulator.ReportConsumableFulfillmentAsync("product1", product1TempTransactionId);

                switch (result)
                {
                    case FulfillmentResult.Succeeded:
                        product1NumFulfillments++;
                        Log("Product 1 was fulfilled. You are now able to buy product 1 again.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.NothingToFulfill:
                        Log("There is nothing to fulfill. You must purchase product 1 before it can be fulfilled.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.PurchasePending:
                        Log("Purchase hasn't completed yet. Wait and try again.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.PurchaseReverted:
                        Log("Purchase was reverted before fulfillment.", NotifyType.StatusMessage);
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.ServerError:
                        Log("There was an error when fulfilling.", NotifyType.StatusMessage);
                        break;
                }
            }
            catch (Exception)
            {
                Log("There was an error when fulfilling.", NotifyType.ErrorMessage);
            }
        }

        private async void FulfillProduct2Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLocallyFulfilled("product2", product2TempTransactionId))
            {
                GrantFeatureLocally("product2", product2TempTransactionId);
            }

            try {
                FulfillmentResult result = await CurrentAppSimulator.ReportConsumableFulfillmentAsync("product2", product2TempTransactionId);

                switch (result)
                {
                    case FulfillmentResult.Succeeded:
                        product2NumFulfillments++;
                        Log("Product 2 was fulfilled. You are now able to buy product 2 again.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.NothingToFulfill:
                        Log("There is nothing to fulfill. You must purchase product 2 before it can be fulfilled.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.PurchasePending:
                        Log("Purchase hasn't completed yet. Wait and try again.", NotifyType.StatusMessage);
                        break;
                    case FulfillmentResult.PurchaseReverted:
                        Log("Purchase was reverted before fulfillment.", NotifyType.StatusMessage);
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.ServerError:
                        Log("There was an error when fulfilling.", NotifyType.StatusMessage);
                        break;
                }
            }
            catch (Exception)
            {
                Log("There was an error when fulfilling.", NotifyType.ErrorMessage);
            }
        }

        private void Log(string message, NotifyType type, int blankLines = 0)
        {
            string logMessage = message + "\n\n";
            if (blankLines == 1)
            {
                logMessage += "\n";
            }
            else if (blankLines != 2)
            {
                logMessage += "\n\n";
            }
            logMessage += GetPurchaseAndFulfillmentResults();
            rootPage.NotifyUser(logMessage, type);
        }

        private Guid product1TempTransactionId = Guid.Empty;
        private int product1NumPurchases = 0;
        private int product1NumFulfillments = 0;

        private Guid product2TempTransactionId = Guid.Empty;
        private int product2NumPurchases = 0;
        private int product2NumFulfillments = 0;

        private Dictionary<string, List<Guid>> grantedConsumableTransactionIds = new Dictionary<string, List<Guid>>();

        private string GetPurchaseAndFulfillmentResults()
        {
            string message = "Product 1 has been purchased " + product1NumPurchases + " time" + (product1NumPurchases == 1 ? "" : "s") + " and fulfilled " + product1NumFulfillments + " time" + (product1NumFulfillments == 1 ? "" : "s") + ".";
            message += "\n" + "Product 2 has been purchased " + product2NumPurchases + " time" + (product2NumPurchases == 1 ? "" : "s") + " and fulfilled " + product2NumFulfillments + " time" + (product2NumFulfillments == 1 ? "" : "s") + ".";
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
        }

        private Boolean IsLocallyFulfilled(string productId, Guid transactionId)
        {
            return grantedConsumableTransactionIds.ContainsKey(productId) && grantedConsumableTransactionIds[productId].Contains(transactionId);
        }

    }
}
