//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
    var ProductPurchaseStatus = Windows.ApplicationModel.Store.ProductPurchaseStatus;
    var FulfillmentResult = Windows.ApplicationModel.Store.FulfillmentResult;

    var page = WinJS.UI.Pages.define("/html/in-app-purchase-consumables.html", {
        ready: function (element, options) {
            document.getElementById("buyAndFulfillProduct1Button").addEventListener("click", purchaseAndFulfillProduct1, false);

            // Initialize the license proxy file
            loadInAppPurchaseConsumablesProxyFile();
            numberOfConsumablesPurchased = 0;
            grantedIds = {
                "product1": []
            };
        }
    });

    function loadInAppPurchaseConsumablesProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("in-app-purchase-consumables.xml").done(
                    function (file) {
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                        // setup product upsell messages
                        currentApp.loadListingInformationAsync().done(
                            function (listing) {
                                var product1 = listing.productListings.lookup("product1");
                                document.getElementById("product1Message").innerText = "You can buy " + product1.name + " for: " + product1.formattedPrice + ".";
                            },
                            function () {
                                log("LoadListingInformationAsync API call failed", "sample", "error");
                            });
                        log("", "sample", "status");
                    });
            });
    }

    function purchaseAndFulfillProduct1() {
        log("Buying product 1...", "sample", "status");
        currentApp.requestProductPurchaseAsync("product1").done(
            function (purchaseResults) {
                if (purchaseResults.status === ProductPurchaseStatus.succeeded) {
                    grantFeatureLocally("product1", purchaseResults.transactionId);
                    fulfillProduct1("product1", purchaseResults.transactionId);
                } else if (purchaseResults.status === ProductPurchaseStatus.notFulfilled) {
                    if (!isLocallyFulfilled("product1", purchaseResults.transactionId)) {
                        grantFeatureLocally("product1", purchaseResults.transactionId);
                    }
                    fulfillProduct1("product1", purchaseResults.transactionId);
                } else if (purchaseResults.status === ProductPurchaseStatus.notPurchased) {
                    log("Product 1 was not purchased.", "sample", "status");
                }
            },
            function () {
                log("Unable to buy product 1.", "sample", "error");
            });
    }

    function fulfillProduct1(productId, transactionId) {
        currentApp.reportConsumableFulfillmentAsync(productId, transactionId).done(
            function (result) {
                switch (result) {
                    case FulfillmentResult.succeeded:
                        log("You bought and fulfilled product 1.", "sample", "status");
                        break;
                    case FulfillmentResult.nothingToFulfill:
                        log("There is no purchased product 1 to fulfill.", "sample", "status");
                        break;
                    case FulfillmentResult.purchasePending:
                        log("You bought product 1. The purchase is pending so we cannot fulfill the product.", "sample", "status");
                        break;
                    case FulfillmentResult.purchaseReverted:
                        log("You bought product 1. But your purchase has been reverted.", "sample", "status");
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.serverError:
                        log("You bought product 1. There was an error when fulfilling.", "sample", "status");
                        break;
                }
            },
            function (error) {
                log("You bought Product 1. There was an error when attempting to fulfill.", "sample", "error");
            });
    }

    function log(message, tag, type) {
        var logMessage = message + "\n\n" + getFulfillmentResults();
        WinJS.log && WinJS.log(logMessage, tag, type);
    }

    var numberOfConsumablesPurchased = 0;
    var grantedIds = {
        "product1": []
    };
    
    function getFulfillmentResults() {
        var message = "Product 1 has been fulfilled " + numberOfConsumablesPurchased + " time" + (numberOfConsumablesPurchased === 1 ? "" : "s") + ".";
        return message;
    }

    // Keeps a record of which ones we have granted
    function grantFeatureLocally(productId, transactionId) {
        var nextIndex = grantedIds[productId].length;
        grantedIds[productId][nextIndex] = transactionId;

        // Grant the user the content, such as by increasing some kind of asset count
        numberOfConsumablesPurchased++;
    }

    // Checks if we have granted to this transactionId before
    function isLocallyFulfilled(productId, transactionId) {
        for (var i in grantedIds[productId]) {
            if (grantedIds[productId][i] === transactionId) {
                return true;
            }
        }
        return false;
    }

})();
