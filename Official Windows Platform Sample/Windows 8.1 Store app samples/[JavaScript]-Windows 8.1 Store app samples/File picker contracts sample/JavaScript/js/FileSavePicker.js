//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var fileSavePickerUI;
    var sampleTitle = "File picker app extension sample";

    var scenarios = [
        { url: "/html/fileSavePickerScenario1.html", title: "Save a file to app's storage" },
        { url: "/html/fileSavePickerScenario2.html", title: "Fail to save a file" },
        { url: "/html/fileSavePickerScenario3.html", title: "Save to cached file" },
    ];

    function activated(eventObject) {
        //to identify if app is launched for fileOpenPicker
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.fileSavePicker) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            fileSavePickerUI = eventObject.detail.fileSavePickerUI;
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = scenarios[0].url;
                return WinJS.Navigation.navigate(url, fileSavePickerUI);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, { navigationState: eventObject.detail.state, fileSavePickerUI: fileSavePickerUI }).then(function () {
            //WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
