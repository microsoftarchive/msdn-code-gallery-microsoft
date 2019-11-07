//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    "use strict";

    var sampleTitle = "Secondary Tile JS";

    var scenarios = [
        { url: "/html/PinTile.html", title: "Pin Tile" },
        { url: "/html/UnpinTile.html", title: "Unpin Tile" },
        { url: "/html/EnumerateTiles.html", title: "Enumerate Tiles" },
        { url: "/html/TilePinned.html", title: "Is Tile Pinned?" },
        { url: "/html/LaunchedFromSecondaryTile.html", title: "Show Activation Arguments" },
        { url: "/html/SecondaryTileNotification.html", title: "Secondary Tile notifications" },
        { url: "/html/PinFromAppbar.html", title: "Pin/Unpin Through Appbar" },
        { url: "/html/UpdateAsync.html", title: "Update Secondary Tile Default logo" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.arguments !== "") {
                // Activation arguments are present that were declared when the secondary tile was pinned to Start.
                eventObject.setPromise(WinJS.UI.processAll().done(function () {
                    // Navigate to Scenario 4, where the user will be shown the activation arguments
                    return WinJS.Navigation.navigate(scenarios[4].url, eventObject.detail.arguments);
                }));
            } else {
                // Use setPromise to indicate to the system that the splash screen must not be torn down
                // until after processAll and navigate complete asynchronously.
                eventObject.setPromise(WinJS.UI.processAll().done(function () {
                    // Navigate to either the first scenario or to the last running scenario
                    // before suspension or termination.
                    var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                    return WinJS.Navigation.navigate(url);
                }));
            }
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).done(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
