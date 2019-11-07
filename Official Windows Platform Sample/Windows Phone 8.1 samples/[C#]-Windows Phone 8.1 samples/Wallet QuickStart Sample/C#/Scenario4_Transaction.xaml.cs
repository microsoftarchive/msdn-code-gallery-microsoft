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
    /// Adding a tranaction to a Wallet item. 
    /// </summary>
    public sealed partial class Scenario4 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        // The wallet item that used in this scenario.
        private WalletItem walletItem;

        public Scenario4()
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

        private async void Transaction_Click(object sender, RoutedEventArgs e)
        {
            await this.AddTransactionAsync();
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

        private async Task AddTransactionAsync()
        {

            if (walletItem == null)
            {
                rootPage.NotifyUser("Item does not exist. Add item using Scenario2", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                // Create a transaction.
                WalletTransaction walletTransaction = new WalletTransaction();
                walletTransaction.Description = "Double tall latte";
                walletTransaction.DisplayAmount = "$3.27";

                // The date and time of the transaction
                walletTransaction.TransactionDate = DateTime.Now;

                // Don't display the time of the transaction, just the date.
                walletTransaction.IgnoreTimeOfDay = false;

                // A string representing where the transaction took place.
                walletTransaction.DisplayLocation = "Contoso on 5th";

                // Add the transaction to the TransactionHistory of this item.
                walletItem.TransactionHistory.Add("txnid123", walletTransaction);

                // Update the item in Wallet.
                await wallet.UpdateAsync(walletItem);
                rootPage.NotifyUser("Transaction added.", NotifyType.StatusMessage);
            }
            catch(Exception ex)
            {
                string errorMessage = String.Format("Could not add transaction. {0}", ex.Message);
                rootPage.NotifyUser(errorMessage, NotifyType.ErrorMessage);
            }
        }


    }
}
