//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "USB CDC ACM";

    var scenarios = [
        { url: "/html/scenario1_initialize.html", title: "Select & Initialize" },
        { url: "/html/scenario2_read.html", title: "Read Data" },
        { url: "/html/scenario3_write.html", title: "Write Data" },
        { url: "/html/scenario4_loopback.html", title: "Loopback Test" }
    ];

    /// <summary>
    /// The code section where we check for the ActivationKind to be device (Device launched this app) is found
    /// This code was adapted from the "Removable storage sample" on MSDN.
    /// </summary>
    /// <param name="args">Details about the activation request.</param>
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
        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.device) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                // It doesn't matter which scenario it is because the app must have at least loaded the first
                // scenario (loads the framework) before it can go to any other scenario
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;

                // The deviceInformationId is the same id found in a DeviceInformation object, so it can be used
                // with UsbDevice.fromIdAsync()
                // The eventObject.detail.Verb is the verb that is provided in the appxmanifest for this specific device
                WinJS && WinJS.log("The app was launched by device id: " + eventObject.detail.deviceInformationId
                    + "\nVerb: " + eventObject.detail.verb, "sample", "status");

                // Note that we are passing the device information id to the scenario that is being loaded.
                // This is not required, but is the general pattern suggested by the sample reference above.
                return WinJS.Navigation.navigate(url, eventObject.detail.deviceInformationId);
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
