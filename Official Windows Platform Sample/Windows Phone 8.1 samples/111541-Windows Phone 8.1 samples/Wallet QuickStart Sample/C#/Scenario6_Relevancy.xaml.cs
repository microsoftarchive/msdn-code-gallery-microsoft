// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Wallet;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace WalletQuickstart
{
    /// <summary>
    /// Setting relevant date and relevant locations on a Wallet item.
    /// </summary>
    public sealed partial class Scenario6 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        // The wallet item that used in this scenario.
        private WalletItem walletItem;

        public Scenario6()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            // Request an instance of the default Wallet store.
            wallet = await WalletManager.RequestStoreAsync();

            // Get the wallet item to be used in this scenario.
            walletItem = await wallet.GetWalletItemAsync("CoffeeLoyalty123");
        }

        private async void Relevancy_Click(object sender, RoutedEventArgs e)
        {
            await this.AddRelevancyAsync();
        }
       
        private async void View_Click(object sender, RoutedEventArgs e)
        {
            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
            }
            else
            {
                await wallet.ShowAsync(walletItem.Id);
            }
        }

        /// <summary>
        /// Sets when this item is relevant.
        /// When an item is considered relevant, it will be promoted to the top spot in the Wallet summary view list
        /// and a toast notifcation will also be shown in a pop-up on the user's phone. 
        /// </summary>
        /// <returns>An asynchronous action.</returns>
        public async Task AddRelevancyAsync()
        {
            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                // For this example, we will clear all relevancy data from the wallet item 
                // to ensure that adding the item below does not fail because it already exists.
                walletItem.RelevantLocations.Clear();

                // Create a new relevant location object.
                WalletRelevantLocation location = new WalletRelevantLocation();
                location.DisplayMessage = "Welcome to Contoso Coffee on 5th";

                // Set the desired location.
                var position = new Windows.Devices.Geolocation.BasicGeoposition();
                position.Latitude = 47.63;
                position.Longitude = -122.2147;
                location.Position = position;

                // Add this location to the RelevantLocations collection on the item.
                // Note: The key for each entry must be unique in the collection. 
                walletItem.RelevantLocations.Add("5thId", location);

                // Add a relevant date.
                walletItem.RelevantDate = DateTime.Now;
                walletItem.RelevantDateDisplayMessage = "Free coffee day";

                // Update the item in Wallet.
                await wallet.UpdateAsync(walletItem);
                rootPage.NotifyUser("Relevancy data added. Open Wallet to see this card promoted to the top of the list because it is relevant (relevant date was defined as today).", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = String.Format("Could not add relevancy data. {0}", ex.Message);
                rootPage.NotifyUser(errorMessage, NotifyType.ErrorMessage);
            }

        }

    }
}
