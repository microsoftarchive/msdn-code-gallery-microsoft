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
    /// Adding a custom verb, or action, to a Wallet item.
    /// </summary>
    public sealed partial class Scenario5 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        // The wallet item that used in this scenario.
        private WalletItem walletItem;

        public Scenario5()
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

        private async void CustomVerb_Click(object sender, RoutedEventArgs e)
        {
            await this.AddVerbAsync();
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
        /// Adds a verb to a Wallet item.  When the user taps the verb, the application
        /// that owns this item will be launched with specific arguments so that
        /// it can perform the action
        /// </summary>
        /// <returns>An asynchronous action.</returns>
        public async Task AddVerbAsync()
        {
            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                
                // Add a verb to the wallet item. In this example, we are adding 
                // a verb to allow the user to add funds to their account.
                walletItem.Verbs.Add("addfunds", new WalletVerb("Add Funds"));

                // Update the item in Wallet.
                await wallet.UpdateAsync(walletItem);

                rootPage.NotifyUser("Custom verb added.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = String.Format("Could not add custom verb. {0}", ex.Message);
                rootPage.NotifyUser(errorMessage, NotifyType.ErrorMessage);
            }
        }


    }
}
