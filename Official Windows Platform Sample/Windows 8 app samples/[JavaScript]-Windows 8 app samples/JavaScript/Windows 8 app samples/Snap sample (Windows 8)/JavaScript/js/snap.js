//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/snap.html", {
        ready: function (element, options) {
            // Register for the window resize handler
            window.addEventListener("resize", onResize);

            // Bind a click event to the unsnap button
            document.getElementById("programmaticUnsnap").addEventListener("click", onUnsnapButtonClicked, false);

            // Update view on load (because you can be launched into any view state)
            updateView();
        }
    });

    function onResize() {
        // Update view for the new window size
        updateView();
    }

    function onUnsnapButtonClicked() {
        // Disable the button so that the user doesn't try to repeatedly press the button
        document.getElementById("programmaticUnsnap").disabled = true;

        // Attempt unsnap
        if (!Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {

            // Unsnap failed. Show error.
            WinJS.log && WinJS.log("Programmatic unsnap failed", "sample", "status");
        }

        document.getElementById("programmaticUnsnap").disabled = false;
    }

    function updateView() {
        // Query for the current view state
        var myViewState = Windows.UI.ViewManagement.ApplicationView.value;
      
        var viewStates = Windows.UI.ViewManagement.ApplicationViewState;
        var statusText;

        // Assign text according to view state
        switch (myViewState) {
            case viewStates.snapped:
                statusText = "This app is snapped!";
                break;
            case viewStates.filled:
                statusText = "This app is in filled state!";
                break;
            case viewStates.fullScreenLandscape:
                statusText = "This app is full screen landscape!";
                break;
            case viewStates.fullScreenPortrait:
                statusText = "This app is full screen portrait!";
                break;
            default:
                statusText = "Error: Invalid view state returned.";
                break;
        }

        // Display text
        WinJS.log && WinJS.log(statusText, "sample", "status");
    }


})();
