//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "NavBar control sample";

    var scenarios = [
        { url: "/html/1-CreateNavBar.html", title: "Create NavBar" },
        { url: "/html/2-UseData.html", title: "Create NavBar using data" },
        { url: "/html/3-UseVerticalLayout.html", title: "Create NavBar with vertical layout" },
        { url: "/html/4-Navigate.html", title: "Navigate using NavBar" },
        { url: "/html/5-UseSearchControl.html", title: "Put search control in NavBar" },
        { url: "/html/6-UseSplitButton.html", title: "Use NavBarCommand with split button" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
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

    WinJS.Namespace.define("Data");

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
