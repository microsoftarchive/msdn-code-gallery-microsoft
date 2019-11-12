//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var deviceAccountID = []; // array used to track GUIDs for each of the Mobile Broadband device

    var page = WinJS.UI.Pages.define("/html/showconnection.html", {
        ready: function (element, options) {
            prepareScenario();
        }
    });

    // Simplifies DOM text input.
    function id(elementId) {
        return document.getElementById(elementId);
    }

    // Looks for the presence of Mobile Broadband devices; if installed,
    // program runs and gets device IDs. However, if there are errors,
    // the buttons are disabled and program doesn't do anything.
    function prepareScenario() {
        try {
            // See if we have any devices with installed applications.

            var mobileBroadbandDevices = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;

            if (mobileBroadbandDevices.size !== 0) { // Device is or was installed AND access enabled

                // We have at least one device. Get the device ID for
                // each.
                for (var i = 0; i < mobileBroadbandDevices.size; i++) {
                    deviceAccountID[i] = mobileBroadbandDevices[i];
                }

                // change Button text to indicate that the button is available
                id("showConnectionButton").innerText = "Show Connection UI";
                id("showConnectionButton").addEventListener("click", showConnectionButton_Click, false);
            }
            else {
                // change Button text to indicate that the button is unavailable
                id("showConnectionButton").innerText = "No available accounts detected";
            }

        } catch (err) { // try catch for testing purposes.  Not required for SDK sample
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

    function showConnectionButton_Click() {
        try {
            // Get the device from provided account ID.
            var mobileBroadbandAccount = Windows.Networking.NetworkOperators.MobileBroadbandAccount.createFromNetworkAccountId(deviceAccountID[0]);

            // Show the connection UI.
            mobileBroadbandAccount.currentNetwork.showConnectionUI();

        } catch (err) {
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

})();
