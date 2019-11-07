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
    /// Lauching Wallet from your app.
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            // Request an instance of the default wallet store
            wallet = await WalletManager.RequestStoreAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Launch Wallet
            await wallet.ShowAsync();
           
        }

    }
}
