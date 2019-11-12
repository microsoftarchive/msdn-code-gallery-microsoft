//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/S1-orientation.html", {
        ready: function (element, options) {

            // Subscribe to window resize events
            window.addEventListener("resize", scenario1ResizeHandler, false);
            // Run layout logic when the page first loads
            invalidateLayout();
        }
    });

    function scenario1ResizeHandler() {
        invalidateLayout();
    }

    function invalidateLayout() {
        if (document.getElementById("outputArea")) {

            // Get the window orientation
            var winOrientation = Windows.UI.ViewManagement.ApplicationView.getForCurrentView().orientation;
            if (winOrientation === Windows.UI.ViewManagement.ApplicationViewOrientation.landscape) {
                // Update grid to stack the boxes horizontally in landscape orientation
                WinJS.Utilities.addClass(document.getElementById("outputArea"), "stackHorizontally");
                WinJS.Utilities.removeClass(document.getElementById("outputArea"), "stackVertically");
                WinJS.log && WinJS.log("Windows orientation is landscape.", "sample", "status");
            } else if (winOrientation === Windows.UI.ViewManagement.ApplicationViewOrientation.portrait) {
                // Update grid to stack the boxes vertically in portrait orientation
                WinJS.Utilities.addClass(document.getElementById("outputArea"), "stackVertically");
                WinJS.Utilities.removeClass(document.getElementById("outputArea"), "stackHorizontally");
                WinJS.log && WinJS.log("Windows orientation is portrait.", "sample", "status");
            }
        }
    }
})();
