//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("startReadButton").addEventListener("click", startRead, false);
            document.getElementById("endReadButton").addEventListener("click", endRead, false);
            document.getElementById("startReadButton").disabled = false;
            document.getElementById("endReadButton").disabled = false;
        },

        unload: function () {
            if (document.getElementById("startReadButton").disabled) {
                // Close the magnetic stripe reader.
                if (_claimedReader !== null) {
                    _claimedReader.removeEventListener("bankcarddatareceived", onBankCardDataReceived);
                    _claimedReader.close();
                    _claimedReader = null;
                }

                _reader = null;
            }
        }
    });

    var _reader = null;
    var _claimedReader = null;

    function startRead() {
        // Get the default magnetic stripe reader.
        Windows.Devices.PointOfService.MagneticStripeReader.getDefaultAsync().then(function (reader) {
            if (reader !== null) {
                _reader = reader;
                WinJS.log && WinJS.log("Default Magnetic Stripe Reader created.", "sample", "status");
                WinJS.log && WinJS.log("Device Id is:" + reader.deviceId, "sample", "status");

                // Claim the magnetic stripe reader for exclusive use.
                reader.claimReaderAsync().done(function (claimedReader) {
                    _claimedReader = claimedReader;
                    claimedReader.isDecodeDataEnabled = true;
                    WinJS.log && WinJS.log("Claim Magnetic Stripe Reader succeeded.", "sample", "status");

                    // Register event listeners
                    claimedReader.addEventListener("bankcarddatareceived", onBankCardDataReceived);

                    // Enable receiving data
                    claimedReader.enableAsync().done(function () {
                        WinJS.log && WinJS.log("Enable Magnetic Stripe Reader succeeded.", "sample", "status");
                        WinJS.log && WinJS.log("Ready to swipe...", "sample", "status");
                        document.getElementById("startReadButton").disabled = true;
                        document.getElementById("endReadButton").disabled = false;
                    }, function error(e) {
                        WinJS.log && WinJS.log("Error enabling reader..." + e.message, "sample", "status");
                    });
                }, function error(e) {
                    WinJS.log && WinJS.log("Could not claim reader..." + e.message, "sample", "status");
                });
            }
            else {
                WinJS.log && WinJS.log("Could not claim reader...", "sample", "status");
            }
               
        }, function error(e) {
            WinJS.log && WinJS.log("Magnetic Stripe Reader not found.  Please connect a Magnetic Stripe Reader.", "sample", "status");
        });
    }

    function onBankCardDataReceived(args) {
        WinJS.log && WinJS.log("Got data", "sample", "status");

        // Display the received data
        document.getElementById("accountNumber").textContent = args.accountNumber;
        document.getElementById("expirationDate").textContent = args.expirationDate;
        document.getElementById("firstName").textContent = args.firstName;
        document.getElementById("middleInitial").textContent = args.middleInitial;
        document.getElementById("serviceCode").textContent = args.serviceCode;
        document.getElementById("suffix").textContent = args.suffix;
        document.getElementById("surname").textContent = args.surname;
        document.getElementById("title").textContent = args.title;
    }

    function endRead() {
        // Remove event listeners and unclaim the reader.
        if (_claimedReader !== null) {
            _claimedReader.removeEventListener("bankcarddatareceived", onBankCardDataReceived);
            _claimedReader.close();
            _claimedReader = null;
        }
        _reader = null;
        WinJS.log("Click the Start Receiving Data Button..", "sample", "status");
        document.getElementById("startReadButton").disabled = false;
        document.getElementById("endReadButton").disabled = true;

        // Clear any displayed data.
        document.getElementById("accountNumber").textContent = "No data";
        document.getElementById("expirationDate").textContent = "No data";
        document.getElementById("firstName").textContent = "No data";
        document.getElementById("middleInitial").textContent = "No data";
        document.getElementById("serviceCode").textContent = "No data";
        document.getElementById("suffix").textContent = "No data";
        document.getElementById("surname").textContent = "No data";
        document.getElementById("title").textContent = "No data";
    }
})();
