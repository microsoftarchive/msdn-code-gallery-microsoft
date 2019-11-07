//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Lock Screen Call JS";

    var scenarios = [
        { url: "/html/Toast.html", title: "Raise call toast" },
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // If activated from a user-session toast, then activate our call page.
                if (eventObject.detail.arguments !== "") {
                    return WinJS.Navigation.navigate("/html/Call.html", eventObject.detail);
                } else {
                    // Otherwise, navigate to either the first scenario or to the last running scenario
                    // before suspension or termination.
                    var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                    return WinJS.Navigation.navigate(url, eventObject.detail.arguments);
                }
            }));
        }
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.lockScreenCall) {
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // If activated from a lock screen toast, then activate our call page.
                return WinJS.Navigation.navigate("/html/Call.html", eventObject.detail);
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

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
