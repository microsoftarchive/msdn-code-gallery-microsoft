//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                var url;
                if (eventObject.detail.arguments !== " " && eventObject.detail.arguments !== "") {
                    var scenarioIndex = SdkSample.getScenarioIndex(eventObject.detail.arguments);
                    url = SdkSample.scenarios.getItem(scenarioIndex).data["url"];
                } else {
                    url = WinJS.Application.sessionState.lastUrl || "/pages/home/home.html";
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
        eventObject.detail.setPromise(p);
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
