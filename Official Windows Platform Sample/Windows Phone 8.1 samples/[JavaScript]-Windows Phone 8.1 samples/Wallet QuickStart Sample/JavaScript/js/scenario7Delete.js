//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario7Delete.html", {
            ready: function (element, options) {
                document.getElementById("deleteItem").addEventListener("click", deleteItem);
                document.getElementById("viewInWallet").addEventListener("click", viewInWallet);
                initialize();
            }
        }),
        wallet,
        walletItem;

    function initialize() {
        Windows.ApplicationModel.Wallet.WalletManager.requestStoreAsync().then(function (walletIn) {
            wallet = walletIn;
            return wallet.getWalletItemAsync("CoffeeLoyalty123");
        }).done(function (walletItemIn) {
            walletItem = walletItemIn;
            if (!walletItem) {
                WinJS.log && WinJS.log("Item does not exist. Add item using Scenario2", "sample", "error");
            }
        }, function (error) {
            WinJS.log && WinJS.log("Error: " + error, "sample", "error");
        });
    }

    function deleteItem() {
        if (walletItem) {
            wallet.deleteAsync(walletItem.id).done(function () {
                walletItem = null;
                WinJS.log && WinJS.log("Item deleted.", "sample", "status");
            }, function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
        }
        else {
            WinJS.log && WinJS.log("Item does not exist. Add item using Scenario2", "sample", "error");
        }
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
            WinJS.log && WinJS.log("Item does not exist. Add item using Scenario2", "sample", "error");
        }
    }
})();
