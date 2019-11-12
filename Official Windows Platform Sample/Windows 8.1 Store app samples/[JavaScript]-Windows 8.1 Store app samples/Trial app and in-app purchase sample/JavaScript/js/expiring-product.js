//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
    var page = WinJS.UI.Pages.define("/html/expiring-product.html", {
        ready: function (element, options) {
            // Initialize the license proxy file
            loadExpiringProductProxyFile();
        },
        unload: function () {
            currentApp.licenseInformation.removeEventListener("licensechanged", expiringProductRefreshScenario);
        }
    });

    function loadExpiringProductProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("expiring-product.xml").done(
                    function (file) {
                        currentApp.licenseInformation.addEventListener("licensechanged", expiringProductRefreshScenario);
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                    });
            });
    }

    function expiringProductRefreshScenario() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.isActive) {
            // license status is active - check the trial status
            if (licenseInformation.isTrial) {
                WinJS.log && WinJS.log("You don't have full license.", "sample", "error");
            } else {
                // display products
                var productLicense1 = licenseInformation.productLicenses.lookup("product1");
                if (productLicense1.isActive) {
                    var longDateFormat = Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate");
                    document.getElementById("product1Message").innerText = "Product 1 expires on: " + longDateFormat.format(productLicense1.expirationDate);
                    var remainingTime = (productLicense1.expirationDate - new Date()) / 86400000;
                    WinJS.log && WinJS.log("Product 1 expires in " + Math.round(remainingTime) + " days.", "sample", "status");
                } else {
                    document.getElementById("scenario3Product1Message").innerText = "Product 1 is not available.";
                }
            }
        } else {
            WinJS.log && WinJS.log("You don't have active license.", "sample", "error");
        }
    }
})();
