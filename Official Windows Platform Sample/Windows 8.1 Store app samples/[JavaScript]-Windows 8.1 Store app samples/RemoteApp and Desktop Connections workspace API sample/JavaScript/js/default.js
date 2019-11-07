//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "WorkspaceBroker API Sample";

    var scenarios = [
        { url: "/html/subscribe.html", title: "Subscribe to RemoteApp and Desktops" },
        { url: "/html/display_and_launch.html", title: "Display and launch subscribed RemoteApp and Desktops" },
        { url: "/html/rdpActivation.html", title: "Activation to connect to an RDP file" },
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                var url = "";

                if ((typeof(eventObject.detail.arguments) === "string") && (eventObject.detail.arguments !== "")) {
                    // Store the activation arguments in a composite object for post-processing (gives us more space in case the RDP contents are large)
                    var applicationData = Windows.Storage.ApplicationData.current;
                    var localSettings = applicationData.localSettings;

                    var composite = new Windows.Storage.ApplicationDataCompositeValue();
                    composite["val"] = eventObject.detail.arguments;
                    localSettings.values["activationArguments"] = composite;

                    // navigate to scenario 3 to handle activation requests 
                    url = scenarios[2].url;
                } else {
                    // Navigate to either the first scenario or to the last running scenario
                    // before suspension or termination.
                    url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                }

                return WinJS.Navigation.navigate(url);
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
