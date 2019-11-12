//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "iframe Sandbox Sample";

    var scenarios = [
        { url: "/html/isolateWebContent.html", title: "Isolate Web Content" },
        { url: "/html/allowScript.html", title: "Allow Script" },
        { url: "/html/allowSameOriginAccess.html", title: "Allow Same Origin Access" },
        { url: "/html/allowFormSubmission.html", title: "Allow Form Submission" },
        { url: "/html/remoteWebContent.html", title: "Remote Web Content" }
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

    function showHostText(iframeElement, textElement) {
        try {
            // Continue on break.  The access denied error is handled below.
            document.getElementById(textElement).innerText = document.getElementById(iframeElement).contentDocument.location.host;
        } catch (err) {
            // Handler for Access Denied error.
            document.getElementById(textElement).innerText = err.message;
        }
        document.getElementById(textElement).style.visibility = "visible";
    }

    function showLeftHost() {
        showHostText("iframe1", "leftHost");
    }

    function showRightHost() {
        showHostText("iframe2", "rightHost");
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
        scenarios: scenarios,
        showLeftHost: showLeftHost,
        showRightHost: showRightHost
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
