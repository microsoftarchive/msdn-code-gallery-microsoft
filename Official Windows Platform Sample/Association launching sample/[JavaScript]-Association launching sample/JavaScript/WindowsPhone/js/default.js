//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var activation = Windows.ApplicationModel.Activation;
    var activationKinds = activation.ActivationKind;

    function activated(eventObject) {
        var url = null;
        var arg = null;
        var homePageUrl = "/pages/home/home.html";

        var activationKind = eventObject.detail.kind;
        var activatedEventArgs = eventObject.detail.detail;

        // Handle launch and continuation activation kinds
        switch (activationKind) {
            case activationKinds.launch:
                url = WinJS.Application.sessionState.lastUrl || homePageUrl;
                break;

            case activationKinds.pickFileContinuation:
                url = WinJS.Application.sessionState.lastUrl || homePageUrl;
                arg = { activationKind: activationKind, activatedEventArgs: activatedEventArgs };
                break;

            case Windows.ApplicationModel.Activation.ActivationKind.file:
                url = SdkSample.scenarios.getAt(2).url;
                arg = eventObject.detail.files;
                break;

            case Windows.ApplicationModel.Activation.ActivationKind.protocol:
                url = SdkSample.scenarios.getAt(3).url;
                arg = eventObject.detail.uri;
                break;

            default:
                return;
        }

        var p = WinJS.UI.processAll().then(function () {
            WinJS.Navigation.history.current.initialPlaceholder = true;

            if ((url !== homePageUrl) && (WinJS.Navigation.history.backStack.length === 0)) {
                // We are going to start the app with a non-home page. 
                // We need to push the home page url to navigation back stack to allow the user to go back to the app's
                // home page by pressing the back button. Otherwise, launching the app from association triggers 
                // will cause the app not being able to go to its home page.                
                WinJS.Navigation.history.backStack.push({ location: homePageUrl });
            }
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
        var isNavigatingBack = eventObject.detail.delta < 0;
        var animationType = WinJS.UI.PageNavigationAnimation.turnstile;
        var animations = WinJS.UI.Animation.createPageNavigationAnimations(animationType, animationType, isNavigatingBack);
        WinJS.Application.sessionState.lastUrl = url;

        var p = animations.exit(host.children).
            then(function () {
                // Call unload method on current scenario, if there is one
                host.winControl && host.winControl.unload && host.winControl.unload();
                WinJS.Utilities.disposeSubTree(host);
                WinJS.Utilities.empty(host);
                return WinJS.UI.Pages.render(url, host, eventObject.detail.state);
            }).
            then(function () { animations.entrance(host.children); });
        p.done();
        eventObject.detail.setPromise(p);
    }

    WinJS.Navigation.addEventListener("navigating", navigating);
    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
