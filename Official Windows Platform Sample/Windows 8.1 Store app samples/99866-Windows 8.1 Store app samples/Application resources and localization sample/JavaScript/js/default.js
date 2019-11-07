//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Application Resources";

    var scenarios = [
        { url: "/html/scenario1.html", title: "File resources" },
        { url: "/html/scenario2.html", title: "String resources in HTML" },
        { url: "/html/scenario3.html", title: "String resources in JavaScript" },
        { url: "/html/scenario4.html", title: "Resources in AppX manifest" },
        { url: "/html/scenario5.html", title: "Using additional resource files" },
        { url: "/html/scenario6.html", title: "Runtime changes/events" },
        { url: "/html/scenario7.html", title: "Working with web services" },
        { url: "/html/scenario8.html", title: "Resources in HTML and binding" },
        { url: "/html/scenario9.html", title: "Resources in HTML and attributes" },
        { url: "/html/scenario10.html", title: "Application languages" },
        { url: "/html/scenario11.html", title: "Override context" },
        { url: "/html/scenario12.html", title: "Resources in web compartment" },
        { url: "/html/scenario13.html", title: "Multi-dimensional fallback" }

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
