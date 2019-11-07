//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var sampleTitle = "Custom Device Access";

    var scenarios = [
        { url: "/html/scenario1_deviceConnect.html", title: "Connecting to the Fx2 Device" },
        { url: "/html/scenario2_deviceProperties.html", title: "Getting and Setting device properties" },
        { url: "/html/scenario3_deviceEvents.html", title: "Registering for device events" },
        { url: "/html/scenario4_deviceAsync.html", title: "Invoking asynchronous device methods" }
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
        WinJS.UI.Pages.render(url, host, evt.detail.state).then(function () {
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
