//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function id(string) {
        return document.getElementById(string);
    }

    var page = WinJS.UI.Pages.define("/html/edgeGestureEvents.html", {
        ready: function (element, options) {
            // Register for Edge Gesture events
            var edgeGesture = Windows.UI.Input.EdgeGesture.getForCurrentView();
            edgeGesture.addEventListener("starting", onStarting);
            edgeGesture.addEventListener("completed", onCompleted);
            edgeGesture.addEventListener("canceled", onCanceled);

            // Display status on page
            id("ScenarioOutput").innerText = "Sample initialized and events registered.";
        }
    });

    // Show Application Edge UI in an entry state. Currently only touch triggers the
    // starting event.
    function onStarting(e) {
        id("ScenarioOutput").innerText = "Invoking with touch.";
    }

    // Show Application Edge UI in an invoked state
    function onCompleted(e) {
        // Determine whether it was touch or keyboard invocation
        if (e.kind === Windows.UI.Input.EdgeGestureKind.touch) {
            id("ScenarioOutput").innerText = "Invoked with touch.";
        }
        else if (e.kind === Windows.UI.Input.EdgeGestureKind.mouse) {
            id("ScenarioOutput").innerText = "Invoked with right-click.";
        }
        else if (e.kind === Windows.UI.Input.EdgeGestureKind.keyboard) {
            id("ScenarioOutput").innerText = "Invoked with keyboard.";
        }
    }

    // Dismiss the attempt to invoke Application Edge UI. If the invoking
    // handler changed the UI state (e.g. showing an overstretched command bar),
    // dismiss it here. Currently only touch triggers the canceled event.
    function onCanceled(e) {
        id("ScenarioOutput").innerText = "Canceled with touch.";
    }
})();
