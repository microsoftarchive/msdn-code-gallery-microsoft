//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="js/util.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/provision-mno.html", {
        ready: function (element, options) {
            document.getElementById("scenario1Open").addEventListener("click", provisionMno, false);
        }
    });

    function provisionMno() {
        var provXml = document.scenario1InputForm.provXml.value;

        if(provXml === "") {
            WinJS.log && WinJS.log("Provisioning XML cannot be empty", "sample", "error");
            return;
        }

        try {
            // Get the network account ID.
            var networkAccIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;

            if (networkAccIds.size === 0) {
                WinJS.log && WinJS.log("No network account ID found", "sample", "error");
                return;
            }

            // For the sake of simplicity, assume we want to use the first account.
            // Refer to the MobileBroadbandAccount API's how to select a specific account ID.
            var accountId = networkAccIds[0];

            // Create provisioning agent for specified network account ID
            var provAgent = Windows.Networking.NetworkOperators.ProvisioningAgent.createFromNetworkAccountId(accountId);

            // Provision using XML
            provAgent.provisionFromXmlDocumentAsync(provXml).done(function (results) {
                if (results.allElementsProvisioned) {
                    // Provisioning is done successfully
                    WinJS.log && WinJS.log("Device was successfully configured", "sample", "status");
                }
                else {
                    // Error has occured during provisioning
                    // And hence displaying result XML containing
                    // errors
                    provisioningUtil.parseResultXML(results.provisionResultsXml);
                }
            }, function (error) {
                WinJS.log && WinJS.log(error.description + ", Error number: " + " 0x" + (0xFFFFFFFF + error.number + 1).toString(16),
                    "sample", "error");
            });
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
