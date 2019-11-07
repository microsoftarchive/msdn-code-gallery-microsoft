//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
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
                    _claimedReader.removeEventListener("aamvacarddatareceived", onAamvaCardDataReceived);
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
                    claimedReader.addEventListener("aamvacarddatareceived", onAamvaCardDataReceived);

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

    function onAamvaCardDataReceived(args) {
        WinJS.log && WinJS.log("Got data", "sample", "status");

        // Display the received data
        document.getElementById("address").textContent = args.address;
        document.getElementById("birthDate").textContent = args.birthDate;
        document.getElementById("city").textContent = args.city;
        document.getElementById("licenseClass").textContent = args.class;
        document.getElementById("endorsements").textContent = args.endorsements;
        document.getElementById("expirationDate").textContent = args.expirationDate;
        document.getElementById("eyeColor").textContent = args.eyeColor;
        document.getElementById("firstName").textContent = args.firstName;
        document.getElementById("gender").textContent = args.gender;
        document.getElementById("hairColor").textContent = args.hairColor;
        document.getElementById("height").textContent = args.height;
        document.getElementById("licenseNumber").textContent = args.licenseNumber;
        document.getElementById("postalCode").textContent = args.postalCode;
        document.getElementById("restrictions").textContent = args.restrictions;
        document.getElementById("state").textContent = args.state;
        document.getElementById("suffix").textContent = args.suffix;
        document.getElementById("surname").textContent = args.surname;
        document.getElementById("weight").textContent = args.weight;
    }

    function endRead() {
        // Remove event listeners and unclaim the reader.
        if (_claimedReader !== null) {
            _claimedReader.removeEventListener("aamvacarddatareceived", onAamvaCardDataReceived);
            _claimedReader.close();
            _claimedReader = null;
        }
        _reader = null;
        WinJS.log("Click the Start Receiving Data Button..", "sample", "status");
        document.getElementById("startReadButton").disabled = false;
        document.getElementById("endReadButton").disabled = true;

        // Clear any displayed data.
        document.getElementById("address").textContent = "No data";
        document.getElementById("birthDate").textContent = "No data";
        document.getElementById("city").textContent = "No data";
        document.getElementById("licenseClass").textContent = "No data";
        document.getElementById("endorsements").textContent = "No data";
        document.getElementById("expirationDate").textContent = "No data";
        document.getElementById("eyeColor").textContent = "No data";
        document.getElementById("firstName").textContent = "No data";
        document.getElementById("gender").textContent = "No data";
        document.getElementById("hairColor").textContent = "No data";
        document.getElementById("height").textContent = "No data";
        document.getElementById("licenseNumber").textContent = "No data";
        document.getElementById("postalCode").textContent = "No data";
        document.getElementById("restrictions").textContent = "No data";
        document.getElementById("state").textContent = "No data";
        document.getElementById("suffix").textContent = "No data";
        document.getElementById("surname").textContent = "No data";
        document.getElementById("weight").textContent = "No data";
    }
})();
