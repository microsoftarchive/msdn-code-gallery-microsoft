// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Wallet;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WalletQuickstart
{
    /// <summary>
    /// Importing a Wallet item programmatically from a Wallet item package. 
    /// </summary>
    public sealed partial class Scenario8 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        // The wallet item that used in this scenario.
        private WalletItem walletItem;

        public Scenario8()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            // Request an instance of the default Wallet store.
            wallet = await WalletManager.RequestStoreAsync();
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            await this.ImportAsync();
        }

        /// <summary>
        /// Imports a wallet item from a .mswallet file.
        /// </summary>
        /// <returns>An asynchronous action.</returns>
        public async Task ImportAsync()
        {
            try
            {
                // Import another membership card, represented by the .mswallet file located in the Assets folder of this sample. 
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///assets/ContosoCoffeePikePlace.mswallet")); 

                if (file == null)
                {
                    this.rootPage.NotifyUser("Could not open file assets/ContosoCoffeePikePlace.mswallet", NotifyType.ErrorMessage);
                    return;
                }

                walletItem = await wallet.ImportItemAsync(file); 
                this.rootPage.NotifyUser("Import succeeded. Tap \"view in wallet\" to see the imported item", NotifyType.StatusMessage);
               
            }
            catch(FileNotFoundException noFile)
            {
                this.rootPage.NotifyUser(noFile.Message, NotifyType.ErrorMessage);
            }
            catch(Exception ex)
            {
                var errorMessage = String.Format("Import failed {0}", ex.Message);
                this.rootPage.NotifyUser(errorMessage, NotifyType.ErrorMessage);
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
