//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2Create.html", {
            ready: function (element, options) {
                document.getElementById("addItem").addEventListener("click", addItem);
                document.getElementById("viewInWallet").addEventListener("click", viewInWallet);
                initialize();
            }
        }),
        wallet;

    function initialize() {
        Windows.ApplicationModel.Wallet.WalletManager.requestStoreAsync().done(
            function (walletIn) {
                wallet = walletIn;
            },
            function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
    }

    function viewInWallet() {
        wallet.getWalletItemAsync("CoffeeLoyalty123").then(function (walletItem) {
            if (walletItem) {
                // If the item exists, show it in Wallet
                // Launch Wallet and navigate to item
                return wallet.showAsync(walletItem.id);
            }
            else {
                // Item does not exist, so just launch Wallet.
                // Alternatively, you could tell the user that the item they want to see does not exist
                // and stay in your app.
                return wallet.showAsync();
            }
        }).done(undefined, function (error) {
            WinJS.log && WinJS.log("Error: " + error, "sample", "error");
        });
    }

    function addItem() {
        var card,
            WalletNS = Windows.ApplicationModel.Wallet;

        card = new WalletNS.WalletItem(WalletNS.WalletItemKind.membershipCard, "Contoso Loyaly Card");

        // Set colors, to give the card our distinct branding.
        card.bodyColor = Windows.UI.Colors.brown;
        card.bodyFontColor = Windows.UI.Colors.white;
        card.headerColor = Windows.UI.Colors.saddleBrown;
        card.headerFontColor = Windows.UI.Colors.white;

        // Set basic properties.
        card.issuerDisplayName = "Contoso Coffee";

        // Set some images.
        WinJS.Promise.join([
            Windows.Storage.StorageFile.getFileFromApplicationUriAsync(new Windows.Foundation.Uri("ms-appx:///assets/coffee336x336.png")).then(function (file) {
                card.logo336x336 = file;
            }),
            Windows.Storage.StorageFile.getFileFromApplicationUriAsync(new Windows.Foundation.Uri("ms-appx:///assets/coffee99x99.png")).then(function (file) {
                card.logo99x99 = file;
            }),
            Windows.Storage.StorageFile.getFileFromApplicationUriAsync(new Windows.Foundation.Uri("ms-appx:///assets/coffee159x159.png")).then(function (file) {
                card.logo159x159 = file;
            }),
            Windows.Storage.StorageFile.getFileFromApplicationUriAsync(new Windows.Foundation.Uri("ms-appx:///assets/header640x130.png")).then(function (file) {
                card.headerBackgroundImage = file;
            })
        ]).then(function () {
            var prop = new WalletNS.WalletItemCustomProperty("Coffee Points", "99");
            prop.detailViewPosition = WalletNS.WalletDetailViewPosition.footerField1;
            prop.summaryViewPosition = WalletNS.WalletSummaryViewPosition.field1;
            card.displayProperties["PointsId"] = prop;

            // Show the branch.
            prop = new WalletNS.WalletItemCustomProperty("Branch", "Contoso on 5th");
            prop.detailViewPosition = WalletNS.WalletDetailViewPosition.headerField1;
            card.displayProperties["BranchId"] = prop;

            // Add the customer account number.
            prop = new WalletNS.WalletItemCustomProperty("Account Number", "12345678");
            prop.detailViewPosition = WalletNS.WalletDetailViewPosition.footerField2;

            // We don't want this field entity extracted as it will be interpreted as a phone number.
            prop.autoDetectLinks = false;
            card.displayProperties["AcctId"] = prop;

            // Encode the user's account number as a Qr Code to be used in the store
            card.barcode = new WalletNS.WalletBarcode(WalletNS.WalletBarcodeSymbology.qr, "12345678");

            // Add a promotional message to the card.
            card.displayMessage = "Tap here for your 15% off coupon";
            card.isDisplayMessageLaunchable = true;

            return wallet.addAsync("CoffeeLoyalty123", card);
        }).done(function () {
            WinJS.log && WinJS.log("Item has been added to Wallet. Tap \"View item in Wallet\" to see it in Wallet.", "sample", "status");
        }, function(error) {
            WinJS.log && WinJS.log("Error: " + error, "sample", "error");
        });
    }
})();
