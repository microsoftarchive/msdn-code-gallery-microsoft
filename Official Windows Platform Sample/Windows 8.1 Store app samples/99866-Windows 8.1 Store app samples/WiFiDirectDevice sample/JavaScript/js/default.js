//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "WiFiDirectDevice Javascript Sample";

    var scenarios = [
        { url: "/html/WiFiDirectDevice.html", title: "Connect to WiFiDirect capable devices" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;

                return WinJS.Navigation.navigate(url, false);
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

    WinJS.Namespace.define("WiFiDirectDeviceHelpers", {
        displayError : function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "error");
        },

        displayStatus: function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "status");
        },

        clearLastError: function (msg) {
        },

        logInfo: function(logElement, message) {
            logElement.innerHTML += message + "<br/>";
        },

        clearLog: function(logElement) {
            logElement.innerHTML = "";
        },

        id : function (elementId) {
            return document.getElementById(elementId);
        }

    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
