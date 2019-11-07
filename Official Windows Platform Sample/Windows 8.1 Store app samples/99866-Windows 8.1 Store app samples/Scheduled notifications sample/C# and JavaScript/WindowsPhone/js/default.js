//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var app = WinJS.Application;
    var nav = WinJS.Navigation;
    var activationKinds = Windows.ApplicationModel.Activation.ActivationKind;

    function activated(eventObject) {
        var activationKind = eventObject.detail.kind;
        var activatedEventArgs = eventObject.detail.detail;

        // Handle launch and continuation activation kinds
        switch (activationKind) {
            case activationKinds.launch:
            case activationKinds.pickFileContinuation:
            case activationKinds.pickSaveFileContinuation:
            case activationKinds.pickFolderContinuation:
            case activationKinds.webAuthenticationBrokerContinuation:
                var p = WinJS.UI.processAll().
                    then(function () {

                        // Navigate to either the first scenario or to the last running scenario
                        // before suspension or termination.
                        var url = "/pages/home/home.html";
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
        var isNavigatingBack = eventObject.detail.delta < 0;
        var animationType = WinJS.UI.PageNavigationAnimation.turnstile;
        var animations = WinJS.UI.Animation.createPageNavigationAnimations(animationType, animationType, isNavigatingBack);

        var p = animations.exit(host.children).
            then(function () {
                // Call unload and dispose methods on current scenario, if any exist
                if (host.winControl) {
                    host.winControl.unload && host.winControl.unload();
                    host.winControl.dispose && host.winControl.dispose();
                }
                WinJS.Utilities.disposeSubTree(host);
                WinJS.Utilities.empty(host);
                return WinJS.UI.Pages.render(url, host, eventObject.detail.state);
            }).
            then(function () {
                var navHistory = nav.history;
                app.sessionState.navigationHistory = {
                    backStack: navHistory.backStack.slice(0),
                    forwardStack: navHistory.forwardStack.slice(0),
                    current: navHistory.current
                };
                app.sessionState.lastUrl = url;
                return animations.entrance(host.children);
            });
        p.done();
        eventObject.detail.setPromise(p);
    }

    nav.addEventListener("navigating", navigating);
    app.addEventListener("activated", activated, false);
    app.start();
})();
