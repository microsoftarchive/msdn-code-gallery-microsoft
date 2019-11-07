//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Indexer JS Sample";

    var scenarios = [
        { url: "/html/addWithAPI.html", title: "Add item to the index using the ContentIndexer" },
        { url: "/html/updateAndDeleteWithAPI.html", title: "Update and delete indexed items using the ContentIndexer" },
        { url: "/html/retrieveWithAPI.html", title: "Retrieve indexed items added using the ContentIndexer" },
        { url: "/html/checkIndexRevision.html", title: "Check the index revision number" },
        { url: "/html/addWithAppContent.html", title: "Add indexed items by using appcontent-ms files" },
        { url: "/html/deleteWithAppContent.html", title: "Delete indexed appcontent-ms files" },
        { url: "/html/retrieveWithAppContent.html", title: "Retrieve indexed properties from appcontent-ms files" }
    ];

    var app = WinJS.Application;

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

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();