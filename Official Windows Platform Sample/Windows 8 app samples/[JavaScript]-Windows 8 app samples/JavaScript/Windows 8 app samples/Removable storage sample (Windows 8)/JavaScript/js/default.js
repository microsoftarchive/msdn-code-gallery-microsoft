//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Removable Storage";

    var scenarios = [
        { url: "/html/s1_listStorages.html", title: "List removable storage devices" },
        { url: "/html/s2_sendToStorage.html", title: "Send file to storage device" },
        { url: "/html/s3_getFromStorage.html", title: "Get image file from storage device" },
        { url: "/html/s4_autoplay.html", title: "Get image file from camera or camera memory (AutoPlay)" }
    ];

    var autoplayScenario = scenarios[3];

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
        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.file) {
            // Navigate to the autoplay scenario, passing in the Autoplay drive.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                return WinJS.Navigation.navigate(autoplayScenario.url, eventObject.detail.files[0]);
            }));

        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.device) {
            // Navigate to the autoplay scenario, passing in the Autoplay device Id.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                return WinJS.Navigation.navigate(autoplayScenario.url, eventObject.detail.deviceInformationId);
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
