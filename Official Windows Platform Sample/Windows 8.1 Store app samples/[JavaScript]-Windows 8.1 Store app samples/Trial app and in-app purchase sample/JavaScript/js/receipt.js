//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
    var page = WinJS.UI.Pages.define("/html/receipt.html", {
        ready: function (element, options) {
            document.getElementById("showReceipt").addEventListener("click", showReceipt, false);
            // Initialize the license proxy file
            loadReceiptProxyFile();
        },
        unload: function () {
            currentApp.licenseInformation.removeEventListener("licensechanged", loadReceiptProxyFile);
        }
    });

    function loadReceiptProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("receipt.xml").done(
                    function (file) {
                        currentApp.licenseInformation.addEventListener("licensechanged", receiptRefreshScenario);
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                    });
            });
    }

    function receiptRefreshScenario() {
    }

    function showReceipt() {
        currentApp.getAppReceiptAsync().done(
            function (receipt) {
                WinJS.log && WinJS.log(receipt, "sample", "status");
            },
            function () {
                WinJS.log && WinJS.log("Get Receipt failed.", "sample", "error");
            }
        );
    }
})();
