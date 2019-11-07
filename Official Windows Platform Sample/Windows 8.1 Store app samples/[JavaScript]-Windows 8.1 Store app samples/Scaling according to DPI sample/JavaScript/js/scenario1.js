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
            WinJS.Resources.addEventListener("contextchanged", refresh, false);
            Windows.Graphics.Display.DisplayInformation.getForCurrentView().addEventListener("dpichanged", initialize);
        },
        unload: function () {
            window.removeEventListener("resize", initialize, false);
            WinJS.Resources.removeEventListener("contextchanged", refresh, false);
            Windows.Graphics.Display.DisplayInformation.getForCurrentView().removeEventListener("dpichanged", initialize);
        }
    });

    function initialize() {
        var displayInformation = Windows.Graphics.Display.DisplayInformation.getForCurrentView();
        var scale = displayInformation.resolutionScale;
        document.getElementById("scalePercentValue").textContent = scale + "%";
        document.getElementById("manualLoadURL").textContent = "http://www.contoso.com/imageScale" + scale + ".png";
        // Get the logical DPI and round to the nearest tenth
        var logicalDPI = Math.round(displayInformation.logicalDpi * 10) / 10;
        document.getElementById("logicalDPIValue").textContent = logicalDPI + " DPI";
        document.getElementById("minPhysicalDPIValue").textContent = getMinDPIForScale(scale);
        document.getElementById("minPhysicalResolutionValue").textContent = getMinResolutionForScale(scale);
    }

    // event handler for when scale changes
    function refresh(e) {

        /*
        If a page contains img elements and the images have scale or other variations,
        the HTML layout engine does not automatically reload those images if the scale 
        is changed (e.g., if a view is moved to a different display that has a 
        different DPI). WinJS.Resources.processAll() will reload strings resources bound 
        using data-win-res attributes, but does not reload images. The following will 
        reload image resources that are referenced using a URI with the ms-appx schema.

        Note: this is required for img elements, but is not required for image resources
        used in a CSS background-image style.

        For more details on using image resources, see the Application Resources code 
        sample.
        */

        /*
        The contextchanged event can occur in relation to several different resource 
        qualifiers. If only certain qualifiers or qualifier values are relevant for an 
        app, e.detail can be tested to filter for relevant changes. This sample has image 
        variants for only scale, so we will test for that particular changes.
        */

        if (e.detail.qualifier === "Scale") {
            var imageElements = document.getElementsByTagName("img");
            for (var i = 0, l = imageElements.length; i < l; i++) {
                var previousSource = imageElements[i].src;
                var uri = new Windows.Foundation.Uri(document.location, previousSource);
                if (uri.schemeName === "ms-appx") {
                    imageElements[i].src = "";
                    imageElements[i].src = previousSource;
                }
            }
        }
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
                resolutionRange = "1440x1080";
                break;
            case 180:
                // Scale 180%
                resolutionRange = "1920x1440";
                break;
            default:
                resolutionRange = "Unknown";
                break;
        }
        return resolutionRange;
    }
})();