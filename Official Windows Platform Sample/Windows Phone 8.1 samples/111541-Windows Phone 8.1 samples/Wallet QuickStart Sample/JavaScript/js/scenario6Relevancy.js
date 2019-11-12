//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6Relevancy.html", {
            ready: function (element, options) {
                document.getElementById("addRelevancyData").addEventListener("click", addRelevancyData);
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

    /// Sets when this item is most relevant.
    /// When an item is considered relevant, it will be promoted to the top spot in the Wallet summary view list
    /// and a toast notifcation will also be shown in a pop-up on the user's phone. 
    /// </summary>
    function addRelevancyData() {
        if (walletItem) {
            walletItem.relevantLocations.clear();

            var location = new Windows.ApplicationModel.Wallet.WalletRelevantLocation();
            location.displayMessage = "Welcome to Contoso Coffee on 5th";
            location.position = {
                altitude: 0,
                latitude: 47.647397,
                longitude: -122.133496
            };

            walletItem.relevantLocations.insert("5thId", location);
            walletItem.relevantDate = new Date();
            walletItem.relevantDateDisplayMessage = "Free coffee day";

            return wallet.updateAsync(walletItem).done(function () {
                WinJS.log && WinJS.log("Relevancy data added. Open Wallet to see this item promoted to the top of the list because it is relevant (relevant date was defined as today).", "sample", "status");
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
