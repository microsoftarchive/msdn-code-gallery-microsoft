//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var app = WinJS.Application;
    var nav = WinJS.Navigation;
    var activationKinds = Windows.ApplicationModel.Activation.ActivationKind;

    function activated(eventObject) {
        var activationKind = eventObject.detail.kind;
        var activatedEventArgs = eventObject.detail.detail;

        switch (activationKind) {
            case activationKinds.launch:
                var p = WinJS.UI.processAll().
                    then(function () {

                        // Navigate to either the first scenario or to the last running scenario
                        // before suspension or termination.
                        var url = SdkSample.scenarios.getAt(0).url;
                        var initialState = {};
                        var navHistory = app.sessionState.navigationHistory;
                        if (navHistory) {
                            nav.history = navHistory;
                            url = navHistory.current.location;
                            initialState = navHistory.current.state || initialState;
                        }
                        initialState.activationKind = activationKind;
                        initialState.activatedEventArgs = activatedEventArgs;
                        nav.history.current.initialPlaceholder = true;
                        return nav.navigate(url, initialState);
                    });

                // Calling done on a promise chain allows unhandled exceptions to propagate.
                p.done();

                // Use setPromise to indicate to the system that the splash screen must not be torn down
                // until after processAll and navigate complete asynchronously.
                eventObject.setPromise(p);
                break;

            default:
                break;
        }
    }

    function navigating(eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload and dispose methods on current scenario, if any exist
        if (host.winControl) {
            host.winControl.unload && host.winControl.unload();
            host.winControl.dispose && host.winControl.dispose();
        }
        WinJS.Utilities.disposeSubTree(host);
        WinJS.Utilities.empty(host);
        WinJS.log && WinJS.log("", "", "status");

        var p = WinJS.UI.Pages.render(url, host, eventObject.detail.state).
            then(function () {
                var navHistory = nav.history;
                app.sessionState.navigationHistory = {
                    backStack: navHistory.backStack.slice(0),
                    forwardStack: navHistory.forwardStack.slice(0),
                    current: navHistory.current
                };
                app.sessionState.lastUrl = url;
            });
        p.done();
        eventObject.detail.setPromise(p);
    }

    nav.addEventListener("navigating", navigating);
    app.addEventListener("activated", activated, false);
    app.start();
})();
