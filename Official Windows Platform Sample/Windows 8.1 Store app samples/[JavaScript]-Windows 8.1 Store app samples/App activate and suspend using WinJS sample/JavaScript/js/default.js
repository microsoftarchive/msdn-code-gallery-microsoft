//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Activation and Lifetiming";

    var scenarios = [
        { url: "/html/activation.html", title: "Activation of a Windows Store app built for Windows using JavaScript" }
    ];

    function activated(eventObject) {
        // The activated event is raised when the app is activated by the system. It tells the app whether it was
        // activated because the user launched it or it was launched by some other means. Use the
        // activated event handler to check the type of activation and respond appropriately to it,
        // and to load any state needed for the activation.

        var url = null;

        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Navigate to either the first scenario or to the last running scenario
            // before suspension or termination.
            url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
        }

        if (url) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    function checkpoint() {
        // An app can be suspended when the user moves it to the background or when the system enters a low power state.
        // When the app is being suspended, it raises the checkpoint event and has a limited amount of time to save its data. If
        // the app's checkpoint event handler doesn't complete within that limited time the system assumes the app has stopped
        // responding and terminates it.

        var host = document.getElementById("contentHost");
        host.winControl && host.winControl.checkpoint && host.winControl.checkpoint();
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

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.addEventListener("checkpoint", checkpoint, false);
    WinJS.Application.start();
})();
