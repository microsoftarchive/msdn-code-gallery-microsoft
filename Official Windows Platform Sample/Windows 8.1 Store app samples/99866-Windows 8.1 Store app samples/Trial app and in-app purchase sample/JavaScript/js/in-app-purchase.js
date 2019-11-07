//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
    var page = WinJS.UI.Pages.define("/html/in-app-purchase.html", {
        ready: function (element, options) {
            document.getElementById("testProduct1").addEventListener("click", testProduct1, false);
            document.getElementById("product1PurchaseButton").addEventListener("click", purchaseProduct1, false);
            document.getElementById("testProduct2").addEventListener("click", testProduct2, false);
            document.getElementById("product2PurchaseButton").addEventListener("click", purchaseProduct2, false);

            // Initialize the license proxy file
            loadInAppPurchaseProxyFile();
        },
        unload: function () {
            currentApp.licenseInformation.removeEventListener("licensechanged", inAppPurchaseRefreshScenario);
        }
    });

    function loadInAppPurchaseProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("in-app-purchase.xml").done(
                    function (file) {
                        currentApp.licenseInformation.addEventListener("licensechanged", inAppPurchaseRefreshScenario);
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                        // setup product upsell messages
                        currentApp.loadListingInformationAsync().done(
                            function (listing) {
                                var product1 = listing.productListings.lookup("product1");
                                var product2 = listing.productListings.lookup("product2");
                                document.getElementById("product1SellMessage").innerText = "You can buy " + product1.name + " for: " + product1.formattedPrice + ".";
                                document.getElementById("product2SellMessage").innerText = "You can buy " + product2.name + " for: " + product2.formattedPrice + ".";
                            },
                            function () {
                                WinJS.log && WinJS.log("LoadListingInformationAsync API call failed", "sample", "error");
                            });
                    });
            });
    }

    function inAppPurchaseRefreshScenario() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.isActive) {
            // license status is active - check the trial status
            if (licenseInformation.isTrial) {
                WinJS.log && WinJS.log("You have only a trial license. You need a full license to make an in-app purchase.", "sample", "error");
            }
        } else {
            WinJS.log && WinJS.log("You don't have active license.", "sample", "error");
        }
    }

    function testProduct1() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.productLicenses.lookup("product1").isActive) {
            WinJS.log && WinJS.log("You can use Product 1.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("You don't own Product 1. You must buy Product 1 before you can use it.", "sample", "error");
        }
    }

    function purchaseProduct1() {
        var licenseInformation = currentApp.licenseInformation;
        if (!licenseInformation.productLicenses.lookup("product1").isActive) {
            WinJS.log && WinJS.log("Buying Product 1...", "sample", "status");
            currentApp.requestProductPurchaseAsync("product1").done(
                function () {
                    if (licenseInformation.productLicenses.lookup("product1").isActive) {
                        WinJS.log && WinJS.log("You bought Product 1.", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log("Product 1 was not purchased.", "sample", "status");
                    }
                },
                function () {
                    WinJS.log && WinJS.log("Unable to buy Product 1.", "sample", "error");
                });
        } else {
            WinJS.log && WinJS.log("You already own Product 1.", "sample", "error");
        }
    }

    function testProduct2() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.productLicenses.lookup("product2").isActive) {
            WinJS.log && WinJS.log("You can use Product 2.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("You don't own Product 2. You must buy Product 2 before you can use it.", "sample", "error");
        }
    }

    function purchaseProduct2() {
        var licenseInformation = currentApp.licenseInformation;
        if (!licenseInformation.productLicenses.lookup("product2").isActive) {
            WinJS.log && WinJS.log("Buying Product 2...", "sample", "status");
            currentApp.requestProductPurchaseAsync("product2").done(
                function () {
                    if (licenseInformation.productLicenses.lookup("product2").isActive) {
                        WinJS.log && WinJS.log("You bought Product 2.", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log("Product 2 was not purchased.", "sample", "status");
                    }
                },
                function () {
                    WinJS.log && WinJS.log("Unable to buy product 2.", "sample", "error");
                });
        } else {
            WinJS.log && WinJS.log("You already own Product 2.", "sample", "error");
        }
    }
})();
