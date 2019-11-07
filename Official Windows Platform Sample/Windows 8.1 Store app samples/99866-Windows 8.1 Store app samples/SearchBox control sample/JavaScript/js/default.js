//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Search Control JS sample";

    var scenarios = [
        { url: "/html/S1-SearchBoxWithSuggestions.html",    title: "App provided suggestions" },
        { url: "/html/S2-SuggestionsEastAsian.html",        title: "Suggestions in East Asian Languages" },
        { url: "/html/S3-SuggestionsWindows.html",          title: "Windows provided suggestions" },
        { url: "/html/S4-SuggestionsOpenSearch.html",       title: "Suggestions from Open Search" },
        { url: "/html/S5-SuggestionsXml.html",              title: "Suggestions and results from XML" },
        { url: "/html/S6-KeyboardFocus.html",               title: "Give the search box focus by typing" }
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
