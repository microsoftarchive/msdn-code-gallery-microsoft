//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Share Source JS";

    var scenarios = [
        { url: "/html/text.html", title: "Share text", applink: "ms-sdk-sharesourcejs:navigate?page=ShareText" },
        { url: "/html/weblink.html", title: "Share weblink", applink: "ms-sdk-sharesourcejs:navigate?page=ShareWebLink" },
        { url: "/html/applicationlink.html", title: "Share application link", applink: "ms-sdk-sharesourcejs:navigate?page=ShareApplicationLink" },
        { url: "/html/image.html", title: "Share an image", applink: "ms-sdk-sharesourcejs:navigate?page=ShareImage" },
        { url: "/html/files.html", title: "Share files", applink: "ms-sdk-sharesourcejs:navigate?page=ShareFiles" },
        { url: "/html/delay-render.html", title: "Share delay rendered files", applink: "ms-sdk-sharesourcejs:navigate?page=ShareDeplayRenderedFiles" },
        { url: "/html/html.html", title: "Share HTML content", applink: "ms-sdk-sharesourcejs:navigate?page=ShareHTML" },
        { url: "/html/custom.html", title: "Share custom data", applink: "ms-sdk-sharesourcejs:navigate?page=ShareCustomData" },
        { url: "/html/fail.html", title: "Fail with display text", applink: "ms-sdk-sharesourcejs:navigate?page=ShareErrorMessage" }
    ];

    function activated(eventObject) {
        var url;
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
            scenarios.forEach(function (scenario) {
                if (scenario.applink === eventObject.detail.uri.absoluteUri) {
                    url = scenario.url;
                }
            });
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
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
        scenarios: scenarios,
        missingTitleError: "Enter a title for what you are sharing and try again."
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
