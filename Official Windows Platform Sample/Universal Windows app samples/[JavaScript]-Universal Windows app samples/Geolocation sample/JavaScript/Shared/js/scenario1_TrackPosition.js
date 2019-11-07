//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var geolocator = new Windows.Devices.Geolocation.Geolocator();
    var disposed;
    var page = WinJS.UI.Pages.define("/html/scenario1_TrackPosition.html", {
        ready: function (element, options) {
            disposed = false;
            document.getElementById("startTrackingButton").addEventListener("click", trackloc, false);
            document.getElementById("stopTrackingButton").addEventListener("click", stoptracking, false);
            document.getElementById("startTrackingButton").disabled = false;
            document.getElementById("stopTrackingButton").disabled = true;

            // For Windows Phone, You must set the MovementThreshold for
            // distance-based tracking or ReportInterval for periodic-
            // based tracking before adding event handlers.
            //
            // Value of 1000 milliseconds (1 second)
            // isn't a requirement, it is just an example.
            geolocator.reportInterval = 1000;
        },
        dispose: function () {
            if (!disposed) {
                disposed = true;
                if (document.getElementById("startTrackingButton").disabled) {
                    geolocator.removeEventListener("positionchanged", onPositionChanged);
                    geolocator.removeEventListener("statuschanged", onStatusChanged);
                }
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

        document.getElementById("latitude").innerText = coord.point.position.latitude;
        document.getElementById("longitude").innerText = coord.point.position.longitude;
        document.getElementById("accuracy").innerText = coord.accuracy;
    }

    function onStatusChanged(e) {
        switch (e.status) {
            case Windows.Devices.Geolocation.PositionStatus.ready:
                // Location platform is providing valid data.
                document.getElementById("status").innerText = "Ready";
                break;
            case Windows.Devices.Geolocation.PositionStatus.initializing:
                // Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                document.getElementById("status").innerText = "Initializing";
                break;
            case Windows.Devices.Geolocation.PositionStatus.noData:
                // Location platform could not obtain location data.
                document.getElementById("status").innerText = "No data";
                break;
            case Windows.Devices.Geolocation.PositionStatus.disabled:
                // The permission to access location data is denied by the user or other policies.
                document.getElementById("status").innerText = "Disabled";

                // Clear cached location data if any
                document.getElementById("latitude").innerText = "No data";
                document.getElementById("longitude").innerText = "No data";
                document.getElementById("accuracy").innerText = "No data";
                break;
            case Windows.Devices.Geolocation.PositionStatus.notInitialized:
                // The location platform is not initialized. This indicates that the application has not made a request for location data.
                document.getElementById("status").innerText = "Not initialized";
                break;
            case Windows.Devices.Geolocation.PositionStatus.notAvailable:
                // The location platform is not available on this version of the OS.
                document.getElementById("status").innerText = "Not available";
                break;
            default:
                document.getElementById("status").innerText = "Unknown";
                break;
        }
    }
})();