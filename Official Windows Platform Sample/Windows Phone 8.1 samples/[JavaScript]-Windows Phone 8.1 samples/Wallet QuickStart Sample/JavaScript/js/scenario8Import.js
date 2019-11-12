//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario8Import.html", {
            ready: function (element, options) {
                document.getElementById("importMswallet").addEventListener("click", importMswallet);
                document.getElementById("viewInWallet").addEventListener("click", viewInWallet);
                initialize();
            }
        }),
        wallet,
        walletItem;

    function initialize() {
        Windows.ApplicationModel.Wallet.WalletManager.requestStoreAsync().done(function (walletIn) {
            wallet = walletIn;
        }, function (error) {
            WinJS.log && WinJS.log("Error: " + error, "sample", "error");
        });
    }

    function importMswallet() {
        Windows.Storage.StorageFile.getFileFromApplicationUriAsync(new Windows.Foundation.Uri("ms-appx:///assets/ContosoCoffeePikePlace.mswallet")).then(function (file) {
            return wallet.importItemAsync(file);
        }).done(function (walletItemIn) {
            walletItem = walletItemIn;
            WinJS.log && WinJS.log("Import succeeded. Tap 'view in wallet' to see the imported item.", "sample", "status");
        }, function (error) {
            WinJS.log && WinJS.log("Error: " + error, "sample", "error");
        });
    }

    function viewInWallet() {
        if (wallet && walletItem) {
            wallet.showAsync(walletItem.id).done(undefined, function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
        }
        else if (!wallet) {
            WinJS.log && WinJS.log("Please wait for wallet to initialize.", "sample", "error");
        }
        else {
            WinJS.log && WinJS.log("Item does not exist. Add item by pressing 'import' first.", "sample", "error");
        }
    }
})();
