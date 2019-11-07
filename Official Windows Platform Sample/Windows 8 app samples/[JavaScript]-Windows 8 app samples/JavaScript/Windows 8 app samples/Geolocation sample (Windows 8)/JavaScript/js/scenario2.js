//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var geolocator;
    var promise;
    var pageLoaded = false;

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("getGeopositionButton").addEventListener("click", getGeoposition, false);
            document.getElementById("cancelGetGeopositionButton").addEventListener("click", cancelGetGeoposition, false);
            document.getElementById("getGeopositionButton").disabled = false;
            document.getElementById("cancelGetGeopositionButton").disabled = true;

            geolocator = Windows.Devices.Geolocation.Geolocator();
            pageLoaded = true;
        },
        unload: function () {
            pageLoaded = false;
            if (document.getElementById("getGeopositionButton").disabled) {
                promise.operation.cancel();
            }
        }
    });

    function getGeoposition() {
        WinJS.log && WinJS.log("Waiting for update...", "sample", "status");

        document.getElementById("getGeopositionButton").disabled = true;
        document.getElementById("cancelGetGeopositionButton").disabled = false;

        promise = geolocator.getGeopositionAsync();
        promise.done(
            function (pos) {
                var coord = pos.coordinate;

                WinJS.log && WinJS.log("Updated", "sample", "status");

                document.getElementById("latitude").innerHTML = coord.latitude;
                document.getElementById("longitude").innerHTML = coord.longitude;
                document.getElementById("accuracy").innerHTML = coord.accuracy;

                document.getElementById("getGeopositionButton").disabled = false;
                document.getElementById("cancelGetGeopositionButton").disabled = true;
            },
            function (err) {
                if (pageLoaded) {
                    WinJS.log && WinJS.log(err.message, "sample", "error");

                    document.getElementById("latitude").innerHTML = "No data";
                    document.getElementById("longitude").innerHTML = "No data";
                    document.getElementById("accuracy").innerHTML = "No data";

                    document.getElementById("getGeopositionButton").disabled = false;
                    document.getElementById("cancelGetGeopositionButton").disabled = true;
                }
            }
        );
    }

    function cancelGetGeoposition() {
        promise.operation.cancel();
    }
})();
