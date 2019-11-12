//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Language Font Mapping";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Get font recommendations for UI elements" },
        { url: "/html/scenario2.html", title: "Get font recommendations for document content" },
        { url: "/html/scenario3.html", title: "Using scale values for a recommended font" },
        { url: "/html/scenario4.html", title: "LanguageFontGroup interaction with font fallback mechanisms" }
    ];

    function activated(e) {
        WinJS.UI.processAll().done(function () {
            // Navigate to either the first scenario or to the last running scenario 
            // before suspension or termination
            var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
            WinJS.Navigation.navigate(url);
        });
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
