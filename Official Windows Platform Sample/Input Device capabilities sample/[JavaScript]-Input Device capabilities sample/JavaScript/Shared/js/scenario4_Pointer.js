//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario4_pointer.html", {
        ready: function (element, options) {
            id("button1").addEventListener("click", getPointerCapabilities, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function getPointerDeviceType(pdt) {
        switch (pdt) {
            case Windows.Devices.Input.PointerDeviceType.touch:
                return "Touch";

            case Windows.Devices.Input.PointerDeviceType.pen:
                return "Pen";

            case Windows.Devices.Input.PointerDeviceType.mouse:
                return "Mouse";
        }
        return "Undefined";
    }  

    function getPointerCapabilities() {
        var pointerDevices = Windows.Devices.Input.PointerDevice.getPointerDevices();
        var htmlWrite = "";
        var i;
        for (i = 0; i < pointerDevices.size; i++) {
            var displayIndex = /*@static_cast(String)*/(i + 1);
            htmlWrite += "<tr><td>(" + displayIndex + ") Pointer Device Type</td>  <td>" + getPointerDeviceType(pointerDevices[i].pointerDeviceType) + "</td></tr>";
            htmlWrite += "<tr><td>(" + displayIndex + ") Is Integrated</td><td>" + /*@static_cast(String)*/pointerDevices[i].isIntegrated + "</td></tr>";
            htmlWrite += "<tr><td>(" + displayIndex + ") Max Contacts</td><td>" + pointerDevices[i].maxContacts + "</td></tr>";
            htmlWrite += "<tr><td>(" + displayIndex + ") Physical Device Rect</td><td>" +
                 pointerDevices[i].physicalDeviceRect.x + "," +
                 pointerDevices[i].physicalDeviceRect.y + "," +
                 pointerDevices[i].physicalDeviceRect.width + "," +
                 pointerDevices[i].physicalDeviceRect.height + "</td></tr>";
            htmlWrite += "<tr><td>(" + displayIndex + ") Screen Rect</td><td>" +
                 pointerDevices[i].screenRect.x + "," +
                 pointerDevices[i].screenRect.y + "," +
                 pointerDevices[i].screenRect.width + "," +
                 pointerDevices[i].screenRect.height + "</td></tr>";
        }
        id("pointerDevices").innerHTML = htmlWrite;
    }
})();
