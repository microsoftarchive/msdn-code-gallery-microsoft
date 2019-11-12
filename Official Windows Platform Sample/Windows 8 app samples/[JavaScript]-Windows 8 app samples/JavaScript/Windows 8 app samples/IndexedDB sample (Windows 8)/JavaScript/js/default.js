//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "IndexedDB";

    var scenarios = [
        { url: "/html/createschema.html", title: "Create IndexedDB Schema" },
        { url: "/html/populatedata.html", title: "Populate Initial Data" },
        { url: "/html/readdata.html", title: "Reading Data" },
        { url: "/html/writedata.html", title: "Writing Data" }
    ];

    var db = null;

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to keep displaying the splash screen
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

        // Call the unload method for the current scenario, if it has one. 
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        db: db,
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
