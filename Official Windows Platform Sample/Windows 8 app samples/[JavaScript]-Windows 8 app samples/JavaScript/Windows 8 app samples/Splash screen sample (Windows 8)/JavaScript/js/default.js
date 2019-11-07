//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Splash screen JS sample";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Registering for dismissal notifications" },
        { url: "/html/scenario2.html", title: "Extending the splash screen" }
    ];

    var splash = null; // Variable to hold the splash screen object.
    var dismissed = false; // Variable to track splash screen dismissal status.
    var coordinates = { x: 0, y: 0, width: 0, height: 0 }; // Object to store splash screen image coordinates. It will be initialized during activation.

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Retrieve splash screen object
            splash = eventObject.detail.splashScreen;

            // Retrieve the window coordinates of the splash screen image.
            SdkSample.coordinates = splash.imageLocation;

            // Register an event handler to be executed when the splash screen has been dismissed.
            splash.addEventListener("dismissed", onSplashScreenDismissed, false);

            // Create and display the extended splash screen using the splash screen object.
            ExtendedSplash.show(splash);

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            window.addEventListener("resize", onResize, false);

            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    function onSplashScreenDismissed() {
        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        SdkSample.dismissed = true;

        // Tear down the app's extended splash screen after completing setup operations here...
        // In this sample, the extended splash screen is torn down when the "Learn More" button is clicked.
        document.getElementById("learnMore").addEventListener("click", ExtendedSplash.remove, false);

        // The following operation is only applicable to this sample to ensure that UI has been updated appropriately.
        // Update scenario 1's output if scenario1.html has already been loaded before this callback executes.
        if (document.getElementById("dismissalOutput")) {
            document.getElementById("dismissalOutput").innerText = "Received the splash screen dismissal event.";
        }
    }

    function onResize() {
        // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
        if (splash) {
            // Update the coordinates of the splash screen image.
            SdkSample.coordinates = splash.imageLocation;
            ExtendedSplash.updateImageLocation(splash);
        }
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        dismissed: dismissed,
        coordinates: coordinates
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
