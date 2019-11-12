//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function id(elementId) {
        return document.getElementById(elementId);
    }

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            id("buttonReset").addEventListener("click", resetHandler, false);
            id("buttonOpen").addEventListener("click", openHandler, false);
        }
    });

    function resetState() {
        // All of this scenario's state is stored in scenario4-web.js.
        // Send a reset message to the Bing Maps iframe.
        var resetData = {
            type: "reset",
            bingMapsKey: id("bingMapsKeyEdit").value
        };

        id("mapsFrame").contentWindow.postMessage(
            JSON.stringify(resetData),
            "*"
            );
    }

    function openHandler() {
        Helpers.getFilesFromOpenPickerAsync().done(function (files) {
            for (var i = 0; i < files.length; i++) {
                sendPinDataAsync(files.getAt(i)).done();
            }
        }, function (error) {
            WinJS.log && WinJS.log("Error: " + error.message, "sample", "error");
        });
    }

    function resetHandler() {
        resetState();
    }

    function sendPinDataAsync(file) {
        var pinData = {
            title: "",
            latitude: 0,
            longitude: 0,
            latitudeString: "",
            longitudeString: "",
            type: "pin"
        };

        return file.properties.getImagePropertiesAsync().then(function (imageProperties) {
            pinData.title = imageProperties.title;
            pinData.latitude = imageProperties.latitude;
            pinData.longitude = imageProperties.longitude;

            if (!imageProperties.latitude || !imageProperties.longitude) {
                WinJS.log && WinJS.log("File: " + file.name + " does not have valid GPS data", "sample", "error");
                return;
            }

            pinData.latitudeString = Helpers.convertLatLongToString(imageProperties.latitude, true);
            pinData.longitudeString = Helpers.convertLatLongToString(imageProperties.longitude, false);

            // Send the pin data to the Bing Maps iframe.
            id("mapsFrame").contentWindow.postMessage(
                JSON.stringify(pinData),
                "*"
                );

            WinJS.log && WinJS.log("Sent image data to Bing Maps control", "sample", "status");
        }).then(null, function (error) {
            WinJS.log && WinJS.log("Error: " + error.message, "sample", "error");
        });
    }
})();
