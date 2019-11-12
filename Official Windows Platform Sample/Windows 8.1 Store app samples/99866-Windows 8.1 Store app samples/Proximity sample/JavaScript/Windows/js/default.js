//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Proximity-specific code
    var g_tapLaunch = false;
    var g_appRole = Windows.Networking.Proximity.PeerRole.peer;

    function getAppRole() {
        return g_appRole;
    }

    function isLaunchedByTap() {
        var tapLaunch = g_tapLaunch;
        g_tapLaunch = false;
        return tapLaunch;
    }

    WinJS.Namespace.define("ProximityHelpers", {
        isLaunchedByTap: isLaunchedByTap,

        getAppRole: getAppRole
    });

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || SdkSample.scenarios.getAt(0).url;

                // This boolean value captures whether this app was launched by tap or not. 
                // If it was launched by tap, the sample automatically kicks off PeerFinder.
                if (eventObject.detail.arguments !== null) {
                    var arg = new String(eventObject.detail.arguments);
                    if (arg.search("Windows.Networking.Proximity.PeerFinder:StreamSocket") !== -1) {

                        g_tapLaunch = true;

                        if (arg.search("Role=Host") !== -1) {
                            g_appRole = Windows.Networking.Proximity.PeerRole.host;
                        }
                        else if (arg.search("Role=Client") !== -1) {
                            g_appRole = Windows.Networking.Proximity.PeerRole.client;
                        }
                        else {
                            g_appRole = Windows.Networking.Proximity.PeerRole.peer;
                        }
                    }
                }

                if (g_tapLaunch) {
                    url = SdkSample.scenarios.getAt(0).url;; // Force scenario 0 if launched by tap to start the PeerFinder.
                }

                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigating", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
