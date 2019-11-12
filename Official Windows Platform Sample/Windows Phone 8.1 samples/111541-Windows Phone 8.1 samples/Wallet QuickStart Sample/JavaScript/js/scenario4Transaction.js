//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4Transaction.html", {
            ready: function (element, options) {
                document.getElementById("addTransaction").addEventListener("click", addTransaction);
                document.getElementById("viewInWallet").addEventListener("click", viewInWallet);
                initialize();
            }
        }),
        wallet,
        walletItem;

    function initialize() {
        Windows.ApplicationModel.Wallet.WalletManager.requestStoreAsync().then(
            function (walletIn) {
                wallet = walletIn;
                return wallet.getWalletItemAsync("CoffeeLoyalty123");
            }).done(function (walletItemIn) {
                walletItem = walletItemIn;
            }, function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
    }

    function addTransaction() {
        var walletTransaction;

        if (walletItem) {
            walletTransaction = new Windows.ApplicationModel.Wallet.WalletTransaction();
            walletTransaction.description = "Double tall latte";
            walletTransaction.displayAmount = "$3.27";

            // The date and time of the transaction
            walletTransaction.TransactionDate = DateTime.Now;

            // Don't display the time of the transaction, just the date.
            walletTransaction.IgnoreTimeOfDay = false;

            // A string representing where the transaction took place.
            walletTransaction.DisplayLocation = "Contoso on 5th";

            // Add the transaction to the TransactionHistory of our walletItem.
            walletItem.transactionHistory.insert("txnid123", walletTransaction);

            // Update the item in Wallet.
            wallet.updateAsync(walletItem).done(function () {
                WinJS.log && WinJS.log("Transaction added.", "sample", "status");
            }, function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
        }
        else {
            WinJS.log && WinJS.log("Item does not exist. Add item using Scenario2.", "sample", "error");
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
