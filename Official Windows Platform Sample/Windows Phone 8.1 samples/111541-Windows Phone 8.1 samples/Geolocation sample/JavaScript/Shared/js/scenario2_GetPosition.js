//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var getGeopositionPromise;
    var desiredAccuracyInMetersElement;
    var setDesiredAccuracyButton;
    var geolocator = new Windows.Devices.Geolocation.Geolocator();
    var disposed;
    var page = WinJS.UI.Pages.define("/html/scenario2_GetPosition.html", {
        ready: function (element, options) {
            disposed = false;
            document.getElementById("getGeopositionButton").addEventListener("click", getGeoposition, false);
            document.getElementById("cancelGetGeopositionButton").addEventListener("click", cancelGetGeoposition, false);
            document.getElementById("getGeopositionButton").disabled = false;
            document.getElementById("cancelGetGeopositionButton").disabled = true;

            desiredAccuracyInMetersElement = document.getElementById("desiredAccuracyInMeters");
            setDesiredAccuracyButton = document.getElementById("setDesiredAccuracyButton");

            desiredAccuracyInMetersElement.addEventListener("textchanged", getDesiredAccuracy, false);
            setDesiredAccuracyButton.addEventListener("click", setDesiredAccuracy, false);
            desiredAccuracyInMetersElement.disabled = true;
            setDesiredAccuracyButton.disabled = true;
        },
        dispose: function () {
            if (!disposed) {
                disposed = true;
                if (document.getElementById("getGeopositionButton").disabled) {
                    getGeopositionPromise.operation.cancel();
                }
            }
        }
    });

    function textChangedHandlerInt(nullAllowed, elementName, e) {
        var goodValue = false;
        var stringValue = e.value;
        var value = parseInt(stringValue);

        if (!value) {
            var msg;
            // value is either empty or alphabetic
            if (0 === stringValue.length) {
                msg = elementName + " needs a value";

                WinJS.log && WinJS.log(msg, "sample", "status");
            } else {
                msg = elementName + " must be a number";

                WinJS.log && WinJS.log(msg, "sample", "status");
            }
        } else {
            goodValue = true;
        }

        if (goodValue) {
            // clear out status message
            WinJS.log && WinJS.log("", "sample", "status");
        }

        return goodValue;
    }

    function setDesiredAccuracy() {
        if (textChangedHandlerInt(false, null, desiredAccuracyInMetersElement)) {
            geolocator.desiredAccuracyInMeters = desiredAccuracyInMetersElement.value;
            document.getElementById("desired").innerHTML = geolocator.desiredAccuracyInMeters;
        }
    }

    function getDesiredAccuracy() {
        textChangedHandlerInt(true, "DesiredAccuracy", desiredAccuracyInMetersElement);
    }

    function getGeoposition() {
        WinJS.log && WinJS.log("Waiting for update...", "sample", "status");

        document.getElementById("getGeopositionButton").disabled = true;
        document.getElementById("cancelGetGeopositionButton").disabled = false;

        getGeopositionPromise = geolocator.getGeopositionAsync();
        getGeopositionPromise.done(
            function (pos) {
                if (!disposed) {
                    var coord = pos.coordinate;

                    WinJS.log && WinJS.log("Updated", "sample", "status");

                    document.getElementById("latitude").innerHTML = coord.point.position.latitude;
                    document.getElementById("longitude").innerHTML = coord.point.position.longitude;
                    document.getElementById("accuracy").innerHTML = coord.accuracy;
                    document.getElementById("source").innerHTML = coord.PositionSource;

                    document.getElementById("getGeopositionButton").disabled = false;
                    document.getElementById("cancelGetGeopositionButton").disabled = true;

                    if (coord.positionSource === Windows.Devices.Geolocation.PositionSource.satellite) {
                        // show labels and satellite data
                        document.getElementById("positionLabel").style.opacity = 1;
                        document.getElementById("position").style.opacity = 1;
                        document.getElementById("horizontalLabel").style.opacity = 1;
                        document.getElementById("horizontal").style.opacity = 1;
                        document.getElementById("verticalLabel").style.opacity = 1;
                        document.getElementById("vertical").style.opacity = 1;
                        document.getElementById("position").innerHTML = coord.satelliteData.positionDilutionOfPrecision;
                        document.getElementById("horizontal").innerHTML = coord.satelliteData.horizontalDilutionOfPrecision;
                        document.getElementById("vertical").innerHTML = coord.satelliteData.verticalDilutionOfPrecision;
                    } else {
                        // hide labels and satellite data
                        document.getElementById("positionLabel").style.opacity = 0;
                        document.getElementById("position").style.opacity = 0;
                        document.getElementById("horizontalLabel").style.opacity = 0;
                        document.getElementById("horizontal").style.opacity = 0;
                        document.getElementById("verticalLabel").style.opacity = 0;
                        document.getElementById("vertical").style.opacity = 0;
                    }

                    // On Windows Phone this is a nullable property
                    if (typeof (geolocator.desiredAccuracyInMeters) !== 'unknown') {
                        document.getElementById("desired").innerHTML = geolocator.desiredAccuracyInMeters;
                    }
                    desiredAccuracyInMetersElement.disabled = false;
                    setDesiredAccuracyButton.disabled = false;
                }
            },
            function (err) {
                if (!disposed) {
                    WinJS.log && WinJS.log(err.message, "sample", "error");

                    document.getElementById("latitude").innerHTML = "No data";
                    document.getElementById("longitude").innerHTML = "No data";
                    document.getElementById("accuracy").innerHTML = "No data";
                    document.getElementById("source").innerHTML = "No data";
                    document.getElementById("desired").innerHTML = "No data";

                    // hide labels and satellite data
                    document.getElementById("positionLabel").style.opacity = 0;
                    document.getElementById("position").style.opacity = 0;
                    document.getElementById("horizontalLabel").style.opacity = 0;
                    document.getElementById("horizontal").style.opacity = 0;
                    document.getElementById("verticalLabel").style.opacity = 0;
                    document.getElementById("vertical").style.opacity = 0;

                    desiredAccuracyInMetersElement.disabled = true;
                    setDesiredAccuracyButton.disabled = true;

                    document.getElementById("getGeopositionButton").disabled = false;
                    document.getElementById("cancelGetGeopositionButton").disabled = true;
                }
            }
        );
    }

    function cancelGetGeoposition() {
        getGeopositionPromise.operation.cancel();
    }
})();