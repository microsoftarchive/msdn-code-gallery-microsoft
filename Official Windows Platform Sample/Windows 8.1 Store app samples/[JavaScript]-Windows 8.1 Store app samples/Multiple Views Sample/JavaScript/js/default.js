//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Multiple views";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Creating and showing multiple views" },
        { url: "/html/scenario2.html", title: "Responding to activation" },
        { url: "/html/scenario3.html", title: "Using animations when switching" }
    ];

    function activated(eventObject) {
        // When an application is activated for a contract, the system automatically brings one of the app's views on screen.
        // The system will always shown the main view of the app (the first view created when the app starts),
        // unless the app has set ApplicationViewSwitcher.DisableShowingMainViewOnActivation to true when launching.
        // In that case, the system will show the most recently used view of the app.
        //
        // In any case, the system tells you the ID of the view it showed. The code below checks if one of its
        // secondary views was shown. See scenario 2 for more details.
        var index = MultipleViews.manager.findViewIndexByViewId(eventObject.detail.currentlyShownApplicationViewId);
        var viewData = null;
        if (index !== null) {
            viewData = MultipleViews.manager.secondaryViews.getAt(index);
        }

        // A secondary view was shown. Send it the activation parameters, which it will use to display
        // the launched protocol
        if (viewData) {
            if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
                viewData.appView.postMessage({
                    handleProtocolLaunch: true,
                    uri: eventObject.detail.uri.absoluteUri
                }, SecondaryViewsHelper.thisDomain);
            }
        } else {
            // The main view was shown. This path is also hit when the app first launches
            //
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.

                // Scenario 2 lets the user choose to show the main view when the app is activated for a contract or show
                // the most recently used view.
                // This checks the user's preference. Changes to DisableShowingMainViewOnactivation are only honored while the application
                // is launching, so be sure to set it then.
                var shouldDisable = Windows.Storage.ApplicationData.current.localSettings.values[MultipleViews.disableMainViewKey];
                if (shouldDisable) {
                    Windows.UI.ViewManagement.ApplicationViewSwitcher.disableShowingMainViewOnActivation();
                }

                var url;
                if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
                    url = scenarios[1].url;
                } else {
                    url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                }

                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one.
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

    // Store some objects used throughout the sample
    WinJS.Namespace.define("MultipleViews", {
        // The ViewMananger class keeps track of all the secondary views currently open
        manager: new SecondaryViewsHelper.ViewManager(),

        // Stores the key of some data saved to local application settings
        disableMainViewKey: "DisableShowingMainViewOnActivation"
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
