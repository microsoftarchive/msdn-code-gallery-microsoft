//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Handling Contact Actions";

    var scenarios = [
        { url: "/html/S1-Call.html", title: "Handling an activation to make a call" },
        { url: "/html/S2-Send-Message.html", title: "Handling an activation to send a message" },
        { url: "/html/S3-Map-Address.html", title: "Handling an activation to map an address" }
    ];

    function activated(eventObject) {
        var url = null;
        var arg = null;

        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.contact) {
            // If activated for a contact, launch the appropriate action handling scenario.
            arg = eventObject.detail;
            if (eventObject.detail.verb === Windows.ApplicationModel.Contacts.ContactLaunchActionVerbs.call) {
                url = scenarios[0].url;
            } else if (eventObject.detail.verb === Windows.ApplicationModel.Contacts.ContactLaunchActionVerbs.message) {
                url = scenarios[1].url;
            } else if (eventObject.detail.verb === Windows.ApplicationModel.Contacts.ContactLaunchActionVerbs.map) {
                url = scenarios[2].url;
            } else {
                WinJS.log && WinJS.log("This app can't handle the contact action verb it was activated for.", "sample", "error");
                return;
            }
        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
            // If activated for a protocol, launch the call scenario
            arg = eventObject.detail;
            url = scenarios[0].url;
        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Otherise, navigate to either the first scenario or to the last running scenario
            // before suspension or termination.
            url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
        }

        if (url !== null) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                return WinJS.Navigation.navigate(url, arg);
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
