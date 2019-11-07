//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                var url;
                if (eventObject.detail.arguments !== "") {
                    var scenarioIndex = SdkSample.getScenarioIndex(eventObject.detail.arguments);
                    url = SdkSample.scenarios.getItem(scenarioIndex).data["url"];
                } else {
                    url = WinJS.Application.sessionState.lastUrl || SdkSample.scenarios.getAt(0).url;
                }

                // Navigate to either the home view or to the last running scenario
                // before suspension or termination or the secnario dictated by the launch arguments.
                return WinJS.Navigation.navigate(url, eventObject.detail.arguments);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigating", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.disposeSubTree(host);
        WinJS.Utilities.empty(host);
        WinJS.log && WinJS.log("", "", "status");

        var p = WinJS.UI.Pages.render(url, host, eventObject.detail.state).
            then(function () {
                WinJS.Application.sessionState.lastUrl = url;
            });
        eventObject.detail.setPromise(p);
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

