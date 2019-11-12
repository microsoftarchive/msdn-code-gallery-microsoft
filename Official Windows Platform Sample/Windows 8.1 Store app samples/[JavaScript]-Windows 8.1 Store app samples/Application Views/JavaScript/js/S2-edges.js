//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S2-edges.html", {
        ready: function (element, options) {
            // Subscribe to window resize events
            window.addEventListener("resize", scenario2ResizeHandler, false);
            // Run layout logic when the page first loads
            invalidateLayout();
        }
    });

    function scenario2ResizeHandler() {
        invalidateLayout();
    }

    function invalidateLayout() {
        if (document.getElementById("outputControl")) {
            // Get an instance of ApplicationView for the current window
            var currentAppView = Windows.UI.ViewManagement.ApplicationView.getForCurrentView();
            if (currentAppView.isFullScreen) {
                // If app is full screen, center the control
                document.getElementById("outputControl").className = "bothEdges";
                WinJS.log && WinJS.log("App window is full screen.", "sample", "status");
            } else if (currentAppView.adjacentToLeftDisplayEdge) {
                // If app is adjacent to the left edge, align control to the left
                document.getElementById("outputControl").className = "leftEdge";
                WinJS.log && WinJS.log("App window is adjacent to the left display edge.", "sample", "status");
            } else if (currentAppView.adjacentToRightDisplayEdge) {
                // If app is adjacent to the right edge, align control to the right
                document.getElementById("outputControl").className = "rightEdge";
                WinJS.log && WinJS.log("App window is adjacent to right display edge.", "sample", "status");
            } else {
                // If app is not adjacent to either side of the screen, center the control
                document.getElementById("outputControl").className = "neitherEdge";
                WinJS.log && WinJS.log("App window is not adjacent to any edges.", "sample", "status");
            }
        }
    }
})();
