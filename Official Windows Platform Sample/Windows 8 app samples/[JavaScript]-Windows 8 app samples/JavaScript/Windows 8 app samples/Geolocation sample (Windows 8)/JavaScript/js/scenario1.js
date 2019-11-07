//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var geolocator;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("startTrackingButton").addEventListener("click", trackloc, false);
            document.getElementById("stopTrackingButton").addEventListener("click", stoptracking, false);
            document.getElementById("startTrackingButton").disabled = false;
            document.getElementById("stopTrackingButton").disabled = true;

            geolocator = new Windows.Devices.Geolocation.Geolocator();
        },
        unload: function () {
            if (document.getElementById("startTrackingButton").disabled) {
                geolocator.removeEventListener("positionchanged", onPositionChanged);
                geolocator.removeEventListener("statuschanged", onStatusChanged);
            }
        }
    });

    function trackloc() {
        WinJS.log && WinJS.log("Waiting for update...", "sample", "status");

        geolocator.addEventListener("positionchanged", onPositionChanged);
        geolocator.addEventListener("statuschanged", onStatusChanged);

        document.getElementById("startTrackingButton").disabled = true;
        document.getElementById("stopTrackingButton").disabled = false;
    }

    function stoptracking() {
        geolocator.removeEventListener("positionchanged", onPositionChanged);
        geolocator.removeEventListener("statuschanged", onStatusChanged);

        document.getElementById("startTrackingButton").disabled = false;
        document.getElementById("stopTrackingButton").disabled = true;
    }

    function onPositionChanged(e) {
        var coord = e.position.coordinate;

        WinJS.log && WinJS.log("Updated", "sample", "status");

        document.getElementById("latitude").innerHTML = coord.latitude;
        document.getElementById("longitude").innerHTML = coord.longitude;
        document.getElementById("accuracy").innerHTML = coord.accuracy;
    }
    
    function onStatusChanged(e) {
        switch (e.status) {
            case Windows.Devices.Geolocation.PositionStatus.ready:
                // Location platform is providing valid data.
                document.getElementById("status").innerHTML = "Ready";
                break;
            case Windows.Devices.Geolocation.PositionStatus.initializing:
                // Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                document.getElementById("status").innerHTML = "Initializing";
                break;
            case Windows.Devices.Geolocation.PositionStatus.noData:
                // Location platform could not obtain location data.
                document.getElementById("status").innerHTML = "No data";
                break;
            case Windows.Devices.Geolocation.PositionStatus.disabled:
                // The permission to access location data is denied by the user or other policies.
                document.getElementById("status").innerHTML = "Disabled";

                // Clear cached location data if any
                document.getElementById("latitude").innerHTML = "No data";
                document.getElementById("longitude").innerHTML = "No data";
                document.getElementById("accuracy").innerHTML = "No data";
                break;
            case Windows.Devices.Geolocation.PositionStatus.notInitialized:
                // The location platform is not initialized. This indicates that the application has not made a request for location data.
                document.getElementById("status").innerHTML = "Not initialized";
                break;
            case Windows.Devices.Geolocation.PositionStatus.notAvailable:
                // The location platform is not available on this version of the OS.
                document.getElementById("status").innerHTML = "Not available";
                break;
            default:
                document.getElementById("status").innerHTML = "Unknown";
                break;
        }
    }
})();
