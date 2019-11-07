// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.ApplicationModel.Wallet;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WalletQuickstart
{
    /// <summary>
    /// Deleting a Wallet item associated with your app. 
    /// </summary>
    public sealed partial class Scenario7 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        // The wallet item that used in this scenario.
        private WalletItem walletItem;

        public Scenario7()
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

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
            }
            else
            {
                await wallet.DeleteAsync(walletItem.Id);
                walletItem = null;
                rootPage.NotifyUser("Item deleted.", NotifyType.StatusMessage);
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
    }
}
