//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3Update.html", {
        ready: function (element, options) {
                coffeePointsInput = document.getElementById("coffeePointsInput");
                document.getElementById("updatePoints").addEventListener("click", updatePoints);
                document.getElementById("viewInWallet").addEventListener("click", viewInWallet);
                initialize();                
            }
        }),
        wallet,
        walletItem,
        coffeePointsInput;

    function initialize() {
        Windows.ApplicationModel.Wallet.WalletManager.requestStoreAsync().then(function (walletIn) {
            wallet = walletIn;
            return wallet.getWalletItemAsync("CoffeeLoyalty123");
        }).done(function (walletItemIn) {
            walletItem = walletItemIn;

            if (walletItem) {
                if (walletItem.displayProperties.hasKey("PointsId")) {
                    coffeePointsInput.value = walletItem.displayProperties.PointsId.value;
                }
                else {
                    WinJS.log && WinJS.log("Item does not have a PointsId property.", "sample", "error");
                }
            }
            else {
                WinJS.log && WinJS.log("Item does not exist. Add item using Scenario2", "sample", "error");
            }
        }, function (error) {
            WinJS.log && WinJS.log("Error: " + error, "sample", "error");
        });
    }

    function updatePoints() {
        if (wallet && walletItem && walletItem.displayProperties.hasKey("PointsId")) {
            walletItem.displayProperties.PointsId.value = coffeePointsInput.value;
            wallet.updateAsync(walletItem).done(function () {
                WinJS.log && WinJS.log("Points updated.", "sample", "status");
            }, function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
        }
        else if (!wallet) {
            WinJS.log && WinJS.log("Please wait for wallet to initialize.", "sample", "error");
        }
        else if (!walletItem) {
            WinJS.log && WinJS.log("Item does not exist. Add item using Scenario2", "sample", "error");
        }
        else {
            WinJS.log && WinJS.log("Item does not have a PointsId property.", "sample", "error");
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
