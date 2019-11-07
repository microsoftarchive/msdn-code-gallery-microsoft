//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    // In order to run this scenario, the application must have a valid Bing Maps
    // developer account and application key.
    // See this web site for more information and to register for an account:
    // http://www.microsoft.com/maps/developers

    "use strict";

    function id(elementId) {
        return document.getElementById(elementId);
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
    window.addEventListener("message", receiveMessage, false);

    var infoBoxes = null;   // Array containing all InfoBoxes, used to track events
    var map = null;         // Bing Maps control
    var mapViewInfo = null; // Map viewport information

    // If the Maps control was not successfully loaded, disable further events
    var mapDisabled = null;

    // initialize() is called at DOMContentLoaded and when "reset" is posted from parent
    function initialize(bingMapsKey) {
        if (map) {
            map.dispose();
        }

        mapDisabled = false;

        // If the Bing Maps AJAX control was not successfully loaded, any attempt to get the
        // Microsoft namespace will raise an exception.
        try {
            var test = Microsoft;
        } catch (e) {
            id("errorText").innerText = "The Bing Maps AJAX control could not be loaded. " +
                "Make sure the application has access to the Internet.";
            mapDisabled = true;
            return;
        }

        infoBoxes = [];
        map = null;

        // Stores the maxima/minima photo GPS coordinates, used to define the viewport.
        // Uses sentry values that will always be overwritten by valid GPS coordinates.
        mapViewInfo = {
            "minLatitude": 91,
            "maxLatitude": -91,
            "minLongitude": 181,
            "maxLongitude": -181
        };

        var mapOptions = {
            credentials: bingMapsKey,
            mapTypeId: Microsoft.Maps.MapTypeId.aerial,
            fixedMapPosition: true,
            width: document.documentElement.clientWidth - 20,
            height: document.documentElement.clientHeight - 20
        };

        map = new Microsoft.Maps.Map(id("mapDiv"), mapOptions);
    }

    function updateMapView() {
        var minLat = mapViewInfo.minLatitude;
        var minLong = mapViewInfo.minLongitude;
        var maxLat = mapViewInfo.maxLatitude;
        var maxLong = mapViewInfo.maxLongitude;

        // Add padding around the min/max coordinate values to make a better view.
        // The minimum of 0.01 degrees guards against the 0 width or 0 height condition.
        var paddingLatitude = Math.max(0.01, (maxLat - minLat) / 10);
        var paddingLongitude = Math.max(0.01, (maxLong - minLong) / 10);

        // Add more padding to the top to make room for the pop-up infoboxes.
        var viewBoundaries = new Microsoft.Maps.LocationRect.fromLocations(
            new Microsoft.Maps.Location(minLat - paddingLatitude, minLong - paddingLongitude),
            new Microsoft.Maps.Location(minLat - paddingLatitude, maxLong + paddingLongitude),
            new Microsoft.Maps.Location(maxLat + paddingLatitude * 7, minLong - paddingLongitude),
            new Microsoft.Maps.Location(maxLat + paddingLatitude * 7, maxLong + paddingLongitude)
            );

        map.setView({ "bounds": viewBoundaries });
    }

    // This document handles two types of messages:
    // 1. "reset": the scenario is reset
    // 2. "pin": JSON containing pin data
    function receiveMessage(e) {
        if (e.origin !== "ms-appx://" + document.location.hostname) {
            return;
        }

        var messageData = JSON.parse(e.data);
        switch (messageData.type) {
            case "reset":
                initialize(messageData.bingMapsKey);
                return;
            case "pin":
                if (mapDisabled) {
                    return;
                }

                var center = new Microsoft.Maps.Location(messageData.latitude, messageData.longitude);
                var pin = new Microsoft.Maps.Pushpin(center, null);

                // "id" is a custom property to associate the Pushpin with an Infobox
                pin.id = infoBoxes.length;
                map.entities.push(pin);

                // Generate the Infobox ("pop-up window" for the pin)
                var infoboxOptions = {
                    title: messageData.title,
                    description: "Latitude: " + messageData.latitudeString +
                             "\nLongitude: " + messageData.longitudeString,
                    visible: false,
                    showPointer: false,
                    showCloseButton: false,
                    offset: new Microsoft.Maps.Point(-50, 50),
                    height: 86 // Fits title and 2 lines of description
                };

                var infoBox = new Microsoft.Maps.Infobox(center, infoboxOptions);
                infoBoxes[infoBoxes.length] = infoBox;
                map.entities.push(infoBox);

                // Update the viewport info
                mapViewInfo.minLatitude = Math.min(mapViewInfo.minLatitude, messageData.latitude);
                mapViewInfo.maxLatitude = Math.max(mapViewInfo.maxLatitude, messageData.latitude);
                mapViewInfo.minLongitude = Math.min(mapViewInfo.minLongitude, messageData.longitude);
                mapViewInfo.maxLongitude = Math.max(mapViewInfo.maxLongitude, messageData.longitude);
                updateMapView();

                // Mouse event handlers
                Microsoft.Maps.Events.addHandler(pin, "mouseover", displayInfoboxWithId);
                Microsoft.Maps.Events.addHandler(pin, "mouseout", hideInfoboxWithId);
                return;
            default:
                id("errorText").innerText = "Invalid message: " + messageData;
                return;
        }
    }

    // Show and hide the corresponding infobox when the cursor hovers over a pin
    function displayInfoboxWithId(e) {
        infoBoxes[e.target.id].setOptions({ visible: true });
    }

    function hideInfoboxWithId(e) {
        infoBoxes[e.target.id].setOptions({ visible: false });
    }
})();
