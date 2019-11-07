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
    var ProductPurchaseDisplayProperties = Windows.ApplicationModel.Store.ProductPurchaseDisplayProperties;

    var page = WinJS.UI.Pages.define("/html/in-app-purchase-large-catalog.html", {
        ready: function (element, options) {
            document.getElementById("buyAndFulfillProduct1Button").addEventListener("click", purchaseAndFulfillProduct1, false);

            // Initialize the license proxy file
            loadInAppPurchaseLargeCatalogProxyFile();
            numberOfConsumablesPurchased = 0;
            grantedIds = {
                "product1": []
            };
        }
    });

    function loadInAppPurchaseLargeCatalogProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("in-app-purchase-large-catalog.xml").done(
                    function (file) {
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                        // setup product upsell messages
                        currentApp.loadListingInformationAsync().done(
                            function (listing) {
                                var product1 = listing.productListings.lookup("product1");
                                document.getElementById("product1Message").innerText = "You can buy " + product1.name + " for: " + product1.formattedPrice + ".";
                                product1ListingName = product1.name;
                            },
                            function () {
                                log("LoadListingInformationAsync API call failed", "sample", "error");
                            });
                        log("", "sample", "status");
                    });
            });
    }

    function purchaseAndFulfillProduct1() {
        var offerId = document.getElementById("offerId").value;
        var displayPropertiesName = document.getElementById("displayPropertiesName").value;
        var displayProperties = new ProductPurchaseDisplayProperties(displayPropertiesName);

        log("Buying product 1...", "sample", "status");
        currentApp.requestProductPurchaseAsync("product1", offerId, displayProperties).done(
            function (purchaseResults) {
                if (purchaseResults.status === ProductPurchaseStatus.succeeded) {
                    grantFeatureLocally("product1", purchaseResults.transactionId);
                    fulfillProduct1("product1", purchaseResults);
                } else if (purchaseResults.status === ProductPurchaseStatus.notFulfilled) {
                    if (!isLocallyFulfilled("product1", purchaseResults.transactionId)) {
                        grantFeatureLocally("product1", purchaseResults.transactionId);
                    }
                    fulfillProduct1("product1", purchaseResults);
                } else if (purchaseResults.status === ProductPurchaseStatus.notPurchased) {
                    log("Product 1 was not purchased.", "sample", "status");
                }
            },
            function () {
                log("Unable to buy product 1.", "sample", "error");
            });
    }

    function fulfillProduct1(productId, purchaseResults) {
        var displayPropertiesName = document.getElementById("displayPropertiesName").value;
        if (displayPropertiesName === "") {
            displayPropertiesName = product1ListingName;
        }
        var offerIdMsg = " with offer id " + purchaseResults.offerId;
        if (!purchaseResults.offerId) {
            offerIdMsg = " with no offer id";
        }
        var purchaseStringSimple = "You bought product 1.";
        if (purchaseResults.status === ProductPurchaseStatus.notFulfilled) {
            purchaseStringSimple = "You already purchased product 1.";
        }

        currentApp.reportConsumableFulfillmentAsync(productId, purchaseResults.transactionId).done(
            function (result) {
                switch (result) {
                    case FulfillmentResult.succeeded:
                        if (purchaseResults.status === ProductPurchaseStatus.notFulfilled) {
                            log("You already purchased " + product1ListingName + offerIdMsg + " and it was just fulfilled.", "sample", "status");
                        } else {
                            log("You bought and fulfilled " + displayPropertiesName + offerIdMsg, "sample", "status");
                        }
                        break;
                    case FulfillmentResult.nothingToFulfill:
                        log("There is no purchased product 1 to fulfill with that transaction id.", "sample", "status");
                        break;
                    case FulfillmentResult.purchasePending:
                        log(purchaseStringSimple + " The purchase is pending so we cannot fulfill the product.", "sample", "status");
                        break;
                    case FulfillmentResult.purchaseReverted:
                        log(purchaseStringSimple + " But your purchase has been reverted.", "sample", "status");
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.serverError:
                        log(purchaseStringSimple + " There was an error when fulfilling.", "sample", "status");
                        break;
                }
            },
            function (error) {
                log(purchaseStringSimple + " There was an error when attempting to fulfill.", "sample", "error");
            });
    }

    function log(message, tag, type) {
        var logMessage = message + "\n\n" + getFulfillmentResults();
        WinJS.log && WinJS.log(logMessage, tag, type);
    }

    var product1ListingName = "";
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
