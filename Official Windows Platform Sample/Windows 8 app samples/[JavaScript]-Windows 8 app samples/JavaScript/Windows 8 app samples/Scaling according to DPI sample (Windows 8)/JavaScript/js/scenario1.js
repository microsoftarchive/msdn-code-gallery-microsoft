//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            initialize();
            window.addEventListener("resize", initialize, false);
            Windows.Graphics.Display.DisplayProperties.addEventListener("logicaldpichanged", initialize);
        },
        unload: function () {
            window.removeEventListener("resize", initialize, false);
            Windows.Graphics.Display.DisplayProperties.removeEventListener("logicaldpichanged", initialize);
        }
    });

    function initialize() {
        document.getElementById("scalePercentValue").textContent = Windows.Graphics.Display.DisplayProperties.resolutionScale + "%";
        // Get the logical DPI and round to the nearest tenth
        var logicalDPI = Math.round(Windows.Graphics.Display.DisplayProperties.logicalDpi * 10) / 10;
        document.getElementById("logicalDPIValue").textContent = logicalDPI + " DPI";
        document.getElementById("minPhysicalDPIValue").textContent = getMinDPIForScale(Windows.Graphics.Display.DisplayProperties.resolutionScale);
        document.getElementById("minPhysicalResolutionValue").textContent = getMinResolutionForScale(Windows.Graphics.Display.DisplayProperties.resolutionScale);
    }

    // Return the minimum native DPI needed for a device to be in the current scale
    function getMinDPIForScale(resolutionScale) {
        var dpiRange = "";

        switch (resolutionScale) {
            case 100:
                // Scale 100%
                dpiRange = "No minimum DPI for this scale";
                break;
            case 140:
                // Scale 140%
                dpiRange = "174 DPI";
                break;
            case 180:
                // Scale 180%
                dpiRange = "240 DPI";
                break;
            default:
                dpiRange = "Unknown";
                break;
        }
        return dpiRange;
    }

    // Return the minimum native resolution needed for a device to be in the current scale
    function getMinResolutionForScale(resolutionScale) {
        var resolutionRange = "";

        switch (resolutionScale) {
            case 100:
                // Scale 100%
                resolutionRange = "1024x768 (min resolution needed to run apps)";
                break;
            case 140:
                // Scale 140%
                resolutionRange = "1920x1080";
                break;
            case 180:
                // Scale 180%
                resolutionRange = "2560x1440";
                break;
            default:
                resolutionRange = "Unknown";
                break;
        }
        return resolutionRange;
    }
})();