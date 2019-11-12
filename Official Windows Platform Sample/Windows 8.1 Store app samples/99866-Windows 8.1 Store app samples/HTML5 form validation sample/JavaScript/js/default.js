//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Form Validation";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Creating a form" },
        { url: "/html/scenario2.html", title: "Validating user input" },
        { url: "/html/scenario3.html", title: "Creating a custom error experience" },
        { url: "/html/scenario4.html", title: "Using validation selectors" },
        { url: "/html/scenario5.html", title: "Submitting via FormData + XHR" }
       
    ];

    function activated(e) {
        WinJS.UI.processAll().then(function () {
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
        WinJS.UI.Pages.render(url, host).then(function () {
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
