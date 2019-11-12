// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Wallet;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WalletQuickstart
{
    /// <summary>
    /// Creating a Wallet item. 
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        // Reference to the main page of the sample.
        private MainPage rootPage;

        // The WalletItemStore object that is used to access Wallet in this scenario.
        private WalletItemStore wallet;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            // request an instance of the default wallet store
            wallet = await WalletManager.RequestStoreAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await AddItemAsync();
        }

        private async void View_Click(object sender, RoutedEventArgs e)
        {
            await ShowWalletItemAsync();
        }

        public async Task ShowWalletItemAsync()
        {
            WalletItem walletItem = await wallet.GetWalletItemAsync("CoffeeLoyalty123");

            // If the item exists, show it in Wallet
            if (walletItem != null)
            {
                // Launch Wallet and navigate to item
                await wallet.ShowAsync(walletItem.Id);
            }
            else
            {
                // Item does not exist, so just launch Wallet.
                // Alternatively, you could tell the user that the item they want to see does not exist
                // and stay in your app.
                await wallet.ShowAsync();
            }
        }

        /// <summary>
        /// Adds a new membership card wallet item to Wallet.
        /// </summary>
        /// <returns></returns>
        private async Task AddItemAsync()
        {
            try
            {
                // Create the membership card.
                WalletItem card = new WalletItem(WalletItemKind.MembershipCard, "Contoso Loyalty Card");

                // Set colors, to give the card our distinct branding.
                card.BodyColor = Windows.UI.Colors.Brown;
                card.BodyFontColor = Windows.UI.Colors.White;
                card.HeaderColor = Windows.UI.Colors.SaddleBrown;
                card.HeaderFontColor = Windows.UI.Colors.White;

                // Set basic properties.
                card.IssuerDisplayName = "Contoso Coffee";

                // Set some images.
                card.Logo336x336 =  await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///assets/coffee336x336.png"));

                card.Logo99x99 =    await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///assets/coffee99x99.png"));

                card.Logo159x159 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///assets/coffee159x159.png"));

                card.HeaderBackgroundImage = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///assets/header640x130.png"));

                // Set the loyalty card points and show them on the detailed view of card and in the list view.
                WalletItemCustomProperty prop = new WalletItemCustomProperty("Coffee Points", "99");
                prop.DetailViewPosition = WalletDetailViewPosition.FooterField1;
                prop.SummaryViewPosition = WalletSummaryViewPosition.Field1;
                card.DisplayProperties["PointsId"] = prop;

                // Show the branch.
                prop = new WalletItemCustomProperty("Branch", "Contoso on 5th");
                prop.DetailViewPosition = WalletDetailViewPosition.HeaderField1;
                card.DisplayProperties["BranchId"] = prop;

                // Add the customer account number.
                prop = new WalletItemCustomProperty("Account Number", "12345678");
                prop.DetailViewPosition = WalletDetailViewPosition.FooterField2;

                // We don't want this field entity extracted as it will be interpreted as a phone number.
                prop.AutoDetectLinks = false;
                card.DisplayProperties["AcctId"] = prop;

                // Encode the user's account number as a Qr Code to be used in the store.
                card.Barcode = new WalletBarcode(WalletBarcodeSymbology.Qr, "12345678");

                // Add a promotional message to the card.
                card.DisplayMessage = "Tap here for your 15% off coupon";
                card.IsDisplayMessageLaunchable = true;

                await wallet.AddAsync("CoffeeLoyalty123", card);

                rootPage.NotifyUser("Item has been added to Wallet. Tap \"View item in Wallet\" to see it in Wallet.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }
    }
}
