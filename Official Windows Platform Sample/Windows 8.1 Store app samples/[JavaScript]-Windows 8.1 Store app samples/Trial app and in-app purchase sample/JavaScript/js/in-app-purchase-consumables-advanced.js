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

    var page = WinJS.UI.Pages.define("/html/in-app-purchase-consumables-advanced.html", {
        ready: function (element, options) {
            document.getElementById("buyProduct1Button").addEventListener("click", purchaseProduct1, false);
            document.getElementById("getUnfulfilledButton1").addEventListener("click", getUnfulfilledConsumables, false);
            document.getElementById("fulfillProduct1Button").addEventListener("click", fulfillProduct1, false);
            document.getElementById("buyProduct2Button").addEventListener("click", purchaseProduct2, false);
            document.getElementById("getUnfulfilledButton2").addEventListener("click", getUnfulfilledConsumables, false);
            document.getElementById("fulfillProduct2Button").addEventListener("click", fulfillProduct2, false);

            // Initialize the license proxy file
            loadInAppPurchaseConsumablesAdvancedProxyFile();

            tempTransactionId = {
                "product1": "00000000-0000-0000-0000-000000000000",
                "product2": "00000000-0000-0000-0000-000000000000",
            };
            numberOfProductPurchases = {
                "product1": 0,
                "product2": 0,
            };
            numberOfProductFulfillments = {
                "product1": 0,
                "product2": 0,
            };
            grantedIds = {
                "product1": [],
                "product2": [],
            };
        }
    });

    function loadInAppPurchaseConsumablesAdvancedProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("in-app-purchase-consumables-advanced.xml").done(
                    function (file) {
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                        // setup product upsell messages
                        currentApp.loadListingInformationAsync().done(
                            function (listing) {
                                var product1 = listing.productListings.lookup("product1");
                                document.getElementById("product1Message").innerText = "You can buy " + product1.name + " for: " + product1.formattedPrice + ".";

                                var product2 = listing.productListings.lookup("product2");
                                document.getElementById("product2Message").innerText = "You can buy " + product2.name + " for: " + product2.formattedPrice + ".";
                            },
                            function () {
                                log("LoadListingInformationAsync API call failed", "sample", "error");
                            });

                        currentApp.getUnfulfilledConsumablesAsync().done(
                            function (unfulfilledList) {
                                unfulfilledList.forEach(function (product) {
                                    // This is where you would normally grant the user their consumable content and call currentApp.reportConsumableFulfillment
                                    tempTransactionId[product.productId] = product.transactionId;
                                });
                            },
                            function (error) {
                                log("Unable to get unfulfilled consumables.", "sample", "error");
                            });

                        log("", "sample", "status");
                    });
            });
    }

    function getUnfulfilledConsumables() {
        currentApp.getUnfulfilledConsumablesAsync().done(
            function (unfulfilledList) {
                var logMessage = "List of unfulfilled consumables:";

                unfulfilledList.forEach(function (product) {
                    logMessage += "\nProduct Id: " + product.productId + " Transaction Id: " + product.transactionId;
                    // This is where you would grant the user their consumable content and call currentApp.reportConsumableFulfillment
                });

                if (unfulfilledList.length === 0) {
                    logMessage = "There are no consumable purchases awaiting fulfillment.";
                }
                log(logMessage, "sample", "status", unfulfilledList.length);
            },
            function (error) {
                log("Unable to get unfulfilled consumables.", "sample", "error");
            });
    }

    function purchaseProduct1() {
        log("Buying product 1...", "sample", "status");
        currentApp.requestProductPurchaseAsync("product1").done(
            function (purchaseResults) {
                if (purchaseResults.status === ProductPurchaseStatus.succeeded) {
                    numberOfProductPurchases["product1"]++;
                    tempTransactionId["product1"] = purchaseResults.transactionId;
                    log("You bought product 1. Transaction Id: " + purchaseResults.transactionId, "sample", "status");

                    // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                } else if (purchaseResults.status === ProductPurchaseStatus.notFulfilled) {
                    tempTransactionId["product1"] = purchaseResults.transactionId;
                    log("You have an unfulfilled copy of product 1. Hit \"Fulfill product 1\" before you can purchase a second copy.", "sample", "status");

                    // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                } else if (purchaseResults.status === ProductPurchaseStatus.notPurchased) {
                    log("Product 1 was not purchased.", "sample", "status");
                }
            },
            function () {
                log("Unable to buy product 1.", "sample", "error");
            });
    }

    function purchaseProduct2() {
        log("Buying product 2...", "sample", "status");
        currentApp.requestProductPurchaseAsync("product2").done(
            function (purchaseResults) {
                if (purchaseResults.status === ProductPurchaseStatus.succeeded) {
                    numberOfProductPurchases["product2"]++;
                    tempTransactionId["product2"] = purchaseResults.transactionId;
                    log("You bought product 2. Transaction Id: " + purchaseResults.transactionId, "sample", "status");
                } else if (purchaseResults.status === ProductPurchaseStatus.notFulfilled) {
                    tempTransactionId["product2"] = purchaseResults.transactionId;
                    log("You have an unfulfilled copy of product 2. Hit \"Fulfill product 2\" before you can purchase a second copy.", "sample", "status");
                } else if (purchaseResults.status === ProductPurchaseStatus.notPurchased) {
                    log("Product 2 was not purchased.", "sample", "status");
                }
            },
            function () {
                log("Unable to buy product 2.", "sample", "error");
            });
    }

    function fulfillProduct1() {
        if (!isLocallyFulfilled("product1", tempTransactionId["product1"])) {
            grantFeatureLocally("product1", tempTransactionId["product1"]);
        }
        currentApp.reportConsumableFulfillmentAsync("product1", tempTransactionId["product1"]).done(
            function (result) {
                switch (result) {
                    case FulfillmentResult.succeeded:
                        numberOfProductFulfillments["product1"]++;
                        log("Product 1 was fulfilled. You are now able to buy product 1 again.", "sample", "status");
                        break;
                    case FulfillmentResult.nothingToFulfill:
                        log("There is nothing to fulfill. You must purchase product 1 before it can be fulfilled.", "sample", "status");
                        break;
                    case FulfillmentResult.purchasePending:
                        log("Purchase hasn't completed yet. Wait and try again.", "sample", "status");
                        break;
                    case FulfillmentResult.purchaseReverted:
                        log("Purchase was reverted before fulfillment.", "sample", "status");
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.serverError:
                        log("There was an error when fulfilling.", "sample", "status");
                        break;
                }
            },
            function (error) {
                log("There was an error when attempting to fulfill.", "sample", "error");
            });
    }

    function fulfillProduct2() {
        if (!isLocallyFulfilled("product2", tempTransactionId["product2"])) {
            grantFeatureLocally("product2", tempTransactionId["product2"]);
        }
        currentApp.reportConsumableFulfillmentAsync("product2", tempTransactionId["product2"]).done(
            function (result) {
                switch (result) {
                    case FulfillmentResult.succeeded:
                        numberOfProductFulfillments["product2"]++;
                        log("Product 2 was fulfilled. You are now able to buy product 2 again.", "sample", "status");
                        break;
                    case FulfillmentResult.nothingToFulfill:
                        log("There is nothing to fulfill. You must purchase product 2 before it can be fulfilled.", "sample", "status");
                        break;
                    case FulfillmentResult.purchasePending:
                        log("Purchase hasn't completed yet. Wait and try again.", "sample", "status");
                        break;
                    case FulfillmentResult.purchaseReverted:
                        log("Purchase was reverted before fulfillment.", "sample", "status");
                        // Since the user's purchase was revoked, they got their money back.
                        // You may want to revoke the user's access to the consumable content that was granted.
                        break;
                    case FulfillmentResult.serverError:
                        log("There was an error when fulfilling.", "sample", "status");
                        break;
                }
            },
            function (error) {
                log("There was an error when attempting to fulfill.", "sample", "error");
            });
    }

    function log(message, tag, type, blankLines) {
        var logMessage = message + "\n\n";
        if (blankLines === 1) {
            logMessage += "\n";
        } else if (blankLines !== 2) {
            logMessage += "\n\n";
        }
        logMessage += getPurchaseAndFulfillmentResults();
        WinJS.log && WinJS.log(logMessage, tag, type);
    }

    var tempTransactionId = {
        "product1": "00000000-0000-0000-0000-000000000000",
        "product2": "00000000-0000-0000-0000-000000000000",
    };

    var numberOfProductPurchases = {
        "product1": 0,
        "product2": 0,
    };

    var numberOfProductFulfillments = {
        "product1": 0,
        "product2": 0,
    };

    var grantedIds = {
        "product1": [],
        "product2": [],
    };
    
    function getPurchaseAndFulfillmentResults() {
        var message = "Product 1 has been purchased " + numberOfProductPurchases["product1"] + " time" + (numberOfProductPurchases["product1"] === 1 ? "" : "s") + " and fulfilled " + numberOfProductFulfillments["product1"] + " time" + (numberOfProductFulfillments["product1"] === 1 ? "" : "s") + ".";
        message += "\n" + "Product 2 has been purchased " + numberOfProductPurchases["product2"] + " time" + (numberOfProductPurchases["product2"] === 1 ? "" : "s") + " and fulfilled " + numberOfProductFulfillments["product2"] + " time" + (numberOfProductFulfillments["product2"] === 1 ? "" : "s") + ".";
        return message;
    }

    // Keeps a record of which ones we have granted
    function grantFeatureLocally(productId, transactionId) {
        var nextIndex = grantedIds[productId].length;
        grantedIds[productId][nextIndex] = transactionId;

        // Grant the user the content, such as by increasing some kind of asset count
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
