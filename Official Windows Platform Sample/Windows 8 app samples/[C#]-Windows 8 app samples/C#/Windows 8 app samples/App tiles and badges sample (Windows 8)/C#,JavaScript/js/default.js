//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Tiles JS";

    var scenarios = [
        { url: "/html/sendTextTile.html", title: "Send tile notification with text" },
        { url: "/html/sendLocalImageTile.html", title: "Send tile notification with local images" },
        { url: "/html/sendWebImageTile.html", title: "Send tile notification with web images" },
        { url: "/html/sendBadge.html", title: "Send badge notification" },
        { url: "/html/previewAllTemplates.html", title: "Preview all tile notification templates" },
        { url: "/html/enableNotificationQueue.html", title: "Enable notification queue and tags" },
        { url: "/html/notificationExpiration.html", title: "Use notification expiration" },
        { url: "/html/imageProtocols.html", title: "Image protocols and baseUri" },
        { url: "/html/globalization.html", title: "Globalization, localization, scale, and accessibility" },
        { url: "/html/imageManipulation.html", title: "Image Manipulation" }
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

    function ensureUnsnapped() {
        // FilePicker APIs will not work if the application is in a snapped state. If an app wants to show a FilePicker while snapped,
        // it must attempt to unsnap first.
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        return ((currentState !== Windows.UI.ViewManagement.ApplicationViewState.snapped) || Windows.UI.ViewManagement.ApplicationView.tryUnsnap());
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        ensureUnsnapped: ensureUnsnapped
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
