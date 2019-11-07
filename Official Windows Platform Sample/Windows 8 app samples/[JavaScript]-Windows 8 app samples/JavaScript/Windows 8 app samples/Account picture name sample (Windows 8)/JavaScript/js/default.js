//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "AccountPictureName";

    var scenarios = [
        { url: "/html/UserDisplayName.html", title: "Get user display name" },
        { url: "/html/UserFirstAndLastName.html", title: "Get user first and last name" },
        { url: "/html/GetAccountPicture.html", title: "Get account picture" },
        { url: "/html/SetAccountPicture.html", title: "Set account picture & listen for changes" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
        else {
            if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
                // Check if the protocol matches the "ms-accountpictureprovider" scheme
                if (eventObject.detail.uri.schemeName === "ms-accountpictureprovider") {
                    // This app was activated via the Account picture apps section in PC Settings.
                    // Here you would do app-specific logic for providing the user with account picture selection UX
                    eventObject.setPromise(WinJS.UI.processAll().then(function () {
                        return WinJS.Navigation.navigate(scenarios[3].url);
                    }));
                }
            }
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

    function ensureUnsnapped() {
        // FilePicker APIs will not work if the application is in a snapped state. If an app wants to show a FilePicker while snapped,
        // it must attempt to unsnap first.
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        var unsnapped = ((currentState !== Windows.UI.ViewManagement.ApplicationViewState.snapped) || Windows.UI.ViewManagement.ApplicationView.tryUnsnap());
        if (!unsnapped) {
            WinJS.log && WinJS.log("Cannot unsnap the sample application.", "sample", "status");
        }
        return unsnapped;
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        ensureUnsnapped: ensureUnsnapped
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
