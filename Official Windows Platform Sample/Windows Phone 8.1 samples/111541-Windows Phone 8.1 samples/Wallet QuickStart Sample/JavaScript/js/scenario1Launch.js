//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1Launch.html", {
            ready: function (element, options) {
                document.getElementById("launch").addEventListener("click", launch);
                initialize();
            }
        }),
        wallet;

    function initialize() {
        Windows.ApplicationModel.Wallet.WalletManager.requestStoreAsync().done(
            function (walletIn) {
                wallet = walletIn;
            },
            function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
    }

    function launch() {
        if (wallet) {
            wallet.showAsync().done(undefined, function (error) {
                WinJS.log && WinJS.log("Error: " + error, "sample", "error");
            });
        }
        else {
            WinJS.log && WinJS.log("Please wait for wallet to initialize.", "sample", "error");
        }
    }

})();
