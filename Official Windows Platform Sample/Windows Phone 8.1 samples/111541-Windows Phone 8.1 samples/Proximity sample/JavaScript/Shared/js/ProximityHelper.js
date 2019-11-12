//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ProxNS = Windows.Networking.Proximity;

    var g_proximityDevice = null;
    function initializeProximityDevice() {
        if (!g_proximityDevice) {
            g_proximityDevice = Windows.Networking.Proximity.ProximityDevice.getDefault();
            // getDefault() will return null if no proximity devices are installed.
            if (!g_proximityDevice) {
                ProximityHelpers.displayError("Failed to get default proximity device, likely none installed.");
                return null;
            }
        }
        return g_proximityDevice;
    }

    WinJS.Namespace.define("ProximityHelpers", {
        displayError: function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "error");
        },

        displayStatus: function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "status");
        },

        initializeProximityDevice: initializeProximityDevice,

        logInfo: function (logElement, message) {
            logElement.innerHTML += message + "<br/>";
        },

        clearLog: function (logElement) {
            logElement.innerHTML = "";
        },

        id: function (elementId) {
            return document.getElementById(elementId);
        }
    });

})();
