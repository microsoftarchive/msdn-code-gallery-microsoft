//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="Windows.Devices.Enumeration.js" />
/// <reference path="Windows.Devices.Enumeration.Pnp.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/containers.html", {
        ready: function (element, options) {
            document.getElementById("enumerateDeviceContainers").addEventListener("click", onEnumerateDeviceContainers, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function displayDeviceContainer(deviceContainer, log) {

        var displayName = deviceContainer.properties["System.ItemNameDisplay"];

        if (displayName !== null) {
            log.innerHTML += "<p style=\"margin:0px 0px 0px 0px; font-size:18px; font-weight:700\">" + displayName + "</p>";
        } else {
            log.innerHTML += "<p style=\"margin:0px 0px 0px 0px; font-size:18px; font-weight:700\">*Unnamed*</p>";
        }
        log.innerHTML += "<p style=\"margin:0px 0px 0px 0px\">Container ID: " + deviceContainer.id + "</p>";

        log.innerHTML += "<p style=\"margin:0px 0px 0px 0px\">property store count is: " + deviceContainer.properties.size + "</p>";
        Object.keys(deviceContainer.properties).forEach(function (key) {
            log.innerHTML += "<p style=\"margin:0px 0px 0px 0px\">" + key + ": " + deviceContainer.properties[key] + "</p>";
        });

        log.innerHTML += "<br /><br />";
    }

    function onEnumerateDeviceContainers() {

        var propertiesToRetrieve = ["System.ItemNameDisplay", "System.Devices.ModelName", "System.Devices.Connected"];

        var DevEnum = Windows.Devices.Enumeration;
        var Pnp = DevEnum.Pnp;
        var pnpObjType = Pnp.PnpObjectType;
        var deviceContainerType = pnpObjType.deviceContainer;

        scenario2Output.innerHTML = "";

        Windows.Devices.Enumeration.Pnp.PnpObject.findAllAsync(deviceContainerType, propertiesToRetrieve).done(function (/*@type(Windows.Devices.Enumeration.DeviceInformationCollection)*/containerCollection) {
            var numContainers = containerCollection.length;
            WinJS.log && WinJS.log(numContainers + " device containers(s) found", "sample", "status");
            for (var i = 0; i < numContainers; i++) {
                displayDeviceContainer(containerCollection[i], id("scenario2Output"));
            }
        },
        function (e) {
            WinJS.log && WinJS.log("Failed to find devices, error: " + e.message, "sample", "error");
        });
    }

})();
