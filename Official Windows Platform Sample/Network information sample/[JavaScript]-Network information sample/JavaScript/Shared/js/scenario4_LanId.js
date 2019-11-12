//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var networkInfo = Windows.Networking.Connectivity.NetworkInformation;
    var page = WinJS.UI.Pages.define("/html/scenario4_LanId.html", {
        ready: function (element, options) {
            document.getElementById("scenario4").addEventListener("click", displayLanIdentifiers, false);
        }
    });

    //
    //Get Lan Identifier Data
    //
    function getLanIdentifierData(lanIdentifier) {
        var lanIdentifierData = "";
        var i = 0;
        try {
            if (lanIdentifier === null) {
                return "";
            }
            if (lanIdentifier.infrastructureId !== null) {
                lanIdentifierData += "Infrastructure Type: " + lanIdentifier.infrastructureId.type + "\n\r";
                lanIdentifierData += "Infrastructure Value: [";
                for (i = 0; i < lanIdentifier.infrastructureId.value.length; i++) {
                    //Display the Infrastructure value array
                    lanIdentifierData += lanIdentifier.infrastructureId.value[i].toString(16) + " ";
                }
                lanIdentifierData += "]\n\r";
            }
            if (lanIdentifier.portId !== null) {
                lanIdentifierData += "Port Type : " + lanIdentifier.portId.type + "\n\r";
                lanIdentifierData += "Port Value: [";
                for (i = 0; i < lanIdentifier.portId.value.length; i++) {
                    //Display the PortId value array
                    lanIdentifierData += lanIdentifier.portId.value[i].toString(16) + " ";
                }
                lanIdentifierData += "]\n\r";
            }
            if (lanIdentifier.networkAdapterId !== null) {
                lanIdentifierData += "Network Adapter Id : " + lanIdentifier.networkAdapterId + "\n\r";
            }
        }

        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
        return lanIdentifierData;
    }

    //
    //Display Lan Identifiers - Infrastructure ID, Port ID, Network Adapter ID
    //
    function displayLanIdentifiers() {
        var lanIdentifier = "";
        try {
            var lanIdentifiers = networkInfo.getLanIdentifiers();
            if (lanIdentifiers.length !== 0) {
                lanIdentifier += "Number of Lan Identifiers retrieved: " + lanIdentifiers.length + "\n\r";
                lanIdentifier += "=============================================\n\r";
                for (var i = 0; i < lanIdentifiers.length; i++) {
                    //Display Lan Identifier data for each identifier
                    lanIdentifier += getLanIdentifierData(lanIdentifiers[i]);
                    lanIdentifier += "----------------------------------------------------------------\n\r";
                }

                OutputText.textContent = lanIdentifier;
                WinJS.log && WinJS.log("Success", "sample", "status");
            }
            else {
                WinJS.log && WinJS.log("No Lan Identifier Data found", "sample", "status");
            }
        }

        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
