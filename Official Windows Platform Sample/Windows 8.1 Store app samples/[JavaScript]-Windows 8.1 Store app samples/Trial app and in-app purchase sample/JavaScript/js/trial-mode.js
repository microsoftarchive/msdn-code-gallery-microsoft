//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
    var page = WinJS.UI.Pages.define("/html/trial-mode.html", {
        ready: function (element, options) {
            document.getElementById("trialProduct").addEventListener("click", runTrialProduct, false);
            document.getElementById("fullProduct").addEventListener("click", runFullProduct, false);
            document.getElementById("trialTime").addEventListener("click", caculateTrialTime, false);
            document.getElementById("convertTrial").addEventListener("click", doTrialConversion, false);

            // Initialize the license proxy file
            loadTrialModeProxyFile();
        },
        unload: function () {
            currentApp.licenseInformation.removeEventListener("licensechanged", trialModeRefreshScenario);
        }
    });

    function loadTrialModeProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("trial-mode.xml").done(
                    function (file) {
                        currentApp.licenseInformation.addEventListener("licensechanged", trialModeRefreshScenario);
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                    });
            });
    }

    function trialModeRefreshScenario() {
        // setup application upsell message
        currentApp.loadListingInformationAsync().done(
        function (listing) {
            document.getElementById("purchasePrice").innerText = "You can buy the full app for: " + listing.formattedPrice + ".";
        }, 
        function () {
            WinJS.log && WinJS.log("LoadListingInformationAsync API call failed", "sample", "error");
        });
        displayCurrentLicenseMode();
    }

    function displayCurrentLicenseMode() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.isActive) {
            if (licenseInformation.isTrial) {
                document.getElementById("currentLicenseMode").innerText = "Current license mode: Trial license";
            } else {
                document.getElementById("currentLicenseMode").innerText = "Current license mode: Full license";
            }
        } else {
            document.getElementById("currentLicenseMode").innerText = "Current license mode: Inactive license";
        }
    }

    function runTrialProduct() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.isActive) {
            if (licenseInformation.isTrial) {
                WinJS.log && WinJS.log("You are using a trial version of this app.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("You no longer have a trial version of this app.", "sample", "error");
            }
        } else {
            WinJS.log && WinJS.log("You don't have a license for this app.", "sample", "error");
        }
    }

    function runFullProduct() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.isActive) {
            if (licenseInformation.isTrial) {
                WinJS.log && WinJS.log("You are using a trial version of this app.", "sample", "error");
            } else {
                WinJS.log && WinJS.log("You are using a fully-licensed version of this app.", "sample", "status");
            }
        } else {
            WinJS.log && WinJS.log("You don't have a license for this app.", "sample", "error");
        }
    }

    function caculateTrialTime() {
        var licenseInformation = currentApp.licenseInformation;
        if (licenseInformation.isActive) {
            if (licenseInformation.isTrial) {
                // calculate remaining trial time
                var remainingTrialTime = (licenseInformation.expirationDate - new Date()) / 86400000;
                WinJS.log && WinJS.log("You can use this app for " + Math.round(remainingTrialTime) + " more days before the trial period ends.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("You have a full license. The trial time is not meaningful.", "sample", "error");
            }
        } else {
            WinJS.log && WinJS.log("Inactive license: trial time unavailable", "sample", "error");
        }
    }

    function doTrialConversion() {
        WinJS.log && WinJS.log("Buying the full license...", "sample", "status");
        var licenseInformation = currentApp.licenseInformation;
        if (!licenseInformation.isActive || licenseInformation.isTrial) {
            currentApp.requestAppPurchaseAsync(false).done(
            function () {
                if (licenseInformation.isActive && !licenseInformation.isTrial) {
                    WinJS.log && WinJS.log("You successfully upgraded your app to the fully-licensed version.", "sample", "status");
                } else {
                    WinJS.log && WinJS.log("You still have a trial license for this app.", "sample", "error");
                }
            },
            function () {
                WinJS.log && WinJS.log("The upgrade transaction failed. You still have a trial license for this app.", "sample", "error");
            });
        } else {
            WinJS.log && WinJS.log("You already bought this app and have a fully-licensed version.", "sample", "error");
        }
    }
})();
