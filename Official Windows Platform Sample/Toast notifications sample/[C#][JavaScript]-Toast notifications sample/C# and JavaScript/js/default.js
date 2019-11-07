//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Toasts";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Creating a text-only toast" },
        { url: "/html/scenario2.html", title: "Using package images with text" },
        { url: "/html/scenario3.html", title: "Adding web images" },
        { url: "/html/scenario4.html", title: "Changing default sounds" },
        { url: "/html/scenario5.html", title: "Responding to toast events" },
        { url: "/html/scenario6.html", title: "Long duration toasts" }
    ];

    function activated(e) {
        e.setPromise(WinJS.UI.processAll().then(function () {
            if (e.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch && e.detail.arguments !== "") {
                // If there is some payload sent by the toast, go to Scenario 5
                return WinJS.Navigation.navigate(scenarios[4].url, e.detail.arguments);
            } else {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }
        }));
    }

    WinJS.Navigation.addEventListener("navigated", function (evt) {
        var url = evt.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        WinJS.UI.Pages.render(url, host).done(function () {
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
