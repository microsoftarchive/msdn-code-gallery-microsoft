//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            initialize();
            window.addEventListener("resize", initialize, false);
            Windows.Graphics.Display.DisplayInformation.getForCurrentView().addEventListener("dpichanged", initialize);
        },
        unload: function () {
            window.removeEventListener("resize", initialize, false);
            Windows.Graphics.Display.DisplayInformation.getForCurrentView().removeEventListener("dpichanged", initialize);
        }
    });

    function initialize() {
        // Get the effective width and round to the nearest tenth
        var effectiveWidth = Math.round(document.documentElement.getBoundingClientRect().width * 10) / 10;
        // Get the effective height and round to the nearest tenth
        var effectiveHeight = Math.round(document.documentElement.getBoundingClientRect().height * 10) / 10;
        document.getElementById("effectiveResolutionValue").textContent = effectiveWidth + "x" + effectiveHeight;
        document.getElementById("overrideFont").textContent = document.getElementById("overrideFont").currentStyle.fontSize + " " + document.getElementById("overrideFont").currentStyle.fontFamily.replace(/\"(.+?)\"/g, "$1");
        updateBoxLabels();
    }

    function updateBoxLabels() {
        var standardSize,
            overrideSize;
        var scale;

        switch (Windows.Graphics.Display.DisplayInformation.getForCurrentView().resolutionScale) {
            default:
            case 100:
                // Scale 100%
                scale = 1.0;
                break;
            case 140:
                // Scale 140%
                scale = 1.4;
                break;
            case 180:
                // Scale 180%
                scale = 1.8;
                break;
        }
        // Retrieve the sub pixel size of the box and compute the physical pixel size with the scale factor.
        var box = document.getElementById("standardBox");
        var size = box.getBoundingClientRect().width;
        document.getElementById("standardBoxPhysicalPixels").textContent = Math.round(size * scale) + " physical px";
        document.getElementById("standardBoxRelativePixels").textContent = Math.round(size * 10) / 10 + " relative px";

        box = document.getElementById("overrideBox");
        size = box.getBoundingClientRect().width;
        document.getElementById("overrideBoxPhysicalPixels").textContent = Math.round(size * scale) + " physical px";
        document.getElementById("overrideBoxRelativePixels").textContent = Math.round(size * 10) / 10 + " relative px";
    }
})();
