//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved

// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.                
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();

    Windows.Graphics.Display.DisplayProperties.autoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.landscape;

    document.addEventListener("DOMContentLoaded", function (e) {
        WinJS.UI.processAll();

        // add all pages to the zoomed in view of the semantic view control
        var zoomedInView = document.getElementById("view-zoomedin").winControl;
        zoomedInView.addPage("/html/welcome.html", "welcome");
        zoomedInView.addPage("/html/appEdgy.html", "appedgy");
        zoomedInView.addPage("/html/systemEdgy.html", "systemedgy");
        zoomedInView.addPage("/html/tap.html", "tap");
        zoomedInView.addPage("/html/pressAndHold.html", "pressandhold");
        zoomedInView.addPage("/html/swipe.html", "swipe");
        zoomedInView.addPage("/html/objectZoom.html", "objectzoom");
        zoomedInView.addPage("/html/rotate.html", "rotate");
        zoomedInView.addPage("/html/zoom.html", "semanticzoom");

        // add preview pages to the zoomed out view of the semantic view control
        var zoomedOutView = document.getElementById("view-zoomedout").winControl;
        zoomedOutView.addPage("/html/welcomePreview.html", "preview-welcome");
        zoomedOutView.addPage("/html/appEdgyPreview.html", "preview-appedgy");
        zoomedOutView.addPage("/html/systemEdgyPreview.html", "preview-systemedgy");
        zoomedOutView.addPage("/html/tapPreview.html", "preview-tap");
        zoomedOutView.addPage("/html/pressAndHoldPreview.html", "preview-pressandhold");
        zoomedOutView.addPage("/html/swipePreview.html", "preview-swipe");
        zoomedOutView.addPage("/html/objectZoomPreview.html", "preview-objectzoom");
        zoomedOutView.addPage("/html/rotatePreview.html", "preview-rotate");
        zoomedOutView.addPage("/html/zoomPreview.html", "preview-semanticzoom");

        window.addEventListener("resize", snapCheck);

        zoomedInView.registerViewChangeHandler(AppBarHandlers.ActivateAppBar);
        zoomedOutView.registerViewChangeHandler(AppBarHandlers.ActivateAppBar);
    });

    function snapCheck() {
        /// <summary> 
        /// Handles the snapped event.
        /// When snapped, the app shows the zoomed out semantic view.
        /// </summary>

        var myViewState = Windows.UI.ViewManagement.ApplicationView.value;
        var viewStates = Windows.UI.ViewManagement.ApplicationViewState;
        var semanticZoom = document.getElementById("semanticZoom").winControl;

        if (myViewState === viewStates.snapped) {
            var zoomedOutView = document.getElementById("view-zoomedout").winControl;

            if (!semanticZoom.zoomedOut) {
                zoomedOutView.snapZoomEvent = true;
                semanticZoom.zoomedOut = true;
            }
        }
        else {
            semanticZoom.locked = false;
        }
    }
})();
