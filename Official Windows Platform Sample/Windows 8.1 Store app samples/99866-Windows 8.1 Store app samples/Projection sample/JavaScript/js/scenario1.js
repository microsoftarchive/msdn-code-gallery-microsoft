//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var ViewManagement = Windows.UI.ViewManagement;
    // Create ready handlers for start/stop projection button.
    var startProjectionButton;
    var stopProjectionButton;
    // Create a new view for projecting.
    var view = MSApp.createNewView("ms-appx:///html/secondaryView.html");
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            startProjectionButton = document.getElementById("startProjectionButton");
            startProjectionButton.addEventListener("click", startProjection, false);
            stopProjectionButton = document.getElementById("stopProjectionButton");
            stopProjectionButton.addEventListener("click", stopProjection, false);
            // Initially Stop Projection button is disabled.
            stopProjectionButton.disabled = true;
        }
    });

    function startProjection() {
        // Disable Start projection button to avoid clicking it twice.
        startProjectionButton.disabled = true;
        // Start projection using the previously created secondary view.
        ViewManagement.ProjectionManager.startProjectingAsync(
            view.viewId,
            ViewManagement.ApplicationView.getForCurrentView().id
        ).done(function () {
            // Enable Stop projection button after the Start projection async call completed.
            stopProjectionButton.disabled = false;
            // Re-enable the Start projection button to allow starting again when stop projection from projection window.
            startProjectionButton.disabled = false;
            // Clear out the previous error message if there is any.
            WinJS.log("", "sample", "status");
        }, function (e) {
            // Re-enable the Start projection button if start projection failed.
            startProjectionButton.disabled = false;
            WinJS.log && WinJS.log(e + "\n The current app or projection view window must be active for startProjectingAsync() to succeed", "sample", "error");
        });
    }

    function stopProjection() {
        // Disable Stop projection button to avoid falsely clicking it twice.
        stopProjectionButton.disabled = true;
        // Stop projecting the view we created if it is existing.
        ViewManagement.ProjectionManager.stopProjectingAsync(
            view.viewId,
            ViewManagement.ApplicationView.getForCurrentView().id
        ).done(function () {
            // Enable Start projection button after the Stop projection async call completed.
            startProjectionButton.disabled = false;
        }, function (e) {
            // Re-enable the Stop projection button if stop projection failed.
            stopProjectionButton.disabled = false;
            WinJS.log && WinJS.log(e + "\n The projection view window must be active for stopProjectingAsync() to succeed", "sample", "error");
        });
    }
})();
