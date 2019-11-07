//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_Mouse.html", {
        ready: function (element, options) {
            id("button1").addEventListener("click", getMouseCapabilities, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function getMouseCapabilities() {
        var mouseCapabilities = new Windows.Devices.Input.MouseCapabilities();
        id("mousePresent").innerHTML = /*@static_cast(String)*/mouseCapabilities.mousePresent;
        id("verticalWheelPresent").innerHTML = /*@static_cast(String)*/mouseCapabilities.verticalWheelPresent;
        id("horizontalWheelPresent").innerHTML = /*@static_cast(String)*/mouseCapabilities.horizontalWheelPresent;
        id("swapButtons").innerHTML = /*@static_cast(String)*/mouseCapabilities.swapButtons;
        id("numberOfButtons").innerHTML = /*@static_cast(String)*/mouseCapabilities.numberOfButtons; 
    }
})();
