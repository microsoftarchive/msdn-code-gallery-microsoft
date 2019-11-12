//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="js/util.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/provision-other-operator.html", {
        ready: function (element, options) {
            document.getElementById("scenario4Open").addEventListener("click", provisionOtherOperator, false);
        }
    });

    function provisionOtherOperator() {
        var provXml = document.scenario4InputForm.provXml.value;

        if (provXml === "") {
            WinJS.log && WinJS.log("Provisioning XML cannot be empty", "sample", "error");
            return;
        }

        try {
            // Create provisioning agent
            var provAgent = new Windows.Networking.NetworkOperators.ProvisioningAgent();

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
