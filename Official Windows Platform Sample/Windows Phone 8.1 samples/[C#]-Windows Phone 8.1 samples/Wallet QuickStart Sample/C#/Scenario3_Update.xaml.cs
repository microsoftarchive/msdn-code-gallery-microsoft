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
    /// Updating an existing Wallet item associated with your app. 
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        // The wallet item that used in this scenario.
        private WalletItem walletItem;

        public Scenario3()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            // Retrieve the wallet item and list the coffee points.
            await this.ListPointsAsync();
        }

        private async Task ListPointsAsync()
        {

            // Request an instance of the default Wallet store.
            wallet = await WalletManager.RequestStoreAsync();

            // Find an existing wallet item.
            walletItem = await wallet.GetWalletItemAsync("CoffeeLoyalty123");

            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
                return;
            }

            if (walletItem.DisplayProperties.ContainsKey("PointsId"))
            {
                CoffePointsValue.Text = walletItem.DisplayProperties["PointsId"].Value;
            }
            else
            {
                rootPage.NotifyUser("Item does not have a PointsId property.", NotifyType.ErrorMessage);
                return;
            }

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

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
                return;
            }

            if (walletItem.DisplayProperties.ContainsKey("PointsId"))
            {
                // Update a property on the wallet item.
                walletItem.DisplayProperties["PointsId"].Value = CoffePointsValue.Text;

                // Update the item in the wallet.
                await wallet.UpdateAsync(walletItem);
                rootPage.NotifyUser("CoffeePoints has been updated in Wallet.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Item does not have a PointsId property.", NotifyType.ErrorMessage);
                return;
            }
        }

    }
}
