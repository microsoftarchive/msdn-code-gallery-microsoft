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
    var page = WinJS.UI.Pages.define("/html/interfaces.html", {
        ready: function (element, options) {
            document.getElementById("enumerateDeviceInterfaces").addEventListener("click", onEnumerateDeviceInterfaces, false);
            document.getElementById("selectInterfaceClass").addEventListener("click", onInterfaceClassChanged, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function onInterfaceClassChanged() {
        // Take the selected class and put it into the class guid edit box
        deviceInterfaceClassText.value = selectInterfaceClass.options[selectInterfaceClass.selectedIndex].value;
    }

    function displayDeviceInterface(deviceInterface, log, index) {

        // Note that the glyph thumbnail is white with a transparent background.
        // Thats why a background color must be set for the image

        log.innerHTML +=
        "<p style=\"margin:0px 0px 0px 0px; font-size:18px; font-weight:700\">" + deviceInterface.name + "</p>" +
        "<p style=\"margin:0px 0px 0px 0px\">Id: " + deviceInterface.id + "</p>" +
        "<p style=\"margin:0px 0px 0px 0px\">IsEnabled: " + deviceInterface.isEnabled + "</p>" +
        "<p style=\"margin:0px 0px 0px 0px\">Thumbnail: <img style=\"height:64px; width:64px\" class=\"thumbnail\" id=\"thumbnail" + index + "\" /></p>" +
        "<p style=\"margin:0px 0px 20px 0px\">Glyph Thumbnail: <img class=\"glyph\" id=\"glyph" + index + "\" /></p>";

        deviceInterface.getThumbnailAsync().done(function (thumbnail) {
            id("thumbnail" + index).src = window.URL.createObjectURL(thumbnail, { oneTimeOnly:true });
        });
        
        deviceInterface.getGlyphThumbnailAsync().done(function (glyph) {
            id("glyph" + index).src = window.URL.createObjectURL(glyph, { oneTimeOnly:true });
        });
    }

    function onEnumerateDeviceInterfaces() {
        // Define the AQS selector to enumerate device instances of the specified interface class 
        var deviceInterfaceClass = deviceInterfaceClassText.value;
        var selector = "System.Devices.InterfaceClassGuid:=\"" + deviceInterfaceClass + "\"";
        //                 + " AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True";

        scenario1Output.innerHTML = "";

        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(selector, null).done(function(/*@type(Windows.Devices.Enumeration.DeviceInformationCollection)*/devinfoCollection) {
            var numDevices = devinfoCollection.length;
            WinJS.log && WinJS.log(numDevices + " device interface(s) found", "sample", "status");
            for (var i = 0; i < numDevices; i++) {
                displayDeviceInterface(devinfoCollection[i], id("scenario1Output"), i);
            }
        },
        function (e) {
            WinJS.log && WinJS.log("Failed to find devices, error: " + e.message, "sample", "error");
        });
    }
})();
