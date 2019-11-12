//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function activated(eventObject) {
        // If activated for file type or protocol, launch the approproate scenario.
        // Otherwise navigate to either the first scenario or to the last running scenario
        // before suspension or termination.
        var url = null;
        var arg = null;
        var selectedScenarioIndex = 0;

        var activationKind = eventObject.detail.kind;
        switch (activationKind) {
            case Windows.ApplicationModel.Activation.ActivationKind.launch:
                if ((eventObject.detail.previousExecutionState === Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated)
                    || !(WinJS.Application.sessionState.lastUrl)) {
                    url = SdkSample.scenarios.getAt(0).url;
                }
                else {
                    url = WinJS.Application.sessionState.lastUrl;
                }
                break;

            case Windows.ApplicationModel.Activation.ActivationKind.file:
                selectedScenarioIndex = 2;
                url = SdkSample.scenarios.getAt(2).url;
                arg = eventObject.detail.files;
                break;

            case Windows.ApplicationModel.Activation.ActivationKind.protocol:
                selectedScenarioIndex = 3;
                url = SdkSample.scenarios.getAt(3).url;
                arg = eventObject.detail.uri;
                break;

            default:
                return;
        }

        var p = WinJS.UI.processAll().then(function () {
            WinJS.Navigation.history.current.initialPlaceholder = true;

            // Highlight the scenario name in the scenario selection pane.
            var scenarioControl = document.getElementById("scenarioControl").winControl;
            scenarioControl.selection.set(selectedScenarioIndex);

            return WinJS.Navigation.navigate(url, arg);
        });
        // Calling done on a promise chain allows unhandled exceptions to propagate.
        p.done();

        // Use setPromise to indicate to the system that the splash screen must not be torn down
        // until after processAll and navigate complete asynchronously.
        eventObject.setPromise(p);
    }

    function navigating(eventObject) {
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
        p.done();
        eventObject.detail.setPromise(p);
    }

    WinJS.Navigation.addEventListener("navigating", navigating);
    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
