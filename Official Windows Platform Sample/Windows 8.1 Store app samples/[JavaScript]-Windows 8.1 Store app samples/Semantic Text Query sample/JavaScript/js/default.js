//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Semantic Text Query";

    var scenarios = [
        { url: "/html/stringMatches.html", title: "Find Scenario" },
        { url: "/html/propertyMatches.html", title: "FindInProperty Scenario" },
        { url: "/html/filePropertiesMatches.html", title: "GetMatchingPropertiesWithRanges Scenario" }
    ];

    // Method used to add bold tags to matches on a string.
    function highlightString(originalString, rangesToHighlight) {
        var pointerPosition = 0;
        var newString = "";
        if (rangesToHighlight.size > 0) {
            var currentAfterPosition = 0;
            rangesToHighlight.forEach(function (textRange) {
                newString += originalString.slice(pointerPosition, textRange.startPosition);
                currentAfterPosition = textRange.startPosition + textRange.length;
                var temp = "<b>" + originalString.slice(textRange.startPosition, currentAfterPosition) + "</b>";
                newString += temp;
                pointerPosition = currentAfterPosition;
            });
            if (pointerPosition !== originalString.length) {
                newString += originalString.slice(pointerPosition, originalString.length);
            }
        }
        else {
            newString = originalString;
        }

        return newString;
    }

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
        scenarios: scenarios,
        highlightString: highlightString
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

