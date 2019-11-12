//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Association Launching";

    var scenarios = [
        { url: "/html/launch-file.html", title: "Launching a file" },
        { url: "/html/launch-uri.html", title: "Launching a URI" },
        { url: "/html/receive-file.html", title: "Receiving a file" },
        { url: "/html/receive-uri.html", title: "Receiving a URI" }
    ];

    function activated(e) {
        // If activated for file type or protocol, launch the approproate scenario.
        // Otherwise navigate to either the first scenario or to the last running scenario
        // before suspension or termination.
        var url = null;
        var arg = null;

        if (e.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.file) {
            url = scenarios[2].url;
            arg = e.detail.files;
        } else if (e.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
            url = scenarios[3].url;
            arg = e.detail.uri;
        } else if (e.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
        }

        if (url !== null) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            e.setPromise(WinJS.UI.processAll().then(function () {
                return WinJS.Navigation.navigate(url, arg);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (evt) {
        var url = evt.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        WinJS.UI.Pages.render(url, host, evt.detail.state).done(function () {
            WinJS.Application.sessionState.lastUrl = url;
        });
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
