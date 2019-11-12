//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var proximityDevice = null;

    var page = WinJS.UI.Pages.define("/html/Scenario4_ProximityDeviceEvents.html", {
        ready: function (element, options) {
            proximityDevice = ProximityHelpers.initializeProximityDevice();
            if (proximityDevice) {
                // Register for device arrived and departed events to know when a proximate device is in range.
                proximityDevice.addEventListener("devicearrived", deviceArrived);
                proximityDevice.addEventListener("devicedeparted", deviceDeparted);
            }
        },
        unload: function () {
            ProximityHelpers.clearLog(ProximityHelpers.id("proximityDeviceEvents_Output"));
            if (proximityDevice) {
                proximityDevice.removeEventListener("devicearrived", deviceArrived);
                proximityDevice.removeEventListener("devicedeparted", deviceDeparted);
            }
        }
    });

    function deviceArrived(arrivingDevice) {
        ProximityHelpers.logInfo(ProximityHelpers.id("proximityDeviceEvents_Output"), "Proximate device arrived");
    }

    function deviceDeparted(departingDevice) {
        ProximityHelpers.logInfo(ProximityHelpers.id("proximityDeviceEvents_Output"), "Proximate device departed");
    }

})();
